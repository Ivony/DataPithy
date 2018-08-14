using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom
{
  public class OrderByItem
  {

    public string FieldAlias { get; }

    public OrderingType Ordering { get; }


    public override string ToString()
    {
      if ( Ordering == OrderingType.Descending )
        return $"{FieldAlias} DESC";

      else
        return FieldAlias;


    }

  }
}
