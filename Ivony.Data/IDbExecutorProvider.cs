using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{

  /// <summary>
  /// IDbExecutor 提供程序
  /// </summary>
  public interface IDbExecutorProvider
  {

    /// <summary>
    /// 获取数据库查询执行器
    /// </summary>
    /// <param name="context">当前数据库访问上下文</param>
    /// <returns></returns>
    IDbExecutor GetDbExecutor( DbContext context );


    /// <summary>
    /// 获取异步数据库查询执行器
    /// </summary>
    /// <param name="context">当前数据库访问上下文</param>
    /// <returns></returns>
    IAsyncDbExecutor GetAsyncDbExecutor( DbContext context );

  }
}
