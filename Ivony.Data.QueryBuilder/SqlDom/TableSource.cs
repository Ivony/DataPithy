using Ivony.Data.SqlDom.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlDom
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


    public JoinedTables InnerJoin( TableSource right, SqlBooleanExpression condition )
    {
      return new JoinedTables( TableJoinType.InnerJoin, this, right, condition );
    }

    public JoinedTables LeftJoin( TableSource right, SqlBooleanExpression condition )
    {
      return new JoinedTables( TableJoinType.LeftOuterJoin, this, right, condition );
    }

    public JoinedTables RightJoin( TableSource right, SqlBooleanExpression condition )
    {
      return new JoinedTables( TableJoinType.RightOuterJoin, this, right, condition );
    }

    public JoinedTables FullJoin( TableSource right, SqlBooleanExpression condition )
    {
      return new JoinedTables( TableJoinType.FullOuterJoin, this, right, condition );
    }

    public JoinedTables CrossJoin( TableSource right )
    {
      return new JoinedTables( this, right );
    }



  }
}
