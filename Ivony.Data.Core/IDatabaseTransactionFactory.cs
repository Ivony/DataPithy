
using System.Data;

namespace Ivony.Data.Core;

public interface IDatabaseTransactionFactory<Command, Connection, Transaction>
  where Connection : IDbConnection
  where Command : IDbCommand
  where Transaction : IDbTransaction
{
  IDatabaseTransaction CreateTransaction( DatabaseWithTransaction<Command, Connection, Transaction> database ) => new DatabaseTransaction<Command, Connection, Transaction>( database );
}