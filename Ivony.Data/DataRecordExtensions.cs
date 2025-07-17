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
  /// 将 IDataRecord 转换为实体
  /// </summary>
  /// <typeparam name="T">实体类型</typeparam>
  /// <param name="dataItem">包含数据的 DataRow</param>
  /// <returns>实体</returns>
  public static T ToEntity<T>( this IDataRecord dataItem )
  {
    return ToEntity<T>( dataItem, null );
  }

  /// <summary>
  /// 将 DataRow 转换为实体
  /// </summary>
  /// <typeparam name="T">实体类型</typeparam>
  /// <param name="record">包含数据的 IDataRecord</param>
  /// <param name="converter">实体转换器</param>
  /// <returns>实体</returns>
  public static T? ToEntity<T>( this IDataRecord record, IEntityConverter<T>? converter )
  {
    if ( record is null )
    {

      if ( typeof( T ).IsValueType )
        throw new ArgumentNullException( nameof( record ) );

      else
        return default( T );//等同于return null
    }

    if ( record.FieldCount == 1 )
    {
      var value = record[0];

      if ( value is T )
        return (T) value;
    }


    var entityConverter = converter ?? EntityConvert<T>.GetConverter();
    return entityConverter.Convert( record );
  }





  private static object sync = new object();
  private static Dictionary<Type, Func<IDataRecord, object>> entityConverterDictionary = new Dictionary<Type, Func<IDataRecord, object>>();


  internal static object ToEntity( this IDataRecord dataItem, Type entityType )
  {
    lock ( sync )
    {
      if ( entityConverterDictionary.ContainsKey( entityType ) )
        return entityConverterDictionary[entityType];


      var method = typeof( DataRecordExtensions )
        .GetMethod( nameof( ToEntity ), new[] { typeof( IDataRecord ) } )!
        .MakeGenericMethod( entityType );

      return entityConverterDictionary[entityType] = (Func<IDataRecord, object>) Delegate.CreateDelegate( typeof( Func<IDataRecord, object> ), method );
    }
  }




}
