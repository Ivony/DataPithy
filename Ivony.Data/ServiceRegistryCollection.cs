using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Ivony.Data
{

  /// <summary>
  /// 实现 ServiceRegistry 的容器
  /// </summary>
  public class ServiceRegistryCollection : KeyedCollection<Type, ServiceRegistry>
  {


    protected override Type GetKeyForItem( ServiceRegistry item )
    {
      return item.ServiceType;
    }


    /// <summary>
    /// 注册一个服务
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <param name="instance">服务实例</param>
    /// <returns>ServiceRegistryColletion 对象，用于链式调用</returns>
    public ServiceRegistryCollection AddService<T>( T instance ) where T : class
    {
      Add( ServiceRegistry.Create( instance ) );
      return this;
    }

    /// <summary>
    /// 注册一个服务
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <param name="factory">创建服务实例的工厂方法</param>
    /// <returns>ServiceRegistryColletion 对象，用于链式调用</returns>
    public ServiceRegistryCollection AddService<T>( Func<IServiceProvider, T> factory ) where T : class
    {
      Add( ServiceRegistry.Create( factory ) );
      return this;
    }

    /// <summary>
    /// 注册一个服务
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <param name="factory">创建服务实例的工厂方法</param>
    /// <returns>ServiceRegistryColletion 对象，用于链式调用</returns>
    public ServiceRegistryCollection AddService<T>( Func<T> factory ) where T : class
    {
      Add( ServiceRegistry.Create( serviceProvider => factory() ) );
      return this;
    }

    /// <summary>
    /// 注册一个服务
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <typeparam name="TInstance">实例类型</typeparam>
    /// <returns>ServiceRegistryColletion 对象，用于链式调用</returns>
    public ServiceRegistryCollection AddService<TService, TInstance>() where TInstance : TService where TService : class
    {
      Add( ServiceRegistry.Create( provider => (TService) CreateInstance( provider, typeof( TInstance ) ) ) );
      return this;
    }

    /// <summary>
    /// 尝试创建服务实例
    /// </summary>
    /// <param name="provider">服务提供程序</param>
    /// <param name="type">实例类型</param>
    /// <returns>服务实例</returns>
    public static object CreateInstance( IServiceProvider provider, Type type )
    {
      throw new NotImplementedException();
    }
  }
}
