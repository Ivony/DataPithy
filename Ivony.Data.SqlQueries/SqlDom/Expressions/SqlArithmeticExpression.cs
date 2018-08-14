using System;
using System.Linq.Expressions;

namespace Ivony.Data.SqlQueries.SqlDom.Expressions
{
  public class SqlArithmeticalExpression : SqlValueExpression
  {

    internal SqlArithmeticalExpression( ExpressionType operation, SqlExpression left, SqlExpression right )
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
        case ExpressionType.Add:
          op = "+";
          break;
        case ExpressionType.Subtract:
          op = "-";
          break;
        case ExpressionType.Multiply:
          op = "*";
          break;
        case ExpressionType.Divide:
          op = "/";
          break;
        default:
          throw new InvalidOperationException();
      }

      return $"({Left} {op} {Right})";

    }
  }
}