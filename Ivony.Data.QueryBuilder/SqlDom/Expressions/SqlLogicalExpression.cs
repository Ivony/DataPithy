using System;
using System.Linq.Expressions;

namespace Ivony.Data.SqlDom.Expressions
{
  public class SqlLogicalExpression : SqlBooleanExpression
  {

    internal SqlLogicalExpression( ExpressionType operation, SqlExpression left, SqlExpression right )
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
        case ExpressionType.AndAlso:
          op = "AND";
          break;
        case ExpressionType.OrElse:
          op = "OR";
          break;
        default:
          throw new InvalidOperationException();
      }

      return $"({Left} {op} {Right})";
    }
  }
}