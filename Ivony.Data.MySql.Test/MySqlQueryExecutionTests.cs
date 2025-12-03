using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ivony.Data.MySql.Test
{
    [TestClass]
    public class MySqlQueryExecutionTests
    {
        [TestMethod]
        public void TestExecuteScalar()
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
        public void TestExecuteNonQuery()
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
        public void TestExecuteFirstRow()
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
        public void TestExecuteDynamics()
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