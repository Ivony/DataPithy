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
    /// 获取系统服务
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <param name="serviceProvider">服务提供程序</param>
    /// <returns></returns>
    public static TService GetService<TService>( this IServiceProvider serviceProvider )
    {
      return (TService) serviceProvider.GetService( typeof( TService ) );
    }


    /// <summary>
    /// 获取异步数据库查询执行器
    /// </summary>
    /// <param name="database">数据库提供程序</param>
    /// <returns>异步数据库查询执行器</returns>
    public static IAsyncDbExecutor GetAsyncDbExecutor( this IDatabase database )
    {
      var executor = database?.GetDbExecutor();
      if ( executor == null )
        return null;

      return executor as IAsyncDbExecutor ?? new AsyncExecutorWrapper( executor );

    }




    /// <summary>
    /// 使用指定的数据库执行一个方法
    /// </summary>
    /// <param name="database">要使用的数据库</param>
    /// <param name="action">要执行的方法</param>
    /// <returns></returns>
    public static void Run( this IDatabase database, Action action )
    {
      using ( Db.UseDatabase( database ) )
      {
        action();
      }
    }


    /// <summary>
    /// 使用指定的数据库访问提供程序执行一个方法
    /// </summary>
    /// <param name="database">要使用的数据库访问提供程序</param>
    /// <param name="action">要执行的方法</param>
    /// <returns></returns>
    public static T Run<T>( this IDatabase database, Func<T> action )
    {
      using ( Db.UseDatabase( database ) )
      {
        return action();
      }
    }

    /// <summary>
    /// 使用指定的数据库访问提供程序异步执行一个方法
    /// </summary>
    /// <param name="database">要使用的数据库访问提供程序</param>
    /// <param name="action">要执行的方法</param>
    /// <returns></returns>
    public static async Task RunAsync( this IDatabase database, Func<Task> action )
    {
      using ( Db.UseDatabase( database ) )
      {
        await action();
      }
    }

    /// <summary>
    /// 使用指定的数据库访问提供程序异步执行一个方法
    /// </summary>
    /// <param name="database">要使用的数据库访问提供程序</param>
    /// <param name="action">要执行的方法</param>
    /// <returns></returns>
    public static async Task<T> RunAsync<T>( this IDatabase database, Func<Task<T>> action )
    {
      using ( Db.UseDatabase( database ) )
      {
        return await action();
      }
    }






  }
}
