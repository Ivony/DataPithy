using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.Configuration
{
  public class DbProviderSelector : IDbProviderSelector
  {
    public IDbProvider GetDbProvider( string name, string connectionString )
    {

      throw new NotImplementedException();
    }
  }
}
