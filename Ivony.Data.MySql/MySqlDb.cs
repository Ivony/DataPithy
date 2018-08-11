using Ivony.Data.Common;
using Ivony.Data.MySqlClient;
using Ivony.Data.Queries;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public static DatabaseContext.Builder UseMySql( this DatabaseContext.Builder builder, string connectionString )
    {
      builder.SetDbProvider( new MySqlDbProvider( connectionString ) );

      return builder;
    }



    /// <summary>
    /// 通过指定的连接字符串构建器创建 MySql 数据库访问器
    /// </summary>
    /// <param name="builder">连接字符串构建器</param>
    /// <param name="configuration">MySql 配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static DatabaseContext.Builder UseMySql( this DatabaseContext.Builder builder, MySqlConnectionStringBuilder connectionBuilder )
    {
      return UseMySql( builder, connectionBuilder.ConnectionString );
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
    public static DatabaseContext.Builder UseMySql( this DatabaseContext.Builder builder, string server, string database, string userID, string password, bool pooling = true )
    {
      var connectionBuilder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Database = database,
        UserID = userID,
        Password = password,
        Pooling = pooling
      };

      return UseMySql( builder, connectionBuilder.ConnectionString );
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
    public static DatabaseContext.Builder UseMySql( this DatabaseContext.Builder builder, string server, uint port, string database, string userID, string password, bool pooling = true )
    {
      var connectionBuilder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Database = database,
        UserID = userID,
        Password = password,
        Pooling = pooling
      };

      return UseMySql( builder, connectionBuilder.ConnectionString );
    }


    /// <summary>
    /// 通过集成身份验证登陆 MySql 数据库，以创建 MySql 数据库访问器
    /// </summary>
    /// <param name="server">数据库服务器实例名称</param>
    /// <param name="database">数据库名称</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="configuration">MySql 数据库配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static DatabaseContext.Builder UseMySql( this DatabaseContext.Builder builder, string server, string database, bool pooling = true )
    {

      var connectionBuilder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Database = database,
        Pooling = pooling
      };

      return UseMySql( builder, connectionBuilder.ConnectionString );
    }




    /// <summary>
    /// 通过集成身份验证登陆 MySql 数据库，以创建 MySql 数据库访问器
    /// </summary>
    /// <param name="dataSource">数据库服务器地址</param>
    /// <param name="port">数据库服务器端口</param>
    /// <param name="database">数据库名称</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <param name="configuration">MySql 数据库配置</param>
    /// <returns>MySql 数据库访问器</returns>
    public static DatabaseContext.Builder UseMySql( this DatabaseContext.Builder builder, string server, uint port, string database, bool pooling = true )
    {

      var connectionBuilder = new MySqlConnectionStringBuilder()
      {
        Server = server,
        Port = port,
        Database = database,
        Pooling = pooling
      };

      return UseMySql( builder, connectionBuilder.ConnectionString );
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
