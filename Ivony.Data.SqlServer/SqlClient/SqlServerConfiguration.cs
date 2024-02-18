using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.SqlClient
{
  /// <summary>
  /// 定义用于 SQL Server 数据库的配置项
  /// </summary>
  public class SqlServerConfiguration
  {

    /// <summary>
    /// 创建一个默认的 SQL Server 数据库配置
    /// </summary>
    public SqlServerConfiguration() { }

    /// <summary>
    /// 从现有的 SQL Server 数据库配置中创建一个数据库配置
    /// </summary>
    /// <param name="configuration">现有的数据库配置</param>
    public SqlServerConfiguration( SqlServerConfiguration configuration ) { }


    /// <summary>
    /// 设置查询超时时间
    /// </summary>
    public TimeSpan? QueryExecutingTimeout { get; set; }
  }
}
