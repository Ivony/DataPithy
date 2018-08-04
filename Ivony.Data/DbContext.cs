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
  public partial class DbContext : IDisposable
  {


    private DbContext() { }


    /// <summary>
    /// 获取父级上下文
    /// </summary>
    public DbContext Parent { get; private set; }



    /// <summary>
    /// 尝试退出当前上下文
    /// </summary>
    public void TryExit()
    {
      Exit( false );

    }


    private void Exit( bool throwException )
    {
      if ( Parent == null )
      {
        if ( throwException )
          throw new InvalidOperationException( "Cannot exit the root context." );
        else
          return;
      }

      var exiter = Db.DbContext.GetExiter( this );
      if ( exiter == null )
      {
        if ( throwException )
          throw new InvalidOperationException( "Context is not in current call stack." );
        else
          return;
      }

      exiter();
    }


    private void Exit()
    {
      Db.ExitContext( this );
    }



    private Action GetExiter( DbContext scope )
    {
      if ( Parent == null )//顶级上下文不能退出
        return null;


      if ( this.Equals( scope ) )
        return () => this.Exit();

      var exiter = this.Parent.GetExiter( scope );
      if ( exiter == null )
        return null;

      return () =>
      {
        this.Exit();
        exiter();
      };
    }


    void IDisposable.Dispose()
    {

      Exit( true );

    }



    /// <summary>
    /// 服务提供程序
    /// </summary>
    public IServiceProvider ServiceProvider { get; private set; }


    /// <summary>
    /// 获取默认数据库名称
    /// </summary>
    public string DefaultDatabase { get; private set; }




    private IReadOnlyDictionary<string, IDbExecutorProvider> providers;

    private IReadOnlyDictionary<Type, object> services;


    /// <summary>
    /// 获取指定类型的服务对象实例
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>服务实例</returns>
    public T GetService<T>()
    {
      if ( services.TryGetValue( typeof( T ), out var service ) )
      {
        if ( service is T instance )
          return instance;

        return ((Func<T>) service)();
      }


      if ( Parent != null )
        return Parent.GetService<T>();

      else
        return ServiceProvider.GetService<T>();
    }


    /// <summary>
    /// 获取当前默认的查询执行器
    /// </summary>
    /// <returns></returns>
    public IDbExecutor GetExecutor( string database = null )
    {
      database = database ?? DefaultDatabase;

      if ( providers.TryGetValue( database, out var dbExecutorProvider ) )
        return dbExecutorProvider.GetDbExecutor( this );

      else if ( Parent != null )
        return Parent.GetExecutor( database );

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


      if ( providers.TryGetValue( database, out var dbExecutorProvider ) )
        return dbExecutorProvider.GetAsyncDbExecutor( this );

      else if ( Parent != null )
        return Parent.GetAsyncExecutor( database );

      else
        return ServiceProvider.GetRequiredService<IAsyncDbExecutor>();
    }


    /// <summary>
    /// 获取当前默认的追踪服务
    /// </summary>
    /// <returns></returns>
    public IDbTraceService GetTraceService()
    {
      return GetService<IDbTraceService>()
        ?? Parent?.GetService<IDbTraceService>()
        ?? ServiceProvider.GetService<IDbTraceService>();
    }

    /// <summary>
    /// 获取当前默认的模板解析器
    /// </summary>
    /// <returns></returns>
    public ITemplateParser GetTemplateParser()
    {
      return GetService<ITemplateParser>()
        ?? Parent?.GetService<ITemplateParser>()
        ?? ServiceProvider.GetRequiredService<ITemplateParser>();
    }


    /// <summary>
    /// 获取当前默认的参数化查询构建器
    /// </summary>
    /// <returns></returns>
    public IParameterizedQueryBuilder GetParameterizedQueryBuilder()
    {

      return GetService<IParameterizedQueryBuilder>()
        ?? Parent?.GetService<IParameterizedQueryBuilder>()
        ?? ServiceProvider.GetRequiredService<IParameterizedQueryBuilder>();

    }



    /// <summary>
    /// 是否自动添加空白字符分隔符
    /// </summary>
    public bool AutoWhitespaceSeparator { get; internal set; } = false;

  }
}
