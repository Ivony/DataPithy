using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;

namespace Ivony.Data.MySql.Test
{
    [TestClass]
    public class MySqlDbBuilderTests
    {
        [TestMethod]
        public void TestBuildWithConnectionString()
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
        public void TestBuildWithBuilderConfiguration()
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBuildWithEmptyConnectionString()
        {
            // Arrange
            var manager = new DatabaseManager();
            var databaseName = "testDatabase";

            // Act
            manager.AddMySql(databaseName, builder =>
            {
                // Don't configure any connection
            });

            // Assert - ExpectedException should catch the exception when database is used
        }

        [TestMethod]
        public void TestChainedConfiguration()
        {
            // Arrange
            var manager = new DatabaseManager();
            var databaseName = "testDatabase";

            // Act
            var result = manager.AddMySql(databaseName, builder =>
            {
                builder.WithServer("localhost")
                       .WithDatabase("test")
                       .WithCredentials("root", "password")
                       .WithPooling(true);
            });

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestConfigureServices()
        {
            // Arrange
            var manager = new DatabaseManager();
            var databaseName = "testDatabase";

            // Act
            var result = manager.AddMySql(databaseName, builder =>
            {
                builder.WithServer("localhost")
                       .WithDatabase("test")
                       .WithCredentials("root", "password")
                       .ConfigureServices(s => s.AddSingleton<TestService>());
            });

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestBuildReturnsValidDatabase()
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
        }

        private class TestService
        {
        }
    }
}