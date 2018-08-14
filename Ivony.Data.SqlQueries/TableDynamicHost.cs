using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Ivony.Data.SqlQueries
{
  public class TableDynamicHost : DynamicObject
  {

    public TableDynamicHost( TableReference


    public override bool TryGetMember( GetMemberBinder binder, out object result )
    {
    }

  }
}
