using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ivony.Data.Configuration.Test
{
  [TestClass]
  public class UnitTest1
  {

    [AssemblyInitialize]
    public void Initialize()
    {
      var services = new ServiceCollection();

      var provider = services.BuildServiceProvider();

    }

    [TestMethod]
    public void TestMethod1()
    {


    }
  }
}
