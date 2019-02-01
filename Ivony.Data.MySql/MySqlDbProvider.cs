using Ivony.Data.Common;
using Ivony.Data.MySqlClient;
using Ivony.Data.Queries;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Ivony.Data
{

  /// <summary>
  /// MySQL 数据库访问提供程序
  /// </summary>
  public class MySqlDbProvider : IDbProvider, IServiceProvider
  {

    /// <summary>
    /// 创建 MySqlDbProvider 对象
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    public MySqlDbProvider( string connectionString, MySqlDbConfiguration configuration )
    {
      ConnectionString = connectionString ?? throw new ArgumentNullException( nameof( connectionString ) );
      Configuration = configuration;
    }


    /// <summary>
    /// MySQL 数据库连接字符串
    /// </summary>
    public string ConnectionString { get; }


    /// <summary>
    /// MySQL 数据库配置信息
    /// </summary>
    public MySqlDbConfiguration Configuration { get; }


    /// <summary>
    /// 创建事务
    /// </summary>
    /// <returns>事务上下文</returns>
    public IDbTransactionContext CreateTransaction()
    {
      return new MySqlDbTransactionContext( this );
    }

    /// <summary>
    /// 获取查询执行器
    /// </summary>
    /// <returns>查询执行器</returns>
    public IDbExecutor GetDbExecutor()
    {
      return new MySqlDbExecutor( this, ConnectionString );
    }





    /// <summary>
    /// 系统服务提供程序
    /// </summary>
    public IServiceProvider ServiceProvider => this;




    private static MySqlParameterizedQueryParser ParameterizedQueryParser { get; } = new MySqlParameterizedQueryParser();

    /// <summary>
    /// 获取服务对象
    /// </summary>
    /// <param name="serviceType">服务类型</param>
    /// <returns></returns>
    public object GetService( Type serviceType )
    {
      if ( serviceType == typeof( IParameterizedQueryParser<MySqlCommand> ) )
        return ParameterizedQueryParser;

      return null;
    }
  }
}
