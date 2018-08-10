using Ivony.Data.Common;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Ivony.Data.MySqlClient
{

  /// <summary>
  /// 实现 MySQL 数据库事务支持
  /// </summary>
  public class MySqlDbTransactionContext : IDbTransactionContext
  {

    internal MySqlDbTransactionContext( string connectionString )
    {
      Connection = new MySqlConnection( connectionString );
    }

    /// <summary>
    /// 获取数据库连接对象
    /// </summary>
    public MySqlConnection Connection { get; }

    /// <summary>
    /// 获取数据库事务对象
    /// </summary>
    public MySqlTransaction Transaction { get; private set; }

    public TransactionStatus Status { get; private set; } = TransactionStatus.NotBeginning;

    private object _sync = new object();

    public void BeginTransaction()
    {
      lock ( _sync )
      {
        if ( Status == TransactionStatus.Running )
          return;

        else if ( Status == TransactionStatus.Completed )
          throw new ObjectDisposedException( "transaction" );


        Connection.Open();
        Transaction = Connection.BeginTransaction();

        Status = TransactionStatus.Running;
      }
    }

    public void Commit()
    {
      lock ( _sync )
      {
        if ( Status == TransactionStatus.NotBeginning )
          throw new InvalidOperationException();

        else if ( Status == TransactionStatus.Completed )
          throw new ObjectDisposedException( "transaction" );

        Transaction.Commit();
        Status = TransactionStatus.Completed;
      }
    }


    public void Rollback()
    {
      lock ( _sync )
      {
        if ( Status == TransactionStatus.NotBeginning )
          throw new InvalidOperationException();

        else if ( Status == TransactionStatus.Completed )
          throw new ObjectDisposedException( "transaction" );

        Transaction.Rollback();
        Status = TransactionStatus.Completed;
      }
    }

    public void Dispose()
    {
      lock ( _sync )
      {
        if ( Status == TransactionStatus.Running )
          Transaction.Rollback();

        Status = TransactionStatus.Completed;
        Connection.Dispose();
        Transaction?.Dispose();
        disposeAction?.Invoke();
      }
    }


    public IDbExecutor GetDbExecutor( DbContext context )
    {
      lock ( _sync )
      {
        if ( Status == TransactionStatus.NotBeginning )
          BeginTransaction();

        if ( Status == TransactionStatus.Completed )
          throw new InvalidOperationException();

        return new MySqlDbExecutorWithTransaction( this );
      }
    }

    public IDbTransactionContext CreateTransaction( DbContext context )
    {
      throw new NotSupportedException( "MySQL database is not supported nested Transaction." );
    }


    private Action disposeAction;

    void IDisposableObjectContianer.RegisterDispose( Action disposeMethod )
    {
      disposeAction += disposeMethod;
    }
  }
}
