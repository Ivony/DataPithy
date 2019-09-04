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
  public class MySqlDatabaseTransaction : DatabaseTransactionBase<MySqlTransaction>
  {

    internal MySqlDatabaseTransaction( MySqlDb database ) : base( database )
    {
      Database = database ?? throw new ArgumentNullException( nameof( database ) );
      Connection = new MySqlConnection( database.ConnectionString );
    }

    /// <summary>
    /// 获取数据库连接对象
    /// </summary>
    public MySqlConnection Connection { get; }

    /// <summary>
    /// 创建事务对象的数据提供程序
    /// </summary>
    public new MySqlDb Database { get; }

    protected override MySqlTransaction BeginTransactionCore()
    {
      Connection.Open();
      return Connection.BeginTransaction();
    }

    protected override IDbExecutor GetDbExecutorCore()
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
