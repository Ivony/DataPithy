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
      public const string DbProvider = ".DbQuery.DbProvider";
    }



    /// <summary>
    /// 指定查询的执行器
    /// </summary>
    /// <param name="query">数据库查询</param>
    /// <param name="executor">用于该查询的执行器</param>
    /// <returns>指定了执行器的数据库查询</returns>
    public static T WithExecutor<T>( this T query, IDbExecutor executor ) where T : IDbQuery
    {

      if ( executor is IAsyncDbExecutor asyncExecutor )
        query.WithExecutor( asyncExecutor );

      else
        query.Configures.SetService( executor );
      return query;
    }

    /// <summary>
    /// 指定查询的执行器
    /// </summary>
    /// <param name="query">数据库查询</param>
    /// <param name="executor">用于该查询的执行器</param>
    /// <returns>指定了执行器的数据库查询</returns>
    public static T WithExecutor<T>( this T query, IAsyncDbExecutor executor ) where T : IDbQuery
    {
      query.Configures.SetService<IDbExecutor>( executor );
      query.Configures.SetService( executor );
      return query;
    }


    /// <summary>
    /// 指定查询所属的数据库连接
    /// </summary>
    /// <param name="query">数据库查询</param>
    /// <param name="provider">数据库提供程序</param>
    /// <returns>指定了数据库连接的数据库查询</returns>
    public static T WithDatabase<T>( this T query, IDbProvider provider ) where T : IDbQuery
    {
      query.Configures.SetService<IDbProvider>( provider );
      return query;
    }



    /// <summary>
    /// 将查询对象配置与另一个配置对象合并
    /// </summary>
    /// <typeparam name="T">查询对象类型</typeparam>
    /// <param name="query">数据库查询</param>
    /// <param name="configures">要合并的配置对象</param>
    /// <returns>合并了指定配置的的数据库查询</returns>
    public static T WithConfigures<T>( this T query, DbQueryConfigures configures ) where T : IDbQuery
    {
      query.Configures.MergeWith( configures );
      return query;
    }



    private static Exception NotSupported( IDbQuery query )
    {
      return new NotSupportedException( $"Execute query failed, there has no executor support query type of \"{query.GetType()}\"" );
    }

    private static Exception NotSupportedAsync( IDbQuery query )
    {
      return new NotSupportedException( $"Try async execute query failed, there has no async executor support query type of \"{query.GetType()}\" . you can try synchronized execute it." );
    }



    /// <summary>
    /// 同步执行查询
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <returns></returns>
    public static IDbExecuteContext Execute( this IDbQuery query )
    {
      var executor = query.Configures?.GetService<IDbExecutor>() ?? Db.DbContext.GetExecutor();
      return executor?.Execute( query ) ?? throw NotSupported( query ); ;
    }


    /// <summary>
    /// 异步执行查询
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消查询标识</param>
    /// <returns></returns>
    public static Task<IAsyncDbExecuteContext> ExecuteAsync( this IDbQuery query, CancellationToken token = default( CancellationToken ) )
    {

      var executor = query.Configures.GetService<IAsyncDbExecutor>() ?? Db.DbContext.GetAsyncExecutor();
      return executor?.ExecuteAsync( query, token ) ?? throw NotSupportedAsync( query ); ;

    }

  }
}
