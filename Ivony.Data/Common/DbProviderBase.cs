using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.Common
{
  /// <summary>
  /// 提供 IDbExecutorProvider 的标准实现
  /// </summary>
  public abstract class DbProviderBase : IDbProvider
  {




    protected abstract IDbExecutor CreateExecutor( DbExecutorBuilder builder );

    public IDbExecutor GetDbExecutor( DbContext context )
    {
      return CreateExecutor( new DbExecutorBuilder() );
    }


    public IAsyncDbExecutor GetAsyncDbExecutor( DbContext context )
    {
      return null;
    }


    public IDbTransactionContext CreateTransaction( DbContext context )
    {
      return null;
    }

    public IAsyncDbTransactionContext CreateAsyncTransaction( DbContext context )
    {
      return null;
    }
  }
}
