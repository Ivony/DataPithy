using System;

using Ivony.Data.Common;
using Ivony.Data.MySqlClient;

using Microsoft.Extensions.DependencyInjection;

#if MySqlConnector
using MySqlConnector;
#else
using MySql.Data.MySqlClient;
#endif

namespace Ivony.Data
{

  /// <summary>
  /// 提供 MySql 数据库支持
  /// </summary>
  public partial class MySqlDb : IDatabase
  {


    #region Connect




    /// <summary>
    /// 通过指定的连接字符串构建器创建 MySql 数据库访问器
    /// </summary>
    /// <param name="builder">连接字符串构建器</param>
    /// <param name="serviceProvider">服务提供程序</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDb Connect( MySqlConnectionStringBuilder builder, IServiceProvider serviceProvider = null )
    {
      return Connect( builder.ConnectionString, serviceProvider );
    }


    /// <summary>
    /// 通过指定的连接字符串构建器创建 MySql 数据库访问器
    /// </summary>
    /// <param name="action">创建连接字符串的方法</param>
    /// <param name="serviceProvider">服务提供程序</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDb Connect( Action<MySqlConnectionStringBuilder> action, IServiceProvider serviceProvider = null )
    {
      var builder = new MySqlConnectionStringBuilder();
      action( builder );
      return Connect( builder.ConnectionString, serviceProvider );
    }


    /// <summary>
    /// 通过指定的用户名和密码登陆 MySql 数据库，以创建 MySql 数据库访问器
    /// </summary>
    /// <param name="server">数据库服务器地址</param>
    /// <param name="database">数据库名称</param>
    /// <param name="userID">登录数据库的用户名</param>
    /// <param name="password">登录数据库的密码</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="serviceProvider">服务提供程序</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDb Connect( string server, string database, string userID, string password, bool pooling = true, IServiceProvider serviceProvider = null )
    {
      var builder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Database = database,
        UserID = userID,
        Password = password,
        Pooling = pooling
      };

      return Connect( builder, serviceProvider );
    }


    /// <summary>
    /// 通过指定的用户名和密码登陆 MySql 数据库，以创建 MySql 数据库访问器
    /// </summary>
    /// <param name="server">数据库服务器地址</param>
    /// <param name="port">数据库服务器端口</param>
    /// <param name="database">数据库名称</param>
    /// <param name="userID">登录数据库的用户名</param>
    /// <param name="password">登录数据库的密码</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="serviceProvider">服务提供程序</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDb Connect( string server, uint port, string database, string userID, string password, bool pooling = true, IServiceProvider serviceProvider = null )
    {
      var builder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Port = port,
        Database = database,
        UserID = userID,
        Password = password,
        Pooling = pooling
      };

      return Connect( builder, serviceProvider );
    }


    /// <summary>
    /// 通过集成身份验证登陆 MySql 数据库，以创建 MySql 数据库访问器
    /// </summary>
    /// <param name="server">数据库服务器地址</param>
    /// <param name="database">数据库名称</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="serviceProvider">服务提供程序</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDb Connect( string server, string database, bool pooling = true, IServiceProvider serviceProvider = null )
    {

      var builder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Database = database,
        Pooling = pooling
      };

      return Connect( builder, serviceProvider );
    }


    /// <summary>
    /// 通过集成身份验证登陆 MySql 数据库，以创建 MySql 数据库访问器
    /// </summary>
    /// <param name="server">数据库服务器地址</param>
    /// <param name="port">数据库服务器端口</param>
    /// <param name="database">数据库名称</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="serviceProvider">服务提供程序</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDb Connect( string server, uint port, string database, bool pooling = true, IServiceProvider serviceProvider = null )
    {

      var builder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Port = port,
        Database = database,
        Pooling = pooling
      };

      return Connect( builder, serviceProvider );
    }


    /// <summary>
    /// 通过指定的连接字符串并创建 MySql 数据库访问器
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="serviceProvider">服务提供程序</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDb Connect( string connectionString, IServiceProvider serviceProvider = null )
    {
      return new MySqlDb( connectionString, serviceProvider );
    }



    #endregion Connect




    /// <summary>
    /// 创建 <see cref="MySqlDb"/> 对象
    /// </summary>
    /// <param name="serviceProvider">系统服务提供程序</param>
    /// <param name="connectionString">连接字符串</param>
    private MySqlDb( string connectionString, IServiceProvider serviceProvider )
    {
      ConnectionString = connectionString ?? throw new ArgumentNullException( nameof( connectionString ) );
      ServiceProvider = CreateServiceProvider( serviceProvider );
    }

    private IServiceProvider CreateServiceProvider( IServiceProvider serviceProvider ) => new MySqlDbServiceProvider( this, serviceProvider );


    /// <summary>
    /// MySQL 数据库连接字符串
    /// </summary>
    public string ConnectionString { get; }


    /// <summary>
    /// 系统服务提供程序
    /// </summary>
    public IServiceProvider ServiceProvider { get; }


    /// <summary>
    /// 创建事务
    /// </summary>
    /// <returns>事务上下文</returns>
    public IDatabaseTransaction CreateTransaction()
    {
      return new MySqlDbTransaction( this );
    }

    /// <summary>
    /// 获取查询执行器
    /// </summary>
    /// <returns>查询执行器</returns>
    public IDbExecutor GetDbExecutor()
    {
      return new MySqlDbExecutor( this );
    }



    /// <summary>
    /// 创建 MySQL 数据库连接
    /// </summary>
    /// <returns>MySQL 数据库连接对象</returns>
    public MySqlConnection CreateConnection() => ServiceProvider.GetRequiredService<IDbConnectionFactory<MySqlConnection>>().CreateConnection( ConnectionString );
  }
}
