using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Ivony.Data.SqlQueries
{
  public class DbNameHost : DynamicObject
  {

    public DbName this[string name]
    {
      get { return new DbName( name ); }
    }


    public override bool TryGetMember( GetMemberBinder binder, out object result )
    {

      result = this[binder.Name];
      return true;

    }
  }
}
