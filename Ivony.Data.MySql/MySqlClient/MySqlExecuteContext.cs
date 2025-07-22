using System;

using Ivony.Data.Common;

#if MySqlConnector
using MySqlConnector;
#else
using MySql.Data.MySqlClient;
#endif

namespace Ivony.Data.MySqlClient
{

  /// <summary>
  /// MySql 数据执行上下文
  /// </summary>
  public class MySqlExecuteContext : AsyncDbExecuteContextBase
  {

    /// <summary>
    /// 创建 MySqlExecuteContext 对象
    /// </summary>
    /// <param name="connection">MySql 数据库连接</param>
    /// <param name="dataReader">MySql 数据读取器</param>
    /// <param name="tracing">用于当前查询的追踪器</param>
    public MySqlExecuteContext( IDisposable connection, MySqlDataReader dataReader, IDbTracing tracing )
      : base( dataReader, tracing )
    {
      MySqlDataReader = dataReader;

      RegisterDispose( connection );
    }

    /// <summary>
    /// 创建 MySqlExecuteContext 对象
    /// </summary>
    /// <param name="transaction">MySql 数据库事务上下文</param>
    /// <param name="dataReader">MySql 数据读取器</param>
    /// <param name="tracing">用于当前查询的追踪器</param>
    public MySqlExecuteContext( MySqlDbTransaction transaction, MySqlDataReader dataReader, IDbTracing tracing )
      : base( dataReader, tracing )
    {
      TransactionContext = transaction;
      MySqlDataReader = dataReader;
    }



    /// <summary>
    /// 数据读取器
    /// </summary>
    public MySqlDataReader MySqlDataReader
    {
      get;
      private set;
    }


    /// <summary>
    /// 数据库事务上下文，如果有的话
    /// </summary>
    public MySqlDbTransaction? TransactionContext
    {
      get;
      private set;
    }


    private Lazy<IDataTableAdapter> _adapter = new Lazy<IDataTableAdapter>( new MySqlTableDataAdapter() );
    protected override IDataTableAdapter DataTableAdapter => _adapter.Value;

  }
}
