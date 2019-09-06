using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 这个接口指示该对象是一个可以执行的查询对象
  /// </summary>
  public interface IDbExecutable
  {

    /// <summary>
    /// 执行查询
    /// </summary>
    /// <returns>执行上下文</returns>
    [EditorBrowsable( EditorBrowsableState.Never )]
    IDbExecuteContext Execute();


    /// <summary>
    /// 异步执行查询
    /// </summary>
    /// <param name="token">取消标识</param>
    /// <returns>异步执行上下文</returns>
    [EditorBrowsable( EditorBrowsableState.Never )]
    Task<IAsyncDbExecuteContext> ExecuteAsync( CancellationToken token );

  }


  internal interface IDbQueryContainer
  {

    DbQuery Query { get; }

  }

}
