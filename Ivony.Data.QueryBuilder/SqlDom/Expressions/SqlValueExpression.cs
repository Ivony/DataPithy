using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data.SqlDom.Expressions
{
  public class SqlValueExpression : SqlExpression
  {



    internal static SqlValueExpression From( object value )
    {
      switch ( value )
      {
        case null:
          return new SqlConstantExpression( DBNull.Value );

        case SqlValueExpression expression:
          return expression;

        default:
          return new SqlConstantExpression( value );
      }
    }



    public static SqlArithmeticalExpression operator +( SqlValueExpression left, SqlValueExpression right )
      => new SqlArithmeticalExpression( ExpressionType.Add, left, right ?? new SqlConstantExpression( null ) );

    public static SqlArithmeticalExpression operator +( SqlValueExpression left, object right )
      => new SqlArithmeticalExpression( ExpressionType.Add, left, new SqlConstantExpression( right ) );


    public static SqlArithmeticalExpression operator -( SqlValueExpression left, SqlValueExpression right )
      => new SqlArithmeticalExpression( ExpressionType.Subtract, left, right ?? new SqlConstantExpression( null ) );

    public static SqlArithmeticalExpression operator -( SqlValueExpression left, object right )
      => new SqlArithmeticalExpression( ExpressionType.Subtract, left, new SqlConstantExpression( right ) );


    public static SqlArithmeticalExpression operator *( SqlValueExpression left, SqlValueExpression right )
      => new SqlArithmeticalExpression( ExpressionType.Multiply, left, right ?? new SqlConstantExpression( null ) );

    public static SqlArithmeticalExpression operator *( SqlValueExpression left, object right )
      => new SqlArithmeticalExpression( ExpressionType.Multiply, left, new SqlConstantExpression( right ) );


    public static SqlArithmeticalExpression operator /( SqlValueExpression left, SqlValueExpression right )
      => new SqlArithmeticalExpression( ExpressionType.Divide, left, right ?? new SqlConstantExpression( null ) );

    public static SqlArithmeticalExpression operator /( SqlValueExpression left, object right )
      => new SqlArithmeticalExpression( ExpressionType.Divide, left, new SqlConstantExpression( right ) );


    public static SqlComparisonExpression operator >( SqlValueExpression left, SqlValueExpression right )
      => new SqlComparisonExpression( ExpressionType.GreaterThan, left, right ?? new SqlConstantExpression( null ) );

    public static SqlComparisonExpression operator >( SqlValueExpression left, object right )
      => new SqlComparisonExpression( ExpressionType.GreaterThan, left, new SqlConstantExpression( right ) );


    public static SqlComparisonExpression operator <( SqlValueExpression left, SqlValueExpression right )
      => new SqlComparisonExpression( ExpressionType.LessThan, left, right ?? new SqlConstantExpression( null ) );

    public static SqlComparisonExpression operator <( SqlValueExpression left, object right )
      => new SqlComparisonExpression( ExpressionType.LessThan, left, new SqlConstantExpression( right ) );


    public static SqlComparisonExpression operator >=( SqlValueExpression left, SqlValueExpression right )
      => new SqlComparisonExpression( ExpressionType.GreaterThanOrEqual, left, right ?? new SqlConstantExpression( null ) );

    public static SqlComparisonExpression operator >=( SqlValueExpression left, object right )
      => new SqlComparisonExpression( ExpressionType.GreaterThanOrEqual, left, new SqlConstantExpression( right ) );


    public static SqlComparisonExpression operator <=( SqlValueExpression left, SqlValueExpression right )
      => new SqlComparisonExpression( ExpressionType.LessThanOrEqual, left, right ?? new SqlConstantExpression( null ) );

    public static SqlComparisonExpression operator <=( SqlValueExpression left, object right )
      => new SqlComparisonExpression( ExpressionType.LessThanOrEqual, left, new SqlConstantExpression( right ) );


    public static SqlComparisonExpression operator ==( SqlValueExpression left, SqlValueExpression right )
      => new SqlComparisonExpression( ExpressionType.Equal, left, right ?? new SqlConstantExpression( null ) );

    public static SqlComparisonExpression operator ==( SqlValueExpression left, object right )
      => new SqlComparisonExpression( ExpressionType.Equal, left, new SqlConstantExpression( right ) );


    public static SqlComparisonExpression operator !=( SqlValueExpression left, SqlValueExpression right )
      => new SqlComparisonExpression( ExpressionType.NotEqual, left, right ?? new SqlConstantExpression( null ) );

    public static SqlComparisonExpression operator !=( SqlValueExpression left, object right )
      => new SqlComparisonExpression( ExpressionType.NotEqual, left, new SqlConstantExpression( right ) );
  }
}
