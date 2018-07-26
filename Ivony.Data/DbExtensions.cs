using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  public static class DbExtensions
  {

    public static DbEnv CreateDbEnvironment( this IServiceCollection services )
    {
      return DbEnv.CreateEnvironment( services );
    }
  }
}
