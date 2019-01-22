using Ivony.Data.SqlQueries.SqlDom.Expressions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data.SqlQueries.SqlDom
{
  public class SelectQuery : SqlTableExpression, IDbExecutable
  {

    public SelectQuery( SelectClause select, FromClause from, WhereClause where, OrderByClause orderBy )
    {
      SelectClause = select;
      FromClause = from;
      WhereClause = where;
      OrderByClause = orderBy;
    }

    public SelectClause SelectClause { get; }
    public FromClause FromClause { get; }
    public WhereClause WhereClause { get; }
    public OrderByClause OrderByClause { get; }


    public override string ToString()
    {
      return $@"{SelectClause}
  {FromClause}
  {WhereClause}
  {OrderByClause}";
    }


    protected virtual ISelectQueryParser CreateParser()
    {
      return Db.DbContext.DefaultDatebase.DbServiceProvider.GetService<ISelectQueryParser>() ?? Db.DbContext.GetService<ISelectQueryParser>();
    }


    IDbExecuteContext IDbExecutable.Execute()
    {
      var parser = CreateParser();
      return parser.ParseSelectQuery( this ).Execute();

    }

    Task<IAsyncDbExecuteContext> IDbExecutable.ExecuteAsync( CancellationToken token )
    {
      var parser = CreateParser();
      return parser.ParseSelectQuery( this ).ExecuteAsync();
    }
  }
}
