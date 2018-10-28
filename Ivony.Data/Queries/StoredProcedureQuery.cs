using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{
  /// <summary>
  /// 存储过程表达式
  /// </summary>
  public class StoredProcedureQuery : IDbQuery
  {

    private string _name;
    private IReadOnlyDictionary<string, object> _parameters;

    /// <summary>
    /// 创建 StoredProcedureExpression 对象
    /// </summary>
    /// <param name="name">存储过程名称</param>
    /// <param name="configures">查询配置对象</param>
    public StoredProcedureQuery( string name, DbQueryConfigures configures = null ) : this( name, new Dictionary<string, object>(), configures ) { }

    /// <summary>
    /// 创建 StoredProcedureExpression 对象
    /// </summary>
    /// <param name="name">存储过程名称</param>
    /// <param name="parameters">存储过程参数列表</param>
    /// <param name="configures">查询配置对象</param>
    public StoredProcedureQuery( string name, IDictionary<string, object> parameters, DbQueryConfigures configures = null )
    {

      _name = name;
      _parameters = new ReadOnlyDictionary<string,object>( parameters );

      Configures = configures ?? new DbQueryConfigures();
    }


    /// <summary>
    /// 存储过程名称
    /// </summary>
    public string Name
    {
      get { return _name; }
    }

    /// <summary>
    /// 存储过程参数列表
    /// </summary>
    public IReadOnlyDictionary<string, object> Parameters
    {
      get { return _parameters; }
    }

    /// <summary>
    /// 应用于此查询的配置项
    /// </summary>
    public DbQueryConfigures Configures { get; }


    /// <summary>
    /// 创建存储过程查询的副本
    /// </summary>
    /// <returns>与当前对象一致的参数化查询对象</returns>
    public StoredProcedureQuery Clone()
    {
      return Clone( null );
    }

    /// <summary>
    /// 创建存储过程查询的副本
    /// </summary>
    /// <param name="configures">查询配置</param>
    /// <returns>与当前对象一致的参数化查询对象</returns>
    public StoredProcedureQuery Clone( DbQueryConfigures configures )
    {
      return new StoredProcedureQuery( Name, new Dictionary<string, object>( Parameters ), configures );
    }


    IDbQuery IDbQuery.Clone( DbQueryConfigures configures )
    {
      throw new NotImplementedException();
    }
  }
}
