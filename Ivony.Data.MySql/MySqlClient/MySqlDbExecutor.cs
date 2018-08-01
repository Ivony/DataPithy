using Ivony.Data.Common;
using Ivony.Data.Queries;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ivony.Data.MySqlClient
{
  /// <summary>
  /// 用于操作 MySQL 的数据库访问工具
  /// </summary>
  public class MySqlDbExecutor : DbExecutorBase, IDbExecutor<ParameterizedQuery>, IDbTransactionProvider<MySqlDbExecutor>
  {



    public MySqlDbExecutor( string connectionString, MySqlDbConfiguration configuration )
    {


      ConnectionString = connectionString ?? throw new ArgumentNullException( nameof( connectionString ) );
      Configuration = configuration;

    }


    protected string ConnectionString
    {
      get;
      private set;
    }



    /// <summary>
    /// 获取当前配置
    /// </summary>
    protected MySqlDbConfiguration Configuration { get; }


    public IDbExecuteContext Execute( ParameterizedQuery query )
    {

      return Execute( CreateCommand( query ), TryCreateTracing( this, query ) );

    }

    protected virtual IDbExecuteContext Execute( MySqlCommand command, IDbTracing tracing )
    {
      try
      {
        TryExecuteTracing( tracing, t => t.OnExecuting( command ) );


        var connection = new MySqlConnection( ConnectionString );
        connection.Open();
        command.Connection = connection;

        if ( Configuration.QueryExecutingTimeout.HasValue )
          command.CommandTimeout = (int) Configuration.QueryExecutingTimeout.Value.TotalSeconds;

        var context = new MySqlExecuteContext( connection, command.ExecuteReader(), tracing );

        TryExecuteTracing( tracing, t => t.OnLoadingData( context ) );

        return context;
      }
      catch ( DbException exception )
      {
        TryExecuteTracing( tracing, t => t.OnException( exception ) );
        throw;
      }
    }



    private MySqlCommand CreateCommand( ParameterizedQuery query )
    {

      return new MySqlParameterizedQueryParser().Parse( query );
    }


    IDbTransactionContext<MySqlDbExecutor> IDbTransactionProvider<MySqlDbExecutor>.CreateTransaction()
    {
      return new MySqlDbTransactionContext( ConnectionString, Configuration );
    }
  }
}
