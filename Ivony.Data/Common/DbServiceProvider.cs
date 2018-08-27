using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.Common
{
  public class DbServiceProvider : IServiceProvider
  {

    public DatabaseContext Context { get; }

    public object GetService( Type serviceType )
    {

      throw new NotImplementedException();
    }
  }
}
