using System;
using System.Data;

namespace Ivony.Data.Core;

public abstract class Database<Command, Connection>( IServiceProvider serviceProvider ) : IDatabase
  where Connection : IDbConnection
  where Command : IDbCommand
{
  public IServiceProvider ServiceProvider => serviceProvider;


  /// <summary>
  /// 数据库连接字符串
  /// </summary>
  public abstract string ConnectionString { get; }

  public virtual IDatabaseTransaction CreateTransaction() => throw new NotSupportedException();

  public virtual IDbExecutor GetDbExecutor() => ServiceProvider.GetRequiredService<IDbExecutorFactory<Command, Connection>>().GetExecutor();








}
