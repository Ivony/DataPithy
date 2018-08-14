using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom
{
  public class DataSchemaReference
  {
    internal DataSchemaReference( string name )
    {
      SchemaName = name;
    }

    public string SchemaName { get; }

    public virtual TableReference this[string tableName] => new TableReference( SchemaName, tableName );

  }


  internal class NullSchemaReference : DataSchemaReference
  {
    private NullSchemaReference() : base( null )
    {

    }

    public override TableReference this[string tableName] => new TableReference( tableName );

    public static DataSchemaReference Instance { get; } = new NullSchemaReference();
  }

}
