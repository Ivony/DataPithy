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
    /// <returns></returns>
    IDbExecutor GetDbExecutor();


    /// <summary>
    /// 创建事务上下文
    /// </summary>
    /// <returns></returns>
    IDbTransactionContext CreateTransaction();



    /// <summary>
    /// 获取一个服务提供程序，用于获取数据库相关的服务
    /// </summary>
    IServiceProvider ServiceProvider { get; }



  }
}
