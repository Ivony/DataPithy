using Ivony.Data;
using Ivony.Data.MySqlClient;
using Ivony.Data.SqlClient;
using System;
using System.Dynamic;
using Ivony.Data.SqlQueries;
using Ivony.Data.SqlQueries.SqlDom;

namespace DynamicTest
{
  class Program
  {
    static void Main( string[] args )
    {

      var select = new SelectQueryBuilder()
        .Select( host => (host.U.ID, host.U.Username, host.P.Email) )
        .Where( host => host.U.Age > 30 | host.P.Email.Like( "%@163.com" ) )
        .Where( host => host.U.ID != null )
        .From( host => host.Users.As( "U" ).InnerJoin( host.UserProfile.As( "P" ) ).On( host.Users.ID == host.UserProfile.ID ) )
        .Build();

      var insert = new InsertQueryBuilder()
        .InsertInto( host => host.Users, host => (host.ID, host.Username, host.Email) )
        .WithValues( ValuesClause.Values( (1, "Ivony", "Ivony@live.com") ) )
        .Build();


      var parser = new SqlQueryParser();

      ShowQuery( parser.ParseSelectQuery( select ) );
      ShowQuery( parser.ParseInsertQuery( insert ) );

      Console.ReadKey();

    }

    private static void ShowQuery( Ivony.Data.Queries.ParameterizedQuery query )
    {
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

      Console.WriteLine();
      Console.WriteLine();
      Console.WriteLine();
      Console.WriteLine();

    }
  }
}
