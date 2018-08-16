using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Ivony.Data.Common;

namespace Ivony.Data.SqlClient
{

  /// <summary>
  /// 实现 SQL Server 事务上下文
  /// </summary>
  public class SqlServerTransactionContext : DbTransactionContextBase<SqlTransaction>
  {
    internal SqlServerTransactionContext( string connectionString )
    {
      Connection = new SqlConnection( connectionString );
    }

    /// <summary>
    ///  SQL Server 连接对象
    /// </summary>
    public SqlConnection Connection { get; }


    /// <summary>
    /// 开始事务
    /// </summary>
    /// <returns></returns>
    protected override SqlTransaction BeginTransactionCore()
    {
      if ( Connection.State == ConnectionState.Closed )
        Connection.Open();
      return Connection.BeginTransaction();
    }


    /// <summary>
    /// 获取查询执行器
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override IDbExecutor GetDbExecutorCore( DatabaseContext context )
    {
      return new SqlDbExecutor( this );
    }


    protected override void DisposeTransaction( SqlTransaction transaction )
    {
      base.DisposeTransaction( transaction );
      Connection.Dispose();
    }

    public override IServiceProvider DbServiceProvider { get; } = new BlankServiceProvider();

  }
}
