using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  public class DbProvider
  {




    public IDbExecutor CreateExecutor()
    {
      return new DbExecutor( this );
    }


    private class DbExecutor : IDbExecutor
    {

      public DbExecutor( DbProvider provider )
      {
        Provider = provider;
      }

      public DbProvider Provider { get; }

      public IDbExecuteContext Execute( IDbQuery query )
      {
        return Provider.CreateExecutor( query )?.Execute( query );
      }
    }


    private class ExecutorItem
    {
      public Func<IDbExecutor> ExecutorFactory { get; set; }

      public Func<IDbQuery, bool> Predicate { get; set; }
    }



    private List<ExecutorItem> Items { get; } = new List<ExecutorItem>();

    protected virtual IDbExecutor CreateExecutor( IDbQuery query )
    {
      return Items.Find( item => item.Predicate( query ) ).ExecutorFactory();
    }



    public DbProvider Register( Func<IDbQuery, bool> predicate, Func<IDbExecutor> executorFactory )
    {
      Items.Add( new ExecutorItem { Predicate = predicate, ExecutorFactory = executorFactory } );
      return this;
    }


    public DbProvider Register( Func<IDbQuery, bool> predicate, IDbExecutor executor )
    {
      return Register( predicate, () => executor );
    }

    public DbProvider Register( Type queryType, IDbExecutor executor )
    {
      return Register( query => queryType.IsAssignableFrom( query.GetType() ), executor );
    }

    public DbProvider Register<T>( IDbExecutor executor ) where T : IDbQuery
    {
      return Register( typeof( T ), executor );
    }

    public DbProvider Register( Type queryType, Func<IDbExecutor> executorFactory )
    {
      return Register( query => queryType.IsAssignableFrom( query.GetType() ), executorFactory );
    }

    public DbProvider Register<T>( Func<IDbExecutor> executorFactory ) where T : IDbQuery
    {
      return Register( typeof( T ), executorFactory );
    }


  }
}
