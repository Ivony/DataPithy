using Ivony.Data.SqlDom.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlDom
{
  public class TableReference : SqlTableExpression
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

  }
}
