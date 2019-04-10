using System;
using System.Collections.Generic;
using System.Text;
using Ivony.Data.Queries;

namespace Ivony.Data.Queries
{


  /// <summary>
  /// 定义参数化查询服务
  /// </summary>
  public interface IParameterizedQueryService
  {



    /// <summary>
    /// 创建参数化查询构建器
    /// </summary>
    /// <returns>参数化查询构建器实现对象，用于创建参数化查询对象</returns>
    IParameterizedQueryBuilder CreateQueryBuild();

    /// <summary>
    /// 获取查询模板解析器
    /// </summary>
    /// <returns>查询模板解析器实现对象</returns>
    ITemplateParser GetTemplateParser();

  }
}
