
using System;
using GameLibrary.Core;
using GameLibrary.gRPC.Client;
using GameLibrary.gRPC.Server;
using GameLibrary.Model;
using Grpc.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameLibrary.gRpc.Client.Test
{
    [TestClass]
    public class GameLibraryClientTest
    {
        #region Constants

        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        private const int GrpcPort = 50052;
        private const string GrpcHost = "127.0.0.1";
        private const string GrpcHostName = "localhost";

        #endregion Constants


        #region Test Cases

        [TestMethod]
        public void GameLibrarygRpcClientTests()
        {
            // Startup the singleton for the ModelAssembler
            GameLibraryAgent.Startup(ConnectionString);

            // Startup the gRPC Web Services server.
            var gameLibraryServer = new Grpc.Core.Server
            {
                Services = { Gamelibrary.GameLibrary.BindService(new GameLibraryServer()) },
                Ports = { new ServerPort(GrpcHostName, GrpcPort, ServerCredentials.Insecure) }
            };

            gameLibraryServer.Start();

            Assert.IsTrue(true);


            // Create a gRPC Web Services client.

            var channel = new Channel($"{GrpcHost}:{GrpcPort}", ChannelCredentials.Insecure);
            var client = new GameLibraryClient(new Gamelibrary.GameLibrary.GameLibraryClient(channel));


            // Get all games

            var games = client.SearchGames(0, "");

            Assert.IsNotNull(games);
            Assert.AreEqual(games.List.Count, 0);


            // Add a game

            var game = new Game(0, "Qbert", "A silly penguin");

            var errorMessage = "";
            var result = client.AddGame(game, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(game.Id > 0);


            // Get the new game by id.

            games = client.SearchGames(game.Id, "");

            Assert.IsNotNull(games);
            Assert.AreEqual(games.List.Count, 1);

            var foundGame = games.List[0];

            Assert.IsNotNull(foundGame);
            Assert.AreEqual(foundGame.Id, game.Id);


            // Get the new game by name.

            games = client.SearchGames(0, game.Name);

            Assert.IsNotNull(games);
            Assert.AreEqual(games.List.Count, 1);

            foundGame = games.List[0];

            Assert.IsNotNull(foundGame);
            Assert.AreEqual(foundGame.Name, game.Name);


            // Edit a game

            var editGame = new Game(game.Id, "Bilbo Baggins", "A silly hobbit who finds a ring");

            errorMessage = "";
            result = client.EditGame(editGame, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(editGame.Id > 0);


            // Get the new game by name.

            games = client.SearchGames(0, editGame.Name);

            Assert.IsNotNull(games);
            Assert.AreEqual(games.List.Count, 1);

            foundGame = games.List[0];

            Assert.IsNotNull(foundGame);
            Assert.AreEqual(foundGame.Name, "Bilbo Baggins");
            Assert.AreEqual(foundGame.Description, "A silly hobbit who finds a ring");


            // Get all games

            games = client.SearchGames(0, "");

            Assert.IsNotNull(games);
            Assert.AreEqual(games.List.Count, 1);


            // Delete a game

            errorMessage = "";
            result = client.DeleteGame(foundGame.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Get all games

            games = client.SearchGames(0, "");

            Assert.IsNotNull(games);
            Assert.AreEqual(games.List.Count, 0);


            // Shutdown the gRPC Web Services server.
            gameLibraryServer.ShutdownAsync().Wait();
        }

        #endregion Test Cases
    }
}
