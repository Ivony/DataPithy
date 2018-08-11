using Ivony.Data.SqlDom.Builders.DynamicHosts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data.SqlDom.Builders
{
  public class SqlExpressionBuilder
  {


    public static TableReference Table( string tableName )
    {
      return new TableReference( tableName );
    }


    public static FieldReference Field( string tableName, string fieldName )
    {
      return new FieldReference( tableName, fieldName );
    }

    public static Expression Dynamic( Func<dynamic, SqlDynamicHost> builder )
    {
      return builder( new DatabaseDynamicHost() ).Expression;
    }




  }
}
