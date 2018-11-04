using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Ivony.Data
{


  public partial class DbContext
  {

    /// <summary>
    /// 辅助构建 DbContext 对象
    /// </summary>
    public class Builder
    {

      internal Builder()
      {
      }


      internal Builder( DbContext parent )
      {
        Parent = parent;
        Properties = new Dictionary<string, object>( parent.Properties );
        DbProvider = parent.DbProvider;
      }



      /// <summary>
      /// 获取父级 DbContext 对象
      /// </summary>
      protected DbContext Parent { get; }



      /// <summary>
      /// 获取数据访问提供程序
      /// </summary>
      protected IDbProvider DbProvider { get; private set; }


      /// <summary>
      /// 设置数据访问提供程序
      /// </summary>
      /// <param name="provider">数据访问提供程序</param>
      /// <returns>设置的数据访问提供程序</returns>
      public Builder SetDbProvider( IDbProvider provider )
      {
        DbProvider = provider;
        return this;
      }




      bool? autoWhiteSpace;
      /// <summary>
      /// 设置自动添加空白分隔符设定
      /// </summary>
      public Builder SetAutoWhitespaceSeparator( bool value )
      {
        autoWhiteSpace = value;
        return this;
      }


      private IDictionary<Type, object> services = new Dictionary<Type, object>();



      private static void ChecktInstanceType( Type serviceType, Type instanceType )
      {
        if ( serviceType.IsAssignableFrom( instanceType ) == false )
          throw new InvalidOperationException( $"Type of instance is \"{instanceType}\", it's cannot register for a service type of \"{serviceType}\"" );
      }


      /// <summary>
      /// 注册一个服务
      /// </summary>
      /// <param name="serviceType">服务类型</param>
      /// <param name="instanceType">实现类型</param>
      /// <returns>DbContext构建器</returns>
      public Builder RegisterService( Type serviceType, Type instanceType )
      {
        if ( serviceType == null )
          throw new ArgumentNullException( nameof( serviceType ) );
        if ( instanceType == null )
          throw new ArgumentNullException( nameof( instanceType ) );

        ChecktInstanceType( serviceType, instanceType );

        services[serviceType] = instanceType;

        return this;
      }


      /// <summary>
      /// 注册一个服务
      /// </summary>
      /// <param name="serviceType"></param>
      /// <param name="serviceInstance"></param>
      /// <returns></returns>
      public Builder RegisterService( Type serviceType, object serviceInstance )
      {
        if ( serviceType == null )
          throw new ArgumentNullException( nameof( serviceType ) );
        if ( serviceInstance == null )
          throw new ArgumentNullException( nameof( serviceInstance ) );


        var instanceType = serviceInstance.GetType();
        ChecktInstanceType( serviceType, instanceType );
        services[serviceType] = serviceInstance;

        return this;
      }


      /// <summary>
      /// 注册一个服务
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="serviceInstance"></param>
      /// <returns></returns>
      public Builder RegisterService<T>( T serviceInstance )
      {
        if ( serviceInstance == null )
          throw new ArgumentNullException( nameof( serviceInstance ) );

        services[typeof( T )] = serviceInstance;

        return this;
      }


      /// <summary>
      /// 注册一个服务
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="serviceFactory"></param>
      /// <returns></returns>
      public Builder RegisterService<T>( Func<T> serviceFactory )
      {
        if ( serviceFactory == null )
          throw new ArgumentNullException( nameof( serviceFactory ) );

        services[typeof( T )] = serviceFactory;

        return this;
      }



      /// <summary>
      /// 获取或设置数据上下文属性信息
      /// </summary>
      protected IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();


      /// <summary>
      /// 设置上下文属性
      /// </summary>
      /// <param name="name">属性名</param>
      /// <param name="value">属性值</param>
      /// <returns></returns>
      public Builder SetProperty( string name, object value )
      {
        Properties[name] = value;
        return this;
      }



      internal DbContext Build()
      {
        DbContext context = new DbContext();

        context.Parent = Parent;

        context.AutoWhitespaceSeparator = autoWhiteSpace ?? Parent?.AutoWhitespaceSeparator ?? false;

        context.services = new ReadOnlyDictionary<Type, object>( services );
        context.Properties = new ReadOnlyDictionary<string, object>( Properties );

        context.DbProvider = DbProvider;

        return context;


      }
    }
  }
}