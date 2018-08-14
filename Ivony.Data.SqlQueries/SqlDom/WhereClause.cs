using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Ivony.Data.SqlQueries.SqlDom.Expressions;

namespace Ivony.Data.SqlQueries.SqlDom
{
  public sealed class WhereClause
  {
    internal WhereClause( SqlBooleanExpression condition )
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
