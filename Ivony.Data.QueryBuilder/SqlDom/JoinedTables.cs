using Ivony.Data.SqlDom.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data.SqlDom
{
  public sealed class JoinedTables : FromSource
  {

    internal JoinedTables( FromSource left, FromSource right ) : this( TableJoinType.CrossJoin, left, right, null ) { }

    internal JoinedTables( TableJoinType joinType, FromSource left, FromSource right, SqlBooleanExpression joinCondition )
    {
      JoinType = joinType;
      Left = left;
      Right = right;
      JoinCondition = joinCondition;
    }

    public TableJoinType JoinType { get; }

    public FromSource Left { get; }

    public FromSource Right { get; }

    public SqlBooleanExpression JoinCondition { get; }



    public override string ToString()
    {
      string op;

      switch ( JoinType )
      {
        case TableJoinType.CrossJoin:
          return $"{Left} CROSS JOIN {Right}";

        case TableJoinType.InnerJoin:
          op = "INNER JOIN";
          break;
        case TableJoinType.LeftOuterJoin:
          op = "LEFT OUTER JOIN";
          break;
        case TableJoinType.RightOuterJoin:
          op = "RIGHT OUTER JOIN";
          break;
        case TableJoinType.FullOuterJoin:
          op = "FULL OUTER JOIN";
          break;
        default:
          throw new InvalidOperationException();
      }

      return $"({Left} {op} {Right} ON ({JoinCondition})";

    }


    public static JoinedTables InnerJoin( TableSource left, TableSource right, SqlBooleanExpression condition )
    {
      return new JoinedTables( TableJoinType.InnerJoin, left, right, condition );
    }

    public static JoinedTables LeftOuterJoin( TableSource left, TableSource right, SqlBooleanExpression condition )
    {
      return new JoinedTables( TableJoinType.LeftOuterJoin, left, right, condition );
    }

    public static JoinedTables RightOuterJoin( TableSource left, TableSource right, SqlBooleanExpression condition )
    {
      return new JoinedTables( TableJoinType.RightOuterJoin, left, right, condition );
    }

    public static JoinedTables FullOuterJoin( TableSource left, TableSource right, SqlBooleanExpression condition )
    {
      return new JoinedTables( TableJoinType.FullOuterJoin, left, right, condition );
    }

    public static JoinedTables CrossJoin( TableSource left, TableSource right )
    {
      return new JoinedTables( left, right );
    }



  }
}
