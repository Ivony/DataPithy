using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom.Expressions
{
  public sealed class SqlLikeExpression : SqlBooleanExpression
  {
    internal SqlLikeExpression( SqlValueExpression left, SqlValueExpression right )
    {
      Left = left;
      Right = right;
    }

    public SqlValueExpression Left { get; }
    public SqlValueExpression Right { get; }
  }
}
