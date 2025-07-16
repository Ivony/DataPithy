using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{
  /// <summary>
  /// 定义实体转换器类型
  /// </summary>
  /// <typeparam name="T">实体类型</typeparam>
  public interface IEntityConverter<T>
  {
    /// <summary>
    /// 将数据写入实体
    /// </summary>
    /// <param name="dataItem">数据行</param>
    /// <returns>转换的实体对象</returns>
    T Convert( IDataRecord dataItem );

    /// <summary>
    /// 是否可重用
    /// </summary>
    bool IsReusable { get; }

  }

  /// <summary>
  /// 定义异步实体转换器类型
  /// </summary>
  /// <typeparam name="T">实体类型</typeparam>
  public interface IAsyncEntityConverter<T> : IEntityConverter<T>
  {
    /// <summary>
    /// 将数据异步写入实体
    /// </summary>
    /// <param name="dataItem">数据行</param>
    /// <param name="cancellationToken">取消标志</param>
    /// <returns>转换的实体对象</returns>
    Task<T> Convert( IDataRecord dataItem, CancellationToken cancellationToken );

  }


#if NET7_0_OR_GREATER
  /// <summary>
  /// 定义可从 <see cref="IDataRecord"/> 转换为强类型的实体类型
  /// </summary>
  /// <typeparam name="T">实例类型</typeparam>
  public interface IEntityConvertable<T> where T : IEntityConvertable<T>
  {
    /// <summary>
    /// 从 <see cref="IDataRecord"/> 转换为强类型的实体对象
    /// </summary>
    /// <param name="dataRecord"></param>
    /// <returns></returns>
    public abstract static T Convert( IDataRecord dataRecord );
  }
#endif
}
