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

      var builder = new SqlSelectQueryBuilder()
        .Select( host => (host.U.ID, host.U.Username, host.P.Email) )
        .Where( host => host.U.Age > 30 | host.P.Email.Like( "%@163.com" ) )
        .Where( host => host.U.ID != null )
        .From( host => host.Users.As( "U" ).InnerJoin( host.UserProfile.As( "P" ) ).On( host.Users.ID == host.UserProfile.ID ) );




      var sqlQuery = builder.Build();

      var query = new SqlQueryParser().ParseSelectQuery( sqlQuery );
      Console.WriteLine( query );

      {
        Console.WriteLine();
        Console.WriteLine();


        var command = new SqlParameterizedQueryParser().Parse( query );

        Console.WriteLine( command.CommandText );
      }

      {

        Console.WriteLine();
        Console.WriteLine();


        var command = new MySqlParameterizedQueryParser().Parse( query );

        Console.WriteLine( command.CommandText );
      }





      Console.ReadKey();

    }
  }
}
