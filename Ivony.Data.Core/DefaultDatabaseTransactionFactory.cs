using System;
using System.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data
{
  /// <summary>
  /// 默认的数据库事务工厂实现
  /// </summary>
  public class DefaultDatabaseTransactionFactory( IServiceProvider serviceProvider ) : IDatabaseTransactionFactory
  {
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    /// <summary>
    /// 创建数据库事务
    /// </summary>
    /// <returns>数据库事务实例</returns>
    public IDatabaseTransaction CreateTransaction()
    {
      var database = _serviceProvider.GetRequiredService<IDatabase>();
      return new DatabaseTransaction( (Database) database );
    }
  }
}