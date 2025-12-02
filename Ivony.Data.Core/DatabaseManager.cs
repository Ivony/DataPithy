using System;
using System.Collections.Generic;

namespace Ivony.Data;

/// <summary>
/// 数据库管理器，用于管理多个数据库连接
/// </summary>
public class DatabaseManager : IDatabaseProvider
{
    private readonly Dictionary<string, IDatabase> _databases = new();
    private IDatabase? _defaultDatabase;

    /// <summary>
    /// 添加数据库到管理器
    /// </summary>
    /// <param name="name">数据库名称</param>
    /// <param name="database">数据库实例</param>
    /// <param name="isDefault">是否设置为默认数据库</param>
    /// <returns>当前数据库管理器实例，支持链式调用</returns>
    public DatabaseManager AddDatabase(string name, IDatabase database, bool isDefault = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Database name cannot be null or whitespace.", nameof(name));

        if (database == null)
            throw new ArgumentNullException(nameof(database));

        _databases[name] = database;

        if (isDefault)
            _defaultDatabase = database;

        return this;
    }

    /// <summary>
    /// 设置默认数据库
    /// </summary>
    /// <param name="name">数据库名称</param>
    /// <returns>当前数据库管理器实例，支持链式调用</returns>
    public DatabaseManager SetDefaultDatabase(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Database name cannot be null or whitespace.", nameof(name));

        if (_databases.TryGetValue(name, out var database))
            _defaultDatabase = database;
        else
            throw new KeyNotFoundException($"Database with name '{name}' not found.");

        return this;
    }

    /// <summary>
    /// 获取默认数据库
    /// </summary>
    /// <returns>默认数据库实例</returns>
    public IDatabase GetDefaultDatabase()
    {
        return _defaultDatabase ?? throw new InvalidOperationException("No default database has been set.");
    }

    /// <summary>
    /// 获取指定名称的数据库
    /// </summary>
    /// <param name="name">数据库名称</param>
    /// <returns>数据库实例</returns>
    public IDatabase GetDatabase(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Database name cannot be null or whitespace.", nameof(name));

        if (_databases.TryGetValue(name, out var database))
            return database;
        else
            throw new KeyNotFoundException($"Database with name '{name}' not found.");
    }

    /// <summary>
    /// 实现 IDatabaseProvider 接口，获取数据库实例
    /// </summary>
    /// <param name="databaseName">数据库名称，null 表示默认数据库</param>
    /// <returns>数据库实例，若未找到则返回 null</returns>
    IDatabase? IDatabaseProvider.GetDatabase(string? databaseName)
    {
        if (string.IsNullOrEmpty(databaseName))
            return _defaultDatabase;

        _databases.TryGetValue(databaseName, out var database);
        return database;
    }
}