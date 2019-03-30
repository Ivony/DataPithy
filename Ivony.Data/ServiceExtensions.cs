using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{

  /// <summary>
  /// 提供服务相关的扩展方法
  /// </summary>
  public static class ServiceExtensions
  {
    /// <summary>
    /// 在现有的 IServiceProvider 基础上扩展
    /// </summary>
    /// <param name="serviceProvider">要扩展的 IServiceProvider</param>
    /// <param name="configure">要扩展注册的服务</param>
    /// <returns>扩展后的 IServiceProvider 对象</returns>
    public static IServiceProvider Expand( this IServiceProvider serviceProvider, Action<ServiceProvider.ServiceRegistration> configure )
    {
      if ( serviceProvider is ServiceProvider instance )
        return instance.Merge( new ServiceProvider( configure ) );


      return new ServiceProvider( serviceProvider, configure );
    }
  }
}
