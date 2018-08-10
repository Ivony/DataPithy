using System;
using System.Data;
using System.Linq;
using Ivony.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Data.Queries;
using Microsoft.Extensions.DependencyInjection;
using static Ivony.Data.Db;

namespace Ivony.Data.Test
{
  [TestClass]
  public class DbContextTest
  {




    [TestMethod]
    public void CurrentContext()
    {


      Assert.IsFalse( Db.Context.Properties.ContainsKey( "Test" ) );

      using ( Db.Enter( builder => { builder.SetProperty( "Test", "Test" ); } ) )
      {
        Assert.AreEqual( Db.Context.Properties["Test"], "Test" );
      }

      Assert.IsFalse( Db.Context.Properties.ContainsKey( "Test" ) );

    }
    [TestMethod]
    public void DisposeContext()
    {

      Assert.IsFalse( Db.Context.Properties.ContainsKey( "Test" ) );

      var scope = Db.Enter( builder => { builder.SetProperty( "Test", "Test1" ); } );
      Assert.AreEqual( Db.Context.Properties["Test"], "Test1" );
      Db.Enter( builder => { builder.SetProperty( "Test", "Test2" ); } );
      Assert.AreEqual( Db.Context.Properties["Test"], "Test2" );
      Db.Enter( builder => { builder.SetProperty( "Test", "Test3" ); } );
      Assert.AreEqual( Db.Context.Properties["Test"], "Test3" );

      scope.Dispose();

      Assert.IsFalse( Db.Context.Properties.ContainsKey( "Test" ) );
    }



    [TestMethod]
    public void TransactionContext()
    {
      using ( Enter( builder => builder.UseMySql( "" ).SetProperty( "Scope", "Global" ) ) )
      {
        using ( EnterTransaction() )
        {
          Enter( builder => builder.SetProperty( "Scope", "Transaction" ) );
          Assert.AreEqual( Db.Context.Properties["Scope"], "Transaction" );
        }

        Assert.AreEqual( Db.Context.Properties["Scope"], "Global" );
      }
    }

  }
}
