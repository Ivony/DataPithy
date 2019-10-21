using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  /// <summary>
  /// 定义数据库异常筛选器
  /// </summary>
  public interface IDbExceptionFilter
  {

    /// <summary>
    /// 当查询出现异常时调用此方法
    /// </summary>
    /// <param name="exception">异常信息</param>
    /// <param name="query">数据库查询对象</param>
    void OnQueryException( Exception exception, DbQuery query );

  }
}
