using System;
using System.Runtime.Serialization;

using Ivony.Data.Queries;

namespace Ivony.Data
{
  /// <summary>
  /// 定义一个描述查询执行过程错误的异常
  /// </summary>
  [Serializable]
  public class DbQueryExecutionException : Exception
  {


    /// <summary>
    /// 创建 DbQueryExecutionException 对象
    /// </summary>
    /// <param name="query">正在执行的查询</param>
    /// <param name="innerException">内部异常</param>
    public DbQueryExecutionException( DbQuery query, Exception innerException ) : base( $"exception in executing query:\n{query}", innerException )
    {
      DbQuery = query;
    }


    protected DbQueryExecutionException( SerializationInfo info, StreamingContext context ) : base( info, context )
    {
    }

    /// <summary>
    /// 正在执行的查询
    /// </summary>
    public DbQuery DbQuery { get; }
  }
}