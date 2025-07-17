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


}
