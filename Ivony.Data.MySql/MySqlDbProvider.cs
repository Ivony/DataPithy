using Ivony.Data.Common;
using Ivony.Data.MySqlClient;
using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  public class MySqlDbProvider : IDbProvider
  {

    public MySqlDbProvider( string connectionString )
    {
      ConnectionString = connectionString ?? throw new ArgumentNullException( nameof( connectionString ) );
    }

    public string ConnectionString { get; }



    public IDbTransactionContext CreateTransaction( DbContext context )
    {
      return new MySqlDbTransactionContext( ConnectionString );
    }

    public IDbExecutor GetDbExecutor( DbContext context )
    {
      return new MySqlDbExecutor( ConnectionString );
    }
  }
}
