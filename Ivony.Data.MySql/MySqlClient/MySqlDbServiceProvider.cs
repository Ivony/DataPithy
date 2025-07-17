using System;

using Ivony.Data.Common;

#if MySqlConnector
using MySqlConnector;
#else
using MySql.Data.MySqlClient;
#endif

namespace Ivony.Data.MySqlClient;
internal class MySqlDbServiceProvider( MySqlDb db, IServiceProvider serviceProvider ) : FallbackServiceProvider( serviceProvider )
{
  MySqlParameterizedQueryParser parser = new MySqlParameterizedQueryParser();
  MySqlConnectionFactory factory = new MySqlConnectionFactory();


  protected override object? FallbackGetService( IServiceProvider serviceProvider, Type serviceType )
  {
    if ( serviceType == typeof( IParameterizedQueryParser<MySqlCommand> ) )
      return parser;

    else if ( serviceType == typeof( IDbConnectionFactory<MySqlConnection> ) )
      return factory;

    else if ( serviceType == typeof( IDatabase ) )
      return db;

    else if ( serviceType == typeof( MySqlDb ) )
      return db;

    else
      return null;
  }
}
