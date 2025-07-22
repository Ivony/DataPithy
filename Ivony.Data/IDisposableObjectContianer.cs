using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  /// <summary>
  /// 定义可销毁对象的容器，当容器销毁时，所有子对象将一并销毁。
  /// </summary>
  public interface IDisposableObjectContainer : IDisposable
  {

    /// <summary>
    /// 注册当对象销毁时要调用的方法
    /// </summary>
    /// <param name="disposable">要一并销毁的对象</param>
    void RegisterDispose( IDisposable disposable );

  }
}
