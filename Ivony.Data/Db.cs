using Ivony.Data.Queries;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
    public static DbContext GetCurrentContext()
    {
      lock ( _sync )
      {
        if ( _root == null )
          _current.Value = _root = CreateDefaultContext();
      }

      return _current.Value = _current.Value ?? _root;
    }



    /// <summary>
    /// 创建新的数据访问上下文
    /// </summary>
    /// <param name="configure">配置数据访问上下文的方法</param>
    /// <returns></returns>
    public static IDisposable NewContext( Action<DbContextBuilder> configure )
    {
      var builder = new DbContextBuilder( GetCurrentContext() );
      configure( builder );

      var result = (DbContextScope) builder.Build();
      _current.Value = result;
      return result;
    }


    internal static void ExitContext( DbContext current, DbContext parent )
    {
      if ( _current.Value != current )
        throw new InvalidOperationException();

      _current.Value = parent;
    }






    /// <summary>
    /// 使用初始化数据访问上下文
    /// </summary>
    public static void InitializeDb( Action<DbContextBuilder> configure )
    {
      lock ( _sync )
      {
        InitializeDb( new ServiceCollection().AddDataPithy().BuildServiceProvider(), configure );
      }
    }


    /// <summary>
    /// 使用指定的服务提供程序初始化数据访问上下文
    /// </summary>
    /// <param name="serviceProvider">服务提供程序</param>
    public static void InitializeDb( this IServiceProvider serviceProvider, Action<DbContextBuilder> configure )
    {
      lock ( _sync )
      {
        if ( _root != null )
          throw new InvalidOperationException("DataPithy is already initialized.");

        var builder = new DbContextBuilder( serviceProvider );
        configure( builder );

        _root = builder.Build();
      }
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
      return GetCurrentContext().GetTemplateParser().ParseTemplate( template );
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




    private static DbContext CreateDefaultContext()
    {

      var services = new ServiceCollection();
      services.AddDataPithy();


      return new DbContext( services.BuildServiceProvider(), new Dictionary<string, IDbExecutorProvider>() );
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

