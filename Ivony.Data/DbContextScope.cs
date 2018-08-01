using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  internal class DbContextScope : DbContext, IDisposable
  {

    public DbContext Parent { get; }


    public DbContextScope( DbContext parent, IDictionary<string, IDbExecutorProvider> dbProviders ) : base( parent.ServiceProvider, dbProviders )
    {
      Parent = parent;
    }




    private bool DisposeWhenParentIs( DbContext parent, Action disposeChild )
    {

      if ( Parent.Equals( parent ) )
      {
        disposeChild();
        Dispose();
        return true;
      }

      var scope = parent as DbContextScope;
      if ( scope == null )
        return false;

      else
        return scope.DisposeWhenParentIs( parent, () => Dispose() );



    }

    private void Dispose()
    {

      Db.ExitContext( this, Parent );
    }

    void IDisposable.Dispose()
    {

      var context = Db.GetCurrentContext();

      var scope = context as DbContextScope;
      scope.DisposeWhenParentIs( this, () => { } );


      Dispose();

    }


  }
}
