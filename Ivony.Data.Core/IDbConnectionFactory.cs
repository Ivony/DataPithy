using System.Data;

namespace Ivony.Data;

/// <summary>
/// 数据库连接工厂
/// </summary>
/// <typeparam name="T">数据库连接类型</typeparam>
public interface IDbConnectionFactory<T> where T : IDbConnection
{

  /// <summary>
  /// 创建数据库连接
  /// </summary>
  /// <param name="connectionString">数据库连接字符串</param>
  /// <returns>数据库连接</returns>
  T CreateConnection( string connectionString );

  /// <summary>
  /// 释放数据库连接
  /// </summary>
  /// <param name="connection">数据库连接</param>
  void ReleaseConnection( T connection );
}

