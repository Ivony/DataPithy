using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public static class DbQueryExtensions
  {


    internal class ConfiguredQuery : IDbQuery, IDbQueryContainer
    {
      public IDbQuery Query { get; }


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
    }



    public static IDbQuery WithExecutor( this IDbQuery query, IDbExecutor executor )
    {
      var configured = query.AsConfiguredQuery();
      configured.SetService( executor );

      return configured;
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
    public static IDbExecuteContext Execute<T>( this T query ) where T : IDbQuery
    {
      var configured = query as ConfiguredQuery;
      var executor = configured?.GetService<IDbExecutor>() ?? Db.Context.GetDbExecutor();

      return executor.Execute( query );
    }


    /// <summary>
    /// 异步执行查询
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消查询标识</param>
    /// <returns></returns>
    public static Task<IAsyncDbExecuteContext> ExecuteAsync<T>( this T query, CancellationToken token = default( CancellationToken ) ) where T : IDbQuery
    {

      throw new NotImplementedException();
#if false

      var configured = query as ConfiguredQuery;
      var dbProvider = configured?.GetService<IDbExecutor>() ?? Db.Context.GetDbExecutor();

      return dbProvider.GetAsyncDbExecutor( query ).ExecuteAsync( query, token );
#endif
    }
  }
}
