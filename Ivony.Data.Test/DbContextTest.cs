using System;
using System.Data;
using System.Linq;
using Ivony.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Data.Queries;
using static Ivony.Data.Db;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ivony.Data.Test
{
  [TestClass]
  public class DbContextTest
  {




    [TestMethod]
    public void CurrentContext()
    {


      Assert.IsFalse( Db.DbContext.Properties.ContainsKey( "Test" ) );

      using ( Db.Enter( builder => { builder.SetProperty( "Test", "Test" ); } ) )
      {
        Assert.AreEqual( Db.DbContext.Properties["Test"], "Test" );
      }

      Assert.IsFalse( Db.DbContext.Properties.ContainsKey( "Test" ) );

    }
    [TestMethod]
    public void DisposeContext()
    {

      Assert.IsFalse( Db.DbContext.Properties.ContainsKey( "Test" ) );

      var scope = Db.Enter( builder => { builder.SetProperty( "Test", "Test1" ); } );
      Assert.AreEqual( Db.DbContext.Properties["Test"], "Test1" );
      Db.Enter( builder => { builder.SetProperty( "Test", "Test2" ); } );
      Assert.AreEqual( Db.DbContext.Properties["Test"], "Test2" );
      Db.Enter( builder => { builder.SetProperty( "Test", "Test3" ); } );
      Assert.AreEqual( Db.DbContext.Properties["Test"], "Test3" );

      scope.Dispose();

      Assert.IsFalse( Db.DbContext.Properties.ContainsKey( "Test" ) );
    }



    [TestMethod]
    public void TransactionContext()
    {
      using ( Enter( builder => builder.UseMySql( "" ).SetProperty( "Scope", "Global" ) ) )
      {
        using ( EnterTransaction() )
        {
          Enter( builder => builder.SetProperty( "Scope", "Transaction" ) );
          Assert.AreEqual( Db.DbContext.Properties["Scope"], "Transaction" );
        }

        Assert.AreEqual( Db.DbContext.Properties["Scope"], "Global" );
      }
    }


    [TestMethod]
    public void TaskTest()
    {

      List<Task<object>> tasks = new List<Task<object>>();
      var random = new Random( DateTime.Now.Millisecond );

      using ( Enter( builder => builder.SetProperty( "Scope", "Global" ) ) )
      {

        tasks.Add( Task.Run( async () =>
        {
          await Task.Delay( TimeSpan.FromMilliseconds( random.Next( 1000 ) ) );
          return Db.DbContext.Properties["Scope"];
        } ) );

        using ( Enter( builder => builder.SetProperty( "Scope", "TaskTest" ) ) )
        {
          tasks.Add( Task.Run( async () =>
          {
            await Task.Delay( TimeSpan.FromMilliseconds( random.Next( 1000 ) ) );
            return Db.DbContext.Properties["Scope"];
          } ) );
        }

        tasks.Add( Task.Run( async () =>
        {
          await Task.Delay( TimeSpan.FromMilliseconds( random.Next( 1000 ) ) );
          return Db.DbContext.Properties["Scope"];
        } ) );
      }

      Task.WaitAll( tasks.ToArray() );

      Assert.AreEqual( tasks[0].Result, "Global" );
      Assert.AreEqual( tasks[1].Result, "TaskTest" );
      Assert.AreEqual( tasks[2].Result, "Global" );



    }


  }
}
