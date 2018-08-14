using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ivony.Data.SqlQueries.SqlDom
{
  public sealed class ValuesClause : InsertValuesSource
  {
    private ValuesClause( ITuple[] values )
    {
      ValuesList = new ReadOnlyCollection<ITuple>( values );
    }

    public IReadOnlyList<ITuple> ValuesList { get; }



    public static ValuesClause Values<T1>( params Tuple<T1>[] values ) => new ValuesClause( values );
    public static ValuesClause Values<T1, T2>( params Tuple<T1, T2>[] values ) => new ValuesClause( values );
    public static ValuesClause Values<T1, T2, T3>( params Tuple<T1, T2, T3>[] values ) => new ValuesClause( values );
    public static ValuesClause Values<T1, T2, T3, T4>( params Tuple<T1, T2, T3, T4>[] values ) => new ValuesClause( values );
    public static ValuesClause Values<T1, T2, T3, T4, T5>( params Tuple<T1, T2, T3, T4, T5>[] values ) => new ValuesClause( values );
    public static ValuesClause Values<T1, T2, T3, T4, T5, T6>( params Tuple<T1, T2, T3, T4, T5, T6>[] values ) => new ValuesClause( values );
    public static ValuesClause Values<T1, T2, T3, T4, T5, T6, T7>( params Tuple<T1, T2, T3, T4, T5, T6, T7>[] values ) => new ValuesClause( values );
    public static ValuesClause Values<T1, T2, T3, T4, T5, T6, T7, TRest>( params Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>[] values ) => new ValuesClause( values );


    public static ValuesClause Values<T1>( params ValueTuple<T1>[] values ) => new ValuesClause( values.Cast<ITuple>().ToArray() );
    public static ValuesClause Values<T1, T2>( params ValueTuple<T1, T2>[] values ) => new ValuesClause( values.Cast<ITuple>().ToArray() );
    public static ValuesClause Values<T1, T2, T3>( params ValueTuple<T1, T2, T3>[] values ) => new ValuesClause( values.Cast<ITuple>().ToArray() );
    public static ValuesClause Values<T1, T2, T3, T4>( params ValueTuple<T1, T2, T3, T4>[] values ) => new ValuesClause( values.Cast<ITuple>().ToArray() );
    public static ValuesClause Values<T1, T2, T3, T4, T5>( params ValueTuple<T1, T2, T3, T4, T5>[] values ) => new ValuesClause( values.Cast<ITuple>().ToArray() );
    public static ValuesClause Values<T1, T2, T3, T4, T5, T6>( params ValueTuple<T1, T2, T3, T4, T5, T6>[] values ) => new ValuesClause( values.Cast<ITuple>().ToArray() );
    public static ValuesClause Values<T1, T2, T3, T4, T5, T6, T7>( params ValueTuple<T1, T2, T3, T4, T5, T6, T7>[] values ) => new ValuesClause( values.Cast<ITuple>().ToArray() );
    public static ValuesClause Values<T1, T2, T3, T4, T5, T6, T7, TRest>( params ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>[] values ) where TRest : struct => new ValuesClause( values.Cast<ITuple>().ToArray() );



  }
}
