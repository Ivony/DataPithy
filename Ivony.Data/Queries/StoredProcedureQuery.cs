using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{
  /// <summary>
  /// 存储过程表达式
  /// </summary>
  public class StoredProcedureQuery : DbQuery
  {

    private readonly string _name;
    private readonly IDictionary<string, object> _parameters;

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
    public StoredProcedureQuery( string name, IDictionary<string, object> parameters, DbQueryConfigures configures = null ) : base( configures )
    {

      _name = name;
      _parameters = parameters;
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
    public IDictionary<string, object> Parameters
    {
      get { return _parameters; }
    }


    /// <summary>
    /// 创建使用指定查询配置的副本
    /// </summary>
    /// <param name="configures">查询配置</param>
    /// <returns>使用指定查询配置的副本</returns>
    protected internal override DbQuery Clone( DbQueryConfigures configures )
    {
      return new StoredProcedureQuery( Name, Parameters, configures );
    }
  }
}
