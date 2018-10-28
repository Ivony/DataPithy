namespace Ivony.Data.Queries
{

  /// <summary>
  /// 定义参数化查询对象构建器
  /// </summary>
  public interface IParameterizedQueryBuilder
  {
    /// <summary>
    /// 用于锁定和同步的对象
    /// </summary>
    object SyncRoot { get; }


    /// <summary>
    /// 添加一个字符到查询文本
    /// </summary>
    /// <param name="ch">要添加到查询文本末尾的字符</param>
    void Append( char ch );


    /// <summary>
    /// 添加一段查询文本
    /// </summary>
    /// <param name="text">要添加到末尾的查询文本</param>
    void Append( string text );


    /// <summary>
    /// 添加一个数据库对象名称
    /// </summary>
    /// <param name="name">数据库对象名称</param>
    void Append( DbName name );


    /// <summary>
    /// 添加一个查询参数
    /// </summary>
    /// <param name="value">参数值</param>
    void AppendParameter( object value );


    /// <summary>
    /// 创建参数化查询对象实例
    /// </summary>
    /// <param name="configures">要应用于该查询的配置信息</param>
    /// <returns>参数化查询对象</returns>
    ParameterizedQuery BuildQuery();

  }
}