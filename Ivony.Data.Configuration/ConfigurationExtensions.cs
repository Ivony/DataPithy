using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.Configuration
{

  /// <summary>
  /// 提供配置相关的扩展方法
  /// </summary>
  public static class ConfigurationExtensions
  {

    /// <summary>
    /// 添加连接字符串服务，从配置中发现和查找连接字符串
    /// </summary>
    /// <param name="services">服务注册容器</param>
    /// <returns>返回服务注册容器</returns>
    public static IServiceCollection AddConnectionStrings( this IServiceCollection services )
    {
      services.AddSingleton<IDbConnectionStringProvider, DbConnectionStringProvider>();
      return services;
    }

  }
}
