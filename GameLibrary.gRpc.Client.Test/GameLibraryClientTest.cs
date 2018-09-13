
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

            #region ReviewTest

            // Get all reviews

            var reviews = client.SearchReview(0, "");

            Assert.IsNotNull(reviews);
            Assert.AreEqual(reviews.List.Count, 0);


            // Add a review

            var review = new Review(0, "Qbert", "A silly penguin", 5);

            errorMessage = "";
            result = client.AddReview(review, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(review.Id > 0);


            // Get the new review by id.

            reviews = client.SearchReview(review.Id, "");

            Assert.IsNotNull(review);
            Assert.AreEqual(reviews.List.Count, 1);

            var foundReview = reviews.List[0];

            Assert.IsNotNull(foundReview);
            Assert.AreEqual(foundReview.Id, review.Id);


            // Get the new review by name.

            reviews = client.SearchReview(0, review.Name);

            Assert.IsNotNull(reviews);
            Assert.AreEqual(reviews.List.Count, 1);

            foundReview = reviews.List[0];

            Assert.IsNotNull(foundReview);
            Assert.AreEqual(foundReview.Name, review.Name);


            // Edit a review

            var editReview = new Review(review.Id, "Bilbo Baggins", "A silly hobbit who finds a ring", 6);

            errorMessage = "";
            result = client.EditReview(editReview, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(editReview.Id > 0);


            // Get the new review by name.

            reviews = client.SearchReview(0, editReview.Name);

            Assert.IsNotNull(reviews);
            Assert.AreEqual(reviews.List.Count, 1);

            foundReview = reviews.List[0];

            Assert.IsNotNull(foundReview);
            Assert.AreEqual(foundReview.Name, "Bilbo Baggins");
            Assert.AreEqual(foundReview.Description, "A silly hobbit who finds a ring");


            // Get all reviews

            reviews = client.SearchReview(0, "");

            Assert.IsNotNull(reviews);
            Assert.AreEqual(reviews.List.Count, 1);


            // Delete a review

            errorMessage = "";
            result = client.DeleteReview(foundReview.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Get all reviews

            reviews = client.SearchReview(0, "");

            Assert.IsNotNull(reviews);
            Assert.AreEqual(reviews.List.Count, 0);

            #endregion ReviewTest

            #region PlatformTest

            // Get all platforms

            var platforms = client.SearchPlatforms(0, "");

            Assert.IsNotNull(platforms);
            Assert.AreEqual(platforms.List.Count, 0);


            // Add a platform

            var platform = new Platform(0, "Qbert", "A silly penguin");

            errorMessage = "";
            result = client.AddPlatform(platform, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(platform.Id > 0);


            // Get the new platform by id.

            platforms = client.SearchPlatforms(platform.Id, "");

            Assert.IsNotNull(platform);
            Assert.AreEqual(platforms.List.Count, 1);

            var foundPlatform = platforms.List[0];

            Assert.IsNotNull(foundPlatform);
            Assert.AreEqual(foundPlatform.Id, platform.Id);


            // Get the new platfrom by name.

            platforms = client.SearchPlatforms(0, platform.Name);

            Assert.IsNotNull(platforms);
            Assert.AreEqual(platforms.List.Count, 1);

            foundPlatform = platforms.List[0];

            Assert.IsNotNull(foundPlatform);
            Assert.AreEqual(foundPlatform.Name, platform.Name);


            // Edit a platform

            var editPlatfrom = new Platform(platform.Id, "Bilbo Baggins", "A silly hobbit who finds a ring");

            errorMessage = "";
            result = client.EditPlatform(editPlatfrom, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(editPlatfrom.Id > 0);


            // Get the new platform by name.

            platforms = client.SearchPlatforms(0, editPlatfrom.Name);

            Assert.IsNotNull(platforms);
            Assert.AreEqual(platforms.List.Count, 1);

            foundPlatform = platforms.List[0];

            Assert.IsNotNull(foundPlatform);
            Assert.AreEqual(foundPlatform.Name, "Bilbo Baggins");
            Assert.AreEqual(foundPlatform.Maker, "A silly hobbit who finds a ring");


            // Get all platforms

            platforms = client.SearchPlatforms(0, "");

            Assert.IsNotNull(platforms);
            Assert.AreEqual(platforms.List.Count, 1);


            // Delete a platform

            errorMessage = "";
            result = client.DeletePlatform(foundPlatform.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Get all platforms

            platforms = client.SearchPlatforms(0, "");

            Assert.IsNotNull(platforms);
            Assert.AreEqual(platforms.List.Count, 0);

            #endregion PlatformTest

            #region GameGenreTest

            // Get all GameGenres

            var gameGenres = client.SearchGameGenres(0);

            Assert.IsNotNull(gameGenres);
            Assert.AreEqual(gameGenres.List.Count, 0);


            // Add a game

             game = new Game(0, "Qbert", "A silly penguin");

             errorMessage = "";
             result = client.AddGame(game, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(game.Id > 0);

            // Add a edited game 

             var editedgame = new Game(0, "Bob", "A silly duck");

            errorMessage = "";
            result = client.AddGame(editedgame, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(editedgame.Id > 0);

            // Add a genre

            genre = new Genre(0, "Qbert", "A silly penguin");

            errorMessage = "";
            result = client.AddGenre(genre, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(genre.Id > 0);

            // Add a edited genre

            var editedgenre = new Genre(0, "joe", "A silly hobo");

            errorMessage = "";
            result = client.AddGenre(editedgenre, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(editedgenre.Id > 0);

            // Add a GameGenre

            var gameGenre = new GameGenre(0, game.Id, genre.Id);

            errorMessage = "";
            result = client.AddGameGenre(gameGenre, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(gameGenre.Id > 0);


            // Get the GameGenre by id.

            
            gameGenres = client.SearchGameGenres(gameGenre.Id);

            Assert.IsNotNull(gameGenre);
            Assert.AreEqual(gameGenres.List.Count, 1);

            var foundgameGenre = gameGenres.List[0];

            Assert.IsNotNull(foundgameGenre);
            Assert.AreEqual(foundgameGenre.Id, gameGenre.Id);
            
            // Edit a GameGenre

            var editgamegenre = new GameGenre(gameGenre.Id, editedgame.Id, editedgenre.Id);

            errorMessage = "";
            result = client.EditGameGenre(editgamegenre, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(editPlatfrom.Id > 0);

            // Get all GameGenres

            gameGenres = client.SearchGameGenres(0);

            Assert.IsNotNull(gameGenres);
            Assert.AreEqual(gameGenres.List.Count, 1);


            // Delete a GameGenre

            errorMessage = "";
            result = client.DeleteGameGenre(foundgameGenre.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // Delete a game

            errorMessage = "";
            result = client.DeleteGame(game.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // Delete edited game

            errorMessage = "";
            result = client.DeleteGame(editedgame.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // Delete a genre

            errorMessage = "";
            result = client.DeleteGenre(genre.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // Delete edited genre

            errorMessage = "";
            result = client.DeleteGenre(editedgenre.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Get all Game Genres

            gameGenres = client.SearchGameGenres(0);

            Assert.IsNotNull(gameGenres);
            Assert.AreEqual(gameGenres.List.Count, 0);

            #endregion PlatformTest

            #region GamePlatformTest

            // Get all GamePlatform

            var gamePlatforms = client.SearchGamePlatforms(0);

            Assert.IsNotNull(gamePlatforms);
            Assert.AreEqual(gamePlatforms.List.Count, 0);


            // Add a game

            game = new Game(0, "Qbert", "A silly penguin");

            errorMessage = "";
            result = client.AddGame(game, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(game.Id > 0);

            // Add a edited game 

            editedgame = new Game(0, "Bob", "A silly duck");

            errorMessage = "";
            result = client.AddGame(editedgame, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(editedgame.Id > 0);

            // Add a platform

            platform = new Platform(0, "Qbert", "A silly penguin");

            errorMessage = "";
            result = client.AddPlatform(platform, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(platform.Id > 0);

            // Add a edited platform

            var editedplatform = new Platform(0, "joe", "A silly hobo");

            errorMessage = "";
            result = client.AddPlatform(editedplatform, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(editedplatform.Id > 0);

            // Add a GamePlattform

            var gamePlatform = new GamePlatform(0, game.Id, platform.Id);

            errorMessage = "";
            result = client.AddGamePlatform(gamePlatform, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(gamePlatform.Id > 0);


            // Get the GamePlatform by id.


            gamePlatforms = client.SearchGamePlatforms(gamePlatform.Id);

            Assert.IsNotNull(gamePlatform);
            Assert.AreEqual(gamePlatforms.List.Count, 1);

            var foundgamePlatform = gamePlatforms.List[0];

            Assert.IsNotNull(foundgamePlatform);
            Assert.AreEqual(foundgamePlatform.Id, gamePlatform.Id);

            // Edit a GamePlatform

            var editgameplatform = new GamePlatform(gamePlatform.Id, editedgame.Id, editedplatform.Id);

            errorMessage = "";
            result = client.EditGamePlatform(editgameplatform, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(editPlatfrom.Id > 0);

            // Get all GamePlatform

            gamePlatforms = client.SearchGamePlatforms(0);

            Assert.IsNotNull(gamePlatforms);
            Assert.AreEqual(gamePlatforms.List.Count, 1);


            // Delete a GamePlatform

            errorMessage = "";
            result = client.DeleteGamePlatform(foundgamePlatform.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // Delete a game

            errorMessage = "";
            result = client.DeleteGame(game.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // Delete edited game

            errorMessage = "";
            result = client.DeleteGame(editedgame.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // Delete a platform

            errorMessage = "";
            result = client.DeletePlatform(platform.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // Delete edited platform

            errorMessage = "";
            result = client.DeleteGenre(editedplatform.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Get all Game Platforms

            gamePlatforms = client.SearchGamePlatforms(0);

            Assert.IsNotNull(gamePlatforms);
            Assert.AreEqual(gamePlatforms.List.Count, 0);

            #endregion PlatformTest



            // Shutdown the gRPC Web Services server.
            gameLibraryServer.ShutdownAsync().Wait();
        }

        #endregion Test Cases
    }
}
