using Ivony.Data.Queries;
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
            Initialize( configure => { } );

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
    public static DbContext Initialize( Action<DbContext.Builder> configure )
    {
      lock ( _sync )
      {
        if ( _root != null )
          throw new InvalidOperationException( "DataPithy is already initialized." );

        return _root = InitializeCore( builder =>
        {
          builder.RegisterService<IParameterizedQueryBuilder>( () => new ParameterizedQueryBuilder() );
          builder.RegisterService( typeof( ITemplateParser ), typeof( TemplateParser ) );

          configure( builder );
        } );
      }
    }




    private static DbContext InitializeCore( Action<DbContext.Builder> configure )
    {
      var builder = new DbContext.Builder();
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
    /// 在当前上下文开启一个事务执行
    /// </summary>
    /// <returns></returns>
    public static IDbTransactionContext EnterTransaction()
    {
      return EnterTransaction( builder => { } );
    }



    /// <summary>
    /// 在当前上下文开启一个事务执行
    /// </summary>
    /// <returns></returns>
    public static IDbTransactionContext EnterTransaction( Action<DbContext.Builder> configure )

    {
      var transaction = DbContext.DbProvider.CreateTransaction( DbContext ) ?? throw new NotSupportedException();

      var transactionContext = Enter( builder =>
      {
        configure( builder ); builder.SetDbProvider( transaction );
      } );
      transaction.RegisterDispose( () => transactionContext.Dispose() );

      return transaction;
    }





    /// <summary>
    /// 创建一个事务并执行
    /// </summary>
    /// <param name="actions"></param>
    public static void Transaction( Action actions )
    {
      using ( var transaction = EnterTransaction() )
      {
        try
        {
          actions();
        }
        catch ( Exception e )
        {
          if ( transaction.Status == TransactionStatus.Running )
            transaction.Rollback();

          if ( e is RollbackException == false )
            throw;
        }
        finally
        {
          if ( transaction.Status == TransactionStatus.Running )
            transaction.Commit();
        }
      }
    }

    /// <summary>
    /// 创建一个事务并执行
    /// </summary>
    /// <param name="actions"></param>
    public static T Transaction<T>( Func<T> actions )
    {
      using ( var transaction = EnterTransaction() )
      {
        try
        {
          return actions();
        }
        catch ( Exception e )
        {
          if ( transaction.Status == TransactionStatus.Running )
            transaction.Rollback();

          if ( e is RollbackException == false )
            throw;

          else
            return default( T );
        }
        finally
        {
          if ( transaction.Status == TransactionStatus.Running )
            transaction.Commit();
        }
      }
    }


    /// <summary>
    /// 回滚事务
    /// </summary>
    public static void Rollback()
    {
      if ( DbContext.DbProvider is IDbTransactionContext == false )
        throw new InvalidOperationException( "Rollback method must invoked in transaction context" );

      throw new RollbackException();
    }

  }
}


