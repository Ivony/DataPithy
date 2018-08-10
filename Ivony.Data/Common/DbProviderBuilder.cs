using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{

  /// <summary>
  /// 辅助构建 IDbProvider 对象
  /// </summary>
  public class DbProviderBuilder : IAsyncDbExecutorRegisterService, IDbExecutorRegisterService
  {



    /// <summary>
    /// 创建 IDbProvider 对象
    /// </summary>
    /// <returns></returns>
    public IDbProvider Build()
    {
      throw new NotImplementedException();
    }



    private class DbProvider : IDbProvider
    {
      private readonly IDbExecutor executor;
      private readonly IAsyncDbExecutor asyncExecutor;

      public DbProvider( IDbExecutor executor, IAsyncDbExecutor asyncExecutor )
      {
        this.executor = executor;
        this.asyncExecutor = asyncExecutor;
      }


      public IAsyncDbTransactionContext CreateAsyncTransaction( DbContext context )
      {
        return null;
      }

      public IDbTransactionContext CreateTransaction( DbContext context )
      {
        return null;
      }

      public IAsyncDbExecutor GetAsyncDbExecutor( DbContext context ) => asyncExecutor;

      public IDbExecutor GetDbExecutor( DbContext context ) => executor;
    }



    private List<(Func<IDbQuery, bool>, Func<IDbQuery, Task<IAsyncDbExecuteContext>>)> asyncs = new List<(Func<IDbQuery, bool>, Func<IDbQuery, Task<IAsyncDbExecuteContext>>)>();
    private List<(Func<IDbQuery, bool>, Func<IDbQuery, IDbExecuteContext>)> execs = new List<(Func<IDbQuery, bool>, Func<IDbQuery, IDbExecuteContext>)>();


    void IAsyncDbExecutorRegisterService.Register( Func<IDbQuery, bool> predicate, Func<IDbQuery, Task<IAsyncDbExecuteContext>> executor )
    {
      asyncs.Add( (predicate, executor) );
    }

    void IDbExecutorRegisterService.Register( Func<IDbQuery, bool> predicate, Func<IDbQuery, IDbExecuteContext> executor )
    {
      execs.Add( (predicate, executor) );
    }






    private class CompositeDbExecutor : IAsyncDbExecutor
    {
      private IReadOnlyCollection<(Func<IDbQuery, bool>, Func<IDbQuery, IDbExecuteContext>)> _executors;
      private IReadOnlyCollection<(Func<IDbQuery, bool>, Func<IDbQuery, Task<IAsyncDbExecuteContext>>)> _asyncExecutors;

      public CompositeDbExecutor(
        IReadOnlyCollection<(Func<IDbQuery, bool>, Func<IDbQuery, IDbExecuteContext>)> execcutors,
        IReadOnlyCollection<(Func<IDbQuery, bool>, Func<IDbQuery, Task<IAsyncDbExecuteContext>>)> asyncExecutors
        )
      {
        _executors = execcutors;
        _asyncExecutors = asyncExecutors;
      }

      public IDbExecuteContext Execute( IDbQuery query )
      {
        foreach ( var item in _executors )
        {
          if ( item.Item1( query ) == false )
            continue;

          var result = item.Item2( query );
          if ( result != null )
            return result;
        }

        foreach ( var item in _asyncExecutors )
        {
          if ( item.Item1( query ) == false )
            continue;

          var result = item.Item2( query ).Result;
          if ( result != null )
            return result;
        }

        return null;
      }

      public Task<IAsyncDbExecuteContext> ExecuteAsync( IDbQuery query, CancellationToken token )
      {
        throw new NotImplementedException();
      }
    }


  }
}
