using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{

  /// <summary>
  /// 代表一个服务注册
  /// </summary>
  public abstract class ServiceRegistry
  {

    /// <summary>
    /// 创建 ServiceRegistry 对象
    /// </summary>
    /// <param name="serviceType">服务类型</param>
    public ServiceRegistry( Type serviceType )
    {
      ServiceType = serviceType;
    }


    /// <summary>
    /// 服务类型
    /// </summary>
    public Type ServiceType { get; }


    /// <summary>
    /// 获取服务实例
    /// </summary>
    /// <param name="provider">当前服务提供程序</param>
    /// <returns>服务实例</returns>
    public abstract object GetService( IServiceProvider provider );



    /// <summary>
    /// 通过服务实例创建服务注册
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <param name="instance">服务实例</param>
    /// <returns>服务注册</returns>
    public static ServiceRegistry Create<T>( T instance ) where T : class
    {
      return new ServiceInstanceRegistry<T>( instance );
    }

    private class ServiceInstanceRegistry<T> : ServiceRegistry where T : class
    {
      private readonly T serviceInstance;

      public ServiceInstanceRegistry( T instance ) : base( typeof( T ) )
      {
        serviceInstance = instance;
      }

      public override object GetService( IServiceProvider provider )
      {
        return serviceInstance;
      }
    }



    public static ServiceRegistry Create<T>( Func<IServiceProvider, T> factory ) where T : class
    {
      return new ServiceFactoryRegistry<T>( factory );
    }

    private class ServiceFactoryRegistry<T> : ServiceRegistry where T : class
    {
      private Func<IServiceProvider, T> factory;

      public ServiceFactoryRegistry( Func<IServiceProvider, T> func ) : base( typeof( T ) )
      {
        factory = func;
      }


      public override object GetService( IServiceProvider provider ) => factory( provider );

    }
  }
}
