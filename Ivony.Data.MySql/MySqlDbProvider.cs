using System;
using System.Collections.Generic;
using Ivony.Data.Common;
using Ivony.Data.MySqlClient;

using MySql.Data.MySqlClient;

namespace Ivony.Data
{

  /// <summary>
  /// MySQL 数据库访问提供程序
  /// </summary>
  public class MySqlDbProvider : IDbProvider
  {

    /// <summary>
    /// 创建 MySqlDbProvider 对象
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="configuration">MySQL 数据库配置信息</param>
    public static MySqlDbProvider Create( string connectionString, MySqlDbConfiguration configuration )
    {
      return new MySqlDbProvider( new ServiceProvider( builder => ConfigureServices( builder.AddService( configuration ) ) ), connectionString );
    }


    /// <summary>
    /// 创建 MySqlDbProvider 对象
    /// </summary>
    /// <param name="connectionStringProvider">连接字符串提供程序</param>
    public MySqlDbProvider( IDbConnectionStringProvider connectionStringProvider )
      : this( connectionStringProvider.ServiceProvider.Expand( ConfigureServices ), connectionStringProvider.GetConnectionString( typeof( MySqlDbProvider ) ) )
    {
    }


    /// <summary>
    /// 创建 MySqlDbProvider 对象
    /// </summary>
    /// <param name="serviceProvider">系统服务提供程序</param>
    /// <param name="connectionString">连接字符串</param>
    public MySqlDbProvider( IServiceProvider serviceProvider, string connectionString )
    {
      ConnectionString = connectionString ?? throw new ArgumentNullException( nameof( connectionString ) );
      ServiceProvider = serviceProvider;
    }


    /// <summary>
    /// MySQL 数据库连接字符串
    /// </summary>
    public string ConnectionString { get; }


    /// <summary>
    /// 系统服务提供程序
    /// </summary>
    public IServiceProvider ServiceProvider { get; }


    /// <summary>
    /// MySQL 数据库配置信息
    /// </summary>
    public MySqlDbConfiguration Configuration => ServiceProvider.GetService<MySqlDbConfiguration>();


    /// <summary>
    /// 创建事务
    /// </summary>
    /// <returns>事务上下文</returns>
    public IDbTransactionContext CreateTransaction()
    {
      return new MySqlDbTransactionContext( this );
    }

    /// <summary>
    /// 获取查询执行器
    /// </summary>
    /// <returns>查询执行器</returns>
    public IDbExecutor GetDbExecutor()
    {
      return new MySqlDbExecutor( this, ConnectionString );
    }




    private static void ConfigureServices( ServiceProvider.ServiceRegistration registration )
    {
      registration.AddService<IParameterizedQueryParser<MySqlCommand>>( new MySqlParameterizedQueryParser() );
    }


  }
}
