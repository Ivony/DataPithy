using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 定义通用的数据库配置项
  /// </summary>
  public class DbConfiguration
  {


    /// <summary>
    /// 创建默认的通用数据库配置
    /// </summary>
    public DbConfiguration() { }


    /// <summary>
    /// 从现有配置中创建通用数据库配置
    /// </summary>
    public DbConfiguration( DbConfiguration configuration )
    {

      QueryExecutingTimeout = configuration.QueryExecutingTimeout;

    }




    /// <summary>
    /// 获取或设置执行默认的查询超时时间
    /// </summary>
    public TimeSpan? QueryExecutingTimeout
    {
      get;
      set;
    }


  }
}
