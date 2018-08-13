using Ivony.Data.SqlDom;
using Ivony.Data.SqlDom.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ivony.Data.QueryBuilders
{
  public class SelectQueryBuilder
  {



    private DatabaseDynamicHost _host = new DatabaseDynamicHost();


    public SelectQueryBuilder Select( Func<dynamic, object> func )
    {
      var result = func( _host );

      if ( result is ITuple tuple )
      {
        AddElements( tuple );
      }
      else if ( result is Array list )
      {
        AddElements( list );
      }
      else
      {
        foreach ( var property in result.GetType().GetProperties() )
        {
          var value = property.GetValue( result );
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

    public SelectQueryBuilder AddElements( ITuple tuple )
    {
      for ( int i = 0; i < tuple.Length; i++ )
        AddElement( tuple[i] );

      return this;
    }

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
          return new SelectElement( field, alias ?? field.FieldName );

        default:
          return new SelectElement( value, alias );
      }
    }


    private List<SelectElement> elements = new List<SelectElement>();


    private SqlBooleanExpression filter;

    public SelectQueryBuilder Where( Func<dynamic, SqlBooleanExpression> func )
    {
      var expression = func( _host );

      filter &= expression;

      return this;
    }


    private FromSource fromSource;

    public SelectQueryBuilder From( Func<dynamic, FromSource> func )
    {
      fromSource = func( _host );
      return this;
    }


    public SelectQuery Build()
    {
      return new SelectQuery( new SelectClause( elements ), new FromClause( fromSource ), new WhereClause( filter ), new OrderByClause() );
    }


  }
}
