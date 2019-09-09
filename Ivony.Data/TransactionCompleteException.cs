using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{

  /// <summary>
  /// 定义事务完成时出现的异常
  /// </summary>
  public class TransactionCompleteException : Exception
  {

    /// <summary>
    /// 创建 <see cref="TransactionCompleteException"/> 实例
    /// </summary>
    /// <param name="message">异常消息</param>
    /// <param name="innerException">具体的异常信息</param>
    public TransactionCompleteException( string message, Exception innerException ) : base( message, innerException ) { }

  }
}
