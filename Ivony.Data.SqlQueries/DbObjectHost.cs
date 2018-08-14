using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries
{
  public static class DbObjectHost
  {
    public static DataSchemaHostBuilder DataSchemas { get; } = new DataSchemaHostBuilder();

    public static DataSchemaHost Tables { get; } = new DataSchemaHost();

    public static DbNameHost Names => new DbNameHost();
  }
  public static class DbDynamicHost
  {
    public static dynamic DataSchemas => DbObjectHost.DataSchemas;

    public static dynamic Tables => DbObjectHost.Tables;

    public static dynamic Names => DbObjectHost.Names;

  }
}
