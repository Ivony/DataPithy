using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{

  /// <summary>
  /// 辅助构建 IDbProvider 对象
  /// </summary>
  public class DbProviderBuilder : IAsyncDbExecutorRegisterService, IAsyncDbTransactionExecutorRegisterService, IDbExecutorRegisterService, IDbTransactionExecutorRegisterService
  {



    /// <summary>
    /// 创建 IDbProvider 对象
    /// </summary>
    /// <returns></returns>
    public IDbProvider Build()
    {
      return null;
    }




    private List<(Func<IDbTransactionContext, IDbQuery, bool>, Func<IDbTransactionContext, IDbQuery, Task<IDbExecuteContext>>)> async_trans = new List<(Func<IDbTransactionContext, IDbQuery, bool>, Func<IDbTransactionContext, IDbQuery, Task<IDbExecuteContext>>)>();
    private List<(Func<IDbQuery, bool>, Func<IDbQuery, Task<IAsyncDbExecuteContext>>)> async_execs = new List<(Func<IDbQuery, bool>, Func<IDbQuery, Task<IAsyncDbExecuteContext>>)>();
    private List<(Func<IDbQuery, bool>, Func<IDbQuery, IDbExecuteContext>)> execs = new List<(Func<IDbQuery, bool>, Func<IDbQuery, IDbExecuteContext>)>();
    private List<(Func<IDbTransactionContext, IDbQuery, bool>, Func<IDbTransactionContext, IDbQuery, IDbExecuteContext>)> trans_execs = new List<(Func<IDbTransactionContext, IDbQuery, bool>, Func<IDbTransactionContext, IDbQuery, IDbExecuteContext>)>();



    void IAsyncDbTransactionExecutorRegisterService.Register( Func<IDbTransactionContext, IDbQuery, bool> predicate, Func<IDbTransactionContext, IDbQuery, Task<IDbExecuteContext>> executor )
    {
      async_trans.Add( (predicate, executor) );
    }

    void IAsyncDbExecutorRegisterService.Register( Func<IDbQuery, bool> predicate, Func<IDbQuery, Task<IAsyncDbExecuteContext>> executor )
    {
      async_execs.Add( (predicate, executor) );
    }

    void IDbExecutorRegisterService.Register( Func<IDbQuery, bool> predicate, Func<IDbQuery, IDbExecuteContext> executor )
    {
      execs.Add( (predicate, executor) );
    }

    void IDbTransactionExecutorRegisterService.Register( Func<IDbTransactionContext, IDbQuery, bool> predicate, Func<IDbTransactionContext, IDbQuery, IDbExecuteContext> executor )
    {
      trans_execs.Add( (predicate, executor) );
    }






    private class ExecutorItem
    {
      public Func<IDbQuery, IDbExecuteContext> Executor { get; set; }

      public Func<IDbQuery, bool> Predicate { get; set; }
    }



    /// <summary>
    /// 协助构建 IDbExecutor 对象
    /// </summary>
    private class ExecutorRegisterService : IDbExecutorRegisterService
    {

      /// <summary>
      /// 创建 IDbExecutor 对象
      /// </summary>
      /// <returns></returns>
      public IDbExecutor BuildExecutor()
      {
        return new DbExecutor( this );
      }


      private class DbExecutor : IDbExecutor
      {

        public DbExecutor( ExecutorRegisterService provider )
        {
          Provider = provider;
        }

        public ExecutorRegisterService Provider { get; }

        public IDbExecuteContext Execute( IDbQuery query )
        {

          foreach ( var executeMethod in Provider.FindExecutors( query ) )
          {
            var result = executeMethod( query );
            if ( result != null )
              return result;
          }

          return null;
        }
      }


      private List<ExecutorItem> Items { get; } = new List<ExecutorItem>();

      /// <summary>
      /// 创建 IDbExecutor 对象
      /// </summary>
      /// <param name="query">要执行的查询</param>
      /// <returns>查询执行器</returns>
      protected virtual Func<IDbQuery, IDbExecuteContext>[] FindExecutors( IDbQuery query )
      {
        return Items.FindAll( item => item.Predicate( query ) ).Select( item => item.Executor ).ToArray();
      }

      void IDbExecutorRegisterService.Register( Func<IDbQuery, bool> predicate, Func<IDbQuery, IDbExecuteContext> executor )
      {
        Items.Add( new ExecutorItem { Predicate = predicate, Executor = executor } );
      }
    }

  }
}
