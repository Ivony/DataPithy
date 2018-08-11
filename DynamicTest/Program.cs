using Ivony.Data;
using Ivony.Data.SqlDom.Builders;
using System;
using System.Dynamic;

namespace DynamicTest
{
  class Program
  {
    static void Main( string[] args )
    {
      Console.WriteLine( "Hello World!" );



      var expression = SqlExpressionBuilder.Dynamic( host => host.Users.Age > 30 );
      


      Console.ReadKey();

    }
  }
}
