using System.Data;

using Ivony.Data.Queries;

namespace Ivony.Data.Core;
public interface IDbCommandFactory<Command> where Command : IDbCommand
{

  Command CreateCommand( DbQuery query );

}