using Ivony.Data.SqlDom.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data.SqlDom
{


  public sealed class SelectElement
  {

    public SelectElement( SqlValueExpression expression, string alias )
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
