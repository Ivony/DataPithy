using Ivony.Data.Queries;
using System;

namespace Ivony.Data
{
  /// <summary>
  /// 提供查询相关的扩展方法
  /// </summary>
  public static class QueryExtensions
  {
    /// <summary>
    /// 获取查询对象关联的数据库实例
    /// </summary>
    /// <param name="query">查询对象</param>
    /// <returns>关联的数据库实例</returns>
    public static IDatabase GetDatabase( this DbQuery query )
    {
      if ( query == null )
        throw new ArgumentNullException( nameof( query ) );

      // 使用反射调用 DbQuery 内部的 protected GetDatabase 方法
      var method = typeof( DbQuery ).GetMethod( "GetDatabase", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic );
      if ( method == null )
        throw new InvalidOperationException( "DbQuery.GetDatabase method not found" );

      return (IDatabase) method.Invoke( query, null );
    }
  }
}