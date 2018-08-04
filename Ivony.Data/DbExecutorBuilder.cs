using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  public class DbExecutorBuilder
  {




    /// <summary>
    /// 创建 IDbExecutor 对象
    /// </summary>
    /// <returns></returns>
    public IDbExecutor CreateExecutor()
    {
      return new DbExecutor( this );
    }


    private class DbExecutor : IDbExecutor
    {

      public DbExecutor( DbExecutorBuilder provider )
      {
        Provider = provider;
      }

      public DbExecutorBuilder Provider { get; }

      public IDbExecuteContext Execute( IDbQuery query )
      {

        foreach ( var executor in Provider.FindExecutors( query ) )
        {
          var result = executor.Execute( query );
          if ( result != null )
            return result;
        }

        return null;
      }
    }


    private class ExecutorItem
    {
      public Func<IDbExecutor> ExecutorFactory { get; set; }

      public Func<IDbQuery, bool> Predicate { get; set; }
    }



    private List<ExecutorItem> Items { get; } = new List<ExecutorItem>();

    /// <summary>
    /// 创建 IDbExecutor 对象
    /// </summary>
    /// <param name="query">要执行的查询</param>
    /// <returns>查询执行器</returns>
    protected virtual IDbExecutor[] FindExecutors( IDbQuery query )
    {
      return Items.FindAll( item => item.Predicate( query ) ).Select( item => item.ExecutorFactory() ).ToArray();
    }



    /// <summary>
    /// 注册一个 IDbExecutor
    /// </summary>
    /// <param name="predicate">需要满足的条件</param>
    /// <param name="executorFactory">构建查询执行器的工厂方法</param>
    /// <returns></returns>
    public DbExecutorBuilder Register( Func<IDbQuery, bool> predicate, Func<IDbExecutor> executorFactory )
    {
      Items.Add( new ExecutorItem { Predicate = predicate, ExecutorFactory = executorFactory } );
      return this;
    }


    /// <summary>
    /// 注册一个查询执行器
    /// </summary>
    /// <param name="predicate">需要满足的条件</param>
    /// <param name="executor">查询执行器</param>
    /// <returns></returns>
    public DbExecutorBuilder Register( Func<IDbQuery, bool> predicate, IDbExecutor executor )
    {
      return Register( predicate, () => executor );
    }

    /// <summary>
    /// 注册一个查询执行器
    /// </summary>
    /// <param name="queryType">适用的查询类型</param>
    /// <param name="executor">查询执行器</param>
    /// <returns></returns>
    public DbExecutorBuilder Register( Type queryType, IDbExecutor executor )
    {
      return Register( query => queryType.IsAssignableFrom( query.GetType() ), executor );
    }

    /// <summary>
    /// 注册一个查询执行器
    /// </summary>
    /// <typeparam name="T">适用的查询类型</typeparam>
    /// <param name="executor">查询执行器</param>
    /// <returns></returns>
    public DbExecutorBuilder Register<T>( IDbExecutor executor ) where T : IDbQuery
    {
      return Register( typeof( T ), executor );
    }

    /// <summary>
    /// 注册一个查询执行器
    /// </summary>
    /// <param name="queryType">适用的查询类型</param>
    /// <param name="executorFactory">构建查询执行器的工厂方法</param>
    /// <returns></returns>
    public DbExecutorBuilder Register( Type queryType, Func<IDbExecutor> executorFactory )
    {
      return Register( query => queryType.IsAssignableFrom( query.GetType() ), executorFactory );
    }

    /// <summary>
    /// 注册一个查询执行器
    /// </summary>
    /// <typeparam name="T">适用的查询类型</typeparam>
    /// <param name="executorFactory">构建查询执行器的工厂方法</param>
    /// <returns>DbExecutorBuilder 对象自身</returns>
    public DbExecutorBuilder Register<T>( Func<IDbExecutor> executorFactory ) where T : IDbQuery
    {
      return Register( typeof( T ), executorFactory );
    }


  }
}
