using System;
using Ivony.Data.MySqlClient;
using MySql.Data.MySqlClient;

namespace Ivony.Data
{

  /// <summary>
  /// 提供 MySql 数据库支持
  /// </summary>
  public static class MySqlDb
  {




    /// <summary>
    /// 通过指定的连接字符串并创建 MySql 数据库访问器
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="configuration">MySql 配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDbProvider Connect( string connectionString, MySqlDbConfiguration configuration = null )
    {
      return MySqlDbProvider.Create( connectionString, configuration ?? DefaultConfiguration );
    }



    /// <summary>
    /// 通过指定的连接字符串构建器创建 MySql 数据库访问器
    /// </summary>
    /// <param name="builder">连接字符串构建器</param>
    /// <param name="configuration">MySql 配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDbProvider Connect( MySqlConnectionStringBuilder builder, MySqlDbConfiguration configuration = null )
    {
      return Connect( builder.GetConnectionString( true ), configuration );
    }





    /// <summary>
    /// 使用 MySQL 数据库
    /// </summary>
    /// <param name="serviceProvider">系统服务提供程序</param>
    /// <param name="connectionString">数据库连接字符串</param>
    /// <returns>MySQL 数据库数据提供程序</returns>
    public static IDbProvider UseMySql( this IServiceProvider serviceProvider, string connectionString )
    {
      return UseMySql( serviceProvider, null, connectionString );
    }



    /// <summary>
    /// 使用 MySQL 数据库
    /// </summary>
    /// <param name="serviceProvider">系统服务提供程序</param>
    /// <param name="databaseName">数据库名称</param>
    /// <param name="connectionString">数据库连接字符串</param>
    /// <returns>MySQL 数据库数据提供程序</returns>
    public static IDbProvider UseMySql( this IServiceProvider serviceProvider, string databaseName, string connectionString )
    {
      var db = new MySqlDbProvider( serviceProvider, connectionString );
      if ( databaseName != null )
        Db.RegisterDatabase( databaseName, db, true );

      else
        Db.UseDatabase( db );

      return db;
    }



    /// <summary>
    /// 通过指定的连接字符串构建器创建 MySql 数据库访问器
    /// </summary>
    /// <param name="action">创建连接字符串的方法</param>
    /// <param name="configuration">MySql 配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDbProvider Connect( Action<MySqlConnectionStringBuilder> action, MySqlDbConfiguration configuration = null )
    {
      var builder = new MySqlConnectionStringBuilder();
      action( builder );
      return Connect( builder.GetConnectionString( true ), configuration );
    }



    /// <summary>
    /// 通过指定的用户名和密码登陆 MySql 数据库，以创建 MySql 数据库访问器
    /// </summary>
    /// <param name="server">数据库服务器地址</param>
    /// <param name="database">数据库名称</param>
    /// <param name="userID">登录数据库的用户名</param>
    /// <param name="password">登录数据库的密码</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="configuration">MySql 数据库配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDbProvider Connect( string server, string database, string userID, string password, bool pooling = true, MySqlDbConfiguration configuration = null )
    {
      var builder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Database = database,
        UserID = userID,
        Password = password,
        Pooling = pooling
      };

      return Connect( builder, configuration );
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
    /// <param name="configuration">MySql 数据库配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDbProvider Connect( string server, uint port, string database, string userID, string password, bool pooling = true, MySqlDbConfiguration configuration = null )
    {
      var builder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Database = database,
        UserID = userID,
        Password = password,
        Pooling = pooling
      };

      return Connect( builder, configuration );
    }


    /// <summary>
    /// 通过集成身份验证登陆 MySql 数据库，以创建 MySql 数据库访问器
    /// </summary>
    /// <param name="server">数据库服务器地址</param>
    /// <param name="database">数据库名称</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="configuration">MySql 数据库配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDbProvider Connect( string server, string database, bool pooling = true, MySqlDbConfiguration configuration = null )
    {

      var builder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Database = database,
        Pooling = pooling
      };

      return Connect( builder, configuration );
    }




    /// <summary>
    /// 通过集成身份验证登陆 MySql 数据库，以创建 MySql 数据库访问器
    /// </summary>
    /// <param name="server">数据库服务器地址</param>
    /// <param name="port">数据库服务器端口</param>
    /// <param name="database">数据库名称</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="configuration">MySql 数据库配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static MySqlDbProvider Connect( string server, uint port, string database, bool pooling = true, MySqlDbConfiguration configuration = null )
    {

      var builder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Port = port,
        Database = database,
        Pooling = pooling
      };

      return Connect( builder, configuration );
    }





    static MySqlDb()
    {
      DefaultConfiguration = new MySqlDbConfiguration();
    }


    /// <summary>
    /// 获取或设置默认配置
    /// </summary>
    public static MySqlDbConfiguration DefaultConfiguration
    {
      get;
      set;
    }


    /// <summary>
    /// 获取或设置 MySql 全局设置
    /// </summary>
    public static MySqlConfiguration MySqlGlobalSettings
    {
      get { return MySqlConfiguration.Settings; }
    }


  }


}
