using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{


  /// <summary>
  /// 定义数据库事务上下文
  /// </summary>
  public interface IDbTransactionContext : IDbProvider, IDisposableObjectContianer
  {
    /// <summary>
    /// 提交事务
    /// </summary>
    void Commit();

    /// <summary>
    /// 回滚事务
    /// </summary>
    void Rollback();


    /// <summary>
    /// 开启事务，若事务创建时已经开启，则调用该方法没有副作用
    /// </summary>
    void BeginTransaction();


    /// <summary>
    /// 获取事务状态
    /// </summary>
    TransactionStatus Status { get; }

  }


  /// <summary>
  /// 定义异步数据库事务上下文
  /// </summary>
  public interface IAsyncDbTransactionContext : IDbTransactionContext
  {


    /// <summary>
    /// 异步提交事务
    /// </summary>
    Task CommitAsync();

    /// <summary>
    /// 异步回滚事务
    /// </summary>
    Task RollbackAsync();

    /// <summary>
    /// 异步开启事务，若事务创建时已经开启，则调用该方法没有副作用
    /// </summary>
    Task BeginTransactionAsync();
  }



  /// <summary>
  /// 获取事务状态
  /// </summary>
  public enum TransactionStatus
  {
    /// <summary>事务尚未开始</summary>
    NotBeginning,

    /// <summary>事务正在执行</summary>
    Running,

    /// <summary>事务已完成</summary>
    Completed
  }

}

