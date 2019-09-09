using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{

  /// <summary>
  /// 用于立即回滚数据库事务的异常对象
  /// </summary>
  internal class RollbackImmediatelyException : Exception
  {
    /// <summary>
    /// 创建 <see cref="RollbackImmediatelyException"/> 对象
    /// </summary>
    public RollbackImmediatelyException() : base( "Transaction will be rollback, don't catch this exception." )
    {
    }
  }
}
