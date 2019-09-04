using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{

  /// <summary>
  /// 定义数据库访问程序工厂类
  /// </summary>
  public interface IDatabaseProvider
  {

    /// <summary>
    /// 创建 <see cref="IDatabase"/> 对象
    /// </summary>
    /// <param name="databaseName">数据库名称</param>
    /// <returns>对应数据库的 <see cref="IDatabase"/> 对象</returns>
    IDatabase CreateDatabase( string databaseName );

  }
}
