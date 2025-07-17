#if MySqlConnector
using MySqlConnector;
#else
using MySql.Data.MySqlClient;
#endif

namespace Ivony.Data.MySqlClient;
internal class MySqlConnectionFactory : IDbConnectionFactory<MySqlConnection>
{
  public MySqlConnection CreateConnection( string connectionString )
  {
    var connection = new MySqlConnection( connectionString );
    connection.Open();
    return connection;
  }
}
