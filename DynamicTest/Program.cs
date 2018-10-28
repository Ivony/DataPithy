using Ivony.Data;
using Ivony.Data.MySqlClient;
using Ivony.Data.SqlClient;
using System;
using System.Dynamic;
using Ivony.Data.SqlQueries;
using Ivony.Data.SqlQueries.SqlDom;



using static Ivony.Data.SqlQueries.DbDynamicHost;


namespace DynamicTest
{

  class Program
  {

    static void Main( string[] args )
    {

      var select = new SelectQueryBuilder()
        .Select( Tables.U.ID, Tables.U.Username, Tables.P.Email )
        .Where( Tables.U.Age > 30 | Tables.P.Email.Like( "%@163.com" ) )
        .Where( Tables.U.ID != null )
        .From( Tables.Users.As( "U" ).InnerJoin( Tables.UserProfile.As( "P" ) ).On( Tables.U.ID == Tables.P.ID ) )
        .Build();

      var insert = new InsertQueryBuilder()
        .InsertInto( Tables.Users, Names.ID, Names.Username, Names.Email )
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
