using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data;

/// <summary>
/// 为 DatabaseManager 提供 MySQL 数据库扩展方法
/// </summary>
public static class MySqlDatabaseManagerExtensions
{
    /// <summary>
    /// 为 DatabaseManager 添加 MySQL 数据库
    /// </summary>
    /// <param name="manager">数据库管理器</param>
    /// <param name="name">数据库名称</param>
    /// <param name="configure">配置 MySQL 数据库构建器的委托</param>
    /// <returns>当前数据库管理器实例，支持链式调用</returns>
    public static DatabaseManager AddMySql(this DatabaseManager manager, string name, Action<MySqlDbBuilder> configure)
    {
        if (manager == null)
            throw new ArgumentNullException(nameof(manager));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Database name cannot be null or whitespace.", nameof(name));

        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        // 注册数据库，使用委托创建数据库实例
        return manager.RegisterDatabase(name, services =>
        {
            // 利用DatabaseManager给出的IServiceCollection对象实例创建MySqlDbBuilder对象
            var builder = new MySqlDbBuilder(services);

            // 调用configure参数给出的方法对MySqlDbBuilder对象做修饰
            configure(builder);

            // 最后调用MySqlDbBuilder对象的Build方法
            return builder.Build();
        });
    }

    /// <summary>
    /// 为 DatabaseManager 添加 MySQL 数据库，使用连接字符串配置
    /// </summary>
    /// <param name="manager">数据库管理器</param>
    /// <param name="name">数据库名称</param>
    /// <param name="connectionString">MySQL 连接字符串</param>
    /// <returns>当前数据库管理器实例，支持链式调用</returns>
    public static DatabaseManager AddMySql(this DatabaseManager manager, string name, string connectionString)
    {
        if (manager == null)
            throw new ArgumentNullException(nameof(manager));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Database name cannot be null or whitespace.", nameof(name));

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or whitespace.", nameof(connectionString));

        // 注册数据库，使用委托创建数据库实例
        return manager.RegisterDatabase(name, services =>
        {
            // 利用DatabaseManager给出的IServiceCollection对象实例创建MySqlDbBuilder对象
            var builder = new MySqlDbBuilder(services);

            // 配置连接字符串
            builder.WithConnection(connectionString);

            // 最后调用MySqlDbBuilder对象的Build方法
            return builder.Build();
        });
    }

}