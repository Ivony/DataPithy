using System.Data;

using Ivony.Data.Queries;

namespace Ivony.Data;
public interface IDbCommandFactory
{

  IDbCommand CreateCommand( DbQuery query );

}