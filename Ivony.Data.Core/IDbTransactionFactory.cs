using System.Data;

namespace Ivony.Data;

/// <summary>
/// 数据库事务工厂
/// </summary>
/// <typeparam name="T">数据库事务类型</typeparam>
public interface IDbTransactionFactory
{
  /// <summary>
  /// 创建数据库事务
  /// </summary>
  /// <param name="connectionString">数据库连接字符串</param>
  /// <returns>数据库事务</returns>
  IDbTransaction CreateTransaction( string connectionString );

  /// <summary>
  /// 释放数据库事务
  /// </summary>
  /// <param name="transaction">数据库事务</param>
  void ReleaseTransaction( IDbTransaction transaction );

}

