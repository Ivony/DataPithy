using Ivony.Data;
using Ivony.Data.QueryBuilders;
using System;
using System.Dynamic;

namespace DynamicTest
{
  class Program
  {
    static void Main( string[] args )
    {
      Console.WriteLine( "Hello World!" );




      var builder = new SelectQueryBuilder();
      builder.Select( host => (host.Users.ID, host.Users.Username) );
      builder.Where( host => host.Users.Age > 30 & host.Users.ID != null );
      builder.From( host => host.Users );




      var query = builder.Build();

      Console.WriteLine( query );

      Console.ReadKey();

    }
  }
}
