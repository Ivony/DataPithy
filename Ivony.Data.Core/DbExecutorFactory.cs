using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Ivony.Data.Core;
public class DbExecutorFactory<Command, Connection>( Database<Command, Connection> database ) : IDbExecutorFactory<Command, Connection>
  where Connection : IDbConnection
  where Command : IDbCommand
{
  public IDbExecutor GetExecutor() => new DbExecutor<Command, Connection>( database );
}
