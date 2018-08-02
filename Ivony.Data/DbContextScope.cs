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






    private void Exit()
    {
      Db.ExitContext( this, Parent );
    }


    private Action GetExiter( DbContextScope scope )
    {
      if ( this.Equals( scope ) )
        return () => this.Exit();

      var exiter = (this.Parent as DbContextScope)?.GetExiter( scope );
      if ( exiter == null )
        return null;

      return () =>
      {
        this.Exit();
        exiter();
      };
    }


    public void Dispose()
    {


      var current = Db.DbContext as DbContextScope;


      var exiter = current?.GetExiter( this );

      exiter?.Invoke();



    }
  }
}
