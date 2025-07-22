using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data.Core;
internal class DatabaseTransaction( Database database ) : Database( database.ServiceProvider ), IDatabaseTransaction
{


  public override string ConnectionString => database.ConnectionString;

  /// <summary>
  /// 数据库事务对象
  /// </summary>
  public IDbTransaction Transaction { get; private set; }

  /// <summary>
  /// 事务状态
  /// </summary>
  public TransactionStatus Status { get; private set; } = TransactionStatus.NotBeginning;


  /// <summary>
  /// 用于同步的对象
  /// </summary>
  public object SyncRoot { get; } = new object();


  /// <summary>
  /// 数据库访问提供程序
  /// </summary>
  public IDatabase Database { get; }



  /// <summary>
  /// 服务提供程序，从数据访问提供程序继承
  /// </summary>
  public IServiceProvider ServiceProvider => Database.ServiceProvider;

  /// <summary>
  /// 获取父级事务，如果有的话
  /// </summary>
  public IDatabaseTransaction ParentTransaction => Database as IDatabaseTransaction;






  /// <summary>
  /// 开始事务
  /// </summary>
  public virtual void BeginTransaction()
  {
    lock ( SyncRoot )
    {
      if ( Status == TransactionStatus.Running )
        return;

      else if ( Status == TransactionStatus.Completed )
        throw new ObjectDisposedException( "transaction" );


      Transaction = CreateDbTransaction();

      Status = TransactionStatus.Running;
    }
  }




  private IDbTransactionFactory factory = database.ServiceProvider.GetRequiredKeyedService<IDbTransactionFactory>( database );

  /// <summary>
  /// 派生类实现此方法创建数据库事务
  /// </summary>
  /// <returns></returns>
  protected virtual IDbTransaction CreateDbTransaction() => factory.CreateTransaction( ConnectionString );

  protected virtual void ReleaseDbTransaction( IDbTransaction transaction ) => factory.ReleaseTransaction( transaction );



  /// <summary>
  /// 提交事务
  /// </summary>
  public virtual void Commit()
  {
    lock ( SyncRoot )
    {
      if ( Status == TransactionStatus.NotBeginning )
        throw new InvalidOperationException();

      else if ( Status == TransactionStatus.Completed )
        throw new ObjectDisposedException( "transaction" );

      Transaction.Commit();
      Status = TransactionStatus.Completed;
    }
  }


  /// <summary>
  /// 回滚事务
  /// </summary>
  public virtual void Rollback()
  {
    lock ( SyncRoot )
    {
      if ( Status == TransactionStatus.NotBeginning )
        throw new InvalidOperationException();

      else if ( Status == TransactionStatus.Completed )
        throw new ObjectDisposedException( "transaction" );

      Transaction.Rollback();
      Status = TransactionStatus.Completed;
    }
  }


  /// <summary>
  /// 销毁事务上下文对象
  /// </summary>
  public virtual void Dispose()
  {
    lock ( SyncRoot )
    {

      var exceptions = new List<Exception>();

      try
      {
        if ( Status == TransactionStatus.Running )
          Transaction.Rollback();
      }
      catch ( Exception e )
      {
        exceptions.Add( e );
      }

      Status = TransactionStatus.Completed;
      ReleaseDbTransaction( Transaction );
      foreach ( var item in disposables )
      {
        try
        {
          item.Dispose();
        }
        catch ( Exception e )
        {
          exceptions.Add( e );
        }
      }

      if ( exceptions.Any() )
        throw new AggregateException( exceptions );
    }
  }



  /// <summary>
  /// 获取查询执行器
  /// </summary>
  /// <returns></returns>
  public virtual IDbExecutor GetDbExecutor()
  {
    lock ( SyncRoot )
    {
      if ( Status == TransactionStatus.NotBeginning )
        BeginTransaction();

      if ( Status == TransactionStatus.Completed )
        throw new InvalidOperationException();

      return GetDbExecutorCore();
    }
  }


  /// <summary>
  /// 派生类实现此方法以获取查询执行器
  /// </summary>
  /// <returns>查询执行器</returns>
  protected virtual IDbExecutor GetDbExecutorCore() => database.ServiceProvider.GetRequiredKeyedService<IDbExecutorFactory>( this ).GetExecutor();



  private ConcurrentBag<IDisposable> disposables = new ConcurrentBag<IDisposable>();
  public void RegisterDispose( IDisposable disposable ) => disposables.Add( disposable );
}