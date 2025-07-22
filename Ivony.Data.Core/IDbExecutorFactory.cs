using System.Data;

namespace Ivony.Data.Core;
public interface IDbExecutorFactory
{

  IDbExecutor GetExecutor();

}
