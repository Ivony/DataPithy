using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Ivony.Data.Common;

#if MySqlConnector
using MySqlConnector;
#else
using MySql.Data.MySqlClient;
#endif

namespace Ivony.Data.MySqlClient
{
  internal class MySqlTableDataAdapter : DataTableAdapter
  {

    public override Task<DataTable> FillDataTableAsync( DbDataReader dataReader, int startRecord, int maxRecords, CancellationToken cancellationToken )
    {
      return Task.FromResult( FillDataTable( dataReader, startRecord, maxRecords ) );
    }
  }
}