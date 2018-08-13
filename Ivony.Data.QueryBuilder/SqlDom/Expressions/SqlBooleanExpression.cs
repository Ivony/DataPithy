using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data.SqlDom.Expressions
{
  public class SqlBooleanExpression : SqlExpression
  {
    public override ExpressionType NodeType => ExpressionType.Extension;

    public override Type Type => typeof( bool );


    public static SqlBooleanExpression operator &( SqlBooleanExpression left, SqlBooleanExpression right )
    {
      if ( left == null )
        return right;

      else if ( right == null )
        return left;

      else if ( left == null && right == null )
        return null;

      return new SqlLogicalExpression( ExpressionType.AndAlso, left, right );
    }

    public static SqlBooleanExpression operator |( SqlBooleanExpression left, SqlBooleanExpression right )
    {
      if ( left == null )
        return right;

      else if ( right == null )
        return left;

      else if ( left == null && right == null )
        return null;

      return new SqlLogicalExpression( ExpressionType.OrElse, left, right );
    }
  }
}
