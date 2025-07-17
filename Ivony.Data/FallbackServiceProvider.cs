using System;

namespace Ivony.Data;


/// <summary>
/// 为服务提供程序提供后备拓展
/// </summary>
/// <param name="serviceProvider"></param>
public abstract class FallbackServiceProvider( IServiceProvider serviceProvider ) : IServiceProvider
{
  object? IServiceProvider.GetService( Type serviceType ) => serviceProvider.GetService( serviceType ) ?? FallbackGetService( serviceProvider, serviceType );

  /// <summary>
  /// 获取后备的服务
  /// </summary>
  /// <param name="serviceProvider">原服务提供程序</param>
  /// <param name="serviceType">服务类型</param>
  /// <returns>服务实例</returns>
  protected abstract object? FallbackGetService( IServiceProvider serviceProvider, Type serviceType );



  /// <summary>
  /// 获取一个空的服务提供程序
  /// </summary>
  public static IServiceProvider Empty { get; } = new EmptyServiceProvider();



  private class EmptyServiceProvider : IServiceProvider
  {
    public object? GetService( Type serviceType ) => null;
  }
}