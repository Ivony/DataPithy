using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  public class BlankServiceProvider : IServiceProvider
  {
    public object GetService( Type serviceType )
    {
      return null;
    }
  }
}
