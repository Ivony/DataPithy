using Ivony.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ivony.Data
{

  /// <summary>
  /// 提供 SQL Server Express 支持
  /// </summary>
  public static class SqlServerExpressDb
  {
    /// <summary>
    /// 通过连接 SQL Server Express LocalDB 实例，创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="database">数据库名称或者数据库文件路径</param>
    /// <param name="configuration">SQL Server 配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlDbExecutor LocalDB( this DbEnv environment, string database )
    {

      var configuration = environment.Services.GetService<IOptions<SqlDbConfiguration>>().Value ?? SqlServerExpressDb.Configuration;
      return SqlExpress( environment, database, @"(LocalDB)\" + configuration.LocalDBInstanceName );

    }



    /// <summary>
    /// 通过连接 SQL Server Express 默认实例，创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="database">数据库名称或者数据库文件路径</param>
    /// <param name="configuration">SQL Server 配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    public static SqlDbExecutor SqlExpress( this DbEnv environment, string database )
    {
      var configuration = environment.Services.GetService<IOptions<SqlDbConfiguration>>().Value ?? SqlServerExpressDb.Configuration;
      return SqlExpress( environment, database, @"(local)\" + configuration.ExpressInstanceName );
    }


    /// <summary>
    /// 通过连接 SQL Server Express 指定实例，创建 SQL Server 数据库访问器
    /// </summary>
    /// <param name="database">数据库名称或者数据库文件路径</param>
    /// <param name="datasource">SQL Server 实例名称</param>
    /// <param name="configuration">SQL Server 配置</param>
    /// <returns>SQL Server 数据库访问器</returns>
    private static SqlDbExecutor SqlExpress( this DbEnv environment, string database, string datasource )
    {
      var builder = new SqlConnectionStringBuilder()
      {
        DataSource = datasource,
        IntegratedSecurity = true,
      };


      if ( database.IndexOfAny( Path.GetInvalidPathChars() ) == -1 && Path.IsPathRooted( database ) )
        builder.AttachDBFilename = database;

      else
        builder.InitialCatalog = database;


      return SqlServerDb.SqlServer( environment, builder.ConnectionString );
    }


    /// <summary>
    /// 获取或修改 SQL Server 默认配置
    /// </summary>
    public static SqlDbConfiguration Configuration
    {
      get { return SqlServerDb.Configuration; }
    }
  }
}
