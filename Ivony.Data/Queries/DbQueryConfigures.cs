using System;
using System.Collections;
using System.Collections.Generic;
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


    private DbQueryConfigures( Dictionary<string, object> settings )
    {
      this._settings = settings;
    }


    /// <summary>
    /// 获取用于储存查询配置的字典对象
    /// </summary>
    private Dictionary<string, object> _settings;

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
    public void SetService<TService>( TService serviceInstance ) where TService : class
    {
      var key = servicePrefix + typeof( TService ).FullName;
      _settings[key] = serviceInstance;
    }


    /// <summary>
    /// 合并两个查询配置对象
    /// </summary>
    /// <param name="configures">要与当前查询配置对象合并的查询配置对象</param>
    /// <returns>合并后的查询配置对象</returns>
    public DbQueryConfigures Merge( DbQueryConfigures configures )
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


      return new DbQueryConfigures( result );
    }



    #region IDictionary<> 实现

    public ICollection<string> Keys => ((IDictionary<string, object>) _settings).Keys;

    public ICollection<object> Values => ((IDictionary<string, object>) _settings).Values;

    public int Count => ((IDictionary<string, object>) _settings).Count;

    public bool IsReadOnly => ((IDictionary<string, object>) _settings).IsReadOnly;

    public object this[string key] { get => ((IDictionary<string, object>) _settings)[key]; set => ((IDictionary<string, object>) _settings)[key] = value; }



    private object ResolveConflict( string key, object mine, object their )
    {
      if ( mine.Equals( their ) )
        return mine;

      else
        throw new DbQueryConfiguresConfilictException();
    }

    public void Add( string key, object value )
    {
      ((IDictionary<string, object>) _settings).Add( key, value );
    }

    public bool ContainsKey( string key )
    {
      return ((IDictionary<string, object>) _settings).ContainsKey( key );
    }

    public bool Remove( string key )
    {
      return ((IDictionary<string, object>) _settings).Remove( key );
    }

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
