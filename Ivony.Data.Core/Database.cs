using System;
using System.Data;

using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data.Core;

public abstract class Database : IDatabase
{

  protected Database( IServiceProvider serviceProvider )
  {
    ServiceProvider = serviceProvider;
    transactionFactory = serviceProvider.GetRequiredKeyedService<IDatabaseTransactionFactory>( this );
  }

  public IServiceProvider ServiceProvider { get; }


  protected IDatabaseTransactionFactory transactionFactory;


  /// <summary>
  /// 数据库连接字符串
  /// </summary>
  public abstract string ConnectionString { get; }

  public virtual IDatabaseTransaction CreateTransaction() => transactionFactory.CreateTransaction();

  public virtual IDbExecutor GetDbExecutor() => ServiceProvider.GetRequiredKeyedService<IDbExecutorFactory>( this ).GetExecutor();








}
