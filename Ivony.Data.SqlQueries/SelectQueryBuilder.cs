using Ivony.Data.SqlQueries.SqlDom;
using Ivony.Data.SqlQueries.SqlDom.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ivony.Data.SqlQueries
{
  public class SelectQueryBuilder
  {





    public SelectQueryBuilder Select( params FieldReference[] fields )
    {
      AddElements( fields );
      return this;
    }
    public SelectQueryBuilder Select( object obj )
    {
      if ( obj is Array list )
      {
        AddElements( list );
      }
#if NETCOREAPP
      else if ( obj is ITuple tuple )
      {
        AddElements( tuple );
      }
#endif

      else
      {
        foreach ( var property in obj.GetType().GetProperties() )
        {
          var value = property.GetValue( obj );
          var alias = property.Name;

          AddElement( value, alias );
        }
      }

      return this;

    }

    public SelectQueryBuilder AddElements( IEnumerable array )
    {
      foreach ( var item in array )
        AddElement( item );
      return this;
    }

#if NETCOREAPP

    public SelectQueryBuilder AddElements( ITuple tuple )
    {
      for ( int i = 0; i < tuple.Length; i++ )
        AddElement( tuple[i] );

      return this;
    }

#endif

    public SelectQueryBuilder AddElement( object value )
    {

      AddElement( value, null );

      return this;
    }

    public SelectQueryBuilder AddElement( object value, string alias )
    {

      elements.Add( CreateSelectElement( SqlValueExpression.From( value ), alias ) );


      return this;
    }

    private SelectElement CreateSelectElement( SqlValueExpression value, string alias )
    {
      switch ( value )
      {
        case FieldReference field:
          return new SelectElementExpression( field, alias ?? field.FieldName );

        default:
          return new SelectElementExpression( value, alias );
      }
    }


    private List<SelectElement> elements = new List<SelectElement>();


    private SqlBooleanExpression filter;

    public SelectQueryBuilder Where( SqlBooleanExpression expression )
    {
      filter &= expression;

      return this;
    }


    private FromSource fromSource;

    public SelectQueryBuilder From( FromSource source )
    {
      fromSource = source;
      return this;
    }


    public SelectQuery Build()
    {
      return new SelectQuery( new SelectClause( elements ), new FromClause( fromSource ), new WhereClause( filter ), new OrderByClause() );
    }


  }
}
