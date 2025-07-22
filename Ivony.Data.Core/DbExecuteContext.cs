using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.Core;
public class DbExecuteContext<Command, Connection>( Command command, IDbTracing tracing ) : IDbExecuteContext, IAsyncDbExecuteContext
  where Command : IDbCommand
  where Connection : IDbConnection
{
  public int RecordsAffected { get; }
  public object SyncRoot { get; }

  public void Dispose()
  {
    throw new NotImplementedException();
  }

  public DataTable LoadDataTable( int startRecord, int maxRecords )
  {
    throw new NotImplementedException();
  }

  public Task<DataTable> LoadDataTableAsync( int startRecord, int maxRecords, CancellationToken token = default )
  {
    throw new NotImplementedException();
  }

  public bool NextResult()
  {
    throw new NotImplementedException();
  }

  public Task<bool> NextResultAsync( CancellationToken cancellationToken = default )
  {
    throw new NotImplementedException();
  }

  public IDataRecord ReadRecord()
  {
    throw new NotImplementedException();
  }

  public Task<IDataRecord> ReadRecordAsync( CancellationToken cancellationToken = default )
  {
    throw new NotImplementedException();
  }

  public void RegisterDispose( IDisposable disposable )
  {
    throw new NotImplementedException();
  }

  public void RegisterExceptionHandler( Action<Exception> exceptionHandler )
  {
    throw new NotImplementedException();
  }
}