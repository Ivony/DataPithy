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

    protected override IDbExecutor CreateExecutor( DbExecutorBuilder builder )
    {
      builder.AddExecutor<ParameterizedQuery>( new SqlDbExecutor( ConnectionString, new SqlDbConfiguration() ) );
      return builder.BuildExecutor();
    }
  }
}
