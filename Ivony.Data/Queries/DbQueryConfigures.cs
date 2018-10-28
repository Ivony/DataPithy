using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Ivony.Data.Queries
{
  /// <summary>
  /// 定义查询配置
  /// </summary>
  public sealed class DbQueryConfigures : IDictionary<string, object>
  {
    private const string servicePrefix = "Service::";



    /// <summary>
    /// 创建 DbQueryConfigures 对象
    /// </summary>
    public DbQueryConfigures()
    {
      _settings = new Dictionary<string, object>();
    }


    private DbQueryConfigures( IDictionary<string, object> settings )
    {
      this._settings = settings;
    }


    /// <summary>
    /// 获取用于储存查询配置的字典对象
    /// </summary>
    private IDictionary<string, object> _settings;

    /// <summary>
    /// 获取与查询关联的指定类型的服务对象
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <returns>服务对象</returns>
    public TService GetService<TService>() where TService : class
    {
      if ( _settings.TryGetValue( servicePrefix + typeof( TService ).FullName, out var value ) == false )
        return default( TService );

      var factory = value as Func<TService>;
      if ( factory != null )
        return factory();


      return value as TService;
    }


    /// <summary>
    /// 设置与查询关联的服务对象
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <param name="serviceInstance">服务对象</param>
    /// <returns>返回自身便于链式调用</returns>
    public DbQueryConfigures SetService<TService>( TService serviceInstance ) where TService : class
    {
      var key = servicePrefix + typeof( TService ).FullName;
      _settings[key] = serviceInstance;
      return this;
    }


    /// <summary>
    /// 合并两个查询配置对象
    /// </summary>
    /// <param name="configures">查询配置</param>
    /// <param name="another">要进行合并的查询配置</param>
    /// <returns>合并后的查询配置对象</returns>
    public static DbQueryConfigures Merge( DbQueryConfigures configures, DbQueryConfigures another )
    {
      return configures.Clone().MergeWith( configures );
    }


    /// <summary>
    /// 与另一个查询配置对象合并
    /// </summary>
    /// <param name="configures">要与当前查询配置对象合并的查询配置对象</param>
    /// <returns>返回自身便于链式调用</returns>
    public DbQueryConfigures MergeWith( DbQueryConfigures configures )
    {

      var result = new Dictionary<string, object>();

      var theirs = configures._settings.Keys;
      var mines = _settings.Keys;

      foreach ( var key in mines.Except( theirs ) )
        result[key] = _settings[key];

      foreach ( var key in theirs.Except( mines ) )
        result[key] = configures._settings[key];



      foreach ( var key in mines.Intersect( theirs ) )
        _settings[key] = ResolveConflict( key, _settings[key], configures._settings[key] );


      _settings = result;
      return this;
    }



    /// <summary>
    /// 制作查询配置的副本
    /// </summary>
    /// <returns>一个包含完全相同的查询配置的查询配置对象</returns>
    public DbQueryConfigures Clone()
    {
      return new DbQueryConfigures( new Dictionary<string, object>( this._settings ) );
    }


    /// <summary>
    /// 制作查询配置的只读副本
    /// </summary>
    /// <returns>一个包含完全相同的查询配置的只读的查询配置对象</returns>
    public DbQueryConfigures AsReadonly()
    {
      return new DbQueryConfigures( new ReadOnlyDictionary<string, object>( this._settings ) );
    }




    #region IDictionary<> 实现

    /// <summary>
    /// 获取所有配置键
    /// </summary>
    public ICollection<string> Keys => ((IDictionary<string, object>) _settings).Keys;

    /// <summary>
    /// 获取所有配置值
    /// </summary>
    public ICollection<object> Values => ((IDictionary<string, object>) _settings).Values;

    /// <summary>
    /// 配置项数量
    /// </summary>
    public int Count => ((IDictionary<string, object>) _settings).Count;

    /// <summary>
    /// 是否只读
    /// </summary>
    public bool IsReadOnly => ((IDictionary<string, object>) _settings).IsReadOnly;

    /// <summary>
    /// 获取配置值
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>配置值</returns>
    public object this[string key] { get => ((IDictionary<string, object>) _settings)[key]; set => ((IDictionary<string, object>) _settings)[key] = value; }



    private static DbQueryConfiguresConflictResolver _conflictResolver = new DbQueryConfiguresConflictResolver();

    private static object ResolveConflict( string key, object mine, object their )
    {
      return _conflictResolver.ResolveConflict( key, mine, their );
    }

    /// <summary>
    /// 添加配置项
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    public void Add( string key, object value )
    {
      ((IDictionary<string, object>) _settings).Add( key, value );
    }

    /// <summary>
    /// 是否包含指定配置键
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>是否包含</returns>
    public bool ContainsKey( string key )
    {
      return ((IDictionary<string, object>) _settings).ContainsKey( key );
    }


    /// <summary>
    /// 移除指定配置
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>是否移除</returns>
    public bool Remove( string key )
    {
      return ((IDictionary<string, object>) _settings).Remove( key );
    }

    /// <summary>
    /// 尝试获取配置值
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    /// <returns>是否获取成功</returns>
    public bool TryGetValue( string key, out object value )
    {
      return ((IDictionary<string, object>) _settings).TryGetValue( key, out value );
    }

    public void Add( KeyValuePair<string, object> item )
    {
      ((IDictionary<string, object>) _settings).Add( item );
    }

    public void Clear()
    {
      ((IDictionary<string, object>) _settings).Clear();
    }

    public bool Contains( KeyValuePair<string, object> item )
    {
      return ((IDictionary<string, object>) _settings).Contains( item );
    }

    public void CopyTo( KeyValuePair<string, object>[] array, int arrayIndex )
    {
      ((IDictionary<string, object>) _settings).CopyTo( array, arrayIndex );
    }

    public bool Remove( KeyValuePair<string, object> item )
    {
      return ((IDictionary<string, object>) _settings).Remove( item );
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
      return ((IDictionary<string, object>) _settings).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IDictionary<string, object>) _settings).GetEnumerator();
    }

    #endregion
  }
}
