using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.MySqlClient
{
  public class MySqlDbProvider : IDbProvider
  {
    public IDbExecutor<T> GetDbExecutor<T>( string connectionString ) where T : IDbQuery
    {
      return MySqlDb.Connect( connectionString ) as IDbExecutor<T>;
    }
  }
}
