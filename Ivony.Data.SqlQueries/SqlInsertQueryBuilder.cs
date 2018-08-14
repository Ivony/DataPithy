using Ivony.Data.SqlQueries.SqlDom;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ivony.Data.SqlQueries
{
  public class SqlInsertQueryBuilder
  {





    public TableReference TargetTable { get; private set; }


    public void InsertInto( TableReference table, Func<dynamic, ITuple> columnsFactory = null )
    {
      TargetTable = table;

      if ( columnsFactory != null )
        AddColumns( columnsFactory( new DbNameHost() ) );

    }

    private void AddColumns( ITuple tuple )
    {

    }

    public void Values( params object[] values )
    {

    }

  }
}
