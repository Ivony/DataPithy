using Ivony.Data.Queries;
using Ivony.Data.SqlDom;
using Ivony.Data.SqlDom.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data.QueryBuilders
{
  public class SqlQueryParser
  {



    protected IParameterizedQueryBuilder Builder { get; private set; }

    protected object SyncRoot { get; } = new object();


    public virtual ParameterizedQuery ParseSelectQuery( SelectQuery query )
    {
      Builder = Db.DbContext.GetParameterizedQueryBuilder();

      ParseSelectClause( query.SelectClause );
      ParseFromClause( query.FromClause );
      ParseWhereClause( query.WhereClause );
      ParseOrderByClause( query.OrderByClause );

      return Builder.BuildQuery( new DbQueryConfigures() );
    }


    protected virtual void ParseSelectClause( SelectClause clause )
    {
      Builder.Append( "SELECT " );
      bool flag = false;
      foreach ( var element in clause.Elements )
      {
        if ( flag )
          Builder.Append( ", " );
        flag = true;

        ParseSelectElement( element );
      }
    }

    protected void ParseSelectElement( SelectElement element )
    {
      ParseExpression( element.Expression );
      Builder.Append( " AS " );
      Builder.AppendName( element.Alias );
    }

    protected virtual void ParseFromClause( FromClause clause )
    {
      Builder.Append( "\nFROM " );
      ParseFromSource( clause.Source );
    }

    protected virtual void ParseFromSource( FromSource source )
    {
      switch ( source )
      {
        case TableSource table:
          ParseTableSource( table );
          return;

        case JoinedTables join:
          ParseJoinedTables( join );
          return;
      }
    }


    protected virtual void ParseWhereClause( WhereClause clause )
    {
      if ( clause == null | clause.Condition == null )
        return;

      Builder.Append( "\nWHERE " );
      ParseExpression( clause.Condition );
    }

    protected virtual void ParseOrderByClause( OrderByClause clause )
    {

      if ( clause == null | clause.OrderByItems.Any() == false )
        return;

      Builder.Append( "\\nORDER BY " );

      bool flag = false;
      foreach ( var item in clause.OrderByItems )
      {
        if ( flag )
          Builder.Append( ", " );

        flag = true;
        ParseOrderByItem( item );
      }
    }

    protected void ParseOrderByItem( OrderByItem item )
    {
      Builder.AppendName( item.FieldAlias );
      if ( item.Ordering == OrderingType.Descending )
        Builder.Append( " DESC" );
    }

    protected virtual void ParseTableSource( TableSource table )
    {
      ParseTableExpression( table.TableExpression );
      Builder.Append( " AS " );
      Builder.AppendName( table.Alias );
    }

    protected virtual void ParseTableExpression( SqlTableExpression expression )
    {
      switch ( expression )
      {
        case TableReference table:
          ParseTable( table );
          return;
      }
    }

    protected virtual void ParseTable( TableReference table )
    {
      Builder.AppendName( table.TableName );
    }

    protected virtual void ParseJoinedTables( JoinedTables tables )
    {
      ParseFromSource( tables.Left );
      Builder.Append( ' ' );
      Builder.Append( GetJoinOperator( tables.JoinType ) );
      Builder.Append( ' ' );
      ParseFromSource( tables.Right );

      if ( tables.JoinType != TableJoinType.CrossJoin )
      {
        Builder.Append( " ON " );
        ParseOnExpression( tables.JoinCondition );
      }


    }

    protected virtual void ParseOnExpression( SqlBooleanExpression expression )
    {
      Builder.Append( '(' );
      ParseBooleanExpression( expression );
      Builder.Append( ')' );
    }


    protected virtual void ParseExpression( SqlExpression expression )
    {
      switch ( expression )
      {
        case SqlBooleanExpression boolean:
          ParseBooleanExpression( boolean );
          return;

        case SqlValueExpression value:
          ParseValueExpression( value );
          return;

        default:
          UnknowExpression( expression );
          return;
      }
    }

    protected virtual void ParseValueExpression( SqlValueExpression expression )
    {
      switch ( expression )
      {
        case FieldReference field:
          ParseFieldReference( field );
          return;

        case SqlConstantExpression constant:
          ParseParameter( constant );
          return;

        case SqlArithmeticalExpression arithmetical:
          ParseArithmeticalExpression( arithmetical );
          return;

        default:
          UnknowExpression( expression );
          return;
      }
    }

    protected virtual void ParseArithmeticalExpression( SqlArithmeticalExpression expression )
    {
      Builder.Append( '(' );
      ParseExpression( expression.Left );

      Builder.Append( GetOperator( expression.Operation ) );

      ParseExpression( expression.Right );
      Builder.Append( ')' );
    }


    protected virtual void ParseFieldReference( FieldReference field )
    {
      Builder.AppendName( field.TableAlias );
      Builder.Append( '.' );
      Builder.AppendName( field.FieldName );
    }

    protected virtual void ParseParameter( SqlConstantExpression constant )
    {
      Builder.AppendParameter( constant.Value );
    }

    protected virtual void ParseBooleanExpression( SqlBooleanExpression expression )
    {
      switch ( expression )
      {
        case SqlLogicalExpression logical:
          ParseLogicalExpression( logical );
          return;

        case SqlComparisonExpression comparision:
          ParseComparisionExpression( comparision );
          return;

        case SqlLikeExpression like:
          ParseLikeExpression( like );
          return;
      }
    }

    protected virtual void ParseLikeExpression( SqlLikeExpression expression )
    {
      Builder.Append( '(' );

      ParseExpression( expression.Left );
      Builder.Append( " LIKE " );
      ParseExpression( expression.Right );

      Builder.Append( ')' );
    }

    protected virtual void ParseComparisionExpression( SqlComparisonExpression expression )
    {
      Builder.Append( '(' );

      if ( expression.Right is SqlDbNullExpression )
      {
        if ( expression.Operation == ExpressionType.Equal )
        {
          ParseExpression( expression.Left );
          Builder.Append( " IS NULL" );
        }
        else if ( expression.Operation == ExpressionType.NotEqual )
        {
          ParseExpression( expression.Left );
          Builder.Append( " IS NOT NULL" );
        }
      }
      else
      {

        ParseExpression( expression.Left );
        Builder.Append( GetOperator( expression.Operation ) );
        ParseExpression( expression.Right );
      }


      Builder.Append( ')' );
    }

    protected virtual void ParseLogicalExpression( SqlLogicalExpression expression )
    {
      Builder.Append( '(' );
      ParseExpression( expression.Left );

      Builder.Append( GetOperator( expression.Operation ) );

      ParseExpression( expression.Right );
      Builder.Append( ')' );
    }



    protected virtual string GetJoinOperator( TableJoinType joinType )
    {
      switch ( joinType )
      {
        case TableJoinType.CrossJoin:
          return "CROSS JOIN";
        case TableJoinType.InnerJoin:
          return "INNER JOIN";
        case TableJoinType.LeftOuterJoin:
          return "LEFT OUTER JOIN";
        case TableJoinType.RightOuterJoin:
          return "RIGHT OUTER JOIN";
        case TableJoinType.FullOuterJoin:
          return "FULL OUTER JOIN";

        default:
          throw new InvalidOperationException();
      }
    }


    protected virtual string GetOperator( ExpressionType operation )
    {
      switch ( operation )
      {
        case ExpressionType.Add:
          return "+";
        case ExpressionType.Subtract:
          return "-";
        case ExpressionType.Multiply:
          return "*";
        case ExpressionType.Divide:
          return "/";


        case ExpressionType.AndAlso:
          return "AND";
        case ExpressionType.OrElse:
          return "OR";


        case ExpressionType.GreaterThan:
          return ">";
        case ExpressionType.GreaterThanOrEqual:
          return ">=";
        case ExpressionType.LessThan:
          return "<";
        case ExpressionType.LessThanOrEqual:
          return "<=";
        case ExpressionType.Equal:
          return "=";
        case ExpressionType.NotEqual:
          return "<>";

        default:
          return UnknowOperator( operation );
      }
    }

    protected virtual string UnknowOperator( ExpressionType operation )
    {
      throw new InvalidOperationException();
    }


    protected void UnknowExpression( SqlExpression expression )
    {
      throw new NotSupportedException();
    }

  }

}

