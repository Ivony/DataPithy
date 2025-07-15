using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Ivony.Data.Common;

namespace Ivony.Data
{


  /// <summary>
  /// 提供 DataTable 相关的查询执行扩展方法
  /// </summary>
  public static class DataTableExecuteExtensions
  {

    /// <summary>
    /// 执行查询并将所有结果集包装成 DataTable 返回
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static DataTable[] ExecuteAllDataTables( this IDbExecutable query )
    {

      List<DataTable> dataTables = new List<DataTable>();

      using ( var context = query.Execute() )
      {

        do
        {
          dataTables.Add( context.LoadDataTable( 0, 0 ) );
        } while ( context.NextResult() );
      }

      return dataTables.ToArray();
    }


    /// <summary>
    /// 异步执行查询并将所有结果集包装成 DataTable 返回
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询结果</returns>
    public static async Task<DataTable[]> ExecuteAllDataTablesAsync( this IDbExecutable query, CancellationToken token = default( CancellationToken ) )
    {

      List<DataTable> dataTables = new List<DataTable>();

      using ( var context = await query.ExecuteAsync( token ) )
      {

        do
        {
          dataTables.Add( context.LoadDataTable( 0, 0 ) );

        } while ( await context.NextResultAsync() );
      }

      return dataTables.ToArray();

    }

    /// <summary>
    /// 执行查询并将第一个结果集包装成 DataTable 返回
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static DataTable ExecuteDataTable( this IDbExecutable query )
    {
      using ( var context = query.Execute() )
      {
        return context.LoadDataTable( 0, 0 );
      }
    }

    /// <summary>
    /// 异步执行查询并将第一个结果集包装成 DataTable 返回
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询结果</returns>
    public static async Task<DataTable> ExecuteDataTableAsync( this IDbExecutable query, CancellationToken token = default( CancellationToken ) )
    {
      using ( var context = await query.ExecuteAsync( token ) )
      {

        return await context.LoadDataTableAsync( 0, 0, token );

      }
    }



    /// <summary>
    /// 执行查询并返回首行
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <returns>查询结果</returns>
    public static DataRow ExecuteFirstRow( this IDbExecutable query )
    {
      //UNDONE

      using ( var context = query.Execute() )
      {
        var data = context.LoadDataTable( 0, 1 );
        if ( data.Rows.Count > 0 )
          return data.Rows[0];

        else
          return null;
      }
    }

    /// <summary>
    /// 异步执行查询并返回首行
    /// </summary>
    /// <param name="query">要执行的查询对象</param>
    /// <param name="token">取消指示</param>
    /// <returns>查询结果</returns>
    public static async Task<DataRow> ExecuteFirstRowAsync( this IDbExecutable query, CancellationToken token = default( CancellationToken ) )
    {
      //UNDONE

      using ( var context = await query.ExecuteAsync( token ) )
      {
        var data = context.LoadDataTable( 0, 1 );
        if ( data.Rows.Count > 0 )
          return data.Rows[0];

        else
          return null;
      }
    }


    /// <summary>
    /// 异步查询结果集以IDataRecord的形式返回
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消标志</param>
    /// <returns>异步查询结果</returns>
    public static async IAsyncEnumerable<IDataRecord> ExecuteDataRecordsAsync( this IDbExecutable query, [EnumeratorCancellation] CancellationToken token = default )
    {

      var result = new List<IDataRecord>();

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
    /// 异步查询结果集以IDataRecord的形式返回
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <returns>异步查询结果</returns>
    public static IEnumerable<IDataRecord> ExecuteDataRecords( this IDbExecutable query )
    {

      var result = new List<IDataRecord>();

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
}