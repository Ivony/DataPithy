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

      Db.InitializeDb( builder => { } );

      Assert.AreEqual( Db.GetCurrentContext().DefaultDatabase, Db.DefaultDatabaseName );

      using ( Db.NewContext( builder => { builder.DefaultDatabase = "Test"; } ) )
      {
        Assert.AreEqual( Db.GetCurrentContext().DefaultDatabase, "Test" );
      }

      Assert.AreEqual( Db.GetCurrentContext().DefaultDatabase, Db.DefaultDatabaseName );

    }

  }
}
