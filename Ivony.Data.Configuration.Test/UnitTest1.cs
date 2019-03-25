using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Ivony.Data.Configuration.Test
{
  [TestClass]
  public class ServiceTest
  {
    public static IServiceProvider ServiceProvider { get; private set; }

    [AssemblyInitialize]
    public static void Initialize( TestContext context )
    {

      var builder = new ConfigurationBuilder();
      builder.AddInMemoryCollection( new Dictionary<string, string>() { ["ConnectionStrings:Default"] = "abc" } );


      var services = new ServiceCollection();

      services.AddSingleton( (IConfiguration) builder.Build() );

      services.AddConnectionStrings();
      services.AddSingleton<IDbProvider, MySqlDbProvider>();



      ServiceProvider = services.BuildServiceProvider();

    }

    [TestMethod]
    public void TestMethod1()
    {
      ServiceProvider.GetService<IDbProvider>();

    }
  }
}
