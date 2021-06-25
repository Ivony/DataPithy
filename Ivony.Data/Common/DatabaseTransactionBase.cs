using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Ivony.Data.Common
{

  /// <summary>
  /// 辅助实现 IDbTransactionContext 接口
  /// </summary>
  /// <typeparam name="T">数据库事务对象类型</typeparam>
  [System.Diagnostics.CodeAnalysis.SuppressMessage( "Design", "CA1063:Implement IDisposable Correctly", Justification = "<挂起>" )]
  public abstract class DatabaseTransactionBase<T> : IDatabaseTransaction where T : IDbTransaction
  {


    /// <summary>
    /// 创建和初始化 DbTransactionContextBase 对象
    /// </summary>
    /// <param name="database">数据库访问提供程序</param>
    protected DatabaseTransactionBase( IDatabase database )
    {
      Database = database;
    }



    /// <summary>
    /// 数据库事务对象
    /// </summary>
    public T Transaction { get; private set; }

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


        Transaction = BeginTransactionCore();

        Status = TransactionStatus.Running;
      }
    }

    /// <summary>
    /// 派生类实现此方法以开启事务
    /// </summary>
    /// <returns></returns>
    protected abstract T BeginTransactionCore();


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
        if ( Status == TransactionStatus.Running )
          Transaction.Rollback();

        Status = TransactionStatus.Completed;
        DisposeTransaction( Transaction );
        foreach ( var item in _disposables )
          item.Dispose();
      }
    }


    /// <summary>
    /// 派生类实现此方法以销毁事务
    /// </summary>
    /// <param name="transaction"></param>
    protected virtual void DisposeTransaction( T transaction )
    {
      if ( transaction != null )
        transaction.Dispose();
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
    protected abstract IDbExecutor GetDbExecutorCore();

    /// <summary>
    /// 创建内嵌事务
    /// </summary>
    /// <returns></returns>
    public virtual IDatabaseTransaction CreateTransaction()
    {
      throw new NotSupportedException( "Database is not supported nested Transaction." );
    }


    private readonly HashSet<IDisposable> _disposables = new HashSet<IDisposable>();

    void IDisposableObjectContainer.RegisterDispose( IDisposable disposable )
    {
      _disposables.Add( disposable );
    }

  }
}
