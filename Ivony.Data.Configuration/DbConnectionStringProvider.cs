using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Ivony.Data.Configuration
{

  /// <summary>
  /// IDbConnectionStringProvider 标准实现
  /// </summary>
  public class DbConnectionStringProvider : IDbConnectionStringProvider
  {

    public DbConnectionStringProvider( IServiceProvider serviceProvider, IConfiguration configuration )
    {
      ServiceProvider = serviceProvider;
      Configuration = configuration;
    }


    /// <summary>
    /// 系统服务提供程序
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// 系统配置
    /// </summary>
    public IConfiguration Configuration { get; }


    string IDbConnectionStringProvider.GetConnectionString( Type dbProviderType )
    {
      return Configuration[$"ConnectionStrings:{dbProviderType.Name}"] ?? Configuration["ConnectionStrings:Default"] ?? throw new InvalidOperationException( $"cannot found connectionString for DbProvider [{dbProviderType.Name}]" );
    }
  }
}
