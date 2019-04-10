using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Ivony.Data.Queries;

namespace Ivony.Data
{
  /// <summary>
  /// 提供 ServiceProvider 的简单实现
  /// </summary>
  public class ServiceProvider : IServiceProvider
  {


    /// <summary>
    /// 服务注册容器
    /// </summary>
    public class ServiceRegistration
    {

      internal ServiceRegistration()
      {
        registration = new Dictionary<Type, Func<IServiceProvider, object>>();
      }



      private Dictionary<Type, Func<IServiceProvider, object>> registration;


      /// <summary>
      /// 添加一个服务注册
      /// </summary>
      /// <typeparam name="TService">服务类型</typeparam>
      /// <param name="instance">服务实例</param>
      /// <returns></returns>
      public ServiceRegistration AddService<TService>( TService instance )
      {
        registration[typeof( TService )] = serviceProvider => instance;
        return this;
      }


      /// <summary>
      /// 添加一个服务注册
      /// </summary>
      /// <typeparam name="TService">服务类型</typeparam>
      /// <param name="factory">服务实例工厂</param>
      /// <returns></returns>
      public ServiceRegistration AddService<TService>( Func<TService> factory )
      {
        registration[typeof( TService )] = serviceProvider => factory();
        return this;
      }


      /// <summary>
      /// 添加一个服务注册
      /// </summary>
      /// <typeparam name="TService">服务类型</typeparam>
      /// <param name="factory">服务实例工厂</param>
      /// <returns></returns>
      public ServiceRegistration AddService<TService>( Func<IServiceProvider, TService> factory )
      {
        registration[typeof( TService )] = serviceProvider => factory( serviceProvider );
        return this;
      }




      internal IReadOnlyDictionary<Type, Func<IServiceProvider, object>> Build()
      {
        return new ReadOnlyDictionary<Type, Func<IServiceProvider, object>>( registration );
      }
    }


    private IReadOnlyDictionary<Type, Func<IServiceProvider, object>> _registration;
    private IServiceProvider _serviceProvider;


    internal ServiceProvider( IServiceProvider serviceProvider, Action<ServiceRegistration> configure )
    {
      _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 与另外一个 ServiceProvider 合并创建新的 ServiceProvider 对象
    /// </summary>
    /// <param name="serviceProvider">要合并的 ServiceProvider 对象</param>
    /// <returns>合并后的 ServiceProvider 对象</returns>
    public ServiceProvider Merge( ServiceProvider serviceProvider )
    {

      var registration = new Dictionary<Type, Func<IServiceProvider, object>>( _registration );

      foreach ( var pair in serviceProvider._registration )
        registration[pair.Key] = pair.Value;

      return new ServiceProvider( new ReadOnlyDictionary<Type, Func<IServiceProvider, object>>( registration ) );

    }


    private ServiceProvider( IReadOnlyDictionary<Type, Func<IServiceProvider, object>> registration )
    {
      _registration = registration;
    }


    /// <summary>
    /// 创建 ServiceProvider 对象
    /// </summary>
    /// <param name="configure">服务注册配置</param>
    public ServiceProvider( Action<ServiceRegistration> configure )
    {
      var registration = new ServiceRegistration();
      configure( registration );

      _registration = registration.Build();
    }


    /// <summary>
    /// 获取指定类型的服务实例
    /// </summary>
    /// <param name="serviceType">服务类型</param>
    /// <returns>服务实例</returns>
    public object GetService( Type serviceType )
    {

      object GetServiceCore( Type type )
      {
        if ( _registration.TryGetValue( type, out var factory ) )
          return factory( this );

        else
          return null;
      }

      return _serviceProvider?.GetService( serviceType ) ?? GetServiceCore( serviceType );
    }



    /// <summary>
    /// 提供一个空白的服务容器
    /// </summary>
    public static IServiceProvider Empty { get; } = new ServiceProvider( builder => { } );

  }
}
