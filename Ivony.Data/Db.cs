using Ivony.Data.Queries;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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


    private static readonly object _sync = new object();


    private static DbContext _root;

    private static AsyncLocal<DbContext> _current = new AsyncLocal<DbContext>();


    /// <summary>
    /// 获取当前数据访问上下文
    /// </summary>
    /// <returns></returns>
    public static DbContext DbContext
    {
      get
      {
        lock ( _sync )
        {
          if ( _current.Value != null )
            return _current.Value;

          if ( _root == null )
            InitializeDb( configure => { } );

          return _current.Value = _root;
        }
      }
    }



    /// <summary>
    /// 进入新的数据访问上下文
    /// </summary>
    /// <param name="configure">配置数据访问上下文的方法</param>
    /// <returns></returns>
    public static IDisposable Enter( Action<DbContext.Builder> configure )
    {
      var builder = new DbContext.Builder( DbContext );
      configure( builder );

      return _current.Value = builder.Build();
    }



    /// <summary>
    /// 退出当前上下文
    /// </summary>
    public static void Exit()
    {
      DbContext.TryExit();
    }



    internal static void ExitContext( DbContext current )
    {
      if ( _current.Value != current )
        throw new InvalidOperationException();

      _current.Value = current.Parent;
    }






    /// <summary>
    /// 初始化根数据访问上下文
    /// </summary>
    public static DbContext InitializeDb( Action<DbContext.Builder> configure )
    {
      lock ( _sync )
      {
        if ( _root != null )
          throw new InvalidOperationException( "DataPithy is already initialized." );

        return _root = InitializeDbContext( new ServiceCollection().AddDataPithy().BuildServiceProvider(), configure );
      }
    }


    /// <summary>
    /// 初始化根数据访问上下文
    /// </summary>
    /// <param name="serviceProvider">服务提供程序</param>
    /// <param name="configure">数据访问上下文配置</param>
    public static IServiceProvider InitializeDb( this IServiceProvider serviceProvider, Action<DbContext.Builder> configure )
    {
      lock ( _sync )
      {
        if ( _root != null )
          throw new InvalidOperationException( "DataPithy is already initialized." );

        _root = InitializeDbContext( serviceProvider, configure );
      }

      return serviceProvider;
    }


    private static DbContext InitializeDbContext( IServiceProvider serviceProvider, Action<DbContext.Builder> configure )
    {
      var builder = new DbContext.Builder( serviceProvider );
      configure( builder );
      return builder.Build();
    }


    /// <summary>
    /// 解析模板表达式，创建参数化查询对象
    /// </summary>
    /// <param name="template">参数化模板</param>
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

      return DbContext.GetTemplateParser().ParseTemplate( template );
    }


    /// <summary>
    /// 解析模板表达式，创建参数化查询对象
    /// </summary>
    /// <param name="text">查询文本</param>
    /// <returns>参数化查询</returns>
    public static ParameterizedQuery Text( string text )
    {
      if ( text == null )
        return null;

      return DbContext.GetTemplateParser().ParseTemplate( FormattableStringFactory.Create( text ) );
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
    /// 配置使用 DataPithy
    /// </summary>
    /// <param name="services">服务配置</param>
    public static IServiceCollection AddDataPithy( this IServiceCollection services )
    {

      services.AddSingleton( typeof( ITemplateParser ), typeof( TemplateParser ) );
      services.AddTransient( typeof( IParameterizedQueryBuilder ), typeof( ParameterizedQueryBuilder ) );

      return services;
    }


    /// <summary>
    /// 默认数据库连接名称
    /// </summary>
    public static string DefaultDatabaseName => "Default";



    public static void Transaction( Action inTransactionActions )
    {
      Transaction( DbContext, inTransactionActions );
    }

    public static void Transaction( this DbContext context, Action actions )
    {
      Transaction( context, null, actions );
    }


    public static void Transaction( string database, Action actions )
    {
      Transaction( DbContext, database, actions );
    }

    public static void Transaction( this DbContext context, string database, Action actions )
    {
      var transaction = context.CreateTransaction( database );

      bool rollbacked = false;
      try
      {
        transaction.BeginTransaction();
        actions();
      }
      catch
      {
        transaction.Rollback();
        rollbacked = true;
        throw;
      }
      finally
      {
        if ( rollbacked == false )
          transaction.Commit();
      }
    }




    public static Task Transaction( Func<Task> asyncActions )
    {
      return Transaction( DbContext, asyncActions );
    }

    public static Task Transaction( this DbContext context, Func<Task> asyncActions )
    {
      return Transaction( context, null, asyncActions );
    }

    public static Task Transaction( string database, Func<Task> asyncActions )
    {
      return Transaction( DbContext, database, asyncActions );
    }

    public static async Task Transaction( this DbContext context, string database, Func<Task> asyncActions )
    {
      var transaction = context.CreateTransaction( database );

      bool rollbacked = false;
      try
      {
        transaction.BeginTransaction();
        await asyncActions();
      }
      catch
      {
        transaction.Rollback();
        rollbacked = true;
        throw;
      }
      finally
      {
        if ( rollbacked == false )
          transaction.Commit();
      }
    }



    public static Task TransactionAsync( Func<Task> asyncActions )
    {
      return TransactionAsync( DbContext, asyncActions );
    }

    public static Task TransactionAsync( this DbContext context, Func<Task> asyncActions )
    {
      return TransactionAsync( context, null, asyncActions );
    }

    public static Task TransactionAsync( string database, Func<Task> asyncActions )
    {
      return TransactionAsync( DbContext, database, asyncActions );
    }

    public static async Task TransactionAsync( this DbContext context, string database, Func<Task> asyncActions )
    {
      var transaction = context.CreateAsyncTransaction( database );

      bool rollbacked = false;
      try
      {
        await transaction.BeginTransactionAsync();
        await asyncActions();
      }
      catch
      {
        await transaction.RollbackAsync();
        rollbacked = true;
        throw;
      }
      finally
      {
        if ( rollbacked == false )
          await transaction.CommitAsync();
      }
    }


  }
}

