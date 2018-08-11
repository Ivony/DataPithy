using Ivony.Data.Common;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Ivony.Data.MySqlClient
{

  /// <summary>
  /// 实现 MySQL 数据库事务支持
  /// </summary>
  public class MySqlDbTransactionContext : DbTransactionContextBase<MySqlTransaction>
  {

    internal MySqlDbTransactionContext( string connectionString )
    {
      Connection = new MySqlConnection( connectionString );
    }

    /// <summary>
    /// 获取数据库连接对象
    /// </summary>
    public MySqlConnection Connection { get; }

    protected override MySqlTransaction BeginTransactionCore()
    {
      Connection.Open();
      return Connection.BeginTransaction();
    }

    protected override IDbExecutor GetDbExecutorCore( DbContext context )
    {
      return new MySqlDbExecutorWithTransaction( this );
    }

    protected override void DisposeTransaction( MySqlTransaction transaction )
    {
      base.DisposeTransaction( transaction );
      Connection.Dispose();
    }

  }
}
