using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlDom.Expressions
{
  public class SqlFieldExpression : SqlValueExpression
  {

    internal SqlFieldExpression( FieldReference field )
    {
      Field = field;
    }

    public FieldReference Field { get; }
  }
}
