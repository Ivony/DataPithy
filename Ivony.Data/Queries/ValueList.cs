using System;
using System.Collections;
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

#if NETCOREAPP


    /// <summary>
    /// 从 Tuple 创建 ValueList 对象
    /// </summary>
    /// <param name="tuple">参数列表</param>
    /// <param name="separator">参数列表分隔符</param>
    /// <returns>参数列表对象</returns>
    public static ValueList Create( ITuple tuple, string separator = ", " )
    {

      var array = new object[tuple.Length];
      for ( int i = 0; i < tuple.Length; i++ )
        array[i] = tuple[i];

      return new ValueList( array, separator );
    }
#endif


    /// <summary>
    /// 从数组创建 ValueList 对象
    /// </summary>
    /// <param name="array">参数列表</param>
    /// <param name="separator">参数列表分隔符</param>
    /// <returns>参数列表对象</returns>
    public static ValueList Create( Array array, string separator = ", " )
    {
      return new ValueList( array, separator );
    }


    /// <summary>
    /// 从可枚举集合创建 ValueList 对象
    /// </summary>
    /// <param name="list">参数列表</param>
    /// <param name="separator">参数列表分隔符</param>
    /// <returns>参数列表对象</returns>
    public static ValueList Create<T>( IEnumerable<T> list, string separator = ", " )
    {
      return new ValueList( list.ToArray(), separator );
    }


    /// <summary>
    /// 创建 ValueList 对象
    /// </summary>
    /// <param name="parameters">参数列表</param>
    /// <param name="separator">分隔符</param>
    private ValueList( Array parameters, string separator = ", " )
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
