using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom
{
  public class FromClause
  {
    public FromClause( FromSource fromSource )
    {
      Source = fromSource;
    }

    public FromSource Source { get; }



    public override string ToString()
    {
      return $"FROM {Source}";
    }

  }
}
