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
    public RollbackImmediatelyException() : this( null ) { }

    /// <summary>
    /// 创建 <see cref="RollbackImmediatelyException"/> 对象
    /// </summary>
    public RollbackImmediatelyException( object result ) : base( "Transaction will be rollback, don't catch this exception." )
    {
      Result = result;
    }


    /// <summary>
    /// 结果，当使用带返回值的 <see cref="Db.Transaction{T}( Func{T} )"/> 或 <see cref="Db.AsyncTransaction{T}( Func{ System.Threading.Tasks.Task {T} })"/> 方法
    /// </summary>
    public object Result { get; }
  }
}
