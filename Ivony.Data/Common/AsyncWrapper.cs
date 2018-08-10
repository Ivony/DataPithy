using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{
  internal class AsyncExecutorWrapper : IAsyncDbExecutor
  {
    private IDbExecutor executor;

    public AsyncExecutorWrapper( IDbExecutor executor )
    {
      this.executor = executor;
    }

    IDbExecuteContext IDbExecutor.Execute( IDbQuery query )
    {
      return executor.Execute( query );
    }

    Task<IAsyncDbExecuteContext> IAsyncDbExecutor.ExecuteAsync( IDbQuery query, CancellationToken token )
    {
      throw new NotSupportedException();
    }
  }
}
