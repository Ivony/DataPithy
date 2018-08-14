using Ivony.Data.SqlQueries.SqlDom.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom
{
  public sealed class TableReference : SqlTableExpression
  {
    internal TableReference( string tableName )
    {
      TableName = tableName;
    }

    public string TableName { get; }


    public TableSource As()
    {
      return As( TableName );
    }

    public static implicit operator TableSource( TableReference table )
    {
      return table.As();
    }


    public override string ToString()
    {
      return TableName;
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
