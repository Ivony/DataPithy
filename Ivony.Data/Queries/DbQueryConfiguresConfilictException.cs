using System;
using System.Runtime.Serialization;

namespace Ivony.Data.Queries
{
  /// <summary>
  /// 描述合并查询配置出现无法解决的冲突的异常
  /// </summary>
  [Serializable]
  internal class DbQueryConfiguresConfilictException : Exception
  {
    public DbQueryConfiguresConfilictException( string key, object value, object otherValue ) : base( $"merge DbQuery configures makes conflicts for configure key \"{key}\"" )
    {
    }
  }
}