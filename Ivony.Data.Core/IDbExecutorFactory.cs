using System.Data;

namespace Ivony.Data;
public interface IDbExecutorFactory
{

  IDbExecutor GetExecutor();

}
