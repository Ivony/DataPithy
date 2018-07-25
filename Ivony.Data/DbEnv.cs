using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Data.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data
{


  /// <summary>
  /// 定义数据访问环境
  /// </summary>
  public class DbEnv
  {

    /// <summary>
    /// 获取服务提供程序
    /// </summary>
    internal IServiceProvider Services { get; private set; }

    /// <summary>
    /// 创建 DbEnv 对象
    /// </summary>
    /// <param name="services">服务提供程序</param>
    private DbEnv() { }


    /// <summary>
    /// 获取数据库访问提供程序
    /// </summary>
    /// <param name="connectionName">连接名称或连接字符串</param>
    /// <returns>数据库访问提供程序</returns>
    public IDbProvider GetDbProvider( string connectionName ) => Services.GetService<IDbProviderFactory>().GetDbProvider( connectionName );


    /// <summary>
    /// 解析模板表达式，创建参数化查询对象
    /// </summary>
    /// <param name="template">参数化模板</param>
    /// <returns>参数化查询</returns>
    public ParameterizedQuery Template( FormattableString template )
    {
      return Services.GetService<ITemplateParser>().ParseTemplate( template );
    }


    /// <summary>
    /// 解析模板表达式，创建参数化查询对象
    /// </summary>
    /// <param name="template">参数化模板</param>
    /// <returns>参数化查询</returns>
    public ParameterizedQuery T( FormattableString template )
    {
      return Template( template );
    }


    /// <summary>
    /// 获取默认的数据库访问基础对象
    /// </summary>
    public static DbEnv Default { get; } = CreateDefault();

    private static DbEnv CreateDefault()
    {
      return CreateEnvironment( services =>
      {
        services.AddSingleton( typeof( IParameterizedQueryBuilder ), typeof( ParameterizedQueryBuilder ) );
        services.AddSingleton( typeof( ITemplateParser ), typeof( TemplateParser ) );
      } );

    }

    /// <summary>
    /// 创建一个新的数据库访问环境
    /// </summary>
    /// <param name="configureServices"></param>
    /// <returns></returns>
    public static DbEnv CreateEnvironment( Action<ServiceCollection> configureServices )
    {
      var services = new ServiceCollection();
      configureServices( services );

      var instance = new DbEnv();

      services.AddSingleton<DbEnv>( instance );
      instance.Services = services.BuildServiceProvider();

      return instance;
    }



  }
}
