
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Ivony.Data.Queries;

namespace Ivony.Data.Core;

public class DbExecutor<Command, Connection>( Database<Command, Connection> database ) : IDbExecutor
  where Connection : IDbConnection
  where Command : IDbCommand
{


  public IDbExecuteContext Execute( DbQuery query )
  {
    try
    {
      var context = Execute( CreateCommand( query ), TryCreateTracing( this, query ) );
      context.RegisterExceptionHandler( e => ExecuteError( e, query ) );
      return context;
    }
    catch ( Exception e )
    {
      throw ExecuteError( e, query );
    }
  }

  protected virtual IDbExecuteContext Execute( Command command, IDbTracing tracing )
  {
    if ( command == null )
      throw new ArgumentNullException( nameof( command ) );

    try
    {
      TryExecuteTracing( tracing, t => t.OnExecuting( command ) );


      var connection = database.ServiceProvider.GetRequiredService<IDbConnectionFactory<Connection>>().CreateConnection( database.ConnectionString );
      command.Connection = connection;
      var context = CreateExecuteContext( command, tracing );
      TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

      return context;

    }
    catch ( DbException exception )
    {
      TryExecuteTracing( tracing, t => t.OnException( exception ) );
      throw;
    }
  }



  protected DbExecuteContext<Command, Connection> CreateExecuteContext( Command command, IDbTracing tracing ) => new DbExecuteContext<Command, Connection>( command, tracing );

  protected Task<DbExecuteContext<Command, Connection>> CreateAsyncExecuteContext( Command command, IDbTracing tracing ) => Task.FromResult( new DbExecuteContext<Command, Connection>( command, tracing ) );




  protected virtual Command CreateCommand( DbQuery query ) => database.ServiceProvider.GetService<IDbCommandFactory<Command>>().CreateCommand( query );

  protected virtual IDbConnection CreateConnection() => database.ServiceProvider.GetRequiredService<IDbConnectionFactory<Connection>>().CreateConnection( database.ConnectionString );


  public async Task<IAsyncDbExecuteContext> ExecuteAsync( DbQuery query, CancellationToken token )
  {
    var command = CreateCommand( query );
    try
    {
      var context = await ExecuteAsync( command, TryCreateTracing( this, query ) );
      context.RegisterExceptionHandler( e => ExecuteError( e, query ) );
      return context;
    }
    catch ( Exception e )
    {
      command.Dispose();
      throw ExecuteError( e, query );
    }
  }

  protected virtual async Task<IAsyncDbExecuteContext> ExecuteAsync( Command command, IDbTracing tracing )
  {

    if ( command == null )
      throw new ArgumentNullException( nameof( command ) );

    try
    {
      TryExecuteTracing( tracing, t => t.OnExecuting( command ) );
      command.Connection = CreateConnection();

      var context = await CreateAsyncExecuteContext( command, tracing );

      TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

      return context;
    }
    catch ( DbException exception )
    {
      TryExecuteTracing( tracing, t => t.OnException( exception ) );
      throw;
    }

  }




  /// <summary>
  /// 获取在追踪数据库查询过程的追踪服务
  /// </summary>
  protected IDbTraceService? TraceService
  {
    get;
    private set;
  }

  /// <summary>
  /// 获取异常筛选器
  /// </summary>
  protected IDbExceptionFilter? ExceptionFilter { get; }


  /// <summary>
  /// 当执行查询时发生异常调用此方法处理
  /// </summary>
  /// <param name="e">异常信息</param>
  /// <param name="query">数据库查询信息</param>
  protected Exception ExecuteError( Exception e, DbQuery query )
  {

    var exception = new DbQueryExecutionException( query, e );



    if ( ExceptionFilter != null )
    {
      try
      {
        ExceptionFilter.OnQueryException( e, query );
      }
      catch ( Exception filterException )
      {
        return new AggregateException( exception, filterException );
      }
    }

    return exception;

  }





  /// <summary>
  /// 尝试创建 IDbTracing 对象
  /// </summary>
  /// <param name="executor">查询执行器</param>
  /// <param name="query">即将执行的查询对象</param>
  /// <returns>追踪该查询执行过程的 IDbTracing 对象</returns>
  protected IDbTracing TryCreateTracing( IDbExecutor executor, DbQuery query )
  {


    var traceSerivce = query.Configures.GetService<IDbTraceService>() ?? TraceService;

    if ( traceSerivce == null )
      return null;

    try
    {
      return traceSerivce.CreateTracing( executor, query );
    }
    catch
    {
      return null;
    }
  }




  /// <summary>
  /// 尝试创建 IDbTracing 对象
  /// </summary>
  /// <param name="query">即将执行的查询对象</param>
  /// <returns>追踪该查询执行过程的 IDbTracing 对象</returns>
  protected IDbTracing TryCreateTracing( DbQuery query )
  {

    return TryCreateTracing( (IDbExecutor) this, query );

  }



  /// <summary>
  /// 尝试执行查询追踪器的一个追踪方法，此方法会自动判断追踪器是否存在以及对调用中出现的异常进行异常屏蔽。
  /// </summary>
  /// <param name="tracing">查询追踪器，如果有的话</param>
  /// <param name="action">要执行的追踪操作</param>
  protected void TryExecuteTracing( IDbTracing tracing, Action<IDbTracing> action )
  {
    if ( tracing == null )
      return;

    try
    {
      action( tracing );
    }
    catch
    {

    }
  }


}

