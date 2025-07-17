using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data;
public static partial class ExecuteExtensions
{


  /// <summary>
  /// 执行查询并将第一个结果集填充动态对象列表
  /// </summary>
  /// <param name="query">要执行的查询</param>
  /// <returns>查询结果</returns>
  public static dynamic[] ExecuteDynamics( this IDbExecutable query )
  {
    var data = ExecuteDataTable( query );
    return data.ToDynamics();
  }

  /// <summary>
  /// 异步执行查询并将第一个结果集填充动态对象列表
  /// </summary>
  /// <param name="query">要执行的查询</param>
  /// <param name="token">取消指示</param>
  /// <returns>查询结果</returns>
  public static async Task<dynamic[]> ExecuteDynamicsAsync( this IDbExecutable query, CancellationToken token = default( CancellationToken ) )
  {
    var data = await ExecuteExtensions.ExecuteDataTableAsync( query, token );
    return data.ToDynamics();
  }



  /// <summary>
  /// 执行查询并将第一个结果集的第一条记录填充动态对象
  /// </summary>
  /// <param name="query">要执行的查询</param>
  /// <returns>查询结果</returns>
  public static dynamic ExecuteDynamicObject( this IDbExecutable query )
  {
    var dataItem = ExecuteFirstRow( query );
    return dataItem.ToDynamic();
  }


  /// <summary>
  /// 异步执行查询并将第一个结果集的第一条记录填充动态对象
  /// </summary>
  /// <param name="query">要执行的查询</param>
  /// <returns>查询结果</returns>
  public static async Task<dynamic> ExecuteDynamicObjectAsync( this IDbExecutable query )
  {
    var dataItem = await ExecuteFirstRowAsync( query );
    return dataItem.ToDynamic();
  }




}
