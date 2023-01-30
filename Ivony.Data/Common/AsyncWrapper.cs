using Ivony.Data.Queries;

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{
  internal class AsyncExecutorWrapper : IAsyncDbExecutor
  {
    private IDbExecutor Executor { get; }

    public AsyncExecutorWrapper( IDbExecutor executor )
    {
      Executor = executor;
    }

    IDbExecuteContext IDbExecutor.Execute( DbQuery query )
    {
      return Executor.Execute( query );
    }

    Task<IAsyncDbExecuteContext> IAsyncDbExecutor.ExecuteAsync( DbQuery query, CancellationToken token )
    {
      return Task.FromResult( (IAsyncDbExecuteContext) new AsyncDbExecuteContextWrapper( Executor.Execute( query ) ) );
    }
  }


  internal class AsyncDbExecuteContextWrapper : IAsyncDbExecuteContext
  {

    public AsyncDbExecuteContextWrapper( IDbExecuteContext context )
    {
      Context = context;
    }



    private IDbExecuteContext Context { get; }

    public int RecordsAffected => Context.RecordsAffected;

    public object SyncRoot => Context.SyncRoot;

    public DataTable LoadDataTable( int startRecord, int maxRecords ) => Context.LoadDataTable( startRecord, maxRecords );

    public bool NextResult() => Context.NextResult();

    public IDataRecord ReadRecord() => Context.ReadRecord();

    public void RegisterDispose( IDisposable disposable ) => Context.RegisterDispose( disposable );

    public void Dispose() => Context.Dispose();


    public async Task<DataTable> LoadDataTableAsync( int startRecord, int maxRecords, CancellationToken cancellationToken = default )
    {
      cancellationToken.ThrowIfCancellationRequested();
      return LoadDataTable( startRecord, maxRecords );
    }


    public async Task<bool> NextResultAsync( CancellationToken cancellationToken )
    {
      cancellationToken.ThrowIfCancellationRequested();
      return NextResult();
    }

    public async Task<IDataRecord> ReadRecordAsync( CancellationToken cancellationToken )
    {
      cancellationToken.ThrowIfCancellationRequested();
      return ReadRecord();
    }
  }


  internal class AsyncDbTransactionContextWrapper : IAsyncDatabaseTransaction, IServiceProvider
  {
    public AsyncDbTransactionContextWrapper( IDatabaseTransaction context )
    {
      _context = context;
    }

    private readonly IDatabaseTransaction _context;

    public TransactionStatus Status => _context.Status;

    public void BeginTransaction() => _context.BeginTransaction();

    public IServiceProvider ServiceProvider => _context.ServiceProvider;

    public IDatabaseTransaction ParentTransaction => _context.ParentTransaction;


    public Task BeginTransactionAsync()
    {
      BeginTransaction();
      return Task.CompletedTask;
    }

    public void Commit()
    {
      _context.Commit();
    }

    public Task CommitAsync()
    {
      Commit();
      return Task.CompletedTask;
    }

    public IDatabaseTransaction CreateTransaction()
    {
      return _context.CreateTransaction();
    }

    public void Dispose()
    {
      _context.Dispose();
    }

    public IDbExecutor GetDbExecutor()
    {
      return _context.GetDbExecutor();
    }

    public void RegisterDispose( IDisposable disposable )
    {
      _context.RegisterDispose( disposable );
    }

    public void Rollback()
    {
      _context.Rollback();
    }

    public Task RollbackAsync()
    {
      Rollback();
      return Task.CompletedTask;
    }

    object IServiceProvider.GetService( Type serviceType )
    {
      return (_context as IServiceProvider)?.GetService( serviceType );
    }
  }
}