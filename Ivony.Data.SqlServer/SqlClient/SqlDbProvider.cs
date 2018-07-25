using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlClient
{
  /// <summary>
  /// 基于 SQL Server 数据库访问的 IDbProvider 实现
  /// </summary>
  public class SqlDbProvider : IDbProvider
  {


    private string _connectionString;


    public IServiceProvider Services { get; }

    /// <summary>
    /// 创建 SqlDbProvider 对象
    /// </summary>
    /// <param name="connectionString">SQL Server 连接字符串</param>
    public SqlDbProvider( IServiceProvider serviceProvider, string connectionString )
    {
      _connectionString = connectionString;

      Services = serviceProvider;
    }

    /// <summary>
    /// 获取指定类型查询的执行器
    /// </summary>
    /// <typeparam name="T">要执行的查询类型</typeparam>
    /// <returns>数据库查询执行器</returns>
    public IDbExecutor<T> GetDbExecutor<T>() where T : IDbQuery
    {
      return SqlServer.Connect( Services, _connectionString ) as IDbExecutor<T>;
    }


    /// <summary>
    /// 获取指定类型查询的异步执行器
    /// </summary>
    /// <typeparam name="T">要执行的查询类型</typeparam>
    /// <returns>异步查询执行器</returns>

    public IAsyncDbExecutor<T> GetAsyncDbExecutor<T>() where T : IDbQuery
    {
      return SqlServer.Connect( Services, _connectionString ) as IAsyncDbExecutor<T>;
    }
  }
}
