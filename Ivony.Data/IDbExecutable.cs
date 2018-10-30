using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
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

    IDbExecuteContext Execute();


    Task<IAsyncDbExecuteContext> ExecuteAsync( CancellationToken token );

  }


  internal interface IDbQueryContainer
  {

    DbQuery Query { get; }

  }

}
