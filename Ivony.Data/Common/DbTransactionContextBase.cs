using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Ivony.Data.Common
{
  public abstract class DbTransactionContextBase<T> : IDbTransactionContext where T : IDbTransaction
  {

    private readonly IDbProvider _dbProvider;


    protected DbTransactionContextBase( IDbProvider dbProvider )
    {
      _dbProvider = dbProvider;
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
    public object Sync { get; } = new object();

    /// <summary>
    /// 服务提供程序，从数据访问提供程序继承
    /// </summary>
    public IServiceProvider ServiceProvider => _dbProvider.ServiceProvider;

    /// <summary>
    /// 获取父级事务，如果有的话
    /// </summary>
    public IDbTransactionContext ParentTransaction => _dbProvider as IDbTransactionContext;

    /// <summary>
    /// 获取属性配置，从数据访问提供程序继承
    /// </summary>
    public IReadOnlyDictionary<string, object> Properties => _dbProvider.Properties;





    /// <summary>
    /// 开始事务
    /// </summary>
    public virtual void BeginTransaction()
    {
      lock ( Sync )
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
      lock ( Sync )
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
      lock ( Sync )
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
      lock ( Sync )
      {
        if ( Status == TransactionStatus.Running )
          Transaction.Rollback();

        Status = TransactionStatus.Completed;
        DisposeTransaction( Transaction );
        disposeAction?.Invoke();
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
    /// 
    /// <returns></returns>
    public virtual IDbExecutor GetDbExecutor()
    {
      lock ( Sync )
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
    /// <param name="context">数据访问上下文</param>
    /// <returns>查询执行器</returns>
    protected abstract IDbExecutor GetDbExecutorCore();

    /// <summary>
    /// 创建内嵌事务
    /// </summary>
    /// 
    /// <returns></returns>
    public virtual IDbTransactionContext CreateTransaction()
    {
      throw new NotSupportedException( "Database is not supported nested Transaction." );
    }


    private Action disposeAction;

    void IDisposableObjectContainer.RegisterDispose( Action disposeMethod )
    {
      disposeAction += disposeMethod;
    }
  }
}
