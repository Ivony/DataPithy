using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data.SqlDom.Builders
{
  public class SelectQueryBuilder
  {


    private Dictionary<string, Expression> elements = new Dictionary<string, Expression>();

    public void AddElement( string alias, Expression expression )
    {
      elements.Add( alias, expression );
    }



  }
}
