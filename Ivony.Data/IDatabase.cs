using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 数据库访问提供程序，代表一个数据库
  /// </summary>
  public interface IDatabase
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
    IDatabaseTransaction CreateTransaction();



    /// <summary>
    /// 获取一个服务提供程序，用于获取数据库相关的服务
    /// </summary>
    IServiceProvider ServiceProvider { get; }



  }
}
