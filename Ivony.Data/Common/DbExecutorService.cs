using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{


  /// <summary>
  /// 提供注册异步查询执行方法的服务
  /// </summary>
  public interface IAsyncDbExecutorRegisterService
  {
    /// <summary>
    /// 添加一个异步查询执行方法
    /// </summary>
    /// <param name="predicate">需要满足的条件</param>
    /// <param name="executor">查询执行方法</param>
    void Register( Func<DbQuery, bool> predicate, Func<DbQuery, Task<IAsyncDbExecuteContext>> executor );
  }

  /// <summary>
  /// 提供注册数据库查询执行方法的服务
  /// </summary>
  public interface IDbExecutorRegisterService
  {
    /// <summary>
    /// 添加一个同步查询执行方法
    /// </summary>
    /// <param name="predicate">需要满足的条件</param>
    /// <param name="executor">查询执行方法</param>
    void Register( Func<DbQuery, bool> predicate, Func<DbQuery, IDbExecuteContext> executor );
  }



  /// <summary>
  /// 辅助构建 IDbExecutor 对象的构建器
  /// </summary>
  public interface IDbExecutorBuilder : IDbExecutorRegisterService
  {
    /// <summary>
    /// 构建 IDbExecutor 对象
    /// </summary>
    /// <returns></returns>
    IDbExecutor Build();
  }


  /// <summary>
  /// 辅助构建 IDbExecutor 对象的构建器
  /// </summary>
  public interface IAsyncDbExecutorBuilder : IAsyncDbExecutorRegisterService
  {
    /// <summary>
    /// 构建 IDbExecutor 对象
    /// </summary>
    /// <returns></returns>
    IAsyncDbExecutor Build();
  }




  /// <summary>
  /// 提供 DbExecutor 注册帮助方法
  /// </summary>
  public static class DbExecutorServiceExtensions
  {



    /// <summary>
    /// 注册一个查询执行器
    /// </summary>
    /// <param name="service">查询执行器注册服务</param>
    /// <param name="predicate">需要满足的条件</param>
    /// <param name="executor">查询执行器</param>
    /// <returns></returns>
    public static IDbExecutorRegisterService AddExecutor( this IDbExecutorRegisterService service, Func<DbQuery, bool> predicate, Func<DbQuery, IDbExecuteContext> executor )
    {
      service.Register( predicate, executor );
      return service;
    }

    /// <summary>
    /// 注册一个查询执行器
    /// </summary>
    /// <param name="service">查询执行器注册服务</param>
    /// <param name="predicate">需要满足的条件</param>
    /// <param name="executor">查询执行器</param>
    /// <returns></returns>
    public static IDbExecutorRegisterService AddExecutor( this IDbExecutorRegisterService service, Func<DbQuery, bool> predicate, IDbExecutor executor )
    {
      return service.AddExecutor( predicate, executor.Execute );
    }


    /// <summary>
    /// 注册一个查询执行器
    /// </summary>
    /// <param name="service">查询执行器注册服务</param>
    /// <param name="predicate">需要满足的条件</param>
    /// <param name="executorFactory">创建查询执行器的工厂方法</param>
    /// <returns></returns>
    public static IDbExecutorRegisterService AddExecutor( this IDbExecutorRegisterService service, Func<DbQuery, bool> predicate, Func<IDbExecutor> executorFactory )
    {
      return service.AddExecutor( predicate, executorFactory().Execute );
    }


    /// <summary>
    /// 注册一个查询执行器
    /// </summary>
    /// <param name="service">查询执行器注册服务</param>
    /// <param name="queryType">适用的查询类型</param>
    /// <param name="executor">查询执行器</param>
    /// <returns></returns>
    public static IDbExecutorRegisterService AddExecutor( this IDbExecutorRegisterService service, Type queryType, Func<DbQuery, IDbExecuteContext> executor )
    {
      return service.AddExecutor( query => queryType.IsAssignableFrom( query.GetType() ), executor );
    }

    /// <summary>
    /// 注册一个查询执行器
    /// </summary>
    /// <param name="service">查询执行器注册服务</param>
    /// <param name="queryType">适用的查询类型</param>
    /// <param name="executor">查询执行器</param>
    /// <returns></returns>
    public static IDbExecutorRegisterService AddExecutor( this IDbExecutorRegisterService service, Type queryType, IDbExecutor executor )
    {
      return service.AddExecutor( query => queryType.IsAssignableFrom( query.GetType() ), executor );
    }

    /// <summary>
    /// 注册一个查询执行器
    /// </summary>
    /// <param name="service">查询执行器注册服务</param>
    /// <typeparam name="T">适用的查询类型</typeparam>
    /// <param name="executor">查询执行器</param>
    /// <returns></returns>
    public static IDbExecutorRegisterService AddExecutor<T>( this IDbExecutorRegisterService service, Func<DbQuery, IDbExecuteContext> executor ) where T : DbQuery
    {
      return service.AddExecutor( typeof( T ), executor );
    }

    /// <summary>
    /// 注册一个查询执行器
    /// </summary>
    /// <param name="service">查询执行器注册服务</param>
    /// <typeparam name="T">适用的查询类型</typeparam>
    /// <param name="executor">查询执行器</param>
    /// <returns></returns>
    public static IDbExecutorRegisterService AddExecutor<T>( this IDbExecutorRegisterService service, IDbExecutor executor ) where T : DbQuery
    {
      return service.AddExecutor( typeof( T ), executor );
    }

  }

}
