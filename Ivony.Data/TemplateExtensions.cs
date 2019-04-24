using System;
using System.Collections.Generic;
using Ivony.Data.Queries;

namespace Ivony.Data
{

  /// <summary>
  /// 有关模板的扩展方法
  /// </summary>
  public static class TemplateExtensions
  {

    /// <summary>
    /// 串联两个参数化查询对象
    /// </summary>
    /// <param name="firstQuery">第一个参数化查询对象</param>
    /// <param name="secondQuery">第二个参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static ParameterizedQuery Concat( this ParameterizedQuery firstQuery, ParameterizedQuery secondQuery )
    {
      return ConcatQueries( firstQuery, secondQuery );
    }

    /// <summary>
    /// 串联两个参数化查询对象
    /// </summary>
    /// <param name="firstQuery">第一个参数化查询对象</param>
    /// <param name="secondQuery">第二个参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static ParameterizedQuery Concat( this ParameterizedQuery firstQuery, FormattableString secondQuery )
    {
      return ConcatQueries( firstQuery, Db.Template( secondQuery ) );
    }

    /// <summary>
    /// 串联多个参数化查询对象
    /// </summary>
    /// <param name="firstQuery">第一个参数化查询对象</param>
    /// <param name="otherQueries">要串联的其他参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static ParameterizedQuery ConcatQueries( this ParameterizedQuery firstQuery, params ParameterizedQuery[] otherQueries )
    {
      var builder = Db.ParameterizedQueryService.CreateQueryBuild();
      var configures = firstQuery.Configures;

      builder.AppendValue( firstQuery );
      foreach ( var query in otherQueries )
      {
        if ( query == null || string.IsNullOrEmpty( query.TextTemplate ) )
          continue;

        configures = DbQueryConfigures.Merge( configures, query.Configures );
        builder.AppendValue( query );
      }

      return builder.BuildQuery( configures );
    }


    /// <summary>
    /// 将数组转换为参数值列表对象
    /// </summary>
    /// <param name="array">要转换的数组对象</param>
    /// <returns>参数列表对象</returns>
    public static ValueList AsValueList( this Array array )
    {
      return ValueList.Create( array );
    }

    /// <summary>
    /// 将数组转换为参数值列表对象
    /// </summary>
    /// <param name="array">要转换的数组对象</param>
    /// <param name="separator">参数列表分隔符</param>
    /// <returns>参数列表对象</returns>
    public static ValueList AsValueList( this Array array, string separator )
    {
      return ValueList.Create( array, separator );
    }



    /// <summary>
    /// 将一个数组当作参数值列表添加到参数化查询构建器中
    /// </summary>
    /// <param name="builder">参数化查询构建器</param>
    /// <param name="array">参数值列表</param>
    public static void AppendValueList( this IParameterizedQueryBuilder builder, Array array )
    {
      builder.AppendValue( array.AsValueList() );
    }

    /// <summary>
    /// 将一个数组当作参数值列表添加到参数化查询构建器中
    /// </summary>
    /// <param name="builder">参数化查询构建器</param>
    /// <param name="array">参数值列表</param>
    /// <param name="separator">分隔参数值的分隔符</param>
    public static void AppendValueList( this IParameterizedQueryBuilder builder, Array array, string separator )
    {
      builder.AppendValue( array.AsValueList( separator ) );
    }

    /// <summary>
    /// 将一个数组当作参数值列表添加到参数化查询构建器中
    /// </summary>
    /// <param name="builder">参数化查询构建器</param>
    /// <param name="list">参数值列表</param>
    public static void AppendValueList<T>( this IParameterizedQueryBuilder builder, IEnumerable<T> list )
    {
      builder.AppendValue( ValueList.Create( list ) );
    }

    /// <summary>
    /// 将一个数组当作参数值列表添加到参数化查询构建器中
    /// </summary>
    /// <param name="builder">参数化查询构建器</param>
    /// <param name="list">参数值列表</param>
    /// <param name="separator">分隔参数值的分隔符</param>
    public static void AppendValueList<T>( this IParameterizedQueryBuilder builder, IEnumerable<T> list, string separator )
    {
      builder.AppendValue( ValueList.Create( list, separator ) );
    }

    /// <summary>
    /// 将一个字符串当作数据库对象名称添加到参数化查询构建器中
    /// </summary>
    /// <param name="builder">参数化查询构建器</param>
    /// <param name="name">数据库对象名称</param>
    public static void AppendName( this IParameterizedQueryBuilder builder, string name )
    {
      builder.Append( new DbName( name ) );
    }

    /// <summary>
    /// 添加一个参数化查询部分组件
    /// </summary>
    /// <param name="builder">参数化查询构建器</param>
    /// <param name="partial">参数化查询部分组件</param>
    /// <remarks>此扩展方法与直接调用 AppendValue 效果相同，但是具有强类型约束，建议使用此方法明确添加部分组件</remarks>
    public static void AppendPartial( this IParameterizedQueryBuilder builder, IParameterizedQueryPartial partial )
    {
      builder.AppendValue( partial );
    }

  }
}
