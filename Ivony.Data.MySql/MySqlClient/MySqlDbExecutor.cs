using Ivony.Data.Common;
using Ivony.Data.Queries;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.MySqlClient
{


  internal class MySqlDbExecutor : MySqlDbExecutorBase
  {

    public MySqlDbExecutor( string connectionString )
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

    public MySqlDbExecutorWithTransaction( MySqlDbTransactionContext transaction )
    {
      Transaction = transaction;
    }

    public MySqlDbTransactionContext Transaction { get; }

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
  internal abstract class MySqlDbExecutorBase : DbExecutorBase, IDbExecutor
  {



    /// <summary>
    /// 获取当前配置
    /// </summary>
    protected MySqlDbConfiguration Configuration => Db.Context.GetConfiguration<MySqlDbConfiguration>();


    public IDbExecuteContext Execute( IDbQuery query )
    {

      var parameterizedQuery = query as ParameterizedQuery;
      if ( parameterizedQuery == null )
        return null;

      return Execute( CreateCommand( parameterizedQuery ), TryCreateTracing( this, query ) );

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
        if ( Configuration.QueryExecutingTimeout.HasValue )
          command.CommandTimeout = (int) Configuration.QueryExecutingTimeout.Value.TotalSeconds;

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

      return new MySqlParameterizedQueryParser().Parse( query );
    }
  }
}
