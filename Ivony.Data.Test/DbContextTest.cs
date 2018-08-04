using System;
using System.Data;
using System.Linq;
using Ivony.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Data.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data.Test
{
  [TestClass]
  public class DbContextTest
  {




    [TestMethod]
    public void CurrentContext()
    {


      Assert.AreEqual( Db.DbContext.DefaultDatabase, Db.DefaultDatabaseName );

      using ( Db.Enter( builder => { builder.SetDefaultDatabase( "Test" ); } ) )
      {
        Assert.AreEqual( Db.DbContext.DefaultDatabase, "Test" );
      }

      Assert.AreEqual( Db.DbContext.DefaultDatabase, Db.DefaultDatabaseName );

    }
    [TestMethod]
    public void DisposeContext()
    {

      Assert.AreEqual( Db.DbContext.DefaultDatabase, Db.DefaultDatabaseName );

      var scope = Db.Enter( builder => { builder.SetDefaultDatabase( "Test1" ); } );
      Assert.AreEqual( Db.DbContext.DefaultDatabase, "Test1" );
      Db.Enter( builder => { builder.SetDefaultDatabase( "Test2" ); } );
      Assert.AreEqual( Db.DbContext.DefaultDatabase, "Test2" );
      Db.Enter( builder => { builder.SetDefaultDatabase( "Test3" ); } );
      Assert.AreEqual( Db.DbContext.DefaultDatabase, "Test3" );

      scope.Dispose();

      Assert.AreEqual( Db.DbContext.DefaultDatabase, Db.DefaultDatabaseName );
    }
  }
}
