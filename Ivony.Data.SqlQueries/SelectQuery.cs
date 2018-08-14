using Ivony.Data.SqlQueries.SqlDom.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom
{
  public class SelectQuery : SqlTableExpression
  {

    public SelectQuery( SelectClause select, FromClause from, WhereClause where, OrderByClause orderBy )
    {
      SelectClause = select;
      FromClause = from;
      WhereClause = where;
      OrderByClause = orderBy;
    }

    public SelectClause SelectClause { get; }
    public FromClause FromClause { get; }
    public WhereClause WhereClause { get; }
    public OrderByClause OrderByClause { get; }


    public override string ToString()
    {
      return $@"{SelectClause}
  {FromClause}
  {WhereClause}
  {OrderByClause}";
    }
  }
}
