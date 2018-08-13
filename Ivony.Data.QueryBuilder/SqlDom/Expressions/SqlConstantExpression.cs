using System;
using System.Linq.Expressions;

namespace Ivony.Data.SqlDom.Expressions
{
  public class SqlConstantExpression : SqlValueExpression
  {

    internal SqlConstantExpression( object value )
    {
      Value = value ?? DBNull.Value;
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