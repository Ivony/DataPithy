using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Ivony.Data.Common;

namespace Ivony.Data.SqlClient
{
  public class SqlDbTransactionContext : DbTransactionContextBase<SqlTransaction>
  {
    internal SqlDbTransactionContext( string connectionString )
    {
      Connection = new SqlConnection( connectionString );
    }

    public SqlConnection Connection { get; }

    protected override SqlTransaction BeginTransactionCore()
    {
      return Connection.BeginTransaction();
    }

    protected override IDbExecutor GetDbExecutorCore( DbContext context )
    {
      return new SqlDbExecutor( this );
    }


  }
}
