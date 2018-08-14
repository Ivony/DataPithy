using Ivony.Data.SqlQueries.SqlDom.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom
{
  public class JoinWithoutOnExpression
  {
    private readonly TableJoinType joinType;
    private readonly TableSource left;
    private readonly TableSource right;

    private JoinWithoutOnExpression( TableJoinType joinType, TableSource left, TableSource right )
    {
      this.joinType = joinType;
      this.left = left;
      this.right = right;
    }



    public JoinedTables On( SqlBooleanExpression condition )
    {
      switch ( joinType )
      {
        case TableJoinType.InnerJoin:
          return JoinedTables.InnerJoin( left, right, condition );

        case TableJoinType.LeftOuterJoin:
          return JoinedTables.LeftOuterJoin( left, right, condition );

        case TableJoinType.RightOuterJoin:
          return JoinedTables.RightOuterJoin( left, right, condition );

        case TableJoinType.FullOuterJoin:
          return JoinedTables.FullOuterJoin( left, right, condition );

        default:
          throw new InvalidOperationException();
      }
    }

    internal static JoinWithoutOnExpression InnerJoin( TableSource left, TableSource right )
    {
      return new JoinWithoutOnExpression( TableJoinType.InnerJoin, left, right );
    }
    internal static JoinWithoutOnExpression LeftOuterJoin( TableSource left, TableSource right )
    {
      return new JoinWithoutOnExpression( TableJoinType.LeftOuterJoin, left, right );
    }
    internal static JoinWithoutOnExpression RightOuterJoin( TableSource left, TableSource right )
    {
      return new JoinWithoutOnExpression( TableJoinType.RightOuterJoin, left, right );
    }
    internal static JoinWithoutOnExpression FullOuterJoin( TableSource left, TableSource right )
    {
      return new JoinWithoutOnExpression( TableJoinType.FullOuterJoin, left, right );
    }
  }
}
