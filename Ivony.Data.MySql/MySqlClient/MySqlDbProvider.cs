using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.MySqlClient
{
  /// <summary>
  /// 基于 MySQL 数据库访问的 IDbProvider 实现
  /// </summary>
  public class MySqlDbProvider : IDbProvider
  {

    private string _connectionString;
    private IServiceProvider _serviceProvider;

    /// <summary>
    /// 创建 MySqlDbProvider 对象
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    public MySqlDbProvider( IServiceProvider serviceProvider, string connectionString )
    {
      _connectionString = connectionString;
      _serviceProvider = serviceProvider;
    }




    /// <summary>
    /// 获取当前上下文服务
    /// </summary>
    public IServiceProvider Services { get; }


    /// <summary>
    /// 获取可以执行指定类型查询的异步查询执行器
    /// </summary>
    /// <typeparam name="T">所需要查询的查询类型</typeparam>
    /// <returns>数据库查询执行器</returns>
    public IAsyncDbExecutor<T> GetAsyncDbExecutor<T>( T query ) where T : IDbQuery
    {
      return (IAsyncDbExecutor<T>) new MySqlDbExecutor( _connectionString, GetConfiguration() );
    }

    private MySqlDbConfiguration GetConfiguration()
    {
      return _serviceProvider.GetService<IOptions<MySqlDbConfiguration>>()?.Value
        ?? ActivatorUtilities.CreateInstance<MySqlDbConfiguration>( _serviceProvider );
    }


    /// <summary>
    /// 获取可以执行指定类型查询的查询执行器
    /// </summary>
    /// <typeparam name="T">所需要查询的查询类型</typeparam>
    /// <returns>数据库查询执行器</returns>
    public IDbExecutor<T> GetDbExecutor<T>( T query ) where T : IDbQuery
    {
      return (IAsyncDbExecutor<T>) new MySqlDbExecutor( _connectionString, GetConfiguration() );
    }
  }
}
