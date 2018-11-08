using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{


  /// <summary>
  /// 提供数据库的一些扩展方法。
  /// </summary>
  public static class DbExtensions
  {


    /// <summary>
    /// 获取异步数据库查询执行器
    /// </summary>
    /// <param name="db">数据库提供程序</param>
    /// <param name="context">当前数据库上下文</param>
    /// <returns>异步数据库查询执行器</returns>
    public static IAsyncDbExecutor GetAsyncDbExecutor( this IDbProvider db, DbContext context )
    {
      var executor = db?.GetDbExecutor( context );
      if ( executor == null )
        return null;

      return executor as IAsyncDbExecutor ?? new AsyncExecutorWrapper( executor );

    }




    /// <summary>
    /// 使用指定的数据库访问提供程序执行一个方法
    /// </summary>
    /// <param name="dbProvider">要使用的数据库访问提供程序</param>
    /// <param name="action">要执行的方法</param>
    /// <returns></returns>
    public static void Run( this IDbProvider dbProvider, Action action )
    {
      using ( DbContext.Enter( builder => builder.SetDbProvider( dbProvider ) ) )
      {
        action();
      }
    }


    /// <summary>
    /// 使用指定的数据库访问提供程序执行一个方法
    /// </summary>
    /// <param name="dbProvider">要使用的数据库访问提供程序</param>
    /// <param name="action">要执行的方法</param>
    /// <returns></returns>
    public static T Run<T>( this IDbProvider dbProvider, Func<T> action )
    {
      using ( DbContext.Enter( builder => builder.SetDbProvider( dbProvider ) ) )
      {
        return action();
      }
    }

    /// <summary>
    /// 使用指定的数据库访问提供程序异步执行一个方法
    /// </summary>
    /// <param name="dbProvider">要使用的数据库访问提供程序</param>
    /// <param name="action">要执行的方法</param>
    /// <returns></returns>
    public static async Task RunAsync( this IDbProvider dbProvider, Func<Task> action )
    {
      using ( DbContext.Enter( builder => builder.SetDbProvider( dbProvider ) ) )
      {
        await action();
      }
    }

    /// <summary>
    /// 使用指定的数据库访问提供程序异步执行一个方法
    /// </summary>
    /// <param name="dbProvider">要使用的数据库访问提供程序</param>
    /// <param name="action">要执行的方法</param>
    /// <returns></returns>
    public static async Task<T> RunAsync<T>( this IDbProvider dbProvider, Func<Task<T>> action )
    {
      using ( DbContext.Enter( builder => builder.SetDbProvider( dbProvider ) ) )
      {
        return await action();
      }
    }






  }
}
