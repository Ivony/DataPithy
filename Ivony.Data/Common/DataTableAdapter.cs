using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{
  /// <summary>
  /// 定义用于填充 DataTable 的 DataAdapter
  /// </summary>
  public class DataTableAdapter : DataAdapter
  {


    /// <summary>
    /// 使用指定范围内的行填充 DataTable 并返回。
    /// </summary>
    /// <param name="dataReader">用来读取数据的 DataReader</param>
    /// <param name="startRecord">要填充的起始记录位置</param>
    /// <param name="maxRecords">最多填充的记录条数</param>
    /// <returns>填充好的 DataTable</returns>
    public DataTable FillDataTable( IDataReader dataReader, int startRecord, int maxRecords )
    {
      var dataTable = new DataTable();
      base.Fill( new[] { dataTable }, dataReader, startRecord, maxRecords );

      return dataTable;
    }


    /// <summary>
    /// 使用指定范围内的行异步填充 DataTable 并返回。
    /// </summary>
    /// <param name="dataReader">用来读取数据的 DataReader</param>
    /// <param name="startRecord">要填充的起始记录位置</param>
    /// <param name="maxRecords">最多填充的记录条数</param>
    /// <returns>填充好的 DataTable</returns>
    public async Task<DataTable> FillDataTableAsync( DbDataReader dataReader, int startRecord, int maxRecords, CancellationToken cancellationToken )
    {

      var dataTable = new DataTable();

      base.FillSchema( dataTable, SchemaType.Mapped, dataReader );

      var array = new object[dataReader.FieldCount];
      var count = 0;


      while ( await dataReader.ReadAsync( cancellationToken ) )
      {

        if ( startRecord > 0 )
        {
          startRecord--;
          continue;
        }

        if ( maxRecords > 0 )
        {
          count++;
          if ( count > maxRecords )
            break;
        }


        dataReader.GetValues( array );
        dataTable.Rows.Add( array );

      }

      dataTable.AcceptChanges();
      return dataTable;
    }

  }
}
