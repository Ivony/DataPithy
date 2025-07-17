using System;
using Microsoft.Data.SqlClient;

using Ivony.Data.Common;

namespace Ivony.Data.SqlClient;
internal class SqlServerServiceProvider( SqlServerDb db, IServiceProvider serviceProvider ) : FallbackServiceProvider( serviceProvider )
{

  private static readonly SqlParameterizedQueryParser parser = new();
  private static readonly SqlServerConnectionFactory factory = new();

  protected override object? FallbackGetService( IServiceProvider serviceProvider, Type serviceType )
  {
    if ( serviceType == typeof( IParameterizedQueryParser<SqlCommand> ) )
      return parser;

    else if ( serviceType == typeof( IDbConnectionFactory<SqlConnection> ) )
      return factory;

    else if ( serviceType == typeof( IDatabase ) )
      return db;

    else if ( serviceType == typeof( SqlServerDb ) )
      return db;

    else
      return null;
  }
}