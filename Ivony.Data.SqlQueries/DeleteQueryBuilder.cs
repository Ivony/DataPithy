using Ivony.Data.SqlQueries.SqlDom;
using Ivony.Data.SqlQueries.SqlDom.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlQueries
{
  public class DeleteQueryBuilder
  {



    public TableReference TargetTable { get; private set; }
    public SqlBooleanExpression Condition { get; private set; }

    public DeleteQueryBuilder Delete( TableReference table )
    {
      TargetTable = table;
      return this;
    }

    public DeleteQueryBuilder Where( SqlBooleanExpression condition )
    {
      Condition = condition;
      return this;
    }



  }
}
