using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Data
{

  /// <summary>
  /// 定义 IDbExecutor 的提供程序
  /// </summary>
  public interface IDbProvider
  {

    /// <summary>
    /// 获取指定类型查询的执行器
    /// </summary>
    /// <typeparam name="T">要执行的查询类型</typeparam>
    /// <returns>数据库查询执行器</returns>
    IDbExecutor<T> GetDbExecutor<T>( T query ) where T : IDbQuery;

    /// <summary>
    /// 获取指定类型查询的异步执行器
    /// </summary>
    /// <typeparam name="T">要执行的查询类型</typeparam>
    /// <returns>异步查询执行器</returns>
    IAsyncDbExecutor<T> GetAsyncDbExecutor<T>( T query ) where T : IDbQuery;

  }
}
