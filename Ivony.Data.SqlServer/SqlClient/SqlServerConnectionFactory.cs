using Microsoft.Data.SqlClient;

namespace Ivony.Data.SqlClient;

internal class SqlServerConnectionFactory : IDbConnectionFactory<SqlConnection>
{
  public SqlConnection CreateConnection( string connectionString ) => new SqlConnection( connectionString );

  public void ReleaseConnection( SqlConnection connection ) => connection.Dispose();
}