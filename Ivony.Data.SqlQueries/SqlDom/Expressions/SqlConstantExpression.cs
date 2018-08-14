using System;
using System.Linq.Expressions;

namespace Ivony.Data.SqlQueries.SqlDom.Expressions
{
  public sealed class SqlConstantExpression : SqlValueExpression
  {

    internal SqlConstantExpression( object value )
    {
      Value = value ?? System.DBNull.Value;
    }

    public object Value { get; }

    public override string ToString()
    {
      if ( Value is DBNull )
        return "<null>";

      else
        return Value.ToString();
    }
  }
}