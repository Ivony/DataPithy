using System;
using Ivony.Data.Queries;

namespace Ivony.Data
{
  internal class DefaultServiceProvider : IServiceProvider
  {


    private static readonly IServiceProvider serviceProvider = new DefaultServiceProvider();
    private static readonly ITemplateParser templateParser = new TemplateParser( serviceProvider );


    public object GetService( Type serviceType )
    {

      if ( serviceType == typeof( ITemplateParser ) )
        return templateParser;

      if ( serviceType == typeof( IParameterizedQueryBuilder ) )
        return new ParameterizedQueryBuilder();


      else
        return null;
    }

  }
}
