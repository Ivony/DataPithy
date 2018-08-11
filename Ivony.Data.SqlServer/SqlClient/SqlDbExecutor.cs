using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Collections;
using System.Configuration;
using System.Threading.Tasks;
using Ivony.Data.Queries;

using System.Linq;
using System.Threading;
using System.Data.Common;
using Ivony.Data.Common;

namespace Ivony.Data.SqlClient
{
  /// <summary>
  /// 用于操作 SQL Server 的数据库访问工具
  /// </summary>
  public class SqlDbExecutor : DbExecutorBase, IDbExecutor, IAsyncDbExecutor
  {



    /// <summary>
    /// 创建 SqlServer 数据库查询执行程序
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    public SqlDbExecutor( string connectionString )
    {

      ConnectionString = connectionString ?? throw new ArgumentNullException( nameof( connectionString ) );
      Configuration = Db.DbContext.GetConfiguration<SqlServerConfiguration>();

    }


    /// <summary>
    /// 创建在事务中执行的 SqlServer 数据库查询执行程序
    /// </summary>
    /// <param name="transaction">数据库事务上下文（如果在事务中执行的话）</param>
    public SqlDbExecutor( SqlServerTransactionContext transaction )
    {
      Transaction = transaction ?? throw new ArgumentNullException( nameof( transaction ) );
      ConnectionString = Transaction.Connection.ConnectionString;
      Configuration = Db.DbContext.GetConfiguration<SqlServerConfiguration>();

    }




    /// <summary>
    /// 获取当前连接字符串
    /// </summary>
    protected string ConnectionString { get; }

    /// <summary>
    /// 如果在事务中执行，获取事务上下文对象
    /// </summary>
    protected SqlServerTransactionContext Transaction { get; }


    protected SqlServerConfiguration Configuration { get; }




    /// <summary>
    /// 执行查询命令并返回执行上下文
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <param name="tracing">用于追踪查询过程的追踪器</param>
    /// <returns>查询执行上下文</returns>
    protected virtual IDbExecuteContext Execute( SqlCommand command, IDbTracing tracing = null )
    {

      try
      {
        TryExecuteTracing( tracing, t => t.OnExecuting( command ) );

        var connection = Transaction?.Connection ?? new SqlConnection( ConnectionString );
        if ( connection.State == ConnectionState.Closed )
          connection.Open();
        command.Connection = connection;
        command.Transaction = Transaction?.Transaction;

        if ( Configuration.QueryExecutingTimeout.HasValue )
          command.CommandTimeout = (int) Configuration.QueryExecutingTimeout.Value.TotalSeconds;


        var reader = command.ExecuteReader();
        var context = new SqlDbExecuteContext( reader, tracing, () => OnExecuted( connection ) );

        TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

        return context;

      }
      catch ( DbException exception )
      {
        TryExecuteTracing( tracing, t => t.OnException( exception ) );
        throw;
      }
    }

    private SqlConnection GetConnection()
    {

      if ( Transaction != null )
        return Transaction.Connection;

      else
      {
        var connection = new SqlConnection( ConnectionString );
        connection.Open();
        return connection;
      }
    }

    /// <summary>
    /// 异步执行查询命令并返回执行上下文
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <param name="token">取消指示</param>
    /// <param name="tracing">用于追踪查询过程的追踪器</param>
    /// <returns>查询执行上下文</returns>
    protected virtual async Task<IAsyncDbExecuteContext> ExecuteAsync( SqlCommand command, CancellationToken token, IDbTracing tracing = null )
    {
      try
      {
        TryExecuteTracing( tracing, t => t.OnExecuting( command ) );

        var connection = Transaction?.Connection ?? new SqlConnection( ConnectionString );
        if ( connection.State == ConnectionState.Closed )
          await connection.OpenAsync( token );
        command.Connection = connection;

        if ( Configuration.QueryExecutingTimeout.HasValue )
          command.CommandTimeout = (int) Configuration.QueryExecutingTimeout.Value.TotalSeconds;


        var reader = await command.ExecuteReaderAsync( token );
        var context = new SqlDbExecuteContext( reader, tracing, () => OnExecuted( connection ) );

        TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

        return context;
      }
      catch ( DbException exception )
      {
        TryExecuteTracing( tracing, t => t.OnException( exception ) );
        throw;
      }
    }

    private void OnExecuted( SqlConnection connection )
    {
      if ( Transaction == null )
        connection.Dispose();
    }

    IDbExecuteContext IDbExecutor.Execute( IDbQuery query )
    {
      var parameterizedQuery = query as ParameterizedQuery;
      if ( parameterizedQuery == null )
        return null;

      return Execute( CreateCommand( parameterizedQuery ), TryCreateTracing( this, query ) );
    }

    Task<IAsyncDbExecuteContext> IAsyncDbExecutor.ExecuteAsync( IDbQuery query, CancellationToken token )
    {
      var parameterizedQuery = query as ParameterizedQuery;
      if ( parameterizedQuery == null )
        return null;

      return ExecuteAsync( CreateCommand( parameterizedQuery ), token, TryCreateTracing( this, query ) );
    }

    /// <summary>
    /// 从参数化查询创建查询命令对象
    /// </summary>
    /// <param name="query">参数化查询对象</param>
    /// <returns>SQL 查询命令对象</returns>
    protected SqlCommand CreateCommand( ParameterizedQuery query )
    {
      return new SqlParameterizedQueryParser().Parse( query );
    }

  }

}
