using Ivony.Data.Queries;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 提供数据访问基本工具方法
  /// </summary>
  public static class Db
  {


    private static readonly AsyncLocal<IDatabase> databaseHost = new AsyncLocal<IDatabase>();
    private static readonly IList<IDatabaseProvider> providers = new List<IDatabaseProvider>();


    /// <summary>
    /// 当前的数据库访问提供程序
    /// </summary>
    public static IDatabase? CurrentDatabase => databaseHost.Value ?? Database( null );





    /// <summary>
    /// 获取指定的数据库
    /// </summary>
    /// <param name="name">数据库名称</param>
    /// <returns>是否成功获取</returns>
    public static IDatabase? Database( string? name )
    {

      foreach ( var factory in providers )
      {
        var database = factory.GetDatabase( name );

        if ( database != null )
          return database;
      }


      return null;
    }



    /// <summary>
    /// 获取默认的服务提供程序
    /// </summary>
    public static IServiceProvider? ServiceProvider => CurrentDatabase?.ServiceProvider;


    /// <summary>
    /// 临时使用另一个数据库
    /// </summary>
    /// <param name="database">数据库名称</param>
    /// <returns></returns>
    public static IDisposable UseDatabase( string database )
    {
      return UseDatabase( Database( database ) ?? throw new InvalidOperationException( $"database \"{database}\" is not registered." ) );
    }

    /// <summary>
    /// 临时使用另一个数据库
    /// </summary>
    /// <param name="database">数据库名称</param>
    /// <returns></returns>
    public static IDisposable UseDatabase( IDatabase database )
    {
      if ( database == null )
        throw new ArgumentNullException( nameof( database ) );

      return DbContext.EnterContext( database );
    }




    /// <summary>
    /// 添加一个数据库访问工厂对象
    /// </summary>
    /// <param name="provider">数据库访问程序工厂</param>
    public static void AddDatabaseProvider( IDatabaseProvider provider )
    {
      providers.Add( provider );
    }


    /// <summary>
    /// 移除一个数据库访问工厂对象
    /// </summary>
    /// <param name="provider">数据库访问程序工厂</param>
    public static bool RemoveDatabaseProvider( IDatabaseProvider provider )
    {
      return providers.Remove( provider );
    }




    /// <summary>
    /// 用指定的字符串连接多个部分查询对象
    /// </summary>
    /// <param name="separator">分隔符</param>
    /// <param name="queries">要连接的部分查询对象列表</param>
    /// <returns></returns>
    /// <remarks>如果第一个对象是查询对象，则会从这个对象拷贝查询配置</remarks>
    public static ParameterizedQuery Join( string separator, params IParameterizedQueryPartial[] queries )
    {
      if ( queries == null )
        return null;

      if ( queries.Length == 0 )
        return Text( "" );


      var configures = (queries[0] as DbQuery)?.Configures ?? new DbQueryConfigures();
      return queries.AsValueList( separator ).AsQuery( configures.Clone() );

    }


    /// <summary>
    /// 将 IParameterizedQueryPartial 对象转换为 ParameterizedQuery 对象
    /// </summary>
    /// <param name="partial">要转换的 IParameterizedQueryPartial 对象</param>
    /// <param name="configures">要使用的查询配置对象</param>
    /// <returns></returns>
    public static ParameterizedQuery AsQuery( this IParameterizedQueryPartial partial, DbQueryConfigures configures = null )
    {
      var builder = ParameterizedQueryService.CreateQueryBuild();
      builder.AppendPartial( partial );
      return builder.BuildQuery( configures ?? new DbQueryConfigures() );
    }






    /// <summary>
    /// 解析模板表达式，创建参数化查询对象
    /// </summary>
    /// <param name="query">参数化模板</param>
    /// <returns>参数化查询</returns>
    public static ParameterizedQuery T( ParameterizedQuery query )
    {
      return query;
    }

    /// <summary>
    /// 解析模板表达式，创建参数化查询对象
    /// </summary>
    /// <param name="template">参数查询对象</param>
    /// <returns>参数化查询</returns>
    public static ParameterizedQuery T( FormattableString template )
    {
      return Template( template );
    }




    /// <summary>
    /// 解析模板表达式，创建参数化查询对象
    /// </summary>
    /// <param name="template">参数化模板</param>
    /// <returns>参数化查询</returns>
    public static ParameterizedQuery Template( FormattableString template )
    {
      if ( template == null )
        return null;

      return ParameterizedQueryService.GetTemplateParser().ParseTemplate( template );
    }


    /// <summary>
    /// 获取参数化查询服务
    /// </summary>
    public static IParameterizedQueryService ParameterizedQueryService => ServiceProvider.GetService<IParameterizedQueryService>() ?? Queries.ParameterizedQueryService.Instance;


    /// <summary>
    /// 解析模板表达式，创建参数化查询对象
    /// </summary>
    /// <param name="text">查询文本</param>
    /// <returns>参数化查询</returns>
    public static ParameterizedQuery Text( string text )
    {
      if ( text == null )
        return null;

      return ParameterizedQueryService.GetTemplateParser().ParseTemplate( FormattableStringFactory.Create( text ) );
    }


    /// <summary>
    /// 将文本字符串转换为数据库查询对象
    /// </summary>
    /// <param name="queryText">查询文本</param>
    /// <returns>数据库查询对象</returns>
    public static ParameterizedQuery AsTextQuery( this string queryText )
    {
      return Db.Text( queryText );
    }



    /// <summary>
    /// 开启并进入一个事务上下文
    /// </summary>
    /// <returns></returns>
    public static IDatabaseTransaction EnterTransaction()
    {
      return EnterTransaction( CurrentDatabase );
    }

    /// <summary>
    /// 开启并进入一个事务上下文
    /// </summary>
    /// <param name="database">要开启事务的数据库</param>
    /// <returns></returns>
    public static IDatabaseTransaction EnterTransaction( this IDatabase database )
    {
      var transaction = database.CreateTransaction() ?? throw new NotSupportedException();
      return EnterTransaction( transaction );
    }


    /// <summary>
    /// 进入一个事务上下文
    /// </summary>
    /// <param name="transaction">要进入的事务</param>
    /// <returns>事务上下文</returns>
    public static IDatabaseTransaction EnterTransaction( IDatabaseTransaction transaction )
    {
      if ( transaction == null )
        throw new ArgumentNullException( nameof( transaction ) );


      var context = DbContext.EnterContext( transaction );
      transaction.RegisterDispose( context );
      transaction.BeginTransaction();

      return transaction;
    }






    private class DbContext : IDisposable
    {

      public static IDisposable EnterContext( IDatabase current )
      {
        var context = new DbContext( databaseHost.Value, current );
        databaseHost.Value = context.Current;
        return context;
      }

      private DbContext( IDatabase parent, IDatabase current )
      {
        Parent = parent;
        Current = current;
      }

      public IDatabase Parent { get; }
      public IDatabase Current { get; }

      public void Dispose()
      {
        if ( databaseHost.Value != Current )
          throw new InvalidOperationException();//UNDONE 异常详细信息

        databaseHost.Value = Parent;
      }
    }



    /// <summary>
    /// 创建一个事务并异步执行
    /// </summary>
    /// <param name="actions">要执行的操作</param>
    /// <returns>用于等待操作完成的 <see cref="Task"/> 对象</returns>
    public static Task AsyncTransaction( Func<Task> actions )
    {
      return CurrentDatabase.AsyncTransaction( async () =>
      {
        await actions();
        return (object) null;
      } );
    }

    /// <summary>
    /// 创建一个事务并异步执行
    /// </summary>
    /// <param name="actions">要执行的操作</param>
    /// <returns>用于等待操作完成的 <see cref="Task"/> 对象</returns>
    public static Task<T> AsyncTransaction<T>( Func<Task<T>> actions )
    {
      return CurrentDatabase.AsyncTransaction( actions );
    }


    /// <summary>
    /// 创建一个事务并异步执行
    /// </summary>
    /// <param name="database">要开启事务的 <see cref="IDatabase"/> 对象</param>
    /// <param name="actions">要执行的操作</param>
    /// <returns>用于等待操作完成的 <see cref="Task"/> 对象</returns>
    public static async Task<T> AsyncTransaction<T>( this IDatabase database, Func<Task<T>> actions )
    {
      using ( var transaction = database.CreateTransaction() ?? throw new NotSupportedException() )
      {

        var asyncTransaction = transaction as IAsyncDatabaseTransaction;

        if ( asyncTransaction != null )
          await asyncTransaction.BeginTransactionAsync();

        else
          transaction.BeginTransaction();


        EnterTransaction( transaction );

        try
        {
          return await actions();
        }
        catch ( Exception e )
        {
          RollbackWhenError( transaction, e );

          if ( e is RollbackImmediatelyException rollback )
            return (T) rollback.Result;

          else
            throw;
        }
        finally
        {
          if ( transaction.Status == TransactionStatus.Running )
          {
            if ( asyncTransaction != null )
              await asyncTransaction.CommitAsync();

            else
              transaction.Commit();
          }
        }
      }
    }


    /// <summary>
    /// 创建一个事务并执行
    /// </summary>
    /// <param name="actions">要在事务中执行的操作</param>
    public static void Transaction( Action actions )
    {
      CurrentDatabase.Transaction( () =>
      {
        actions();
        return (object) null;
      } );
    }


    /// <summary>
    /// 创建一个事务并执行
    /// </summary>
    /// <param name="actions">要在事务中执行的操作</param>
    public static T Transaction<T>( Func<T> actions )
    {
      return Transaction( CurrentDatabase, actions );
    }

    /// <summary>
    /// 创建一个事务并执行
    /// </summary>
    /// <param name="database">创建事务的数据库</param>
    /// <param name="actions">要在事务中执行的操作</param>
    public static T Transaction<T>( this IDatabase database, Func<T> actions )
    {
      using ( var transaction = database.EnterTransaction() )
      {
        try
        {
          return actions();
        }
        catch ( Exception e )
        {
          RollbackWhenError( transaction, e );

          if ( e is RollbackImmediatelyException rollback )
            return (T) rollback.Result;

          else
            throw;
        }
        finally
        {
          if ( transaction.Status == TransactionStatus.Running )
            transaction.Commit();
        }
      }
    }

    private static void RollbackWhenError( IDatabaseTransaction transaction, Exception e )
    {
      try
      {
        if ( transaction.Status == TransactionStatus.Running )
          transaction.Rollback();
      }
      catch ( Exception ex )
      {

        var exception = new TransactionCompleteException( "exception in auto rollback database transaction.", ex );

        if ( e is RollbackImmediatelyException )
          throw exception;

        else
          throw new AggregateException( exception, e );
      }
    }

    /// <summary>
    /// 创建一个 DbName 对象
    /// </summary>
    /// <param name="name">数据对象名</param>
    /// <returns></returns>
    public static DbName Name( string name )
    {
      return new DbName( name );
    }


    /// <summary>
    /// 回滚事务
    /// </summary>
    public static void Rollback()
    {
      Rollback( null );
    }

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <param name="result">回滚后要返回的值</param>
    public static void Rollback( object result )
    {
      if ( Db.CurrentDatabase is IDatabaseTransaction == false )
        throw new InvalidOperationException( "Rollback method must invoked in transaction context" );

      throw new RollbackImmediatelyException( result );
    }

  }
}


