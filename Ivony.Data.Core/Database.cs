using System;
using System.Data;

using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data;

public class Database(IServiceProvider serviceProvider) : IDatabase
{

  public IServiceProvider ServiceProvider  => serviceProvider;

  public virtual IDatabaseTransaction CreateTransaction() => ServiceProvider.GetRequiredService<IDatabaseTransactionFactory>().CreateTransaction();

  public virtual IDbExecutor GetDbExecutor() => ServiceProvider.GetRequiredService<IDbExecutorFactory>().GetExecutor();

}
