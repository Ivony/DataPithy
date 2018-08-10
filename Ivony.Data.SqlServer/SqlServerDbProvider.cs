using Ivony.Data.Common;
using Ivony.Data.Queries;
using Ivony.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  public class SqlServerDbProvider : DbProviderBase
  {
    public SqlServerDbProvider( string connectionString )
    {
      ConnectionString = connectionString ?? throw new ArgumentNullException( nameof( connectionString ) );
    }

    public string ConnectionString { get; private set; }


    public override IDbExecutor GetDbExecutor( DbContext context )
    {
      return new SqlDbExecutor( ConnectionString, new SqlDbConfiguration() );
    }

    public override IAsyncDbExecutor GetAsyncDbExecutor( DbContext context )
    {
      return new SqlDbExecutor( ConnectionString, new SqlDbConfiguration() );
    }
  }
}
