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


      internal Builder( DbContext context )
      {
        DbContext = context;
        Properties = new Dictionary<string, object>( context.Properties );
        DbProvider = context.DbProvider;

        Services = new ServiceRegistryCollection();
      }



      /// <summary>
      /// 获取父级 DbContext 对象
      /// </summary>
      public DbContext DbContext { get; }



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


      /// <summary>
      /// 获取服务容器，用于服务注册。
      /// </summary>
      public ServiceRegistryCollection Services { get; }


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

        context.Parent = DbContext;

        context.AutoWhitespaceSeparator = autoWhiteSpace ?? DbContext?.AutoWhitespaceSeparator ?? false;

        context.Properties = new ReadOnlyDictionary<string, object>( Properties );
        context.Services = Services;

        context.DbProvider = DbProvider;

        return context;


      }
    }
  }
}