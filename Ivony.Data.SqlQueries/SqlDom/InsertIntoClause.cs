using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom
{
  public sealed class InsertIntoClause
  {

    public InsertIntoClause( TableReference table )
    {
      Table = table;
    }

    public TableReference Table { get; }

    public override string ToString()
    {
      return $"INSERT INTO {Table}";
    }
  }
}
