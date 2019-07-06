﻿using Ivony.Data.Queries;
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


    private static readonly object sync = new object();
    private static readonly IDictionary<string, IDbProvider> _databases = new Dictionary<string, IDbProvider>();



    private static AsyncLocal<IDbProvider> currentHost = new AsyncLocal<IDbProvider>();


    private static IDbProvider _default;


    /// <summary>
    /// 当前的数据库访问提供程序
    /// </summary>
    public static IDbProvider CurrentDatabase => currentHost.Value ?? _default;


    /// <summary>
    /// 设置默认数据库
    /// </summary>
    /// <param name="database">默认数据库</param>
    /// <param name="force">即便已经有默认数据库仍然要覆盖已有的设置</param>
    public static IDbProvider SetDefaultDatabase( IDbProvider database, bool force = false )
    {
      lock ( sync )
      {
        if ( force || _default == null )
          _default = database;

        return _default;
      }
    }




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
    public static IServiceProvider ServiceProvider => CurrentDatabase?.ServiceProvider ?? defaultServiceProvider;


    private static IServiceProvider defaultServiceProvider = Ivony.Data.ServiceProvider.Empty;



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
    public static IDisposable UseDatabase( IDbProvider database )
    {
      if ( database == null )
        throw new ArgumentNullException( nameof( database ) );

      return DbContext.EnterContext( database );
    }





    /// <summary>
    /// 注册一个数据库
    /// </summary>
    /// <param name="name">数据库名称</param>
    /// <param name="database">数据访问提供程序</param>
    /// <param name="immediately">是否立即使用这个数据库</param>
    public static void RegisterDatabase( string name, IDbProvider database, bool immediately = false )
    {
      lock ( sync )
      {
        _databases.Add( name, database );

        if ( immediately )
          UseDatabase( name );
      }
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
    public static IDbTransactionContext EnterTransaction()
    {
      var transaction = CurrentDatabase.CreateTransaction() ?? throw new NotSupportedException();
      return EnterTransaction( transaction );
    }


    /// <summary>
    /// 进入一个事务上下文
    /// </summary>
    /// <param name="transaction">要进入的事务</param>
    /// <returns>事务上下文</returns>
    public static IDbTransactionContext EnterTransaction( IDbTransactionContext transaction )
    {
      if ( transaction == null )
        throw new ArgumentNullException( nameof( transaction ) );


      transaction.RegisterDispose( DbContext.EnterContext( transaction ) );
      transaction.BeginTransaction();

      return transaction;
    }




    private class DbContext : IDisposable
    {

      public static IDisposable EnterContext( IDbProvider current )
      {
        var context = new DbContext( currentHost.Value, current );
        currentHost.Value = context.Current;
        return context;
      }

      private DbContext( IDbProvider parent, IDbProvider current )
      {
        Parent = parent;
        Current = current;
      }

      public IDbProvider Parent { get; }
      public IDbProvider Current { get; }

      public void Dispose()
      {
        if ( currentHost.Value != Current )
          throw new InvalidOperationException();//UNDONE 异常详细信息

        currentHost.Value = Parent;
      }
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
      if ( Db.CurrentDatabase is IDbTransactionContext == false )
        throw new InvalidOperationException( "Rollback method must invoked in transaction context" );

      throw new RollbackException();
    }

  }
}


