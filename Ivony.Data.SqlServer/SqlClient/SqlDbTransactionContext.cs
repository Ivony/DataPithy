using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

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

    public IDbExecutor GetExecutor()
    {
      throw new NotImplementedException();
    }

    public void Rollback()
    {
      throw new NotImplementedException();
    }
  }
}
