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

    public MySqlConnection Connection { get; }

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
        Dispose();
      }
    }


    public IDbExecutor GetExecutor()
    {
      lock ( _sync )
      {
        if ( Status == TransactionStatus.NotBeginning )
          BeginTransaction();

        return new MySqlDbExecutorWithTransaction( this );
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
        Dispose();
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
      }
    }

  }
}
