using System;
using System.Data;

namespace Ivony.Data
{
  public static class DataRevordExtensions
  {

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
}
