using System;
using System.Data;

using Ivony.Data.Common;
using Ivony.Data.Queries;

using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data.Core;
internal class DbCommandFactory( IDatabase database ) : IDbCommandFactory
{
  public IDbCommand CreateCommand( DbQuery query ) => database.ServiceProvider.GetRequiredKeyedService<IDbQueryResolver>( database ).ResolveCommand( query );
}
