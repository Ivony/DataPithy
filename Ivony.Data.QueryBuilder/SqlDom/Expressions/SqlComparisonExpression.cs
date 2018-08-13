using System;
using System.Linq.Expressions;

namespace Ivony.Data.SqlDom.Expressions
{
  public class SqlComparisonExpression : SqlBooleanExpression
  {

    internal SqlComparisonExpression( ExpressionType operation, SqlExpression left, SqlExpression right )
    {
      Operation = operation;
      Left = left;
      Right = right;
    }

    public ExpressionType Operation { get; }
    public SqlExpression Left { get; }
    public SqlExpression Right { get; }


    public override string ToString()
    {
      string op;

      switch ( Operation )
      {
        case ExpressionType.LessThan:
          op = "<";
          break;
        case ExpressionType.LessThanOrEqual:
          op = "<=";
          break;
        case ExpressionType.GreaterThan:
          op = ">";
          break;
        case ExpressionType.GreaterThanOrEqual:
          op = ">=";
          break;
        case ExpressionType.Equal:
          op = "=";
          break;
        case ExpressionType.NotEqual:
          op = "<>";
          break;
        default:
          throw new InvalidOperationException();
      }

      return $"({Left} {op} {Right})";
    }
  }
}
