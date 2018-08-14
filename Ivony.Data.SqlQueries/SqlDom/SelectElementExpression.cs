using Ivony.Data.SqlQueries.SqlDom.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom
{


  public sealed class SelectElementExpression : SelectElement
  {

    public SelectElementExpression( SqlValueExpression expression, string alias )
    {
      Alias = alias ?? throw new ArgumentNullException( nameof( alias ) );
      Expression = expression ?? throw new ArgumentNullException( nameof( expression ) );
    }

    public string Alias { get; }

    public SqlValueExpression Expression { get; }


    public override string ToString()
    {
      return $"{Expression} AS {Alias}";
    }

  }
}
