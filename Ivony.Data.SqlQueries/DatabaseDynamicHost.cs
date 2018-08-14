using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Ivony.Data.SqlQueries
{
  public class DatabaseDynamicHost : DynamicObject
  {

    public override bool TryGetMember( GetMemberBinder binder, out object result )
    {
      result = this[binder.Name];
      return true;
    }

    public TableDynamicHost this[string tableName]
    {
      get { return new TableDynamicHost( tableName ); }
    }

  }
}
