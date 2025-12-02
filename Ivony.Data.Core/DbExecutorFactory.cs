using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Ivony.Data;
public class DbExecutorFactory( Database database ) : IDbExecutorFactory
{
  public IDbExecutor GetExecutor() => new DbExecutor( database );
}
