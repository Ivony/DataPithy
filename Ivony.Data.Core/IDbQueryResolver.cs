using System.Data;

using Ivony.Data.Queries;

namespace Ivony.Data.Core;

internal interface IDbQueryResolver
{
  IDbCommand ResolveCommand( DbQuery query );
}