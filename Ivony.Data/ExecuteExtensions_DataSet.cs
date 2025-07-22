using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace Ivony.Data
{
  /// <summary>
  /// 为系统的 DataSet 和 DataTable 对象提供扩展方法
  /// </summary>
  public static partial class ExecuteExtensions
  {






    /// <summary>
    /// 执行查询并返回第一列数据
    /// </summary>
    /// <typeparam name="T">列类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <returns>第一列的数据</returns>
    public static T[] ExecuteFirstColumn<T>( this IDbExecutable query )
    {
      return ExecuteDataTable( query ).Column<T>();
    }

    /// <summary>
    /// 异步执行查询并返回第一列数据
    /// </summary>
    /// <typeparam name="T">列类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <returns>第一列的数据</returns>
    public async static Task<T[]> ExecuteFirstColumnAsync<T>( this IDbExecutable query )
    {
      return (await ExecuteDataTableAsync( query )).Column<T>();
    }


    /// <summary>
    /// 执行查询并将数据转换为 DataRowView 集合返回
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <returns>转换为 DataRowView 的数据集合</returns>
    public static IEnumerable<DataRowView> ExecuteDataRowViews( this IDbExecutable query )
    {
      return ExecuteDataTable( query ).GetRowViews();
    }


    /// <summary>
    /// 异步执行查询并将数据转换为 DataRowView 集合返回
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <returns>转换为 DataRowView 的数据集合</returns>
    public async static Task<IEnumerable<DataRowView>> ExecuteDataRowViewsAsync( this IDbExecutable query )
    {
      return (await ExecuteDataTableAsync( query )).GetRowViews();
    }


    /// <summary>
    /// 执行查询并将第一行数据数据转换为 DataRowView 返回
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <returns>转换为 DataRowView 的数据集合</returns>
    public static DataRowView ExecuteFirstDataRowView( this IDbExecutable query )
    {
      return ExecuteDataTable( query ).GetRowViews().FirstOrDefault();
    }

    /// <summary>
    /// 异步执行查询并将第一行数据数据转换为 DataRowView 返回
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <returns>转换为 DataRowView 的数据集合</returns>
    public async static Task<DataRowView> ExecuteFirstDataRowViewAsync( this IDbExecutable query )
    {
      return (await ExecuteDataTableAsync( query )).GetRowViews().FirstOrDefault();
    }

  }
}
