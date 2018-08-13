using Ivony.Data.SqlDom;
using Ivony.Data.SqlDom.Expressions;
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
      result = this[binder.Name];
      return true;
    }



    public FieldReference this[string name]
    {
      get { return new FieldReference( TableName, name ); }
    }



    public static implicit operator TableReference( TableDynamicHost host )
    {
      return new TableReference( host.TableName );
    }
    public static implicit operator TableSource( TableDynamicHost host )
    {
      return (TableReference) host;
    }



    public JoinWithoutOnExpression InnerJoin( TableSource right )
      => JoinWithoutOnExpression.InnerJoin( this, right );

    public JoinedTables InnerJoin( TableSource right, SqlBooleanExpression condition )
      => JoinedTables.InnerJoin( this, right, condition );




    public JoinWithoutOnExpression LeftOuterJoin( TableSource right )
      => JoinWithoutOnExpression.LeftOuterJoin( this, right );

    public JoinedTables LeftOuterJoin( TableSource right, SqlBooleanExpression condition )
      => JoinedTables.LeftOuterJoin( this, right, condition );




    public JoinWithoutOnExpression RightOuterJoin( TableSource right )
      => JoinWithoutOnExpression.RightOuterJoin( this, right );

    public JoinedTables RightOuterJoin( TableSource right, SqlBooleanExpression condition )
      => JoinedTables.RightOuterJoin( this, right, condition );




    public JoinWithoutOnExpression FullOuterJoin( TableSource right )
      => JoinWithoutOnExpression.FullOuterJoin( this, right );

    public JoinedTables FullOuterJoin( TableSource right, SqlBooleanExpression condition )
      => JoinedTables.FullOuterJoin( this, right, condition );



    public JoinedTables Join( TableSource right )
      => CrossJoin( right );

    public JoinedTables CrossJoin( TableSource right )
      => JoinedTables.CrossJoin( this, right );

    public JoinedTables Join( TableSource right, SqlBooleanExpression condition )
      => InnerJoin( right, condition );





  }
}