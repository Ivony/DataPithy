using Ivony.Data.Common;
using MySqlConnector;

namespace Ivony.Data.MySqlClient
{

  /// <summary>
  /// 实现 MySQL 参数化查询解析器
  /// </summary>
  public class MySqlParameterizedQueryParser : ParameterizedQueryParser<MySqlCommand, MySqlParameter>
  {
    /// <summary>
    /// 获取参数占位符
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="index">参数位置</param>
    /// <param name="parameter">输出参数对象</param>
    /// <returns>参数占位符</returns>
    protected override string GetParameterPlaceholder( object value, int index, out MySqlParameter parameter )
    {
      var name = "?Param" + index;
      parameter = new MySqlParameter( name, value );

      return name;
    }

    /// <summary>
    /// 创建 MySqlCommand 对象
    /// </summary>
    /// <param name="commandText">命令文本</param>
    /// <param name="parameters">命令参数</param>
    /// <returns></returns>
    protected override MySqlCommand CreateCommand( string commandText, MySqlParameter[] parameters )
    {
      var command = new MySqlCommand( commandText );
      command.Parameters.AddRange( parameters );

      return command;
    }

    /// <summary>
    /// 解析 MySQL 数据库对象名称
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>对象名称表达式</returns>
    protected override string ParseDbName( string name )
    {
      return $"`{name.Replace( "`", "``" )}`";
    }
  }
}
