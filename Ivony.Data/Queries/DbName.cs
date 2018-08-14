using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.Queries
{

  /// <summary>
  /// 表示数据库对象名称
  /// </summary>
  public class DbName
  {

    /// <summary>
    /// 创建 DbName 对象
    /// </summary>
    /// <param name="name">名称</param>
    public DbName( string name )
    {
      Name = name;
    }

    /// <summary>
    /// 对象名称
    /// </summary>
    public string Name { get; }


    /// <summary>
    /// 重写 ToString 方法，输出数据对象名称
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return Name;
    }

  }
}
