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
    /// 带有配置参数的数据库查询对象
    /// </summary>
    public class ConfiguredQuery<T> : IDbQueryContainer where T : IDbQuery
    {
      /// <summary>
      /// 数据库查询对象
      /// </summary>
      public T Query { get; }

      IDbQuery IDbQueryContainer.Query => this.Query;



      /// <summary>
      /// 创建 ConfiguredQuery 对象
      /// </summary>
      /// <param name="query">数据库查询</param>
      internal ConfiguredQuery( T query )
      {
        Query = query;
        Configures = new DbQueryConfigures();
      }

      /// <summary>
      /// 获取附加于查询之上的配置
      /// </summary>
      public DbQueryConfigures Configures { get; }

    }



    /// <summary>
    /// 指定查询的执行器
    /// </summary>
    /// <param name="query">数据库查询</param>
    /// <param name="executor">用于该查询的执行器</param>
    /// <returns>指定了执行器的数据库查询</returns>
    public static ConfiguredQuery<T> WithExecutor<T>( this ConfiguredQuery<T> query, IDbExecutor executor ) where T : IDbQuery
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
    public static ConfiguredQuery<T> WithExecutor<T>( this ConfiguredQuery<T> query, IAsyncDbExecutor executor ) where T : IDbQuery
    {
      query.Configures.SetService<IDbExecutor>( executor );
      query.Configures.SetService( executor );
      return query;
    }


    /// <summary>
    /// 指定查询所属的数据库连接
    /// </summary>
    /// <param name="query">数据库查询</param>
    /// <param name="database">该查询应该在哪个数据库连接上执行</param>
    /// <returns></returns>
    public static ConfiguredQuery<T> WithDatabase<T>( this ConfiguredQuery<T> query, string database ) where T : IDbQuery
    {
      query.Configures[ConfigureKeys.Database] = database;
      return query;
    }


    /// <summary>
    /// 配置查询参数
    /// </summary>
    /// <typeparam name="T">查询类型</typeparam>
    /// <param name="query">要配置的数据库查询对象</param>
    /// <returns>可配置的查询对象</returns>
    public static ConfiguredQuery<T> Configure<T>( this T query ) where T : IDbQuery
    {
      return new ConfiguredQuery<T>( query );
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
      return Db.DbContext.GetExecutor()?.Execute( query ) ?? throw NotSupported( query );
    }

    /// <summary>
    /// 同步执行查询
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <returns></returns>
    public static IDbExecuteContext Execute<T>( this ConfiguredQuery<T> query ) where T : IDbQuery
    {

      var executor = query.Configures.GetService<IDbExecutor>() ?? Db.DbContext.GetExecutor();

      return executor?.Execute( query.Query ) ?? throw NotSupported( query.Query ); ;
    }


    /// <summary>
    /// 异步执行查询
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消查询标识</param>
    /// <returns></returns>
    public static Task<IAsyncDbExecuteContext> ExecuteAsync( this IDbQuery query, CancellationToken token = default( CancellationToken ) )
    {

      return Db.DbContext.GetAsyncExecutor()?.ExecuteAsync( query, token ) ?? throw NotSupportedAsync( query ); ;

    }

    /// <summary>
    /// 异步执行查询
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消查询标识</param>
    /// <returns></returns>
    public static Task<IAsyncDbExecuteContext> ExecuteAsync<T>( this ConfiguredQuery<T> query, CancellationToken token = default( CancellationToken ) ) where T : IDbQuery
    {

      var executor = query.Configures.GetService<IAsyncDbExecutor>() ?? Db.DbContext.GetAsyncExecutor();

      return executor?.ExecuteAsync( query.Query, token ) ?? throw NotSupportedAsync( query.Query ); ;

    }
  }
}
