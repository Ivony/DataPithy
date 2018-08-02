using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Ivony.Data.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data
{
  /// <summary>
  /// 数据访问上下文
  /// </summary>
  public class DbContext
  {


    internal DbContext( IServiceProvider serviceProvider, IDictionary<string, IDbExecutorProvider> dbProviders )
    {
      ServiceProvider = serviceProvider;
      _dbProviders = new ReadOnlyDictionary<string, IDbExecutorProvider>( dbProviders );
    }


    /// <summary>
    /// 服务提供程序
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    private readonly ReadOnlyDictionary<string, IDbExecutorProvider> _dbProviders;

    /// <summary>
    /// 获取当前数据访问上下文的数据访问提供程序
    /// </summary>
    public IReadOnlyDictionary<string, IDbExecutorProvider> DbProviders => _dbProviders;

    /// <summary>
    /// 获取默认数据库名称
    /// </summary>
    public string DefaultDatabase { get; internal set; }



    /// <summary>
    /// 获取当前默认的查询执行器
    /// </summary>
    /// <returns></returns>
    public IDbExecutor GetExecutor( string database = null )
    {
      database = database ?? DefaultDatabase;


      if ( DbProviders.TryGetValue( database, out var dbExecutorProvider ) )
        return dbExecutorProvider.GetDbExecutor( this );

      else
        return ServiceProvider.GetRequiredService<IDbExecutor>();
    }



    /// <summary>
    /// 获取当前默认的异步查询执行器
    /// </summary>
    /// <returns></returns>
    public IAsyncDbExecutor GetAsyncExecutor( string database = null )
    {
      database = database ?? DefaultDatabase;


      if ( DbProviders.TryGetValue( database, out var dbExecutorProvider ) )
        return dbExecutorProvider.GetAsyncDbExecutor( this );

      else
        return ServiceProvider.GetRequiredService<IAsyncDbExecutor>();
    }


    /// <summary>
    /// 获取当前默认的追踪服务
    /// </summary>
    /// <returns></returns>
    public IDbTraceService GetTraceService()
    {
      return ServiceProvider.GetService<IDbTraceService>();
    }

    /// <summary>
    /// 获取当前默认的模板解析器
    /// </summary>
    /// <returns></returns>
    public ITemplateParser GetTemplateParser()
    {
      return ServiceProvider.GetRequiredService<ITemplateParser>();
    }


    /// <summary>
    /// 获取当前默认的参数化查询构建器
    /// </summary>
    /// <returns></returns>
    public IParameterizedQueryBuilder GetParameterizedQueryBuilder()
    {
      return ServiceProvider.GetRequiredService<IParameterizedQueryBuilder>();
    }



    /// <summary>
    /// 是否自动添加空白字符分隔符
    /// </summary>
    public bool AutoWhitespaceSeparator { get; set; } = false;

  }
}
