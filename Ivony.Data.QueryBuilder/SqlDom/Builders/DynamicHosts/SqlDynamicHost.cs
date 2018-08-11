using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;

using static System.Linq.Expressions.Expression;


namespace Ivony.Data.SqlDom.Builders.DynamicHosts
{
  public class SqlDynamicHost : DynamicObject
  {


    public SqlDynamicHost( object value )
    {
      Expression = GetExpression( value );
    }

    protected SqlDynamicHost( System.Linq.Expressions.Expression expression )
    {
      Expression = expression;
    }


    private Expression GetExpression( object value )
    {
      return (value as SqlDynamicHost)?.Expression ?? Constant( value );
    }

    public Expression Expression { get; }



    public override bool TryBinaryOperation( BinaryOperationBinder binder, object arg, out object result )
    {
      result = new SqlDynamicHost( MakeBinary( binder.Operation, Expression, GetExpression( arg ) ) );
      return true;
    }


    public override bool TryConvert( ConvertBinder binder, out object result )
    {
      if ( binder.ReturnType == typeof( System.Linq.Expressions.Expression ) )
        result = Expression;

      return base.TryConvert( binder, out result );
    }
  }
}
