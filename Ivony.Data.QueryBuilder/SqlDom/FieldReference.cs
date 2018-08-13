using Ivony.Data.SqlDom;
using Ivony.Data.SqlDom.Expressions;
using System;
using System.Linq.Expressions;

namespace Ivony.Data.SqlDom
{
  public sealed class FieldReference : SqlValueExpression
  {

    internal FieldReference( string tableAlias, string fieldName )
    {
      TableAlias = tableAlias;
      FieldName = fieldName;
    }

    public string TableAlias { get; }
    public string FieldName { get; }

    public override string ToString()
    {
      return TableAlias + "." + FieldName;
    }
  }
}