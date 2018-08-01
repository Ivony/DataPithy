using System;
using System.Collections.Generic;
using System.Text;
using Ivony.Data.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data
{
  public class DbContext
  {

    internal DbContext( IServiceProvider serviceProvider )
    {
      ServiceProvider = serviceProvider;
    }


    public IServiceProvider ServiceProvider { get; }


    public IDbProvider GetDbProvider()
    {
      return ServiceProvider.GetRequiredService<IDbProvider>();
    }

    public IDbTraceService GetTraceService()
    {
      return ServiceProvider.GetService<IDbTraceService>();
    }

    public ITemplateParser GetTemplateParser()
    {
      return ServiceProvider.GetRequiredService<ITemplateParser>();
    }


    public IParameterizedQueryBuilder GetParameterizedQueryBuilder()
    {
      return ServiceProvider.GetRequiredService<IParameterizedQueryBuilder>();
    }
  }
}
