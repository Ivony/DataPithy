using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using Ivony.Data.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data
{
  /// <summary>
  /// 数据访问上下文
  /// </summary>
  public partial class DbContext : IServiceProvider, IDisposable
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
    /// 获取默认数据库名称
    /// </summary>
    public string DefaultDatabase { get; private set; }


    private IReadOnlyDictionary<string, IDbProvider> providers;

    private IReadOnlyDictionary<Type, object> services;


    /// <summary>
    /// 获取指定类型的服务对象实例
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>服务实例</returns>
    public T GetService<T>() where T : class
    {

      {
        var instance = this as T;
        if ( instance != null )
          return instance;
      }

      if ( services.TryGetValue( typeof( T ), out var service ) )
      {
        if ( service is T instance )
          return instance;

        if ( service is Type instanceType )
          return (T) ActivatorUtilities.CreateInstance( this, instanceType );

        return ((Func<T>) service)();
      }


      if ( Parent != null )
        return Parent.GetService<T>();

      return default( T );
    }

    object IServiceProvider.GetService( Type serviceType )
    {
      return typeof( DbContext ).GetMethod( "GetService" ).MakeGenericMethod( serviceType ).Invoke( this, new object[0] );
    }



    /// <summary>
    /// 获取当前默认的查询执行器
    /// </summary>
    /// <returns></returns>
    public IDbExecutor GetExecutor( string database = null )
    {
      database = database ?? DefaultDatabase;

      return GetExecutor( this, database );
    }

    private IDbExecutor GetExecutor( DbContext context, string database )
    {
      var transaction = context.GetTransaction( database );
      if ( transaction != null )
        return transaction.GetExecutor();

      if ( providers.TryGetValue( database, out var dbExecutorProvider ) )
        return dbExecutorProvider.GetDbExecutor( context );

      else if ( Parent != null )
        return Parent.GetExecutor( context, database );

      else
        return null;
    }

    private IDbTransactionContext GetTransaction( string database )
    {

      if ( _transactions.TryGetValue( database, out var transaction ) )
      {
        if ( transaction.Status == TransactionStatus.Completed )
        {
          _transactions.Remove( database );
          return null;
        }

        return transaction;
      }

      return null;

    }

    private IDictionary<string, IDbTransactionContext> _transactions = new Dictionary<string, IDbTransactionContext>();



    /// <summary>
    /// 创建数据库事务
    /// </summary>
    /// <param name="database">指定的数据库</param>
    /// <returns></returns>
    internal IDbTransactionContext CreateTransaction( string database = null )
    {
      database = database ?? DefaultDatabase;

      return CreateTransaction( this, database );
    }

    private IDbTransactionContext CreateTransaction( DbContext context, string database )
    {
      if ( providers.TryGetValue( database, out var dbProvider ) )
        return dbProvider.CreateTransaction( context );

      else if ( context.Parent != null )
        return Parent.CreateTransaction( context, database );

      else
        return null;
    }


    /// <summary>
    /// 在当前上下文开启一个事务执行
    /// </summary>
    /// <param name="database"></param>
    /// <returns></returns>
    public IDbTransactionContext BeginTransaction( string database = null )
    {
      database = database ?? DefaultDatabase;


      if ( _transactions.TryGetValue( database, out var transaction ) )
      {
        if ( transaction.Status == TransactionStatus.Completed )
          _transactions.Remove( database );

        else
          throw new InvalidOperationException( $"数据库 {database} 在当前上下文正在执行另一个事务" );
      }

      transaction = CreateTransaction( database );
      if ( transaction == null )
        throw new NotSupportedException();

      _transactions[database] = transaction;
      transaction.BeginTransaction();
      return transaction;
    }




    /// <summary>
    /// 获取指定类型的配置对象
    /// </summary>
    /// <typeparam name="T">配置类型</typeparam>
    /// <returns></returns>
    public T GetConfiguration<T>()
    {
      return ActivatorUtilities.CreateInstance<T>( this );
    }





    /// <summary>
    /// 获取当前默认的异步查询执行器
    /// </summary>
    /// <returns></returns>
    public IAsyncDbExecutor GetAsyncExecutor( string database = null )
    {
      database = database ?? DefaultDatabase;

      return GetAsyncExecutor( this, database );
    }

    private IAsyncDbExecutor GetAsyncExecutor( DbContext context, string database )
    {
      if ( providers.TryGetValue( database, out var dbExecutorProvider ) )
        return dbExecutorProvider.GetAsyncDbExecutor( context );

      else if ( Parent != null )
        return Parent.GetAsyncExecutor( context, database );

      return null;
    }



    /// <summary>
    /// 创建异步数据库事务
    /// </summary>
    /// <param name="database">指定的数据库</param>
    /// <returns></returns>
    public IAsyncDbTransactionContext CreateAsyncTransaction( string database = null )
    {
      database = database ?? DefaultDatabase;

      return CreateAsyncTransaction( this, database );
    }

    private IAsyncDbTransactionContext CreateAsyncTransaction( DbContext context, string database )
    {
      if ( providers.TryGetValue( database, out var dbProvider ) )
        return dbProvider.CreateAsyncTransaction( context );

      else if ( context.Parent != null )
        return Parent.CreateAsyncTransaction( context, database );

      else
        return null;
    }




    /// <summary>
    /// 获取当前默认的追踪服务
    /// </summary>
    /// <returns></returns>
    public IDbTraceService GetTraceService()
    {
      return GetService<IDbTraceService>()
        ?? Parent?.GetService<IDbTraceService>();

    }

    /// <summary>
    /// 获取当前默认的模板解析器
    /// </summary>
    /// <returns></returns>
    public ITemplateParser GetTemplateParser()
    {
      return GetService<ITemplateParser>()
        ?? Parent?.GetService<ITemplateParser>();

    }


    /// <summary>
    /// 获取当前默认的参数化查询构建器
    /// </summary>
    /// <returns></returns>
    public IParameterizedQueryBuilder GetParameterizedQueryBuilder()
    {

      return GetService<IParameterizedQueryBuilder>()
        ?? Parent?.GetService<IParameterizedQueryBuilder>();

    }


    /// <summary>
    /// 是否自动添加空白字符分隔符
    /// </summary>
    public bool AutoWhitespaceSeparator { get; internal set; } = false;

  }
}
