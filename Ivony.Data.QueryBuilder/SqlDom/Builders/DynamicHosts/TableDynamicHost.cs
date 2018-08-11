using System.Dynamic;

namespace Ivony.Data.SqlDom.Builders.DynamicHosts
{
  internal class TableDynamicHost : DynamicObject
  {

    public TableDynamicHost( string name )
    {
      TableName = name;
    }

    public string TableName { get; }



    public override bool TryGetMember( GetMemberBinder binder, out object result )
    {
      result = new FieldDynamicHost( TableName, binder.Name );
      return true;
    }

  }
}