using System;
using System.Data;

namespace Ivony.Data.Core;
internal class DatabaseTransaction<Command, Connection, Transaction>( DatabaseWithTransaction<Command, Connection, Transaction> database ) : IDatabaseTransaction
  where Command : IDbCommand
  where Connection : IDbConnection
  where Transaction : IDbTransaction
{
  public TransactionStatus Status { get; }
  public IDatabaseTransaction ParentTransaction { get; }
  public IServiceProvider ServiceProvider { get; }

  public void BeginTransaction()
  {
    throw new NotImplementedException();
  }

  public void Commit()
  {
    throw new NotImplementedException();
  }

  public IDatabaseTransaction CreateTransaction()
  {
    throw new NotImplementedException();
  }

  public void Dispose()
  {
    throw new NotImplementedException();
  }

  public IDbExecutor GetDbExecutor()
  {
    throw new NotImplementedException();
  }

  public void RegisterDispose( IDisposable disposable )
  {
    throw new NotImplementedException();
  }

  public void Rollback()
  {
    throw new NotImplementedException();
  }
}