using System;
using Ivony.Data.Queries;

namespace Ivony.Data
{
  public interface ITemplateParser
  {
    ParameterizedQuery ParseTemplate( FormattableString template );
  }
}