using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{

  /// <summary>
  /// 定义参数值列表
  /// </summary>
  public class ValueList : IParameterizedQueryPartial
  {


    private Array _parameters;
    private string _separator;



    /// <summary>
    /// 从 Tuple 创建 ValueList 对象
    /// </summary>
    /// <param name="tuple">Tuple 对象</param>
    /// <param name="separator">参数列表分隔符</param>
    /// <returns>参数列表对象</returns>
    public static ValueList Create( ITuple tuple, string separator = ", " )
    {

      var array = new object[tuple.Length];
      for ( int i = 0; i < tuple.Length; i++ )
        array[i] = tuple[i];

      return new ValueList( array );
    }


    /// <summary>
    /// 创建 ValueList 对象
    /// </summary>
    /// <param name="parameters">参数列表</param>
    /// <param name="separator">分隔符</param>
    public ValueList( Array parameters, string separator = ", " )
    {
      if ( parameters.Rank != 1 )
        throw new ArgumentException( "参数列表必须是一维数组", "parameters" );

      _parameters = parameters;
      _separator = separator;
    }


    /// <summary>
    /// 实现 AppendTo 方法，将自身加入到参数化查询中
    /// </summary>
    /// <param name="builder"></param>
    public void AppendTo( IParameterizedQueryBuilder builder )
    {

      for ( int i = 0; i < _parameters.Length; i++ )
      {
        builder.AppendValue( _parameters.GetValue( i ) );

        if ( i < _parameters.Length - 1 )
          builder.Append( _separator );

      }
    }
  }
}
