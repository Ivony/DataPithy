using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data;

/// <summary>
/// 数据库查询结果集流式处理扩展方法
/// </summary>
public static class FlowExecuteExtensions
{


  /// <summary>
  /// 异步枚举查询结果集
  /// </summary>
  /// <param name="query">要执行的查询</param>
  /// <param name="token">取消标志</param>
  /// <returns>异步查询结果</returns>
  public static async IAsyncEnumerable<IDataRecord> EnumerateDataRecordsAsync( this IDbExecutable query, [EnumeratorCancellation] CancellationToken token = default )
  {
    using ( var context = await query.ExecuteAsync( token ) )
    {
      while ( true )
      {
        var record = await context.ReadRecordAsync( token );
        if ( record == null )
          yield break;

        yield return record;
      }
    }
  }


  /// <summary>
  /// 枚举查询结果集
  /// </summary>
  /// <param name="query">要执行的查询</param>
  /// <returns>异步查询结果</returns>
  public static IEnumerable<IDataRecord> EnumerateDataRecords( this IDbExecutable query )
  {
    using ( var context = query.Execute() )
    {
      while ( true )
      {
        var record = context.ReadRecord();
        if ( record == null )
          yield break;

        yield return record;
      }
    }
  }


  /// <summary>
  /// 枚举查询结果并转换为实体对象
  /// </summary>
  /// <typeparam name="T">实体类型</typeparam>
  /// <param name="query">查询对象</param>
  /// <returns>实体对象枚举</returns>

  public static IEnumerable<T> EnumerateEntities<T>( this IDbExecutable query )
  {
    foreach ( var record in query.EnumerateDataRecords() )
      yield return record.ToEntity<T>();
  }


  /// <summary>
  /// 异步枚举查询结果并转换为实体对象
  /// </summary>
  /// <typeparam name="T">实体类型</typeparam>
  /// <param name="query">查询对象</param>
  /// <param name="cancellationToken">取消令牌</param>
  /// <returns>实体对象枚举</returns>
  public static async IAsyncEnumerable<T> EnumerateEntitiesAsync<T>( this IDbExecutable query, [EnumeratorCancellation] CancellationToken cancellationToken = default )
  {
    await foreach ( var record in query.EnumerateDataRecordsAsync( cancellationToken ) )
      yield return record.ToEntity<T>();
  }

}
