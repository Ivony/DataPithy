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
    public RollbackException() : base( "Transaction will be rollback, don't catch this exception." )
    {
    }
  }
}
