using Ivony.Data.Queries;
using Ivony.Data.SqlQueries.SqlDom;

namespace Ivony.Data.SqlQueries
{
  public interface IInsertQueryParser
  {
    ParameterizedQuery ParseInsertQuery( InsertQuery query );
  }
}