using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ivony.Data.Common;

namespace Ivony.Data
{


  /// <summary>
  /// 提供数据库的一些扩展方法。
  /// </summary>
  public static class DbExtensions
  {




    public static IAsyncDbExecutor GetAsyncDbExecutor( this IDbProvider dbProvider )
    {
      var executor = dbProvider.GetDbExecutor();
      return executor as IAsyncDbExecutor ?? new AsyncExecutorWrapper( executor );
    }


  }
}
