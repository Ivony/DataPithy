using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom
{
  public class InsertQuery
  {

    public InsertQuery( InsertIntoClause into, InsertColumns columns, InsertValuesSource values )
    {
      Into = into;
      Columns = columns;
      Values = values;
    }

    public InsertIntoClause Into { get; }
    public InsertColumns Columns { get; }
    public InsertValuesSource Values { get; }
  }
}
