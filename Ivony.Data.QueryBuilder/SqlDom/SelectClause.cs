using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Ivony.Data.SqlDom
{
  public class SelectClause
  {
    public SelectClause( List<SelectElement> elements )
    {
      Elements = new ReadOnlyCollection<SelectElement>( elements );
    }

    public IReadOnlyList<SelectElement> Elements { get; }


    public override string ToString()
    {
      return $"SELECT {string.Join( ", ", Elements )}";
    }

  }
}
