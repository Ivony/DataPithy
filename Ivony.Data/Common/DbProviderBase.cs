using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.Common
{


  public class DbProviderBase : IDbProvider
  {
    public virtual IDbTransactionContext CreateTransaction( DbContext context )
    {
      return null;
    }

    public virtual IDbExecutor GetDbExecutor( DbContext context )
    {
      return null;
    }
  }
}
