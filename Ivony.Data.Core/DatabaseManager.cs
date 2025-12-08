using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data;

/// <summary>
    /// 数据库管理器，用于管理多个数据库连接
    /// </summary>
    public class DatabaseManager : IDatabaseProvider
    {
        private readonly Dictionary<string, IDatabase> _databases = new();
        private string? _defaultDatabaseName;
        private readonly object _lock = new();

        /// <summary>
        /// 内部服务集合，用于注册公共服务
        /// </summary>
        private readonly IServiceCollection _services = new ServiceCollection();

        /// <summary>
        /// 初始化数据库管理器
        /// </summary>
        public DatabaseManager()
        {
            // 注册默认实现类
            _services.AddSingleton<IDatabaseTransactionFactory, DefaultDatabaseTransactionFactory>();
        }

    /// <summary>
    /// 注册数据库到管理器
    /// </summary>
    /// <param name="name">数据库名称</param>
    /// <param name="databaseBuilder">用于创建数据库实例的委托</param>
    /// <returns>当前数据库管理器实例，支持链式调用</returns>
    public DatabaseManager RegisterDatabase(string name, Func<IServiceCollection, IDatabase> databaseBuilder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Database name cannot be null or whitespace.", nameof(name));

        if (databaseBuilder == null)
            throw new ArgumentNullException(nameof(databaseBuilder));

        lock (_lock)
        {
            // 在注册时创建数据库实例，创建_services的副本，避免databaseBuilder修改影响其他数据库实例
            IServiceCollection servicesCopy = new ServiceCollection();
            foreach (var service in _services)
                servicesCopy.Add(service);

            var database = databaseBuilder(servicesCopy);
            _databases[name] = database;

            // 如果是第一个数据库，自动设为默认数据库
            if (_defaultDatabaseName == null)
                _defaultDatabaseName = name;
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
            if (_databases.ContainsKey(name))
                _defaultDatabaseName = name;
            else
                throw new KeyNotFoundException($"Database with name '{name}' not found.");
        }

        return this;
    }

    /// <summary>
    /// 配置公共服务
    /// </summary>
    /// <param name="configure">用于配置服务的委托</param>
    /// <returns>当前数据库管理器实例，支持链式调用</returns>
    public DatabaseManager ConfigureGlobalServices(Action<IServiceCollection> configure)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        configure(_services);
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
            // 确定要使用的数据库名称
            var databaseName = name ?? _defaultDatabaseName;
            if (databaseName == null)
                return null;

            // 直接返回存储的数据库实例
            if (_databases.TryGetValue(databaseName, out var database))
                return database;
            else
                return null;
        }
    }
}
