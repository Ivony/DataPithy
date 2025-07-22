using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.Core;

public interface IDataTableAdapter
{
  /// <summary>
  /// 使用指定范围内的行填充 DataTable 并返回。
  /// </summary>
  /// <param name="dataReader">用来读取数据的 DataReader</param>
  /// <param name="startRecord">要填充的起始记录位置</param>
  /// <param name="maxRecords">最多填充的记录条数</param>
  /// <returns>填充好的 DataTable</returns>
  DataTable FillDataTable( IDataReader dataReader, int startRecord, int maxRecords );


  /// <summary>
  /// 使用指定范围内的行异步填充 DataTable 并返回。
  /// </summary>
  /// <param name="dataReader">用来读取数据的 DataReader</param>
  /// <param name="startRecord">要填充的起始记录位置</param>
  /// <param name="maxRecords">最多填充的记录条数</param>
  /// <returns>填充好的 DataTable</returns>
  Task<DataTable> FillDataTableAsync( DbDataReader dataReader, int startRecord, int maxRecords, CancellationToken cancellationToken );
}