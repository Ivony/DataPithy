using Ivony.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivony.Data.Common;
using System.IO;
using Ivony.Data.Queries;

namespace Ivony.Data
{

  /// <summary>
  /// 提供 SQL Server 数据库访问支持
  /// </summary>
  public class SqlServerDb : IDatabase
  {


    /// <summary>
    /// 创建 SqlServerDbProvider 对象
    /// </summary>
    /// <param name="connectionString">SQL Server 连接字符串</param>
    public SqlServerDb( string connectionString )
      : this( Ivony.Data.ServiceProvider.Empty, connectionString ) { }

    /// <summary>
    /// 创建 SqlServerDbProvider 对象
    /// </summary>
    /// <param name="serviceProvider">系统服务提供程序</param>
    /// <param name="connectionString">SQL Server 连接字符串</param>
    public SqlServerDb( IServiceProvider serviceProvider, string connectionString )
    {
      ServiceProvider = serviceProvider;
      ConnectionString = connectionString ?? throw new ArgumentNullException( nameof( connectionString ) );
    }

    /// <summary>
    /// 获取服务提供程序
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// 连接字符串
    /// </summary>
    public string ConnectionString { get; private set; }

    /// <summary>
    /// 获取 SQL Server 查询执行器
    /// </summary>
    /// <returns></returns>
    public IDbExecutor GetDbExecutor()
    {
      return new SqlDbExecutor( this );
    }

    /// <summary>
    /// 创建数据库事务
    /// </summary>
    /// <returns></returns>
    public IDatabaseTransaction CreateTransaction()
    {
      return new SqlServerDatabaseTransaction( this );
    }



    /// <summary>
    /// 通过指定的连接字符串并创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlServerDb Connect( string connectionString )
    {
      return new SqlServerDb( connectionString );
    }

    /// <summary>
    /// 通过指定的用户名和密码登陆 SQL Server 数据库，以创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="dataSource">数据库服务器实例名称</param>
    /// <param name="initialCatalog">数据库名称</param>
    /// <param name="userID">登录数据库的用户名</param>
    /// <param name="password">登录数据库的密码</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlServerDb Connect( string dataSource, string initialCatalog, string userID, string password, bool pooling = true )
    {
      var builder = new SqlConnectionStringBuilder()
      {
        DataSource = dataSource,
        InitialCatalog = initialCatalog,
        IntegratedSecurity = false,
        UserID = userID,
        Password = password,
        Pooling = pooling
      };


      return Connect( builder );
    }

    /// <summary>
    /// 通过集成身份验证登陆 SQL Server 数据库，以创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="dataSource">数据库服务器实例名称</param>
    /// <param name="initialCatalog">数据库名称</param>
    /// <param name="pooling">是否启用连接池（默认启用）</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlServerDb Connect( string dataSource, string initialCatalog, bool pooling = true )
    {
      var builder = new SqlConnectionStringBuilder()
      {
        DataSource = dataSource,
        InitialCatalog = initialCatalog,
        IntegratedSecurity = true,
        Pooling = pooling
      };

      return Connect( builder );
    }

    /// <summary>
    /// 通过指定的连接字符串并创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="builder">数据库连接字符串构建器</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlServerDb Connect( SqlConnectionStringBuilder builder )
    {
      return Connect( builder.ConnectionString );
    }

    /// <summary>
    /// 通过指定的连接字符串并创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="configure">配置数据库连接字符串的方法</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlServerDb Connect( Action<SqlConnectionStringBuilder> configure )
    {
      var builder = new SqlConnectionStringBuilder();
      configure( builder );
      return Connect( builder.ConnectionString );
    }






    static SqlServerDb()
    {
      LocalDBInstanceName = "v11.0";
      ExpressInstanceName = "SQLEXPRESS";
    }


    internal static string LocalDBInstanceName { get; set; }

    internal static string ExpressInstanceName { get; set; }
  }
}
