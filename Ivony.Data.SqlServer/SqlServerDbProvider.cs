using Ivony.Data.Common;
using Ivony.Data.Queries;
using Ivony.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{

  /// <summary>
  /// SqlServer 数据库提供程序
  /// </summary>
  public class SqlServerDbProvider : IDbProvider
  {
    /// <summary>
    /// 创建 SqlServerDbProvider 对象
    /// </summary>
    /// <param name="connectionString">SQL Server 连接字符串</param>
    public SqlServerDbProvider( string connectionString ) : this( new EmptyServiceProvider(), connectionString ) { }

    /// <summary>
    /// 创建 SqlServerDbProvider 对象
    /// </summary>
    /// <param name="serviceProvider">系统服务提供程序</param>
    /// <param name="connectionString">SQL Server 连接字符串</param>
    public SqlServerDbProvider( IServiceProvider serviceProvider, string connectionString )
    {
      ServiceProvider = serviceProvider;
      ConnectionString = connectionString ?? throw new ArgumentNullException( nameof( connectionString ) );
    }


    private class EmptyServiceProvider : IServiceProvider
    {
      public object GetService( Type serviceType )
      {
        return null;
      }
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
    public IDbTransactionContext CreateTransaction()
    {
      return new SqlServerTransactionContext( this );
    }


  }
}
