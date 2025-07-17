using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Ivony.Data.Common;
using Ivony.Data.Queries;


#if MySqlConnector
using MySqlConnector;
#else
using MySql.Data.MySqlClient;
#endif


namespace Ivony.Data.MySqlClient;



internal class MySqlDbExecutor( MySqlDb provider ) : MySqlDbExecutorBase( provider )
{




  protected override MySqlCommand ApplyConnection( MySqlCommand command )
  {
    if ( command is null )
      throw new ArgumentNullException( nameof( command ) );

    command.Connection = provider.CreateConnection();
    return command;
  }


  protected override MySqlExecuteContext CreateExecuteContext( MySqlCommand command, IDbTracing tracing )
  {
    if ( command is null ) throw new ArgumentNullException( nameof( command ) );
    try
    {
      return new MySqlExecuteContext( command.Connection, command.ExecuteReader(), tracing );
    }
    catch
    {
      command.Connection.Dispose();
      throw;
    }
  }
  protected override async Task<MySqlExecuteContext> CreateExecuteContextAsync( MySqlCommand command, IDbTracing tracing )
  {
    if ( command is null ) throw new ArgumentNullException( nameof( command ) );
    try
    {
#if MySqlConnector
      return new MySqlExecuteContext( command.Connection, await command.ExecuteReaderAsync(), tracing );
#else
      return new MySqlExecuteContext( command.Connection, (MySqlDataReader) await command.ExecuteReaderAsync(), tracing );
#endif
    }
    catch
    {
      command.Connection.Dispose();
      throw;
    }
  }
}



internal class MySqlDbExecutorWithTransaction : MySqlDbExecutorBase
{

  public MySqlDbExecutorWithTransaction( MySqlDbTransaction transaction ) : base( transaction.Database )
  {
    Transaction = transaction;
  }

  public MySqlDbTransaction Transaction { get; }


  protected override MySqlCommand ApplyConnection( MySqlCommand command )
  {
    command.Connection = Transaction.Connection;
    command.Transaction = Transaction.Transaction;

    return command;
  }


  protected override MySqlExecuteContext CreateExecuteContext( MySqlCommand command, IDbTracing tracing )
  {
    if ( command is null ) throw new ArgumentNullException( nameof( command ) );

    return new MySqlExecuteContext( Transaction, command.ExecuteReader(), tracing );
  }

  protected override async Task<MySqlExecuteContext> CreateExecuteContextAsync( MySqlCommand command, IDbTracing tracing )
  {
    if ( command is null ) throw new ArgumentNullException( nameof( command ) );

#if MySqlConnector
    return new MySqlExecuteContext( Transaction, await command.ExecuteReaderAsync(), tracing );
#else
    return new MySqlExecuteContext( Transaction, (MySqlDataReader) await command.ExecuteReaderAsync(), tracing );
#endif
  }

}



/// <summary>
/// 用于操作 MySQL 的数据库访问工具
/// </summary>
internal abstract class MySqlDbExecutorBase( IDatabase database ) : DbExecutorBase( database ), IDbExecutor, IAsyncDbExecutor
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

  protected virtual IDbExecuteContext Execute( MySqlCommand command, IDbTracing tracing )
  {
    if ( command == null )
      throw new ArgumentNullException( nameof( command ) );

    try
    {
      TryExecuteTracing( tracing, t => t.OnExecuting( command ) );

      var flag = ApplyConnection( command );
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



  protected abstract MySqlExecuteContext CreateExecuteContext( MySqlCommand command, IDbTracing tracing );

  protected abstract Task<MySqlExecuteContext> CreateExecuteContextAsync( MySqlCommand command, IDbTracing tracing );



  protected abstract MySqlCommand ApplyConnection( MySqlCommand command );


  private static ParameterizedQuery AsParameterizedQuery( DbQuery query )
  {
    var parameterizedQuery = query as ParameterizedQuery;

    if ( parameterizedQuery == null )
      throw new NotSupportedException( $"not support query of type \"{query.GetType().FullName}\"" );
    return parameterizedQuery;
  }


  private MySqlCommand CreateCommand( DbQuery query )
  {
    var parameterizedQuery = AsParameterizedQuery( query );

    var parser = Database.ServiceProvider.GetService<IParameterizedQueryParser<MySqlCommand>>();
    if ( parser == null )
      throw new InvalidOperationException( "service of type \"IParameterizedQueryParser<MySqlCommand>\" is not registered." );

    return parser.Parse( parameterizedQuery );
  }

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

  protected virtual async Task<IAsyncDbExecuteContext> ExecuteAsync( MySqlCommand command, IDbTracing tracing )
  {

    if ( command == null )
      throw new ArgumentNullException( nameof( command ) );

    try
    {
      TryExecuteTracing( tracing, t => t.OnExecuting( command ) );
      command = ApplyConnection( command );

      var context = await CreateExecuteContextAsync( command, tracing );

      TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

      return context;
    }
    catch ( DbException exception )
    {
      TryExecuteTracing( tracing, t => t.OnException( exception ) );
      throw;
    }

  }
}
