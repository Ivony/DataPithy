using Ivony.Data.SqlQueries.SqlDom;

namespace Ivony.Data.SqlQueries
{

  public class DataSchemaHostBuilder
  {
    public DataSchemaReference this[string name] => new DataSchemaReference( name );

  }

  public class DataSchemaHost
  {
    internal DataSchemaHost() { }

    public TableReference this[string name] => new TableReference( name );


  }
}