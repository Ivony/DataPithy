using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  public class TableReference : RowSetExpression
  {
    internal TableReference( string tablename )
    {
      TableName = tablename;
    }

    public string TableName { get; }

  }
}
