using System;
using System.Linq.Expressions;

namespace Ivony.Data
{
  public class FieldReference : Expression
  {

    internal FieldReference( string tableAlias, string fieldName )
    {
      TableAlias = tableAlias;
      FieldName = fieldName;
    }

    public string TableAlias { get; }
    public string FieldName { get; }


    public override ExpressionType NodeType => ExpressionType.Extension;

    public override Type Type => typeof( int );

    public override string ToString()
    {
      return TableAlias + "." + FieldName;
    }

  }
}