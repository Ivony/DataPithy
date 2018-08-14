using Ivony.Data.SqlQueries.SqlDom.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom
{

  public sealed class TableSource : FromSource
  {
    internal TableSource( SqlTableExpression table, string alias )
    {
      TableExpression = table;
      Alias = alias;
    }

    public SqlTableExpression TableExpression { get; }

    public string Alias { get; }



    public override string ToString()
    {

      return $"{TableExpression} AS {Alias}";
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
