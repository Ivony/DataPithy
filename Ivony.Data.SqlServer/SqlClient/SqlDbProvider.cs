using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlClient
{
  public class SqlDbProvider : IDbProvider
  {
    public IDbExecutor<T> GetDbExecutor<T>( string connectionString ) where T : IDbQuery
    {
      return SqlServer.Connect( connectionString ) as IDbExecutor<T>;
    }
  }
}
