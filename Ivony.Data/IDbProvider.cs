using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// IDbExecutor 提供程序
  /// </summary>
  public interface IDbProvider
  {

    /// <summary>
    /// 获取数据库查询执行器
    /// </summary>
    /// <param name="context">当前数据库访问上下文</param>
    /// <returns></returns>
    IDbExecutor GetDbExecutor( DatabaseContext context );


    /// <summary>
    /// 创建事务上下文
    /// </summary>
    /// <param name="context">当前数据库访问上下文</param>
    /// <returns></returns>
    IDbTransactionContext CreateTransaction( DatabaseContext context);


  }
}
