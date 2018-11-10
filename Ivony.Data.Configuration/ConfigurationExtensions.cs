using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.Configuration
{
  public static class ConfigurationExtensions
  {

    public static DbContext.Builder UseConfiguration( DbContext.Builder builder, IConfiguration configuration )
    {
      throw new NotImplementedException();
    }


    public static IDbProvider GetDbProvider( IConfiguration configuration, string name )
    {

      var connectionString = configuration.GetConnectionString( "name" );


    }


  }
}
