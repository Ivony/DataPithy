using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data;
/// <summary>
/// 提供查询方法扩展
/// </summary>
public static partial class ExecuteExtensions
{



  /// <summary>
  /// 执行查询并返回首行首列
  /// </summary>
  /// <param name="query">要执行的查询对象</param>
  /// <returns>查询结果</returns>
  public static object ExecuteScalar( this IDbExecutable query )
  {
    using ( var context = query.Execute() )
    {
      var record = context.ReadRecord();
      if ( record != null && record.FieldCount > 0 )
        return record[0];

      else
        return null;
    }
  }

  /// <summary>
  /// 异步执行查询并返回首行首列
  /// </summary>
  /// <param name="query">要执行的查询对象</param>
  /// <param name="token">取消指示</param>
  /// <returns>查询结果</returns>
  public static async Task<object> ExecuteScalarAsync( this IDbExecutable query, CancellationToken token = default( CancellationToken ) )
  {
    using ( var context = await query.ExecuteAsync( token ) )
    {

      var record = await context.ReadRecordAsync();
      if ( record != null && record.FieldCount > 0 )
        return record[0];

      else
        return null;
    }
  }




  /// <summary>
  /// 执行没有结果的查询
  /// </summary>
  /// <param name="query">要执行的查询对象</param>
  /// <returns>查询所影响的行数</returns>
  public static int ExecuteNonQuery( this IDbExecutable query )
  {
    using ( var context = query.Execute() )
    {
      return context.RecordsAffected;
    }
  }

  /// <summary>
  /// 异步执行没有结果的查询
  /// </summary>
  /// <param name="query">要执行的查询对象</param>
  /// <param name="token">取消指示</param>
  /// <returns>查询所影响的行数</returns>
  public static async Task<int> ExecuteNonQueryAsync( this IDbExecutable query, CancellationToken token = default( CancellationToken ) )
  {
    using ( var context = await query.ExecuteAsync( token ) )
    {
      return context.RecordsAffected;
    }
  }



  /// <summary>
  /// 执行查询并返回首行首列
  /// </summary>
  /// <typeparam name="T">返回值类型</typeparam>
  /// <param name="query">要执行的查询对象</param>
  /// <returns>查询结果</returns>
  public static T ExecuteScalar<T>( this IDbExecutable query )
  {
    return DbValueConverter.ConvertFrom<T>( ExecuteScalar( query ) );
  }


  /// <summary>
  /// 异步执行查询并返回首行首列
  /// </summary>
  /// <typeparam name="T">返回值类型</typeparam>
  /// <param name="query">要执行的查询对象</param>
  /// <param name="token">取消指示</param>
  /// <returns>查询结果</returns>
  public async static Task<T> ExecuteScalarAsync<T>( this IDbExecutable query, CancellationToken token = default( CancellationToken ) )
  {
    return DbValueConverter.ConvertFrom<T>( await ExecuteScalarAsync( query, token ) );
  }
}

