using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data.SqlDom.Expressions
{
  public class SqlExpression : Expression
  {

    public override ExpressionType NodeType => ExpressionType.Extension;

    public override Type Type => typeof( object );

  }
}
