using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom.Expressions
{
  public class SqlDbNullExpression : SqlValueExpression
  {

    public override string ToString()
    {
      return "NULL";
    }

  }
}
