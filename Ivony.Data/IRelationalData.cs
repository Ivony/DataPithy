using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  /// <summary>
  /// 表示一组关系数据记录的集合，提供只读列表访问能力
  /// </summary>
  public interface IRelationalData : IReadOnlyList<IRelationalDataRecord>
  {

  }

  /// <summary>
  /// 表示一条关系数据记录，提供字段值访问能力
  /// </summary>
  public interface IRelationalDataRecord
  {
    /// <summary>
    /// 根据字段名获取指定类型的字段值
    /// </summary>
    /// <typeparam name="T">字段值的类型</typeparam>
    /// <param name="name">字段名</param>
    /// <returns>指定类型的字段值</returns>
    T FieldValue<T>( string name );

  }

  /// <summary>
  /// 表示关系数据的架构信息，提供字段映射的只读列表访问能力
  /// </summary>
  public interface IRelationalDataSchema : IReadOnlyList<IRelationalDataFieldMapping>
  {

  }

  /// <summary>
  /// 表示关系数据字段的映射信息，包含字段名称、数据类型和数据库类型
  /// </summary>
  public interface IRelationalDataFieldMapping
  {
    /// <summary>
    /// 获取字段名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 获取字段的数据类型
    /// </summary>
    Type DataType { get; }

    /// <summary>
    /// 获取字段的数据库类型
    /// </summary>
    System.Data.DbType DbType { get; }

  }
}
