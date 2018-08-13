using Ivony.Data.SqlDom;
using System.Dynamic;

namespace Ivony.Data.QueryBuilders
{
  public class TableDynamicHost : DynamicObject
  {

    public TableDynamicHost( string name )
    {
      TableName = name;
    }

    public string TableName { get; }



    public override bool TryGetMember( GetMemberBinder binder, out object result )
    {
      result = new FieldReference( TableName, binder.Name );
      return true;
    }


    public static implicit operator TableReference( TableDynamicHost host )
    {
      return new TableReference( host.TableName );
    }
    public static implicit operator TableSource( TableDynamicHost host )
    {
      return (TableReference) host;
    }


  }
}