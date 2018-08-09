using Ivony.Data.Common;
using Ivony.Data.MySqlClient;
using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  public class MySqlDbProvider : DbProviderBase
  {

    public MySqlDbProvider( string connectionString )
    {
      ConnectionString = connectionString ?? throw new ArgumentNullException( nameof( connectionString ) );
    }

    public string ConnectionString { get; }

    protected override IDbExecutor CreateExecutor( DbExecutorBuilder builder )
    {
      builder.AddExecutor<ParameterizedQuery>( new MySqlDbExecutor( ConnectionString, new MySqlDbConfiguration() ) );
      return builder.BuildExecutor();
    }
  }
}
