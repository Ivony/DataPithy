using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom
{
  public sealed class DataSchemaReference
  {
    internal DataSchemaReference( string name )
    {
      SchemaName = name;
    }

    public string SchemaName { get; }

    public TableReference this[string tableName] => new TableReference( SchemaName, tableName );

  }
}
