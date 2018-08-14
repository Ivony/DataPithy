using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom
{
  public class SelectElementStar : SelectElement
  {


    internal SelectElementStar()
    {

    }

    internal SelectElementStar( string tableAlias )
    {
      TableAlias = tableAlias;
    }

    public string TableAlias { get; }
  }
}
