using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ivony.Data.MySql.Test
{
    [TestClass]
    public class MySqlConnectionTests
    {
        [TestMethod]
        public void TestConnectionStringProperty()
        {
            // Arrange
            var manager = new DatabaseManager();
            var databaseName = "testDatabase";
            var expectedConnectionString = "server=localhost;database=test;uid=root;pwd=password;";

            // Act
            var result = manager.AddMySql(databaseName, expectedConnectionString);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(manager, result);
        }
    }
}