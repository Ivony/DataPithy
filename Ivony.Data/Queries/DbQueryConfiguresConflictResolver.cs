using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.Queries
{
  public class DbQueryConfiguresConflictResolver
  {

    /// <summary>
    /// 尝试解决合并时的冲突
    /// </summary>
    /// <param name="key">合并键</param>
    /// <param name="value">当前值</param>
    /// <param name="conflict">冲突值</param>
    /// <returns>合并后的值</returns>
    public object ResolveConflict( string key, object value, object conflict )
    {
      if ( value == null )
        return conflict;

      if ( conflict == null )
        return value;

      if ( object.Equals( value, conflict ) )
        return value;

      throw new DbQueryConfiguresConfilictException( key, value, conflict );
    }

  }
}
