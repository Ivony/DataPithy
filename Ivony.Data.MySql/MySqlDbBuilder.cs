using System;
using Microsoft.Extensions.DependencyInjection;

#if MySqlConnector
using MySqlConnector;
#else
using MySql.Data.MySqlClient;
#endif

namespace Ivony.Data;

/// <summary>
/// 提供构建 MySQL 数据库访问器的构建器类
/// </summary>
public class MySqlDbBuilder
{
    private readonly MySqlConnectionStringBuilder _connectionStringBuilder = new();
    private readonly IServiceCollection _serviceCollection;

    /// <summary>
    /// 创建 <see cref="MySqlDbBuilder"/> 实例
    /// </summary>
    internal MySqlDbBuilder(IServiceCollection services)
    {
        _serviceCollection = services;
    }

    /// <summary>
    /// 设置连接字符串或使用连接字符串构建器配置连接
    /// </summary>
    /// <param name="connectionString">MySQL 连接字符串</param>
    /// <returns>当前构建器实例，支持链式调用</returns>
    public MySqlDbBuilder WithConnection(string connectionString)
    {
        _connectionStringBuilder.ConnectionString = connectionString;
        return this;
    }

    /// <summary>
    /// 使用连接字符串构建器配置连接
    /// </summary>
    /// <param name="builder">MySQL 连接字符串构建器</param>
    /// <returns>当前构建器实例，支持链式调用</returns>
    public MySqlDbBuilder WithConnection(MySqlConnectionStringBuilder builder)
    {
        _connectionStringBuilder.ConnectionString = builder.ConnectionString;
        return this;
    }

    /// <summary>
    /// 设置服务器地址
    /// </summary>
    /// <param name="server">服务器地址</param>
    /// <returns>当前构建器实例，支持链式调用</returns>
    public MySqlDbBuilder WithServer(string server)
    {
        _connectionStringBuilder.Server = server;
        return this;
    }

    /// <summary>
    /// 设置服务器端口
    /// </summary>
    /// <param name="port">服务器端口</param>
    /// <returns>当前构建器实例，支持链式调用</returns>
    public MySqlDbBuilder WithPort(uint port)
    {
        _connectionStringBuilder.Port = port;
        return this;
    }

    /// <summary>
    /// 设置数据库名称
    /// </summary>
    /// <param name="database">数据库名称</param>
    /// <returns>当前构建器实例，支持链式调用</returns>
    public MySqlDbBuilder WithDatabase(string database)
    {
        _connectionStringBuilder.Database = database;
        return this;
    }

    /// <summary>
    /// 设置登录凭据
    /// </summary>
    /// <param name="userId">用户名</param>
    /// <param name="password">密码</param>
    /// <returns>当前构建器实例，支持链式调用</returns>
    public MySqlDbBuilder WithCredentials(string userId, string password)
    {
        _connectionStringBuilder.UserID = userId;
        _connectionStringBuilder.Password = password;
        return this;
    }

    /// <summary>
    /// 设置是否启用连接池
    /// </summary>
    /// <param name="pooling">是否启用连接池</param>
    /// <returns>当前构建器实例，支持链式调用</returns>
    public MySqlDbBuilder WithPooling(bool pooling = true)
    {
        _connectionStringBuilder.Pooling = pooling;
        return this;
    }

    /// <summary>
    /// 配置服务集合，可在构建过程中注册或替换内置服务
    /// </summary>
    /// <param name="configureAction">配置服务集合的委托</param>
    /// <returns>当前构建器实例，支持链式调用</returns>
    public MySqlDbBuilder ConfigureServices(Action<IServiceCollection> configureAction)
    {
        configureAction(_serviceCollection);
        return this;
    }

    /// <summary>
    /// 构建 IDatabase 对象
    /// </summary>
    /// <returns>构建好的 IDatabase 对象</returns>
    public IDatabase Build()
    {
        var connectionString = _connectionStringBuilder.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string is not configured.");
        }

        // 直接使用内部服务集合创建服务提供程序
        return new MySqlDb(connectionString, _serviceCollection.BuildServiceProvider());
    }
}