using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Ivony.Data
{
  public class DbTransactionService
  {

    private AsyncLocal<IDbTransactionContext> _transaction = new AsyncLocal<IDbTransactionContext>();

    public IDbTransactionContext Transaction => _transaction.Value;


    public void EnterTransaction( IDbTransactionContext transaction )
    {
      _transaction.Value = transaction;
    }

    public void ExitTransaction( IDbTransactionContext transaction )
    {
      _transaction.Value = null;
    }


  }
}
