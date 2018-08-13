using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data.SqlDom.Expressions
{
  public class SqlParameterExpression : SqlValueExpression
  {

    public SqlParameterExpression( ParameterExpression parameter )
    {
      Parameter = parameter;
    }

    public ParameterExpression Parameter { get; }
  }
}
