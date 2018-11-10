using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Ivony.Data
{
  /// <summary>
  /// 定义数据库访问提供程序选择器
  /// </summary>
  public interface IDbProviderSelector
  {

    /// <summary>
    /// 获取数据库提供程序
    /// </summary>
    /// <param name="name">数据库连接字符串名称</param>
    /// <param name="connectionString">数据库连接字符串</param>
    /// <returns></returns>
    IDbProvider GetDbProvider( string name, string connectionString );

  }


  /// <summary>
  /// IDbProviderSelector 默认实现
  /// </summary>
  public class DbProviderSelector : IDbProviderSelector
  {


    private static Lazy<Type[]> providerTypes;

    static DbProviderSelector()
    {
      providerTypes = new Lazy<Type[]>( () =>
      {

        return AppDomain
          .CurrentDomain.GetAssemblies().SelectMany( assembly => assembly.GetExportedTypes() )
          .Where( type => typeof( IDbProvider ).IsAssignableFrom( type ) )
          .ToArray();

      } );
    }


    /// <summary>
    /// 获取数据库提供程序
    /// </summary>
    /// <param name="name">数据库连接字符串名称</param>
    /// <param name="connectionString">数据库连接字符串</param>
    public IDbProvider GetDbProvider( string name, string connectionString )
    {

      var type = GetDbProviderType( name, connectionString );
      if ( type == null )
        return null;


      return null;

    }


    protected virtual Type GetDbProviderType( string name, string connectionString )
    {
      var types = providerTypes.Value;
      if ( types.Length == 1 )
        return types[0];

      return null;
    }
  }
}
