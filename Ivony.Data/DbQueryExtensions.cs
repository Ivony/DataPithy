using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public static class DbQueryExtensions
  {



    /// <summary>
    /// 带有配置参数的数据库查询对象
    /// </summary>
    public class ConfiguredQuery : IDbQueryContainer
    {
      /// <summary>
      /// 数据库查询对象
      /// </summary>
      public IDbQuery Query { get; }


      /// <summary>
      /// 创建 ConfiguredQuery 对象
      /// </summary>
      /// <param name="query">数据库查询</param>
      public ConfiguredQuery( IDbQuery query )
      {
        Query = query;
      }


      private Dictionary<Type, object> serviceDictionary = new Dictionary<Type, object>();

      internal void SetService<T>( T service )
      {
        SetService( typeof( T ), service );
      }

      private void SetService<T>( Type type, T service )
      {
        serviceDictionary[type] = service;
      }

      internal T GetService<T>()
      {
        return (T) GetService( typeof( T ) );
      }

      private object GetService( Type type )
      {
        return serviceDictionary.GetValueOrDefault( type );
      }


      private Dictionary<string, string> _properties;
      internal void SetProperty( string name, string value )
      {
        _properties[name] = value;
      }

      internal string GetProperty( string name )
      {
        return _properties.GetValueOrDefault( name );
      }


    }



    /// <summary>
    /// 指定查询的执行器
    /// </summary>
    /// <param name="query">数据库查询</param>
    /// <param name="executor">用于该查询的执行器</param>
    /// <returns>指定了执行器的数据库查询</returns>
    public static ConfiguredQuery WithExecutor( this ConfiguredQuery query, IDbExecutor executor )
    {
      query.SetService( executor );
      return query;
    }



    private static readonly string DatabasePropertyName = ".Database";

    /// <summary>
    /// 指定查询所属的数据库连接
    /// </summary>
    /// <param name="query">数据库查询</param>
    /// <param name="database">该查询应该在哪个数据库连接上执行</param>
    /// <returns></returns>
    public static ConfiguredQuery WithDatabase( this ConfiguredQuery query, string database )
    {
      query.SetProperty( DatabasePropertyName, database );
      return query;
    }


    private static ConfiguredQuery AsConfiguredQuery( this IDbQuery query )
    {
      return query as ConfiguredQuery ?? new ConfiguredQuery( query );
    }


    /// <summary>
    /// 同步执行查询
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <returns></returns>
    public static IDbExecuteContext Execute( this IDbQuery query )
    {
      return Db.GetCurrentContext().GetExecutor()?.Execute( query );
    }

    /// <summary>
    /// 同步执行查询
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <returns></returns>
    public static IDbExecuteContext Execute<T>( this ConfiguredQuery query ) where T : IDbQuery
    {

      var executor = query.GetService<IDbExecutor>() ?? Db.GetCurrentContext().GetExecutor();

      return executor?.Execute( query.Query );
    }


    /// <summary>
    /// 异步执行查询
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消查询标识</param>
    /// <returns></returns>
    public static Task<IAsyncDbExecuteContext> ExecuteAsync( this IDbQuery query, CancellationToken token = default( CancellationToken ) )
    {

      return Db.GetCurrentContext().GetAsyncExecutor()?.ExecuteAsync( query, token );

    }

    /// <summary>
    /// 异步执行查询
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消查询标识</param>
    /// <returns></returns>
    public static Task<IAsyncDbExecuteContext> ExecuteAsync( this ConfiguredQuery query, CancellationToken token = default( CancellationToken ) )
    {

      var executor = query.GetService<IAsyncDbExecutor>() ?? Db.GetCurrentContext().GetAsyncExecutor();

      return executor?.ExecuteAsync( query.Query, token );

    }
  }
}
