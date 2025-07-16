using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data;


/// <summary>
/// 提供 IDataRecord 的扩展方法
/// </summary>
public static class DataRecordExtensions
{
  /// <summary>
  /// 获取指定字段的值
  /// </summary>
  /// <typeparam name="T">字段值类型</typeparam>
  /// <param name="fieldName">字段名称</param>
  /// <param name="record">数据记录</param>
  /// <returns>字段值</returns>
  public static T FieldValue<T>( this IDataRecord record, string fieldName )
  {
    return FieldValue<T>( record, record.GetOrdinal( fieldName ) );
  }

  /// <summary>
  /// 获取指定字段的值
  /// </summary>
  /// <typeparam name="T">字段值类型</typeparam>
  /// <param name="ordinal">字段索引</param>
  /// <param name="record">数据记录</param>
  /// <returns>字段值</returns>
  private static T FieldValue<T>( IDataRecord record, int ordinal )
  {
    var value = record.GetValue( ordinal );
    return DbValueConverter.ConvertFrom<T>( value, record.GetDataTypeName( ordinal ) );
  }



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
}
