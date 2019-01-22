using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Ivony.Data.Queries;
using System.Text.RegularExpressions;
using Ivony.Data.Common;
using Microsoft.Extensions.DependencyInjection;

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
      var builder = Db.ServiceProvider.GetService<IParameterizedQueryBuilder>();
      var configures = firstQuery.Configures;

      builder.AppendParameter( firstQuery );
      foreach ( var query in otherQueries )
      {
        if ( query == null || string.IsNullOrEmpty( query.TextTemplate ) )
          continue;

        configures = configures.Merge( query.Configures );
        builder.AppendParameter( query );
      }

      return builder.BuildQuery( configures );
    }


    /// <summary>
    /// 将数组转换为参数列表对象
    /// </summary>
    /// <param name="array">要转换的数组对象</param>
    /// <returns>参数列表对象</returns>
    public static ParameterList AsParameters( this Array array )
    {
      return new ParameterList( array );
    }

    /// <summary>
    /// 将数组转换为参数列表对象
    /// </summary>
    /// <param name="array">要转换的数组对象</param>
    /// <returns>参数列表对象</returns>
    public static ParameterList AsParameters( this Array array, string separator )
    {
      return new ParameterList( array, separator );
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

  }
}
