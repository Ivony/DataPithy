using System.Data;

namespace Ivony.Data.Core;
public interface IDbExecutorFactory<TCommand, TConnection>
  where TCommand : IDbCommand
  where TConnection : IDbConnection
{

  IDbExecutor GetExecutor();

}
