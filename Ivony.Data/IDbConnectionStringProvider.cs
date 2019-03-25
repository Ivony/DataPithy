using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{


  /// <summary>
  /// 定义数据库连接字符串提供程序
  /// </summary>
  public interface IDbConnectionStringProvider
  {


    /// <summary>
    /// 系统服务提供程序
    /// </summary>
    IServiceProvider ServiceProvider { get; }


    /// <summary>
    /// 获取默认连接字符串
    /// </summary>
    /// <param name="dbProviderType">数据库提供程序类型</param>
    /// <returns>连接字符串</returns>
    string GetConnectionString( Type dbProviderType );

  }
}
