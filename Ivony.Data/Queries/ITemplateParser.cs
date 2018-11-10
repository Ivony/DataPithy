using System;
using Ivony.Data.Queries;

namespace Ivony.Data
{


  /// <summary>
  /// 定义字符串模板查询解析器
  /// </summary>
  public interface ITemplateParser
  {
    /// <summary>
    /// 解析字符串模板
    /// </summary>
    /// <param name="template">字符串模板</param>
    /// <returns>解析出来的参数化查询对象</returns>
    ParameterizedQuery ParseTemplate( FormattableString template );
  }
}