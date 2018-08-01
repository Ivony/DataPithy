using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{

  /// <summary>
  /// 辅助构建 DbContext 对象
  /// </summary>
  public class DbContextBuilder
  {
    /// <summary>
    /// 父级数据访问上下文
    /// </summary>
    private DbContext parent;




    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceProvider"></param>
    internal DbContextBuilder( IServiceProvider serviceProvider )
    {
      ServiceProvider = serviceProvider;
      DbProviders = new Dictionary<string, IDbExecutorProvider>();
      DefaultDatabase = Db.DefaultDatabaseName;
    }


    internal DbContextBuilder( DbContext parent )
    {
      Parent = parent;
      ServiceProvider = Parent.ServiceProvider;
      DbProviders = new Dictionary<string, IDbExecutorProvider>( parent.DbProviders );
      DefaultDatabase = parent.DefaultDatabase;
    }



    protected DbContext Parent { get; }

    protected IServiceProvider ServiceProvider { get; }



    public IDictionary<string, IDbExecutorProvider> DbProviders { get; }

    public string DefaultDatabase { get; set; }


    internal DbContext Build()
    {
      DbContext context;

      if ( Parent == null )
        context = new DbContext( ServiceProvider, DbProviders );
      else
        context = new DbContextScope( Parent, DbProviders );

      context.DefaultDatabase = DefaultDatabase;
      return context;


    }
  }
}
