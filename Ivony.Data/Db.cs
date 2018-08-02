using Ivony.Data.Queries;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public static class Db
  {


    private static DbContext _default;
    private static readonly object _sync = new object();


    private static DbContext _root;

    private static AsyncLocal<DbContext> _current = new AsyncLocal<DbContext>();


    /// <summary>
    /// 获取当前数据访问上下文
    /// </summary>
    /// <returns></returns>
    public static DbContext DbContext
    {
      get
      {
        lock ( _sync )
        {
          if ( _current.Value != null )
            return _current.Value;

          if ( _root == null )
            InitializeDb( configure => { } );

          return _current.Value = _root;
        }
      }
    }



    /// <summary>
    /// 进入新的数据访问上下文
    /// </summary>
    /// <param name="configure">配置数据访问上下文的方法</param>
    /// <returns></returns>
    public static IDisposable Enter( Action<DbContextConfigure> configure )
    {
      var builder = new DbContextConfigure( DbContext );
      configure( builder );

      var result = (DbContextScope) builder.Build();
      _current.Value = result;
      return result;
    }



    /// <summary>
    /// 退出当前上下文
    /// </summary>
    public static void Exit()
    {
      var scope = DbContext as DbContextScope;
      if ( scope == null )
        throw new InvalidOperationException();

      scope.Dispose();
    }



    internal static void ExitContext( DbContext current, DbContext parent )
    {
      if ( _current.Value != current )
        throw new InvalidOperationException();

      _current.Value = parent;
    }






    /// <summary>
    /// 初始化根数据访问上下文
    /// </summary>
    public static DbContext InitializeDb( Action<DbContextConfigure> configure )
    {
      lock ( _sync )
      {
        if ( _root != null )
          throw new InvalidOperationException( "DataPithy is already initialized." );

        return _root = InitializeDbContext( new ServiceCollection().AddDataPithy().BuildServiceProvider(), configure );
      }
    }


    /// <summary>
    /// 初始化根数据访问上下文
    /// </summary>
    /// <param name="serviceProvider">服务提供程序</param>
    /// <param name="configure">数据访问上下文配置</param>
    public static IServiceProvider InitializeDb( this IServiceProvider serviceProvider, Action<DbContextConfigure> configure )
    {
      lock ( _sync )
      {
        if ( _root != null )
          throw new InvalidOperationException( "DataPithy is already initialized." );

        _root = InitializeDbContext( serviceProvider, configure );
      }

      return serviceProvider;
    }


    private static DbContext InitializeDbContext( IServiceProvider serviceProvider, Action<DbContextConfigure> configure )
    {
      var builder = new DbContextConfigure( serviceProvider );
      builder.DefaultDatabase = DefaultDatabaseName;
      configure( builder );
      return builder.Build();
    }


    /// <summary>
    /// 解析模板表达式，创建参数化查询对象
    /// </summary>
    /// <param name="template">参数化模板</param>
    /// <returns>参数化查询</returns>
    public static ParameterizedQuery T( FormattableString template )
    {
      return Template( template );
    }


    /// <summary>
    /// 解析模板表达式，创建参数化查询对象
    /// </summary>
    /// <param name="template">参数化模板</param>
    /// <returns>参数化查询</returns>
    public static ParameterizedQuery Template( FormattableString template )
    {
      if ( template == null )
        return null;

      return DbContext.GetTemplateParser().ParseTemplate( template );
    }


    /// <summary>
    /// 解析模板表达式，创建参数化查询对象
    /// </summary>
    /// <param name="text">查询文本</param>
    /// <returns>参数化查询</returns>
    public static ParameterizedQuery Text( string text )
    {
      if ( text == null )
        return null;

      return DbContext.GetTemplateParser().ParseTemplate( FormattableStringFactory.Create( text ) );
    }


    /// <summary>
    /// 将文本字符串转换为数据库查询对象
    /// </summary>
    /// <param name="queryText">查询文本</param>
    /// <returns>数据库查询对象</returns>
    public static ParameterizedQuery AsTextQuery( this string queryText )
    {
      return Db.Text( queryText );
    }




    /// <summary>
    /// 配置使用 DataPithy
    /// </summary>
    /// <param name="services">服务配置</param>
    public static IServiceCollection AddDataPithy( this IServiceCollection services )
    {

      services.AddSingleton( typeof( ITemplateParser ), typeof( TemplateParser ) );
      services.AddTransient( typeof( IParameterizedQueryBuilder ), typeof( ParameterizedQueryBuilder ) );

      return services;
    }


    /// <summary>
    /// 默认数据库连接名称
    /// </summary>
    public static string DefaultDatabaseName => "Default";


    public static IDbTransactionContext BeginTransaction()
    {
      throw new NotImplementedException();
    }

  }
}

