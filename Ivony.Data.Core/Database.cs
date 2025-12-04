using System;
using System.Data;

using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data;

public abstract class Database : IDatabase
{
  protected Database(IServiceProvider serviceProvider)
  {
    ServiceProvider = serviceProvider;
  }

  public IServiceProvider ServiceProvider { get; }

  public virtual IDatabaseTransaction CreateTransaction() => ServiceProvider.GetRequiredService<IDatabaseTransactionFactory>().CreateTransaction();

  public virtual IDbExecutor GetDbExecutor() => ServiceProvider.GetRequiredService<IDbExecutorFactory>().GetExecutor();

}
