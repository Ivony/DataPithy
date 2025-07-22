using System.Data;

using Ivony.Data.Queries;

namespace Ivony.Data.Core;
public interface IDbCommandFactory
{

  IDbCommand CreateCommand( DbQuery query );

}