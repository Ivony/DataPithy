using Ivony.Data;
using Ivony.Data.MySqlClient;
using Ivony.Data.QueryBuilders;
using Ivony.Data.SqlClient;
using System;
using System.Dynamic;

namespace DynamicTest
{
  class Program
  {
    static void Main( string[] args )
    {

      Db.Initialize( configure =>
      {
        configure.UseMySql( "192.168.10.163", "Test", "robot", "mvxy8Bsamc2MkdW" );
      } );

      var builder = new SqlSelectQueryBuilder()
        .Select( host => (host.Users.ID, host.Users.Username, host.UserProfie.Email) )
        .Where( host => host.Users.Age > 30 & host.Users.ID != null )
        .From( host => host.Users.InnerJoin( host.UserProfile, host.Users.ID == host.UserProfile.ID ) );




      var sqlQuery = builder.Build();

      var query = new SqlQueryParser().ParseSelectQuery( sqlQuery );
      Console.WriteLine( query );

      Console.WriteLine();
      Console.WriteLine();


      var command = new SqlParameterizedQueryParser().Parse( query );

      Console.WriteLine( command.CommandText );






      Console.ReadKey();

    }
  }
}
