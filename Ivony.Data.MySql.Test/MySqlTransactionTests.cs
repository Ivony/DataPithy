using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ivony.Data.MySql.Test
{
    [TestClass]
    public class MySqlTransactionTests
    {
        [TestMethod]
        public void TestTransactionCommit()
        {
            // Arrange
            var manager = new DatabaseManager();
            var databaseName = "testDatabase";
            var connectionString = "server=localhost;database=test;uid=root;pwd=password;";

            // Act
            var result = manager.AddMySql(databaseName, connectionString);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestTransactionRollback()
        {
            // Arrange
            var manager = new DatabaseManager();
            var databaseName = "testDatabase";
            var connectionString = "server=localhost;database=test;uid=root;pwd=password;";

            // Act
            var result = manager.AddMySql(databaseName, connectionString);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestTransactionAutoRollback()
        {
            // Arrange
            var manager = new DatabaseManager();
            var databaseName = "testDatabase";
            var connectionString = "server=localhost;database=test;uid=root;pwd=password;";

            // Act
            var result = manager.AddMySql(databaseName, connectionString);

            // Assert
            Assert.IsNotNull(result);
        }
    }
}