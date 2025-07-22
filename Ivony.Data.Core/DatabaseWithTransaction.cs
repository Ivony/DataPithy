using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Ivony.Data.Core;
public abstract class DatabaseWithTransaction<Command, Connection, Transaction>( IServiceProvider serviceProvider ) : Database<Command, Connection>( serviceProvider )
  where Command : IDbCommand
  where Connection : IDbConnection
  where Transaction : IDbTransaction
{

  public override IDatabaseTransaction CreateTransaction() => ServiceProvider.GetRequiredService<IDatabaseTransactionFactory<Command, Connection, Transaction>>().CreateTransaction( this );
}
