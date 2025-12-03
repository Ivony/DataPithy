using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ivony.Data.MySql.Test
{
    [TestClass]
    public class MySqlDatabaseManagerExtensionsTests
    {
        [TestMethod]
        public void TestAddMySqlWithConfiguration()
        {
            // Arrange
            var manager = new DatabaseManager();
            var databaseName = "testDatabase";

            // Act
            var result = manager.AddMySql(databaseName, builder =>
            {
                builder.WithServer("localhost")
                       .WithDatabase("test")
                       .WithCredentials("root", "password");
            });

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(manager, result);
        }

        [TestMethod]
        public void TestAddMySqlWithConnectionString()
        {
            // Arrange
            var manager = new DatabaseManager();
            var databaseName = "testDatabase";
            var connectionString = "server=localhost;database=test;uid=root;pwd=password;";

            // Act
            var result = manager.AddMySql(databaseName, connectionString);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(manager, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddMySqlWithNullManager()
        {
            // Arrange
            DatabaseManager manager = null;
            var databaseName = "testDatabase";
            var connectionString = "server=localhost;database=test;uid=root;pwd=password;";

            // Act
            manager.AddMySql(databaseName, connectionString);

            // Assert - ExpectedException should catch the exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddMySqlWithEmptyName()
        {
            // Arrange
            var manager = new DatabaseManager();
            var emptyName = "";
            var connectionString = "server=localhost;database=test;uid=root;pwd=password;";

            // Act
            manager.AddMySql(emptyName, connectionString);

            // Assert - ExpectedException should catch the exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddMySqlWithNullName()
        {
            // Arrange
            var manager = new DatabaseManager();
            string nullName = null;
            var connectionString = "server=localhost;database=test;uid=root;pwd=password;";

            // Act
            manager.AddMySql(nullName, connectionString);

            // Assert - ExpectedException should catch the exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddMySqlWithEmptyConnectionString()
        {
            // Arrange
            var manager = new DatabaseManager();
            var databaseName = "testDatabase";
            var emptyConnectionString = "";

            // Act
            manager.AddMySql(databaseName, emptyConnectionString);

            // Assert - ExpectedException should catch the exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddMySqlWithNullConnectionString()
        {
            // Arrange
            var manager = new DatabaseManager();
            var databaseName = "testDatabase";
            string nullConnectionString = null;

            // Act
            manager.AddMySql(databaseName, nullConnectionString);

            // Assert - ExpectedException should catch the exception
        }

        [TestMethod]
        public void TestAddMySqlReturnsManager()
        {
            // Arrange
            var manager = new DatabaseManager();
            var databaseName = "testDatabase";
            var connectionString = "server=localhost;database=test;uid=root;pwd=password;";

            // Act
            var result = manager.AddMySql(databaseName, connectionString);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(manager, result, "AddMySql should return the same DatabaseManager instance");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddMySqlWithNullConfigure()
        {
            // Arrange
            var manager = new DatabaseManager();
            var databaseName = "testDatabase";
            Action<MySqlDbBuilder> nullConfigure = null;

            // Act
            manager.AddMySql(databaseName, nullConfigure);

            // Assert - ExpectedException should catch the exception
        }
    }
}