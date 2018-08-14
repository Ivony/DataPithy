using Ivony.Data.SqlQueries.SqlDom;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static System.Linq.Expressions.Expression;

namespace Ivony.Data.SqlQueries
{
  public static class DbDynamicHost
  {
    public static dynamic Schemas => DbObjectHost.Schemas;

    public static dynamic Tables => new SchemaDynamicHost( NullSchemaReference.Instance );

    public static dynamic Names => DbObjectHost.Names;

  }

  public class TableDynamicHost : IDynamicMetaObjectProvider
  {

    public TableDynamicHost( TableReference table )
    {
      Table = table;
    }

    public TableReference Table { get; }

    public DynamicMetaObject GetMetaObject( Expression parameter )
    {
      return new MetaObject( parameter, BindingRestrictions.Empty, this );
    }

    private class MetaObject : DynamicMetaObject
    {
      public MetaObject( Expression expression, BindingRestrictions restrictions, TableDynamicHost host ) : base( expression, restrictions, host )
      {
        Table = host.Table;
      }

      public TableReference Table { get; }

      public override DynamicMetaObject BindGetMember( GetMemberBinder binder )
      {

        Expression exp = Expression;
        exp = Convert( exp, typeof( TableDynamicHost ) );
        exp = MakeMemberAccess( exp, typeof( TableDynamicHost ).GetProperty( "Table" ) );
        exp = MakeIndex( exp, typeof( TableReference ).GetProperty( "Item" ), new[] { Expression.Constant( binder.Name ) } );

        return new DynamicMetaObject( exp, BindingRestrictions.GetTypeRestriction( Expression, typeof( TableDynamicHost ) ), Table );
      }


      private static readonly Type table = typeof( TableReference );

      public override DynamicMetaObject BindInvokeMember( InvokeMemberBinder binder, DynamicMetaObject[] args )
      {

        var method = table
          .GetMethods( BindingFlags.Public | BindingFlags.Instance )
          .FirstOrDefault( m => m.Name.Equals( binder.Name, StringComparison.OrdinalIgnoreCase ) && m.GetParameters().Length == args.Length );
        if ( method == null )
          return base.BindInvokeMember( binder, args );

        var exp = Expression;
        exp = Convert( exp, typeof( TableDynamicHost ) );
        exp = MakeMemberAccess( exp, typeof( TableDynamicHost ).GetProperty( "Table" ) );
        exp = Call( exp, method, args.Select( item => item.Expression ).ToArray() );

        return new DynamicMetaObject( exp, BindingRestrictions.GetTypeRestriction( Expression, typeof( TableDynamicHost ) ), Table );
      }
    }

    public static implicit operator TableReference( TableDynamicHost host )
    {
      return host.Table;
    }
  }

  public class SchemaDynamicHost : IDynamicMetaObjectProvider
  {

    public SchemaDynamicHost( DataSchemaReference schema )
    {
      Schema = schema;
    }

    public DataSchemaReference Schema { get; }

    public DynamicMetaObject GetMetaObject( Expression parameter )
    {
      return new MetaObject( parameter, BindingRestrictions.Empty, this );
    }

    private class MetaObject : DynamicMetaObject
    {
      public MetaObject( Expression expression, BindingRestrictions restrictions, SchemaDynamicHost host ) : base( expression, restrictions, host )
      {
        Schema = host.Schema;
      }

      public DataSchemaReference Schema { get; }

      public override DynamicMetaObject BindGetMember( GetMemberBinder binder )
      {
        var result = binder.FallbackGetMember( this );

        Expression exp = Expression;
        exp = Convert( exp, typeof( SchemaDynamicHost ) );
        exp = MakeMemberAccess( exp, typeof( SchemaDynamicHost ).GetProperty( "Schema" ) );
        exp = MakeIndex( exp, typeof( DataSchemaReference ).GetProperty( "Item" ), new[] { Expression.Constant( binder.Name ) } );
        exp = New( typeof( TableDynamicHost ).GetConstructor( new[] { typeof( TableReference ) } ), exp );

        return new DynamicMetaObject( exp, result.Restrictions, Schema );
      }
    }


    public static implicit operator DataSchemaReference( SchemaDynamicHost host )
    {
      return host.Schema;
    }

  }
}
