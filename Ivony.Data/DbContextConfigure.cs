using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{

  /// <summary>
  /// 辅助构建 DbContext 对象
  /// </summary>
  public class DbContextConfigure
  {
    /// <summary>
    /// 父级数据访问上下文
    /// </summary>
    private DbContext parent;




    internal DbContextConfigure( IServiceProvider serviceProvider )
    {
      ServiceProvider = serviceProvider;
      DbProviders = new Dictionary<string, IDbExecutorProvider>();
      DefaultDatabase = Db.DefaultDatabaseName;
    }


    internal DbContextConfigure( DbContext parent )
    {
      Parent = parent;
      ServiceProvider = Parent.ServiceProvider;
      DbProviders = new Dictionary<string, IDbExecutorProvider>( parent.DbProviders );

      DefaultDatabase = parent.DefaultDatabase;
      AutoWhitespaceSeparator = parent.AutoWhitespaceSeparator;
    }



    /// <summary>
    /// 获取父级 DbContext 对象
    /// </summary>
    protected DbContext Parent { get; }

    /// <summary>
    /// 获取服务提供程序
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }



    /// <summary>
    /// 已经注册的数据提供程序
    /// </summary>
    public IDictionary<string, IDbExecutorProvider> DbProviders { get; }


    /// <summary>
    /// 获取或设置自动添加空白分隔符设定
    /// </summary>
    public bool AutoWhitespaceSeparator { get; set; }


    /// <summary>
    /// 获取或设置默认数据库
    /// </summary>
    public string DefaultDatabase { get; set; }


    internal DbContext Build()
    {
      DbContext context;

      if ( Parent == null )
        context = new DbContext( ServiceProvider, DbProviders );
      else
        context = new DbContextScope( Parent, DbProviders );

      context.DefaultDatabase = DefaultDatabase;
      context.AutoWhitespaceSeparator = AutoWhitespaceSeparator;
      return context;


    }
  }
}
