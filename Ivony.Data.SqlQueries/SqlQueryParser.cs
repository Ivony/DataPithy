﻿using Ivony.Data.Queries;
using Ivony.Data.SqlQueries.SqlDom;
using Ivony.Data.SqlQueries.SqlDom.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Ivony.Data.SqlQueries
{
  public class SqlQueryParser : ISelectQueryParser, IInsertQueryParser
  {

    public SqlQueryParser() : this( Db.ParameterizedQueryService)
    {

    }


    public SqlQueryParser( IParameterizedQueryService queryService )
    {
      QueryService = queryService;
    }


    /// <summary>
    /// 参数化查询服务
    /// </summary>
    protected virtual IParameterizedQueryService QueryService { get; }





    protected object SyncRoot { get; } = new object();

    private IParameterizedQueryBuilder builder;



    public virtual ParameterizedQuery ParseSelectQuery( SelectQuery query )
    {
      builder = QueryService.CreateQueryBuild();

      ParseSelectClause(  query.SelectClause );
      ParseFromClause(  query.FromClause );
      ParseWhereClause( query.WhereClause );
      ParseOrderByClause( query.OrderByClause );

      return builder.BuildQuery( new DbQueryConfigures() );
    }

    public virtual ParameterizedQuery ParseInsertQuery( InsertQuery query )
    {
      builder = QueryService.CreateQueryBuild();

      ParseInsertIntoClause( query.Into );
      ParseInsertColumnsClause( query.Columns );
      ParseInsertValues( query.Values );

      return builder.BuildQuery( new DbQueryConfigures() );
    }


    protected virtual void ParseSelectClause( SelectClause clause )
    {
      builder.Append( "SELECT " );
      bool flag = false;
      foreach ( var element in clause.Elements )
      {
        if ( flag )
          builder.Append( ", " );
        flag = true;

        ParseSelectElement( element );
      }
    }

    protected void ParseSelectElement( SelectElement element )
    {
      switch ( element )
      {
        case SelectElementExpression expression:
          ParseSelectElementExpression( expression );
          return;

        case SelectElementStar star:
          ParseSelectElementStar( star );
          return;
      }
    }

    protected void ParseSelectElementExpression( SelectElementExpression element )
    {
      ParseExpression( element.Expression, 0 );
      builder.Append( " AS " );
      builder.AppendName( element.Alias );
    }

    protected void ParseSelectElementStar( SelectElementStar element )
    {
      if ( element.TableAlias != null )
      {
        builder.AppendName( element.TableAlias );
        builder.Append( '.' );
      }

      builder.Append( "*" );
    }

    protected virtual void ParseFromClause( FromClause clause )
    {
      AppendNewLine();
      builder.Append( "FROM " );
      ParseFromSource( clause.Source );
    }

    /// <summary>
    /// 获取当前左侧缩进
    /// </summary>
    protected int PaddingSize { get; private set; }

    /// <summary>
    /// 获取缩进大小
    /// </summary>
    public int IndentSize { get; } = 2;

    protected void AppendNewLine()
    {
      builder.Append( '\n' );
      builder.Append( new string( ' ', PaddingSize ) );
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

      AppendNewLine();
      builder.Append( "WHERE " );
      ParseExpression( clause.Condition, 0 );
    }

    protected virtual void ParseOrderByClause( OrderByClause clause )
    {

      if ( clause == null | clause.OrderByItems.Any() == false )
        return;

      AppendNewLine();
      builder.Append( "ORDER BY " );

      bool flag = false;
      foreach ( var item in clause.OrderByItems )
      {
        if ( flag )
          builder.Append( ", " );

        flag = true;
        ParseOrderByItem( item );
      }
    }

    protected void ParseOrderByItem( OrderByItem item )
    {
      builder.AppendName( item.FieldAlias );
      if ( item.Ordering == OrderingType.Descending )
        builder.Append( " DESC" );
    }

    protected virtual void ParseTableSource( TableSource table )
    {
      ParseTableExpression( table.TableExpression );
      builder.Append( " AS " );
      builder.AppendName( table.Alias );
    }

    protected virtual void ParseTableExpression( SqlTableExpression expression )
    {
      switch ( expression )
      {
        case TableReference table:
          ParseTable( table );
          return;

        case SelectQuery query:

          AppendNewLine();
          builder.Append( '(' );
          PaddingSize += IndentSize;
          AppendNewLine();
          ParseSelectQuery( query );
          PaddingSize -= IndentSize;
          AppendNewLine();
          builder.Append( ')' );
          return;

      }
    }

    protected virtual void ParseTable( TableReference table )
    {
      builder.AppendName( table.TableName );
    }

    protected virtual void ParseJoinedTables( JoinedTables tables )
    {
      ParseFromSource( tables.Left );
      builder.Append( ' ' );
      builder.Append( GetJoinOperator( tables.JoinType ) );
      builder.Append( ' ' );
      ParseFromSource( tables.Right );

      if ( tables.JoinType != TableJoinType.CrossJoin )
        ParseOnClause( tables.JoinCondition );
    }


    protected virtual void ParseOnClause( SqlBooleanExpression expression )
    {
      builder.Append( " ON " );
      ParseExpression( expression, true );
    }





    protected virtual void ParseInsertIntoClause( InsertIntoClause into )
    {
      builder.Append( "INSERT INTO " );
      ParseTable( into.Table );
    }

    protected virtual void ParseInsertColumnsClause( InsertColumns columns )
    {

      builder.Append( '(' );

      bool flag = false;
      foreach ( var name in columns.Columns )
      {
        if ( flag )
          builder.Append( ", " );
        flag = true;
        builder.AppendName( name );
      }

      builder.Append( ')' );
    }

    protected virtual void ParseInsertValues( InsertValuesSource list )
    {
      switch ( list )
      {
        case ValuesClause valuesClase:
          ParseValues( valuesClase );
          return;
      }
    }

    protected virtual void ParseValues( ValuesClause values )
    {
      builder.Append( "VALUES" );

      foreach ( var tuple in values.ValuesList )
      {
        builder.Append( '(' );

        for ( int i = 0; i < tuple.Length; i++ )
        {
          if ( i > 0 )
            builder.Append( ", " );

          builder.AppendParameter( tuple[i] );
        }
        builder.Append( ')' );
      }
    }

    protected virtual void ParseExpression( SqlExpression expression )
    {
      ParseExpression( expression, true );
    }

    protected virtual void ParseExpression( SqlExpression expression, int priority )
    {
      ParseExpression( expression, GetExpressionPriority( expression ) < priority );
    }

    protected virtual void ParseExpression( SqlExpression expression, bool withParenthesis )
    {
      if ( withParenthesis )
        builder.Append( '(' );

      switch ( expression )
      {
        case SqlValueExpression value:
          ParseValueExpression( value );
          break; ;

        case SqlBooleanExpression boolean:
          ParseBooleanExpression( boolean );
          break;

        default:
          UnknowExpression( expression );
          break;
      }

      if ( withParenthesis )
        builder.Append( ')' );

    }

    protected virtual void ParseValueExpression( SqlValueExpression expression )
    {
      switch ( expression )
      {
        case FieldReference field:
          ParseField( field );
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
      ParseExpression( expression.Left, GetExpressionPriority( expression ) );
      builder.Append( GetOperator( expression.Operation ) );
      ParseExpression( expression.Right, GetExpressionPriority( expression ) );
    }


    protected virtual void ParseField( FieldReference field )
    {
      builder.AppendName( field.TableName );
      builder.Append( '.' );
      builder.AppendName( field.FieldName );
    }

    protected virtual void ParseParameter( SqlConstantExpression constant )
    {
      builder.AppendParameter( constant.Value );
    }





    protected virtual void ParseBooleanExpression( SqlBooleanExpression expression )
    {
      switch ( expression )
      {
        case SqlLikeExpression like:
          ParseLikeExpression( like );
          return;

        case SqlComparisonExpression comparision:
          ParseComparisionExpression( comparision );
          return;

        case SqlLogicalExpression logical:
          ParseLogicalExpression( logical );
          return;
      }
    }

    protected virtual void ParseLikeExpression( SqlLikeExpression expression )
    {
      ParseExpression( expression.Left, GetExpressionPriority( expression ) );
      builder.Append( " LIKE " );
      ParseExpression( expression.Right, GetExpressionPriority( expression ) );
    }

    protected virtual void ParseComparisionExpression( SqlComparisonExpression expression )
    {
      if ( expression.Right is SqlDbNullExpression )
      {
        if ( expression.Operation == ExpressionType.Equal )
        {
          ParseExpression( expression.Left, 0 );
          builder.Append( " IS NULL" );
        }
        else if ( expression.Operation == ExpressionType.NotEqual )
        {
          ParseExpression( expression.Left, 0 );
          builder.Append( " IS NOT NULL" );
        }
      }
      else
      {

        ParseExpression( expression.Left, GetExpressionPriority( expression ) );
        builder.Append( GetOperator( expression.Operation ) );
        ParseExpression( expression.Right, GetExpressionPriority( expression ) );
      }
    }

    protected virtual void ParseLogicalExpression( SqlLogicalExpression expression )
    {
      ParseExpression( expression.Left, GetExpressionPriority( expression ) );
      builder.Append( " " + GetOperator( expression.Operation ) + " " );
      ParseExpression( expression.Right, GetExpressionPriority( expression ) );
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


    protected virtual int GetExpressionPriority( SqlExpression expression )
    {
      switch ( expression )
      {

        case SqlArithmeticalExpression arithmetical:
          return GetOperationPriority( arithmetical.Operation );

        case SqlLogicalExpression logical:
          return GetOperationPriority( logical.Operation );

        case SqlComparisonExpression comparison:
          return GetOperationPriority( comparison.Operation );

        case SqlLikeExpression _:
          return 5;

        case FieldReference _:
          return 20;

        case TableReference _:
          return 20;

        case SqlDbNullExpression _:
          return 20;

        default:
          return 100;
      }
    }

    protected virtual int GetOperationPriority( ExpressionType operation )
    {
      switch ( operation )
      {
        case ExpressionType.Multiply:
        case ExpressionType.Divide:
          return 10;

        case ExpressionType.Add:
        case ExpressionType.Subtract:
          return 9;

        case ExpressionType.GreaterThan:
        case ExpressionType.GreaterThanOrEqual:
        case ExpressionType.LessThan:
        case ExpressionType.LessThanOrEqual:
        case ExpressionType.Equal:
        case ExpressionType.NotEqual:
          return 5;

        case ExpressionType.AndAlso:
          return 4;

        case ExpressionType.OrElse:
          return 3;


        default:
          return 100;
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

