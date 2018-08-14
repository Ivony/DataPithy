using Ivony.Data.SqlQueries.SqlDom;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries
{
  public static class DbObjectHost
  {
    public static DataSchemaHostBuilder Schemas { get; } = new DataSchemaHostBuilder();

    public static DataSchemaReference Tables { get; } = NullSchemaReference.Instance;

    public static DbNameHost Names => new DbNameHost();
  }

  public class DataSchemaHostBuilder
  {
    public DataSchemaReference this[string name] => new DataSchemaReference( name );

  }

}
