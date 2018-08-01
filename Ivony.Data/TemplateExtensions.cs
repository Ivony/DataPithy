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
    /// 根据模板表达式创建参数化查询实例
    /// </summary>
    /// <param name="context">当前数据访问上下文</param>
    /// <param name="template">参数化模板</param>
    /// <returns>参数化查询实例</returns>
    public static ParameterizedQuery Template( this DbContext context, FormattableString template )
    {
      var query = context.GetTemplateParser().ParseTemplate( template );
      query.Hosting = context;
      return query;
    }

    /// <summary>
    /// 根据模板表达式创建参数化查询实例
    /// </summary>
    /// <param name="context">当前数据访问上下文</param>
    /// <param name="template">参数化模板</param>
    /// <returns>参数化查询实例</returns>
    public static ParameterizedQuery T( this DbContext context, FormattableString template )
    {
      var query = context.GetTemplateParser().ParseTemplate( template );
      query.Hosting = context;
      return query;
    }


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
    /// 串联多个参数化查询对象
    /// </summary>
    /// <param name="firstQuery">第一个参数化查询对象</param>
    /// <param name="otherQueries">要串联的其他参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static ParameterizedQuery ConcatQueries( this ParameterizedQuery firstQuery, params ParameterizedQuery[] otherQueries )
    {
      var builder = Db.GetCurrentContext().GetParameterizedQueryBuilder();

      firstQuery.AppendTo( builder );
      foreach ( var query in otherQueries )
      {
        if ( query == null || string.IsNullOrEmpty( query.TextTemplate ) )
          continue;

        query.AppendTo( builder );
      }

      return builder.BuildQuery();
    }
  }
}
