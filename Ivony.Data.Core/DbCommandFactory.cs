using System;
using System.Data;

using Ivony.Data.Common;
using Ivony.Data.Queries;

namespace Ivony.Data.Core;
internal class DbCommandFactory<Command>( IDatabase database ) : IDbCommandFactory<Command> where Command : IDbCommand
{
  public Command CreateCommand( DbQuery query )
  {
    var parameterizedQuery = query as ParameterizedQuery;
    if ( parameterizedQuery is not null )
      return database.ServiceProvider.GetRequiredService<IParameterizedQueryParser<Command>>().Parse( parameterizedQuery );


    throw new NotSupportedException();

  }
}
