using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlDom.Expressions
{
  public class SqlTableExpression : SqlExpression
  {


    public TableSource As( string alias )
    {
      return new TableSource( this, alias );
    }


    public override Type Type => typeof( IEnumerable<dynamic> );

  }
}
