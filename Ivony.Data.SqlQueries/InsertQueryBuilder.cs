using Ivony.Data.SqlQueries.SqlDom;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ivony.Data.SqlQueries
{
  public class InsertQueryBuilder
  {





    public InsertIntoClause Into { get; private set; }

    public InsertColumns Columns { get; private set; }

    public InsertValuesSource Values { get; private set; }


    public InsertQueryBuilder InsertInto( TableReference table, params string[] columns )
    {
      Into = new InsertIntoClause( table );
      AddColumns( columns );

      return this;

    }

    public InsertQueryBuilder InsertInto( TableReference table, ITuple columns = null )
    {
      Into = new InsertIntoClause( table );

      if ( columns != null )
        AddColumns( columns );

      return this;
    }

    private void AddColumns( string[] array )
    {
      Columns = new InsertColumns( array );
    }



    private void AddColumns( ITuple tuple )
    {
      var array = new string[tuple.Length];
      for ( int i = 0; i < array.Length; i++ )
        array[i] = tuple[i].ToString();

      AddColumns( array );
    }

    public InsertQueryBuilder WithValues( ValuesClause values )
    {
      Values = values;

      return this;
    }


    public InsertQuery Build()
    {
      return new InsertQuery( Into, Columns, Values );
    }


  }
}
