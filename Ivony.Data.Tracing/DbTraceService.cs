using System;
using System.Collections.Generic;
using System.Text;
using Ivony.Data.Queries;
using Microsoft.Extensions.Logging;

namespace Ivony.Data.Tracing
{
  public class DbTraceService : IDbTraceService
  {

    public DbTraceService( ILogger logger )
    {
      Logger = logger;
    }

    protected ILogger Logger { get; }

    public virtual IDbTracing CreateTracing( IDbExecutor executor, DbQuery query )
    {
      return new DbTracing( query, LogQueryCompleted );
    }

    protected virtual void LogQueryCompleted( DbTracing tracing )
    {
      if ( tracing.Exception != null )
        Logger.LogError( tracing.Exception, $"execute query {tracing.QueryObject} error. elapsed {tracing.QueryTime.TotalMilliseconds}ms." );

      else
        Logger.LogInformation( tracing.Exception, $"execute query {tracing.QueryObject} success. elapsed {tracing.QueryTime.TotalMilliseconds}ms." );

    }
  }
}
