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
    private readonly object _lock = new();

    /// <summary>
    /// 添加数据库到管理器
    /// </summary>
    /// <param name="name">数据库名称</param>
    /// <param name="database">数据库实例</param>
    /// <returns>当前数据库管理器实例，支持链式调用</returns>
    public DatabaseManager AddDatabase(string name, IDatabase database)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Database name cannot be null or whitespace.", nameof(name));

        if (database == null)
            throw new ArgumentNullException(nameof(database));

        lock (_lock)
        {
            _databases[name] = database;

            // 如果是第一个数据库，自动设为默认数据库
            if (_defaultDatabase == null)
                _defaultDatabase = database;
        }

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

        lock (_lock)
        {
            if (_databases.TryGetValue(name, out var database))
                _defaultDatabase = database;
            else
                throw new KeyNotFoundException($"Database with name '{name}' not found.");
        }

        return this;
    }

    /// <summary>
    /// 获取指定名称的数据库
    /// </summary>
    /// <param name="name">数据库名称，null 表示默认数据库</param>
    /// <returns>数据库实例，若未找到则返回 null</returns>
    public IDatabase? GetDatabase(string? name)
    {
        lock (_lock)
        {
            if (name is null)
                return _defaultDatabase;

            if (_databases.TryGetValue(name, out var database))
                return database;
            else
                return null;
        }
    }
}
