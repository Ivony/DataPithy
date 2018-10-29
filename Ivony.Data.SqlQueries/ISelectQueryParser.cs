using Ivony.Data.Queries;
using Ivony.Data.SqlQueries.SqlDom;

namespace Ivony.Data.SqlQueries
{
  public interface ISelectQueryParser
  {
    ParameterizedQuery ParseSelectQuery( SelectQuery query );
  }
}