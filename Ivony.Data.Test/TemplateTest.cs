using Ivony.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ivony.Data.Test
{
  [TestClass]
  public class TemplateTest
  {

    public TestContext TestContext { get; set; }


    [TestInitialize]
    public void Initialize()
    {
    }


    [TestMethod]
    public void FormattableTemplate()
    {

      var userId = 0;
      var query = DbEnv.Default.T( $"SELECT * FROM Users WHERE UserID = {userId}" );

      var parser = new MySqlParameterizedQueryParser();
      var command = parser.Parse( query );

    }
  }
}
