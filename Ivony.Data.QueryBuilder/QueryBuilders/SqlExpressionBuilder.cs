using Ivony.Data.QueryBuilders;
using Ivony.Data.SqlDom;
using Ivony.Data.SqlDom.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data.QueryBuilders
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

  }
}
