using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data
{

  /// <summary>
  /// 提供数据访问基本工具方法
  /// </summary>
  public static class Db
  {


    private static readonly object sync = new object();
    private static readonly IDictionary<string, IDbProvider> _databases;




    private static IDbProvider _default;

    private static AsyncLocal<IDbTransactionContext> transactionHost = new AsyncLocal<IDbTransactionContext>();

    /// <summary>
    /// 默认的数据库访问提供程序
    /// </summary>
    public static IDbProvider DefaultDatebase => transactionHost.Value ?? _default;


    /// <summary>
    /// 获取指定的数据库
    /// </summary>
    /// <param name="name">数据库名称</param>
    /// <returns>是否成功获取</returns>
    public static IDbProvider Database( string name )
    {
      if ( _databases.TryGetValue( name, out var dbProvider ) )
        return dbProvider;

      else
        return null;
    }



    /// <summary>
    /// 获取默认的服务提供程序
    /// </summary>
    public static IServiceProvider ServiceProvider { get; }





    /// <summary>
    /// 注册一个数据库
    /// </summary>
    /// <param name="name">数据库名称</param>
    /// <param name="database">数据访问提供程序</param>
    public static void RegisterDatabase( string name, IDbProvider database )
    {
      lock ( sync )
      {
        _databases.Add( name, database );
        if ( DefaultDatebase == null )
          _default = database;
      }
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

      return ServiceProvider.GetService<ITemplateParser>().ParseTemplate( template );
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

      return ServiceProvider.GetService<ITemplateParser>().ParseTemplate( FormattableStringFactory.Create( text ) );
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
    public static IDbTransactionContext EnterTransaction()
    {
      var transaction = DefaultDatebase.CreateTransaction() ?? throw new NotSupportedException();

      return EnterTransaction( transaction );
    }


    /// <summary>
    /// 进入一个事务上下文
    /// </summary>
    /// <param name="transaction"></param>
    /// <returns></returns>
    public static IDbTransactionContext EnterTransaction( IDbTransactionContext transaction )
    {
      if ( transaction == null )
        throw new ArgumentNullException( nameof( transaction ) );

      throw new NotImplementedException();
    }




    /// <summary>
    /// 退出事务上下文
    /// </summary>
    /// <param name="transaction">事务上下文</param>
    public static void ExitTransaction( IDbTransactionContext transaction )
    {
      if ( transaction == null )
        throw new ArgumentNullException( nameof( transaction ) );

      if ( transactionHost.Value != transaction )
        throw new InvalidOperationException();

      transactionHost.Value = transaction.ParentTransaction;
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
      if ( Db.DefaultDatebase is IDbTransactionContext == false )
        throw new InvalidOperationException( "Rollback method must invoked in transaction context" );

      throw new RollbackException();
    }

  }
}


