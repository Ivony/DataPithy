using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{


  /// <summary>
  /// 提供数据库的一些扩展方法。
  /// </summary>
  public static class DbExtensions
  {


    /// <summary>
    /// 获取异步数据库查询执行器
    /// </summary>
    /// <param name="db">数据库提供程序</param>
    /// <param name="context">当前数据库上下文</param>
    /// <returns>异步数据库查询执行器</returns>
    public static IAsyncDbExecutor GetAsyncDbExecutor( this IDbProvider db, DbContext context )
    {
      var executor = db?.GetDbExecutor( context );
      if ( executor == null )
        return null;

      return executor as IAsyncDbExecutor ?? new AsyncExecutorWrapper( executor );

    }


  }
}
