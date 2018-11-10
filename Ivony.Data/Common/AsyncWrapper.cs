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

    public void RegisterDispose( Action disposeMethod ) => Context.RegisterDispose( disposeMethod );

    public void Dispose() => Context.Dispose();


    public Task<DataTable> LoadDataTableAsync( int startRecord, int maxRecords, CancellationToken token = default( CancellationToken ) )
      => Task.FromResult( LoadDataTable( startRecord, maxRecords ) );


    public Task<bool> NextResultAsync() => Task.FromResult( NextResult() );

    public Task<IDataRecord> ReadRecordAsync() => Task.FromResult( ReadRecord() );
  }


  internal class AsyncDbTransactionContextWrapper : IAsyncDbTransactionContext, IServiceProvider
  {
    public AsyncDbTransactionContextWrapper( IDbTransactionContext context )
    {
      _context = context;
    }

    private readonly IDbTransactionContext _context;

    public TransactionStatus Status => _context.Status;

    public void BeginTransaction() => _context.BeginTransaction();

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

    public IDbTransactionContext CreateTransaction( DbContext context )
    {
      return _context.CreateTransaction( context );
    }

    public void Dispose()
    {
      _context.Dispose();
    }

    public IDbExecutor GetDbExecutor( DbContext context )
    {
      return _context.GetDbExecutor( context );
    }

    public void RegisterDispose( Action disposeMethod )
    {
      _context.RegisterDispose( disposeMethod );
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