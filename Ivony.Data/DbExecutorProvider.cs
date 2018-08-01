using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  public class DbExecutorProvider : IDbExecutorProvider
  {


    public static IDbExecutorProvider Create( Func<IDbExecutor> executorFactory, Func<IAsyncDbExecutor> asyncExecutorFactory )
    {
      return new DbExeutorProviderWrapper( executorFactory, asyncExecutorFactory );
    }

    private DbExecutorProvider() { }

    private class DbExeutorProviderWrapper : IDbExecutorProvider
    {
      private readonly Func<IDbExecutor> executorFactory;
      private readonly Func<IAsyncDbExecutor> asyncExecutorFactory;

      public DbExeutorProviderWrapper( Func<IDbExecutor> executorFactory, Func<IAsyncDbExecutor> asyncExecutorFactory )
      {
        this.executorFactory = executorFactory ?? throw new ArgumentNullException( nameof( executorFactory ) );
        this.asyncExecutorFactory = asyncExecutorFactory ?? throw new ArgumentNullException( nameof( asyncExecutorFactory ) );
      }

      public IAsyncDbExecutor GetAsyncDbExecutor( DbContext context )
      {
        return asyncExecutorFactory();
      }

      public IDbExecutor GetDbExecutor( DbContext context )
      {
        return executorFactory();
      }
    }

    public IAsyncDbExecutor GetAsyncDbExecutor( DbContext context )
    {
      throw new NotImplementedException();
    }

    public IDbExecutor GetDbExecutor( DbContext context )
    {
      throw new NotImplementedException();
    }
  }
}
