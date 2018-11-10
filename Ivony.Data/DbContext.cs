using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ivony.Data.Common;
using Ivony.Data.Queries;

namespace Ivony.Data
{
  /// <summary>
  /// 数据访问上下文
  /// </summary>
  public partial class DbContext : IServiceProvider
  {


    private DbContext() { }


    /// <summary>
    /// 获取父级上下文
    /// </summary>
    public DbContext Parent { get; private set; }






    private static readonly object _sync = new object();

    private static AsyncLocal<DbContext> _current = new AsyncLocal<DbContext>();

    /// <summary>
    /// 获取当前数据访问上下文
    /// </summary>
    /// <returns></returns>
    public static DbContext Current
    {
      get
      {
        lock ( _sync )
        {
          if ( _current.Value != null )
            return _current.Value;

          SetContext( Initialize() );
          return _current.Value;
        }
      }
    }


    private static DbContext Initialize()
    {
      var builder = new DbContext.Builder();

      builder.Services.AddService<IParameterizedQueryBuilder>( provider => new ParameterizedQueryBuilder() );
      builder.Services.AddService<ITemplateParser, TemplateParser>();

      return builder.Build();
    }




    private class Scope : IDisposable
    {
      public DbContext DbContext { get; }

      public Scope( DbContext context )
      {
        DbContext = context;
      }


      public void Dispose()
      {
        Exit( Current );

      }

      private void Exit( DbContext current )
      {
        if ( current == null )
          return;

        if ( current == DbContext )
        {
          if ( current.Parent != null )
            SetContext( current.Parent );
        }


        Exit( current.Parent );
      }
    }


    private static void SetContext( DbContext context )
    {
      _current.Value = context;
    }



    private bool IsAncestorOf( DbContext context )
    {
      if ( context.Parent == this )
        return true;

      else
        return IsAncestorOf( context.Parent );
    }





    /// <summary>
    /// 从数据库访问上下文派生出另一个数据库访问上下文
    /// </summary>
    /// <param name="configure">配置派生上下文的方法</param>
    /// <returns>派生的数据库访问上下文</returns>
    public DbContext Derive( Action<Builder> configure )
    {
      var builder = new Builder( this );
      configure( builder );

      return builder.Build();
    }



    /// <summary>
    /// 派生一个数据库访问上下文并设置为当前上下文
    /// </summary>
    /// <param name="configure">配置派生上下文的方法</param>
    /// <returns>一个实现了 IDisosable 的对象，用于退出派生的上下文</returns>
    public static IDisposable Enter( Action<Builder> configure )
    {
      var context = Current.Derive( configure );
      SetContext( context );
      return new Scope( context );
    }




    /// <summary>
    /// 需要使用的数据库访问提供程序
    /// </summary>
    public IDbProvider DbProvider { get; private set; }


    private IReadOnlyDictionary<Type, object> services;


    private static readonly MethodInfo getServiceMethod = typeof( DbContext ).GetMethod( "GetService", 1, new Type[0] );

    /// <summary>
    /// 获取指定类型的服务对象实例
    /// </summary>
    /// <param name="serviceType">服务类型</param>
    /// <returns>服务实例</returns>
    public object GetService( Type serviceType )
    {

      var instance = DbProvider?.GetService( serviceType );
      if ( instance != null )
        return instance;

      if ( serviceType.IsAssignableFrom( GetType() ) )
        return this;


      if ( services.TryGetValue( serviceType, out var service ) )
      {


      }




      return getServiceMethod.MakeGenericMethod( serviceType ).Invoke( this, new object[0] );
    }


    /// <summary>
    /// 获取指定类型的服务对象实例
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>服务实例</returns>
    public T GetService<T>() where T : class
    {

      {
        var instance = DbProvider?.GetService( typeof( T ) );
        if ( instance != null )
          return (T) instance;
      }

      {
        var instance = this as T;
        if ( instance != null )
          return instance;
      }

      if ( services.TryGetValue( typeof( T ), out var service ) )
      {
        if ( service is T instance )
          return instance;

        if ( service is Type instanceType )
          return (T) CreateInstance( instanceType );

        return ((Func<T>) service)();
      }


      if ( Parent != null )
        return Parent.GetService<T>();

      return default( T );
    }

    private object CreateInstance( Type instanceType )
    {
      throw new NotImplementedException();
    }




    /// <summary>
    /// 创建数据库事务
    /// </summary>
    /// <returns></returns>
    internal IDbTransactionContext CreateTransaction()
    {
      return DbProvider.CreateTransaction( this ) ?? throw new NotSupportedException();
    }

    /// <summary>
    /// 创建异步数据库事务
    /// </summary>
    /// <returns></returns>
    internal IAsyncDbTransactionContext CreateAsyncTransaction()
    {
      var transaction = CreateTransaction() ?? throw new NotSupportedException();
      return transaction as IAsyncDbTransactionContext ?? new AsyncDbTransactionContextWrapper( transaction );
    }



    /// <summary>
    /// 获取查询执行器
    /// </summary>
    /// <returns></returns>
    public IDbExecutor GetExecutor()
    {
      return DbProvider.GetDbExecutor( this );
    }



    /// <summary>
    /// 获取异步查询执行器
    /// </summary>
    /// <returns></returns>
    public IAsyncDbExecutor GetAsyncExecutor()
    {

      return DbProvider.GetAsyncDbExecutor( this );

    }






    /// <summary>
    /// 获取指定类型的配置对象
    /// </summary>
    /// <typeparam name="T">配置类型</typeparam>
    /// <returns></returns>
    public T GetConfiguration<T>()
    {
      return (T) CreateInstance( typeof( T ) );
    }





    /// <summary>
    /// 获取当前默认的追踪服务
    /// </summary>
    /// <returns></returns>
    public IDbTraceService GetTraceService()
    {
      return GetService<IDbTraceService>()
        ?? Parent?.GetService<IDbTraceService>();

    }

    /// <summary>
    /// 获取当前默认的模板解析器
    /// </summary>
    /// <returns></returns>
    public ITemplateParser GetTemplateParser()
    {
      return GetService<ITemplateParser>()
        ?? Parent?.GetService<ITemplateParser>();

    }


    /// <summary>
    /// 获取当前默认的参数化查询构建器
    /// </summary>
    /// <returns></returns>
    public IParameterizedQueryBuilder GetParameterizedQueryBuilder()
    {

      return GetService<IParameterizedQueryBuilder>()
        ?? Parent?.GetService<IParameterizedQueryBuilder>();

    }


    /// <summary>
    /// 获取当前上下文的属性设置
    /// </summary>
    public IReadOnlyDictionary<string, object> Properties { get; private set; }



    /// <summary>
    /// 是否自动添加空白字符分隔符
    /// </summary>
    public bool AutoWhitespaceSeparator { get; private set; } = false;

  }
}
