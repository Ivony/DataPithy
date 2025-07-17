using Ivony.Data.Common;

using Microsoft.Data.SqlClient;

namespace Ivony.Data.SqlClient
{


  /// <summary>
  /// 实现 SQL Server 执行上下文
  /// </summary>
  public class SqlDbExecuteContext : AsyncDbExecuteContextBase
  {


    /// <summary>
    /// 创建 SqlExecuteContext 对象
    /// </summary>
    /// <param name="reader">数据读取器</param>
    /// <param name="tracing">用于记录此次查询过程的的查询追踪器</param>
    /// <param name="disposeMethod">当上下文销毁时要执行的方法</param>
    internal SqlDbExecuteContext( SqlDataReader reader, IDbTracing tracing )
      : base( reader, tracing )
    {
      SqlDataReader = reader;
    }


    /// <summary>
    /// 数据读取器
    /// </summary>
    public SqlDataReader SqlDataReader
    {
      get;
      private set;
    }
  }
}
