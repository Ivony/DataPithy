using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom
{
  public sealed class InsertColumns
  {
    public InsertColumns( string[] columns )
    {
      Columns = columns;
    }
    public string[] Columns { get; }
  }



}
