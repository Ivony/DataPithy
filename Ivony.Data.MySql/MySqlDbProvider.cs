using Ivony.Data.Common;
using Ivony.Data.MySqlClient;
using Ivony.Data.Queries;
using MySql.Data.MySqlClient;
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


    public IDbTransactionContext CreateTransaction( DatabaseContext context )
    {
      return new MySqlDbTransactionContext( this );
    }

    public IDbExecutor GetDbExecutor( DatabaseContext context )
    {
      return new MySqlDbExecutor( ConnectionString );
    }

    public object GetService( Type serviceType )
    {
      if ( serviceType == typeof( IParameterizedQueryParser<MySqlCommand> ) )
        return new MySqlParameterizedQueryParser();

      return null;
    }
  }
}
