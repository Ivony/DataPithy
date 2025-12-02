using System.Data;

using Ivony.Data.Queries;

namespace Ivony.Data;

internal interface IDbQueryResolver
{
  IDbCommand ResolveCommand( DbQuery query );
}