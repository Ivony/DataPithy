using Ivony.Data.Common;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{
  /// <summary>
  /// 提供面向 Entity 的扩展方法
  /// </summary>
  public static partial class ExecuteExtensions
  {



    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public static T[] ExecuteEntities<T>( this IDbExecutable query, IEntityConverter<T>? converter = null )
    {
      return EnumerateEntities<T>( query, converter ).ToArray(); ;
    }


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public async static Task<T[]> ExecuteEntitiesAsync<T>( this IDbExecutable query, IEntityConverter<T>? converter = null, CancellationToken token = default )
    {

      var result = new List<T>();

      await foreach ( var item in EnumerateEntitiesAsync( query, converter, token ) )
        result.Add( item );

      return result.ToArray();
    }


    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public static T? ExecuteEntity<T>( this IDbExecutable query, IEntityConverter<T>? converter = null ) => EnumerateEntities<T>( query, converter ).FirstOrDefault();
    


    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public async static Task<T?> ExecuteEntityAsync<T>( this IDbExecutable query, IEntityConverter<T>? converter = null, CancellationToken token = default )
    {
      await foreach ( var item in EnumerateEntitiesAsync<T>( query, converter, token ) )
      {
        return item;
      }

      return default;
    }










  }
}
