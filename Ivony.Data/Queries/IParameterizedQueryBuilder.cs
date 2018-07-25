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
    /// 添加一个查询参数
    /// </summary>
    /// <param name="value">参数值</param>
    void AppendParameter( object value );


    /// <summary>
    /// 在当前位置添加一个部分查询
    /// </summary>
    /// <param name="partial">要添加的部分查询对象</param>
    void AppendPartial( IParameterizedQueryPartial partial );


    /// <summary>
    /// 创建参数化查询对象实例
    /// </summary>
    /// <returns>参数化查询对象</returns>
    ParameterizedQuery BuildQuery();
  }
}