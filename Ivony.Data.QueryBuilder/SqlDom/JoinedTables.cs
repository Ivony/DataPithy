using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data
{
  public class JoinedTables
  {

    public TableJoinType JoinType { get; }

    public TableSource Left { get; }

    public TableSource Right { get; }

    public Expression JoinCondition { get; }


  }
}
