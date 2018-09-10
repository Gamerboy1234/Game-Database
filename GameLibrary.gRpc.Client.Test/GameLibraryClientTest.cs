
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


            #region GameTest
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

            #endregion GameTest

            #region GenreTest

            // Get all genres

            var genres = client.SearchGenres(0, "");

            Assert.IsNotNull(genres);
            Assert.AreEqual(genres.List.Count, 0);


            // Add a genre

            var genre = new Genre(0, "Qbert", "A silly penguin");

             errorMessage = "";
             result = client.AddGenre(genre, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(genre.Id > 0);


            // Get the new genre by id.

            genres = client.SearchGenres(genre.Id, "");

            Assert.IsNotNull(genres);
            Assert.AreEqual(genres.List.Count, 1);

            var foundGenre = genres.List[0];

            Assert.IsNotNull(foundGenre);
            Assert.AreEqual(foundGenre.Id, genre.Id);


            // Get the new genre by name.

            genres = client.SearchGenres(0, genre.Name);

            Assert.IsNotNull(genres);
            Assert.AreEqual(genres.List.Count, 1);

            foundGenre = genres.List[0];

            Assert.IsNotNull(foundGenre);
            Assert.AreEqual(foundGenre.Name, genre.Name);


            // Edit a genre

            var editGenre = new Genre(genre.Id, "Bilbo Baggins", "A silly hobbit who finds a ring");

            errorMessage = "";
            result = client.EditGenre(editGenre, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(editGenre.Id > 0);


            // Get the new genre by name.

            genres = client.SearchGenres(0, editGenre.Name);

            Assert.IsNotNull(genres);
            Assert.AreEqual(genres.List.Count, 1);

            foundGenre = genres.List[0];

            Assert.IsNotNull(foundGenre);
            Assert.AreEqual(foundGenre.Name, "Bilbo Baggins");
            Assert.AreEqual(foundGenre.Description, "A silly hobbit who finds a ring");


            // Get all genres

            genres = client.SearchGenres(0, "");

            Assert.IsNotNull(genres);
            Assert.AreEqual(genres.List.Count, 1);


            // Delete a genre

            errorMessage = "";
            result = client.DeleteGenre(foundGenre.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Get all genres

            genres = client.SearchGenres(0, "");

            Assert.IsNotNull(genres);
            Assert.AreEqual(genres.List.Count, 0);

            #endregion GenreTest

            #region RatingTest

            // Get all ratings

            var ratings = client.SearchRatings(0, "");

            Assert.IsNotNull(ratings);
            Assert.AreEqual(ratings.List.Count, 0);


            // Add a rating

            var rating = new Rating(0, "Qbert", "A silly penguin", "Symbol");

             errorMessage = "";
             result = client.AddRating(rating, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(rating.Id > 0);


            // Get the new rating by id.

            ratings = client.SearchRatings(rating.Id, "");

            Assert.IsNotNull(rating);
            Assert.AreEqual(ratings.List.Count, 1);

            var foundRating = ratings.List[0];

            Assert.IsNotNull(foundRating);
            Assert.AreEqual(foundRating.Id, rating.Id);


            // Get the new rating by name.

            ratings = client.SearchRatings(0, rating.Name);

            Assert.IsNotNull(ratings);
            Assert.AreEqual(ratings.List.Count, 1);

            foundRating = ratings.List[0];

            Assert.IsNotNull(foundRating);
            Assert.AreEqual(foundRating.Name, rating.Name);


            // Edit a rating

            var editRating = new Rating(rating.Id, "Bilbo Baggins", "A silly hobbit who finds a ring", "5");

            errorMessage = "";
            result = client.EditRating(editRating, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(editRating.Id > 0);


            // Get the new rating by name.

            ratings = client.SearchRatings(0, editRating.Name);

            Assert.IsNotNull(ratings);
            Assert.AreEqual(ratings.List.Count, 1);

            foundRating = ratings.List[0];

            Assert.IsNotNull(foundRating);
            Assert.AreEqual(foundRating.Name, "Bilbo Baggins");
            Assert.AreEqual(foundRating.Description, "A silly hobbit who finds a ring");


            // Get all ratings

            ratings = client.SearchRatings(0, "");

            Assert.IsNotNull(ratings);
            Assert.AreEqual(ratings.List.Count, 1);


            // Delete a rating

            errorMessage = "";
            result = client.DeleteRating(foundRating.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Get all ratings

            ratings = client.SearchRatings(0, "");

            Assert.IsNotNull(ratings);
            Assert.AreEqual(ratings.List.Count, 0);

            #endregion RatingTest


            // Shutdown the gRPC Web Services server.
            gameLibraryServer.ShutdownAsync().Wait();
        }

        #endregion Test Cases
    }
}
