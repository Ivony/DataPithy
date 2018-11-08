using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.Configuration
{
  public class DbService
  {

    public DbService( IConfiguration configuration )
    {
      var database = configuration.GetSection( "Ivony.Data:Database" )?.Value ?? "Default";
      var connection = configuration.GetConnectionString( database );
    }

  }
}
