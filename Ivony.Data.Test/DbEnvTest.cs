using Ivony.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ivony.Data.Test
{
  [TestClass]
  public class DbEnvTest
  {

    public TestContext TestContext { get; set; }


    [TestInitialize]
    public void Initialize()
    {
    }


    [TestMethod]
    public void AsServiceProvider()
    {

      var env = DbEnv.CreateEnvironment( services => { } );

      Assert.AreEqual( env, env.Services.GetService<DbEnv>() );


    }
  }
}
