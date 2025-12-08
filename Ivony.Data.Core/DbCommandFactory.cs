using System;
using System.Data;

using Ivony.Data.Queries;

using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data;
internal class DbCommandFactory() : IDbCommandFactory
{
  public IDbCommand CreateCommand( DbQuery query ) => query.GetDatabase().ServiceProvider.GetRequiredService<IDbQueryResolver>().ResolveCommand( query );
}
