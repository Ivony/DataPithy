using Ivony.Data.Common;
using Ivony.Data.Queries;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.MySqlClient
{


  internal class MySqlDbExecutor : MySqlDbExecutorBase
  {

    public MySqlDbExecutor( MySqlDb provider, string connectionString ) : base( provider )
    {
      ConnectionString = connectionString ?? throw new ArgumentNullException( nameof( connectionString ) );
    }

    protected string ConnectionString { get; }

    protected override MySqlConnection CreateConnection()
    {
      var connection = new MySqlConnection( ConnectionString );
      connection.Open();
      return connection;
    }

    protected override MySqlExecuteContext CreateExecuteContext( MySqlConnection connection, MySqlDataReader reader, IDbTracing tracing )
    {
      return new MySqlExecuteContext( connection, reader, tracing );
    }
  }



  internal class MySqlDbExecutorWithTransaction : MySqlDbExecutorBase
  {

    public MySqlDbExecutorWithTransaction( MySqlDbTransaction transaction ) : base( transaction.Database )
    {
      Transaction = transaction;
    }

    public MySqlDbTransaction Transaction { get; }

    protected override MySqlConnection CreateConnection()
    {
      return Transaction.Connection;
    }

    protected override MySqlExecuteContext CreateExecuteContext( MySqlConnection connection, MySqlDataReader reader, IDbTracing tracing )
    {
      return new MySqlExecuteContext( Transaction, reader, tracing );
    }

  }



  /// <summary>
  /// 用于操作 MySQL 的数据库访问工具
  /// </summary>
  internal abstract class MySqlDbExecutorBase : DbExecutorBase, IDbExecutor, IAsyncDbExecutor
  {
    protected MySqlDbExecutorBase( IDatabase database ) : base( database )
    {
    }


    public IDbExecuteContext Execute( DbQuery query )
    {
      var parameterizedQuery = AsParameterizedQuery( query );

      try
      {
        return Execute( CreateCommand( parameterizedQuery ), TryCreateTracing( this, query ) );
      }
      catch ( Exception e )
      {
        throw ExecuteError( e, query );
      }
    }

    private static ParameterizedQuery AsParameterizedQuery( DbQuery query )
    {
      var parameterizedQuery = query as ParameterizedQuery;

      if ( parameterizedQuery == null )
        throw new NotSupportedException( $"not support query of type \"{query.GetType().FullName}\"" );
      return parameterizedQuery;
    }

    protected virtual IDbExecuteContext Execute( MySqlCommand command, IDbTracing tracing )
    {
      if ( command == null )
        throw new ArgumentNullException( nameof( command ) );

      try
      {
        TryExecuteTracing( tracing, t => t.OnExecuting( command ) );
        var connection = CreateConnection();

        command.Connection = connection;

        var context = CreateExecuteContext( connection, command.ExecuteReader(), tracing );

        TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

        return context;
      }
      catch ( DbException exception )
      {
        TryExecuteTracing( tracing, t => t.OnException( exception ) );
        throw;
      }
    }



    protected abstract MySqlExecuteContext CreateExecuteContext( MySqlConnection connection, MySqlDataReader reader, IDbTracing tracing );

    protected abstract MySqlConnection CreateConnection();

    private MySqlCommand CreateCommand( ParameterizedQuery query )
    {

      var parser = Database.ServiceProvider.GetService<IParameterizedQueryParser<MySqlCommand>>();
      if ( parser == null )
        throw new InvalidOperationException( "service of type \"IParameterizedQueryParser<MySqlCommand>\" is not registered." );

      return parser.Parse( query );
    }

    public Task<IAsyncDbExecuteContext> ExecuteAsync( DbQuery query, CancellationToken token )
    {
      throw new NotImplementedException();
    }
  }
}
