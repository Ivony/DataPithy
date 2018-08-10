using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Ivony.Data.Common;

namespace Ivony.Data.SqlClient
{
  public class SqlDbTransactionContext : IDbTransactionContext
  {


    public SqlDbTransactionContext( string connectionString )
    {
      Connection = new SqlConnection( connectionString );
    }

    public SqlConnection Connection { get; }
    public SqlTransaction Transaction { get; private set; }



    public TransactionStatus Status { get; private set; } = TransactionStatus.NotBeginning;

    TransactionStatus IDbTransactionContext.Status => throw new NotImplementedException();

    public void BeginTransaction()
    {
      Transaction = Connection.BeginTransaction();
    }

    public void Commit()
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }

    public void Rollback()
    {
      throw new NotImplementedException();
    }


    public IDbExecutor GetDbExecutor()
    {
      throw new NotImplementedException();
    }

    public IDbExecutor GetDbExecutor( DbContext context )
    {
      throw new NotImplementedException();
    }

    public IDbTransactionContext CreateTransaction( DbContext context )
    {

      throw new InvalidOperationException();
    }


    void IDisposableObjectContianer.RegisterDispose( Action disposeMethod )
    {
      throw new NotImplementedException();
    }

  }
}
