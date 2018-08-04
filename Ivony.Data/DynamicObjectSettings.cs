using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  public class DynamicObjectSettings
  {

    public Func<string, object> FieldNotFound { get; set; }

  }
}
