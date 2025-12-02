using System;

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

        // 创建并配置 MySqlDbBuilder
        var builder = new MySqlDbBuilder();
        configure(builder);

        // 构建数据库实例
        var database = builder.Build();

        // 添加到数据库管理器
        return manager.AddDatabase(name, database);
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
        return AddMySql(manager, name, builder => builder.WithConnection(connectionString));
    }


}