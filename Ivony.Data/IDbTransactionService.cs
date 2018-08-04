using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Ivony.Data
{
  /// <summary>
  /// 定义数据库事务服务
  /// </summary>
  public interface IDbTransactionService
  {

    /// <summary>
    /// 创建一个事务上下文
    /// </summary>
    /// <param name="context">当前数据访问上下文</param>
    /// <returns></returns>
    IDbTransactionContext CreateTransaction( DbContext context );

  }


  public class DbTransactionService : IDbTransactionService
  {
    public IDbTransactionContext CreateTransaction( DbContext context )
    {
      return new TransactionContext();
    }
  }

  internal class TransactionContext : IDbTransactionContext
  {
    public void BeginTransaction()
    {
      throw new NotImplementedException();
    }

    public void Commit()
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }

    public void Rollback()
    {
      throw new NotImplementedException();
    }
  }
}
