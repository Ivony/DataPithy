using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{

  /// <summary>
  /// 代表一个查询对象
  /// </summary>
  public abstract class DbQuery : IDbExecutable
  {


    /// <summary>
    /// 创建 DbQuery 对象
    /// </summary>
    /// <param name="configures">查询配置</param>
    protected DbQuery( DbQueryConfigures configures )
    {
      Configures = configures ?? new DbQueryConfigures();
    }//禁止直接从此类型派生


    /// <summary>
    /// 获取查询配置
    /// </summary>
    public DbQueryConfigures Configures { get; }

    /// <summary>
    /// 产生一个不支持查询执行的异常
    /// </summary>
    /// <returns></returns>
    protected virtual Exception NotSupported()
    {
      return new NotSupportedException( $"Execute query failed, there has no executor support query type of \"{GetType()}\"" );
    }


    /// <summary>
    /// 产生一个不支持异步执行的异常
    /// </summary>
    /// <returns></returns>
    protected virtual Exception NotSupportedAsync()
    {
      return new NotSupportedException( $"Try async execute query failed, there has no async executor support query type of \"{GetType()}\" . you can try synchronized execute it." );
    }



    /// <summary>
    /// 同步执行查询
    /// </summary>
    /// <returns></returns>
    [EditorBrowsable( EditorBrowsableState.Never )]
    public IDbExecuteContext Execute()
    {
      var executor = Configures?.GetService<IDbExecutor>() ?? GetDatabase().GetDbExecutor();
      return executor?.Execute( this ) ?? throw NotSupported(); ;
    }


    /// <summary>
    /// 异步执行查询
    /// </summary>
    /// <param name="token">取消查询标识</param>
    /// <returns></returns>
    [EditorBrowsable( EditorBrowsableState.Never )]
    public Task<IAsyncDbExecuteContext> ExecuteAsync( CancellationToken token = default )
    {

      var executor = Configures.GetService<IAsyncDbExecutor>() ?? GetDatabase().GetAsyncDbExecutor();
      return executor?.ExecuteAsync( this, token ) ?? throw NotSupportedAsync(); ;

    }


    /// <summary>
    /// 获取当前查询关联的 IDatabase 对象
    /// </summary>
    /// <returns>关联的 IDatabase 对象，若找不到关联的 IDatabase 对象则抛出异常</returns>
    protected IDatabase GetDatabase()
    {
      return Configures?.GetService<IDatabase>() ?? Db.CurrentDatabase
        ?? throw NoSpecifiedDatabase();
    }

    private static Exception NoSpecifiedDatabase()
    {
      return new InvalidOperationException( "no specified database in current context. is not initialize database? invoke UseDatabase or WithDatabase method to specify a database" );
    }



    /// <summary>
    /// 制作查询对象的副本
    /// </summary>
    /// <param name="configures">所需要采用的配置对象</param>
    /// <returns>查询对象的副本</returns>
    protected internal abstract DbQuery Clone( DbQueryConfigures configures );

  }
}
