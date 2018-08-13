using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Ivony.Data.SqlDom.Expressions;

namespace Ivony.Data.SqlDom
{
  public class WhereClause
  {
    public WhereClause( SqlBooleanExpression condition )
    {
      Condition = condition;
    }

    public SqlBooleanExpression Condition { get; }

    public override string ToString()
    {
      return $"WHERE {Condition}";
    }

  }
}
