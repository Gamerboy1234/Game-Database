
using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sql.Server.Connection.Test
{
    [TestClass]
    public class SqlServerConnectionTest
    {
        #region Private Members
        // connecting to server
        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        #endregion Private Members


        #region Test Cases

        [TestMethod]
        public void SqlServerConnectionConstructorAndConnectionTest()
        {
            var connection = new SqlServerConnection();

            Assert.AreEqual(connection.IsConnected, false);

            var result = connection.Connect(ConnectionString, 0);

            Assert.AreEqual(result, true);

            Assert.AreEqual(connection.IsConnected, true);

            connection = new SqlServerConnection(ConnectionString, 0);

            Assert.AreEqual(connection.IsConnected, true);
        }

        [TestMethod]
        public void SqlServerConnectionExecuteCommandAndExecuteQueryTest()
        {
            // Set Data
            var connection = new SqlServerConnection(ConnectionString, 0);

            Assert.AreEqual(connection.IsConnected, true);

            /*var errorMessage = "";
            var result = connection.ExecuteCommand($"INSERT INTO Game (Name, Description) VALUES ('Qbert', 'A funny little penguin')", ref errorMessage, out var gameId);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(gameId > 0);

            result = connection.ExecuteCommand($"INSERT INTO GameRating(GameId, RatingId, Notes) VALUES(1069, 1, 'Very cool game.')", ref errorMessage, out var gameRatingId);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(gameRatingId > 0);

            result = connection.ExecuteCommand($"UPDATE Game SET Description = 'A funny little ardvark' WHERE Id = {gameId}", ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            var dataSet = connection.ExecuteQuery($"SELECT Id, Name, Description FROM Game WHERE Id = {gameId}", ref errorMessage);

            Assert.IsNotNull(dataSet);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            result = connection.ExecuteCommand($"INSERT INTO Game (Name, Description) VALUES ('Halo Combat Evloved', 'Really awesome action game with one of the best storylines in gaming')", ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(gameId > 0);

            result = connection.ExecuteCommand($"INSERT INTO Game (Name, Description) VALUES ('Dragon Age Orgins', 'One of the Best RPGs ever made')", ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(gameId > 0);

            result = connection.ExecuteCommand($"INSERT INTO Game (Name, Description) VALUES ('Mass Effect', 'Ya bacally just star trek')", ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(gameId > 0);

            result = connection.ExecuteCommand($"UPDATE Game SET Description = 'Really good game' WHERE Name = 'Mass Effect'", ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(gameId > 0);

            // TODO: Check the data

            result = connection.ExecuteCommand($"INSERT INTO GameImage (GameId) VALUES ({gameId})", ref errorMessage, out var gameImageId);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(gameImageId > 0);

            var imageBytes = new byte[1000];

            for (var index = 0; index < 1000; index++)
            {
                imageBytes[index] = (byte)(index + 1);
            }

            result = connection.WriteBlobData($"UPDATE GameImage SET Image=@Image WHERE Id = {gameImageId}", "Image", "varbinary", null, imageBytes);

            Assert.IsTrue(result);

            var dataResult = connection.ReadBlobData($"SELECT Image FROM GameImage WHERE Id = {gameImageId}", 0);
            
            // Delete Commands
            Assert.IsNotNull(dataResult);

            result = connection.ExecuteCommand($"DELETE GameImage WHERE Id = {gameImageId}", ref errorMessage);

            Assert.IsTrue(result);

            result = connection.ExecuteCommand($"DELETE Game WHERE Id = {gameId}", ref errorMessage);

            Assert.IsTrue(result);

            result = connection.ExecuteCommand($"DELETE Game WHERE Name = 'Dragon Age Orgins'", ref errorMessage);

            Assert.IsTrue(result);

            result = connection.ExecuteCommand($"DELETE Game WHERE Name = 'Halo Combat Evolved'", ref errorMessage);

            Assert.IsTrue(result);

            result = connection.ExecuteCommand($"DELETE Game WHERE Name = 'Mass Effect'", ref errorMessage);

            Assert.IsTrue(result);
            
            result = connection.ExecuteCommand($"DELETE Game WHERE Name = 'Qbert'", ref errorMessage);

            Assert.IsTrue(result);

            result = connection.ExecuteCommand($"DELETE Game", ref errorMessage);

            Assert.IsTrue(result);*/
        }

        #endregion Test Cases
    }
}
