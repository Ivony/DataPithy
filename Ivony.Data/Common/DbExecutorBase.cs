using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{

  /// <summary>
  /// 辅助实现数据库查询器的基类
  /// </summary>
  public abstract class DbExecutorBase
  {



    /// <summary>
    /// 初始化 DbExecuterBase 类型
    /// </summary>
    protected DbExecutorBase()
    {
      TraceService = Db.DbContext.GetTraceService();
    }


    /// <summary>
    /// 获取在追踪数据库查询过程的追踪服务
    /// </summary>
    protected IDbTraceService TraceService
    {
      get;
      private set;
    }



    /// <summary>
    /// 尝试创建 IDbTracing 对象
    /// </summary>
    /// <param name="executor">查询执行器</param>
    /// <param name="query">即将执行的查询对象</param>
    /// <returns>追踪该查询执行过程的 IDbTracing 对象</returns>
    protected IDbTracing TryCreateTracing( IDbExecutor executor, IDbQuery query )
    {

      if ( TraceService == null )
        return null;

      IDbTracing tracing;
      try
      {
        tracing = TraceService.CreateTracing( executor, query );
      }
      catch
      {
        return null;
      }

      return tracing;
    }




    /// <summary>
    /// 尝试创建 IDbTracing 对象
    /// </summary>
    /// <param name="query">即将执行的查询对象</param>
    /// <returns>追踪该查询执行过程的 IDbTracing 对象</returns>
    protected IDbTracing TryCreateTracing( IDbQuery query )
    {

      return TryCreateTracing( (IDbExecutor) this, query );

    }



    /// <summary>
    /// 尝试执行查询追踪器的一个追踪方法，此方法会自动判断追踪器是否存在以及对调用中出现的异常进行异常屏蔽。
    /// </summary>
    /// <param name="tracing">查询追踪器，如果有的话</param>
    /// <param name="action">要执行的追踪操作</param>
    protected void TryExecuteTracing( IDbTracing tracing, Action<IDbTracing> action )
    {
      if ( tracing == null )
        return;

      try
      {
        action( tracing );
      }
      catch
      {

      }
    }


  }
}
