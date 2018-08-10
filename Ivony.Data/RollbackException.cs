using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{

  /// <summary>
  /// 用于回滚数据库事务的异常对象
  /// </summary>
  public class RollbackException : Exception
  {
    /// <summary>
    /// 创建 RollbackException 对象
    /// </summary>
    public RollbackException() : base( "Transaction will be rollback, don't catch this exception." )
    {
    }
  }
}
