using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Ivony.Data;
/// <summary>
/// 数据库执行器工厂，用于创建 <see cref="IDbExecutor"/> 实例。
/// </summary>
public class DbExecutorFactory(IDbConnectionFactory connectionFactory, IDbCommandFactory commandFactory) : IDbExecutorFactory
{
  public IDbExecutor GetExecutor() => new DbExecutor(connectionFactory, commandFactory);
}
