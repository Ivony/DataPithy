using System;

using Ivony.Data.MySqlClient;

using Microsoft.Extensions.DependencyInjection;

#if MySqlConnector
using MySqlConnector;
#else
using MySql.Data.MySqlClient;
#endif

namespace Ivony.Data;


/// <summary>
/// 提供 MySql 数据库支持
/// </summary>
public partial class MySqlDb : Database
{
    /// <summary>
    /// 使用指定的连接字符串和服务提供程序初始化 MySqlDb 实例。
    /// </summary>
    /// <param name="connectionString">MySQL 数据库连接字符串。</param>
    /// <param name="serviceProvider">用于依赖注入的服务提供程序。</param>
    internal MySqlDb(string connectionString, IServiceProvider serviceProvider) : base(serviceProvider)
    {
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }



    /// <summary>
    /// MySQL 数据库连接字符串
    /// </summary>
    public override string ConnectionString { get; }

}
