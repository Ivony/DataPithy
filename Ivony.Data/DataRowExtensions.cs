using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Ivony.Data;


/// <summary>
/// 提供针对 <see cref="DataRow"/> 的扩展方法
/// </summary>
public static class DataRowExtensions
{
  private static readonly object sync = new object();
  private static Dictionary<Type, Func<DataRow, DataColumn, object>> dbValueConverterDictionary = new Dictionary<Type, Func<DataRow, DataColumn, object>>();


  internal static object FieldValue( this DataRow dataItem, DataColumn column, Type valueType ) => GetFieldValueMethod( valueType )( dataItem, column );



  private static readonly MethodInfo fieldValueMethod = typeof( DataRowExtensions ).GetMethod( nameof( FieldValue ), 1, new[] { typeof( DataRow ), typeof( DataColumn ) } );

  private static Func<DataRow, DataColumn, object> GetFieldValueMethod( Type valueType )
  {
    lock ( sync )
    {
      if ( dbValueConverterDictionary.ContainsKey( valueType ) )
        return dbValueConverterDictionary[valueType];


      var method = fieldValueMethod.MakeGenericMethod( valueType );

      return dbValueConverterDictionary[valueType] = (Func<DataRow, DataColumn, object>) Delegate.CreateDelegate( typeof( Func<DataRow, DataColumn, object> ), method );
    }
  }




  /// <summary>
  /// 获取指定字段的值
  /// </summary>
  /// <typeparam name="T">值类型</typeparam>
  /// <param name="dataRow">数据行</param>
  /// <param name="columnIndex">要返回其值的列索引</param>
  /// <returns>强类型的值</returns>
  public static T FieldValue<T>( this DataRow dataRow, int columnIndex ) => FieldValue<T>( dataRow, dataRow.Table.Columns[columnIndex] );

  /// <summary>
  /// 获取指定字段的值
  /// </summary>
  /// <typeparam name="T">值类型</typeparam>
  /// <param name="dataRow">数据行</param>
  /// <param name="columnName">要返回其值的列名称</param>
  /// <returns>强类型的值</returns>
  public static T FieldValue<T>( this DataRow dataRow, string columnName ) => FieldValue<T>( dataRow, dataRow.Table.Columns[columnName] );





  /// <summary>
  /// 获取指定字段的值
  /// </summary>
  /// <param name="column">要返回其值的列</param>
  /// <param name="dataRow">数据行</param>
  /// <typeparam name="T">字段值的类型</typeparam>
  /// <returns>字段的的值</returns>
  public static T FieldValue<T>( this DataRow dataRow, DataColumn column )
  {
    if ( dataRow == null )
      throw new ArgumentNullException( "dataRow" );

    if ( column == null )
      throw new ArgumentNullException( "column" );


    try
    {
      return DbValueConverter.ConvertFrom<T>( dataRow[column] );
    }
    catch ( Exception e )
    {
      e.Data.Add( "DataColumnName", column.ColumnName );
      throw;
    }
  }


  /// <summary>
  /// 将 DataRow 转换为实体
  /// </summary>
  /// <typeparam name="T">实体类型</typeparam>
  /// <param name="dataItem">包含数据的 DataRow</param>
  /// <returns>实体</returns>
  public static T ToEntity<T>( this DataRow dataItem ) => dataItem.AsDataRecord().ToEntity<T>();

  /// <summary>
  /// 将 DataRow 转换为实体
  /// </summary>
  /// <typeparam name="T">实体类型</typeparam>
  /// <param name="dataItem">包含数据的 DataRow</param>
  /// <param name="converter">实体转换器</param>
  /// <returns>实体对象</returns>
  public static T ToEntity<T>( this DataRow dataItem, IEntityConverter<T> converter ) => dataItem.AsDataRecord().ToEntity<T>( converter );





  /// <summary>
  /// 将 DataRow 转换为 IDataRecord
  /// </summary>
  /// <param name="dataRow">数据行</param>
  /// <returns>转换后的 IDataRecord </returns>
  public static IDataRecord AsDataRecord( this DataRow dataRow )
  {
    return new DataRecord( dataRow );
  }


  private class DataRecord : IDataRecord
  {
    private DataRow _dataRow;

    public DataRecord( DataRow dataRow )
    {
      _dataRow = dataRow;
    }

    public object this[int i] => _dataRow[i];
    public object this[string name] => _dataRow[name];

    public int FieldCount => _dataRow.Table.Columns.Count;

    public bool GetBoolean( int i ) => _dataRow.FieldValue<bool>( i );

    public byte GetByte( int i ) => _dataRow.FieldValue<byte>( i );

    public long GetBytes( int i, long fieldOffset, byte[] buffer, int bufferoffset, int length ) => throw new NotSupportedException();

    public char GetChar( int i ) => _dataRow.FieldValue<char>( i );

    public long GetChars( int i, long fieldoffset, char[] buffer, int bufferoffset, int length ) => throw new NotSupportedException();

    public IDataReader GetData( int i ) => null;

    public string GetDataTypeName( int i ) => throw new NotSupportedException();

    public DateTime GetDateTime( int i ) => _dataRow.FieldValue<DateTime>( i );

    public decimal GetDecimal( int i ) => _dataRow.FieldValue<decimal>( i );

    public double GetDouble( int i ) => _dataRow.FieldValue<double>( i );

    public Type GetFieldType( int i ) => _dataRow.Table.Columns[i].DataType;

    public float GetFloat( int i ) => _dataRow.FieldValue<float>( i );

    public Guid GetGuid( int i ) => _dataRow.FieldValue<Guid>( i );

    public short GetInt16( int i ) => _dataRow.FieldValue<short>( i );

    public int GetInt32( int i ) => _dataRow.FieldValue<int>( i );

    public long GetInt64( int i ) => _dataRow.FieldValue<long>( i );

    public string GetName( int i ) => _dataRow.FieldValue<string>( i );

    public int GetOrdinal( string name ) => _dataRow.Table.Columns[name].Ordinal;

    public string GetString( int i ) => _dataRow.FieldValue<string>( i );

    public object GetValue( int i ) => _dataRow.ItemArray[i];

    public int GetValues( object[] values )
    {
      _dataRow.ItemArray.CopyTo( values, 0 );
      return Math.Min( _dataRow.ItemArray.Length, values.Length );

    }

    public bool IsDBNull( int i ) => _dataRow.IsNull( i );
  }


}


