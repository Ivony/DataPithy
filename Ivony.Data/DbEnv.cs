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
    public IServiceProvider Services { get; private set; }

    /// <summary>
    /// 创建 DbEnv 对象
    /// </summary>
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

    private static DbEnv CreateDefault() => CreateEnvironment( services => { } );

    /// <summary>
    /// 创建一个新的数据库访问环境
    /// </summary>
    /// <returns></returns>
    public static DbEnv CreateEnvironment( Action<ServiceCollection> configureServices )
    {
      var services = new ServiceCollection();

      services.AddTransient( typeof( IParameterizedQueryBuilder ), typeof( ParameterizedQueryBuilder ) );
      services.AddSingleton( typeof( ITemplateParser ), typeof( TemplateParser ) );

      configureServices( services );

      return CreateEnvironment( services );

    }


    /// <summary>
    /// 创建一个新的数据库访问环境
    /// </summary>
    /// <param name="services">要注册在环境中的服务列表</param>
    /// <returns></returns>
    public static DbEnv CreateEnvironment( IServiceCollection services )
    {

      var instance = new DbEnv();
      services.AddSingleton( instance );
      instance.Services = services.BuildServiceProvider();

      return instance;
    }
  }
}
