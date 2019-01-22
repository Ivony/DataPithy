using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom.Expressions
{
#pragma warning disable CS0660 // 类型定义运算符 == 或运算符 !=，但不重写 Object.Equals(object o)
#pragma warning disable CS0661 // 类型定义运算符 == 或运算符 !=，但不重写 Object.GetHashCode()
  public class SqlValueExpression : SqlExpression
  {



    internal static SqlValueExpression From( object value )
    {
      switch ( value )
      {
        case null:
          return Constant( System.DBNull.Value );

        case SqlValueExpression expression:
          return expression;

        default:
          return Constant( value );
      }
    }


    public static new SqlValueExpression Constant( object value )
    {
      if ( value == null || value is DBNull )
        return DBNull();

      else
        return new SqlConstantExpression( value );
    }

    public static SqlDbNullExpression DBNull()
    {
      return new SqlDbNullExpression();
    }



    public SqlLikeExpression Like( string value )
    {
      return new SqlLikeExpression( this, Constant( value ) );
    }


    public SqlLikeExpression Like( SqlValueExpression value )
    {
      return new SqlLikeExpression( this, value );
    }



    public static SqlArithmeticalExpression operator +( SqlValueExpression left, SqlValueExpression right )
      => new SqlArithmeticalExpression( ExpressionType.Add, left, right ?? Constant( null ) );

    public static SqlArithmeticalExpression operator +( SqlValueExpression left, object right )
      => new SqlArithmeticalExpression( ExpressionType.Add, left, Constant( right ) );


    public static SqlArithmeticalExpression operator -( SqlValueExpression left, SqlValueExpression right )
      => new SqlArithmeticalExpression( ExpressionType.Subtract, left, right ?? Constant( null ) );

    public static SqlArithmeticalExpression operator -( SqlValueExpression left, object right )
      => new SqlArithmeticalExpression( ExpressionType.Subtract, left, Constant( right ) );


    public static SqlArithmeticalExpression operator *( SqlValueExpression left, SqlValueExpression right )
      => new SqlArithmeticalExpression( ExpressionType.Multiply, left, right ?? Constant( null ) );

    public static SqlArithmeticalExpression operator *( SqlValueExpression left, object right )
      => new SqlArithmeticalExpression( ExpressionType.Multiply, left, Constant( right ) );


    public static SqlArithmeticalExpression operator /( SqlValueExpression left, SqlValueExpression right )
      => new SqlArithmeticalExpression( ExpressionType.Divide, left, right ?? Constant( null ) );

    public static SqlArithmeticalExpression operator /( SqlValueExpression left, object right )
      => new SqlArithmeticalExpression( ExpressionType.Divide, left, Constant( right ) );


    public static SqlComparisonExpression operator >( SqlValueExpression left, SqlValueExpression right )
      => new SqlComparisonExpression( ExpressionType.GreaterThan, left, right ?? Constant( null ) );

    public static SqlComparisonExpression operator >( SqlValueExpression left, object right )
      => new SqlComparisonExpression( ExpressionType.GreaterThan, left, Constant( right ) );


    public static SqlComparisonExpression operator <( SqlValueExpression left, SqlValueExpression right )
      => new SqlComparisonExpression( ExpressionType.LessThan, left, right ?? Constant( null ) );

    public static SqlComparisonExpression operator <( SqlValueExpression left, object right )
      => new SqlComparisonExpression( ExpressionType.LessThan, left, Constant( right ) );


    public static SqlComparisonExpression operator >=( SqlValueExpression left, SqlValueExpression right )
      => new SqlComparisonExpression( ExpressionType.GreaterThanOrEqual, left, right ?? Constant( null ) );

    public static SqlComparisonExpression operator >=( SqlValueExpression left, object right )
      => new SqlComparisonExpression( ExpressionType.GreaterThanOrEqual, left, Constant( right ) );


    public static SqlComparisonExpression operator <=( SqlValueExpression left, SqlValueExpression right )
      => new SqlComparisonExpression( ExpressionType.LessThanOrEqual, left, right ?? Constant( null ) );

    public static SqlComparisonExpression operator <=( SqlValueExpression left, object right )
      => new SqlComparisonExpression( ExpressionType.LessThanOrEqual, left, Constant( right ) );


    public static SqlComparisonExpression operator ==( SqlValueExpression left, SqlValueExpression right )
      => new SqlComparisonExpression( ExpressionType.Equal, left, right ?? Constant( null ) );

    public static SqlComparisonExpression operator ==( SqlValueExpression left, object right )
      => new SqlComparisonExpression( ExpressionType.Equal, left, Constant( right ) );


    public static SqlComparisonExpression operator !=( SqlValueExpression left, SqlValueExpression right )
      => new SqlComparisonExpression( ExpressionType.NotEqual, left, right ?? Constant( null ) );

    public static SqlComparisonExpression operator !=( SqlValueExpression left, object right )
      => new SqlComparisonExpression( ExpressionType.NotEqual, left, Constant( right ) );
  }
}
