using Ivony.Data.SqlQueries.SqlDom.Expressions;
using System;
using System.Linq.Expressions;

namespace Ivony.Data.SqlQueries.SqlDom
{
  public sealed class FieldReference : SqlValueExpression
  {

    internal FieldReference( string schemaName, string tableName, string fieldName )
    {
      SchemaName = schemaName;
      TableName = tableName;
      FieldName = fieldName;
    }
    internal FieldReference( string tableAlias, string fieldName ) : this( null, tableAlias, fieldName )
    {
    }

    public string SchemaName { get; }

    public string TableName { get; }

    public string FieldName { get; }

    public override string ToString()
    {
      return TableName + "." + FieldName;
    }
  }
}