using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.Common
{
  public class DbProviderBase : IDbProvider
  {
    public virtual IAsyncDbTransactionContext CreateAsyncTransaction( DbContext context )
    {
      return CreateTransaction(context) as IAsyncDbTransactionContext;
    }

    public virtual IDbTransactionContext CreateTransaction( DbContext context )
    {
      return null;
    }

    public virtual IAsyncDbExecutor GetAsyncDbExecutor( DbContext context )
    {
      return GetDbExecutor( context ) as IAsyncDbExecutor;
    }

    public virtual IDbExecutor GetDbExecutor( DbContext context )
    {
      return null;
    }
  }
}
