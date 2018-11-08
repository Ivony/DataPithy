using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ivony.Data.Common;
using Ivony.Data.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data
{
  /// <summary>
  /// 数据访问上下文
  /// </summary>
  public partial class DbContext : IServiceProvider, IDisposable
  {


    private DbContext() { }


    /// <summary>
    /// 获取父级上下文
    /// </summary>
    public DbContext Parent { get; private set; }



    /// <summary>
    /// 尝试退出当前上下文
    /// </summary>
    public void TryExit()
    {
      Exit( false );

    }


    private void Exit()
    {
      Db.ExitContext( this );
    }


    private void Exit( bool throwException )
    {
      if ( Parent == null )
      {
        if ( throwException )
          throw new InvalidOperationException( "Cannot exit the root context." );
        else
          return;
      }

      var exiter = Db.DbContext.GetExiter( this );
      if ( exiter == null )
      {
        if ( throwException )
          throw new InvalidOperationException( "Context is not in current call stack." );
        else
          return;
      }

      exiter();
    }



    private Action GetExiter( DbContext scope )
    {
      if ( Parent == null )//顶级上下文不能退出
        return null;


      if ( this.Equals( scope ) )
        return () => this.Exit();

      var exiter = this.Parent.GetExiter( scope );
      if ( exiter == null )
        return null;

      return () =>
      {
        this.Exit();
        exiter();
      };
    }




    private bool _disposed = false;


    void IDisposable.Dispose()
    {
      if ( _disposed )//确保只被执行一次。
        return;

      Exit( true );
      _disposed = true;



      var disposable = DbProvider as IDisposable;
      disposable?.Dispose();
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
        var instance = (DbProvider as IServiceProvider)?.GetService( typeof( T ) );
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
          return (T) ActivatorUtilities.CreateInstance( this, instanceType );

        return ((Func<T>) service)();
      }


      if ( Parent != null )
        return Parent.GetService<T>();

      return default( T );
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
      return ActivatorUtilities.CreateInstance<T>( this );
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
