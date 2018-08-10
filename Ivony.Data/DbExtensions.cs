using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{


  /// <summary>
  /// 提供数据库的一些扩展方法。
  /// </summary>
  public static class DbExtensions
  {

    /// <summary>
    /// 在指定的事务中执行操作并提交
    /// </summary>
    /// <param name="transaction">要在其中执行操作的事务</param>
    /// <param name="actions">要执行的操作</param>
    public static void Run( this IDbTransactionContext transaction, Action actions )
    {

      transaction.BeginTransaction();

      try
      {
        actions();
      }
      catch ( Exception e )
      {
        if ( transaction.Status == TransactionStatus.Running )
          transaction.Rollback();

        if ( e is RollbackException == false )
          throw;
      }
      finally
      {
        if ( transaction.Status == TransactionStatus.Running )
          transaction.Commit();
      }

    }

    /// <summary>
    /// 在指定的事务中执行操作并提交
    /// </summary>
    /// <param name="transaction">要在其中执行操作的事务</param>
    /// <param name="actions">要执行的操作</param>
    public static async Task RunAsync( this IAsyncDbTransactionContext transaction, Action actions )
    {

      bool rollbacked = false;
      try
      {
        await transaction.BeginTransactionAsync();
        actions();
      }
      catch ( Exception e )
      {
        await transaction.RollbackAsync();
        rollbacked = true;

        if ( e is RollbackException == false )
          throw;
      }
      finally
      {
        if ( rollbacked == false )
          await transaction.CommitAsync();
      }

    }

    /// <summary>
    /// 在指定的事务中执行操作并提交
    /// </summary>
    /// <param name="transaction">要在其中执行操作的事务</param>
    /// <param name="actions">要执行的操作</param>
    public static async Task RunAsync( this IAsyncDbTransactionContext transaction, Func<Task> actions )
    {

      bool rollbacked = false;
      try
      {
        await transaction.BeginTransactionAsync();
        await actions();
      }
      catch ( Exception e )
      {
        await transaction.RollbackAsync();
        rollbacked = true;

        if ( e is RollbackException == false )
          throw;
      }
      finally
      {
        if ( rollbacked == false )
          await transaction.CommitAsync();
      }

    }

  }
}
