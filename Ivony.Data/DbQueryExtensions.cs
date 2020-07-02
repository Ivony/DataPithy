using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{


  /// <summary>
  /// 提供针对数据库查询的扩展方法
  /// </summary>
  public static class DbQueryExtensions
  {



    private static class ConfigureKeys
    {
      public const string Executor = ".DbQuery.Executor";
      public const string Database = ".DbQuery.Database";
    }



    /// <summary>
    /// 指定查询的执行器
    /// </summary>
    /// <param name="query">数据库查询</param>
    /// <param name="executor">用于该查询的执行器</param>
    /// <returns>指定了执行器的数据库查询</returns>
    public static T WithExecutor<T>( this T query, IDbExecutor executor ) where T : DbQuery
    {
      query.Configures.SetService( executor );
      return query;
    }

    /// <summary>
    /// 指定查询的执行器
    /// </summary>
    /// <param name="query">数据库查询</param>
    /// <param name="executor">用于该查询的执行器</param>
    /// <returns>指定了执行器的数据库查询</returns>
    public static T WithExecutor<T>( this T query, IAsyncDbExecutor executor ) where T : DbQuery
    {
      query.Configures.SetService<IDbExecutor>( executor );
      query.Configures.SetService( executor );
      return query;
    }


    /// <summary>
    /// 指定查询所属的数据库连接
    /// </summary>
    /// <param name="query">数据库查询</param>
    /// <param name="database">数据库提供程序</param>
    /// <returns></returns>
    public static T WithDatabase<T>( this T query, IDatabase database ) where T : DbQuery
    {
      query.Configures.SetService<IDatabase>( database );
      return query;
    }


    /// <summary>
    /// 指定查询所属的数据库连接
    /// </summary>
    /// <param name="query">数据库查询</param>
    /// <param name="database">数据库名称</param>
    /// <returns></returns>
    public static T WithDatabase<T>( this T query, string database ) where T : DbQuery
    {
      query.Configures.SetService<IDatabase>( Db.Database( database ) ?? throw new InvalidOperationException( $"database \"{database}\" is not registered." ) );
      return query;
    }


    /// <summary>
    /// 使用指定的追踪服务
    /// </summary>
    /// <typeparam name="T">数据库查询类型</typeparam>
    /// <param name="query">数据库查询</param>
    /// <param name="traceService">要使用的追踪服务</param>
    public static T WithTraceService<T>( this T query, IDbTraceService traceService ) where T : DbQuery
    {
      query.Configures.SetService<IDbTraceService>( traceService );
      return query;
    }



    /// <summary>
    /// 创建查询的只读副本
    /// </summary>
    /// <typeparam name="T">查询类型</typeparam>
    /// <param name="query">查询对象</param>
    /// <returns>查询的只读副本</returns>
    public static T AsReadonly<T>( this T query ) where T : DbQuery
    {
      return (T) query.Clone( query.Configures.AsReadonly() );
    }

    /// <summary>
    /// 创建查询的副本
    /// </summary>
    /// <typeparam name="T">查询类型</typeparam>
    /// <param name="query">查询对象</param>
    /// <returns>查询的副本</returns>
    public static T Clone<T>( this T query ) where T : DbQuery
    {
      return (T) query.Clone( query.Configures.Clone() );
    }


  }
}
