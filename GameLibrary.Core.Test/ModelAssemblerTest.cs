using GameImageLibrary.Model;
using GameLibrary.Core;
using GameLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenreLibrary.Core.Test
{
    [TestClass]
    public class ModelAssemblerTest
    {
        #region Private Members

        // connecting to server
        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        #endregion Private Members

        #region Game Test Cases

        [TestMethod]
        public void ModelAssemblerConstructorTestGame()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);
        }

        [TestMethod]
        public void ModelAssemblerGameTest()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);

            // Get All Games

            var games = modelAssembler.GetGames();

            if (games.List.Count > 0)
            {
                Assert.IsTrue(games.List.Count > 0);
            }

            // Add a Game

            var game = new Game(
                0,
                "Defender",
                "Awesome Space Game");

            var errorMessage = "";
            var result = modelAssembler.AddOrEditGame(game, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(game.Id > 0);

            // Get Game by Id

            var foundGame = modelAssembler.GetGameById(game.Id, ref errorMessage);

            Assert.IsNotNull(foundGame);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(game.Id, foundGame.Id);

            // Get Game by Name

            foundGame = modelAssembler.GetGameByName(game.Name, ref errorMessage);

            Assert.IsNotNull(foundGame);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(game.Name, foundGame.Name);

            // Fail on game add attempt because the game already exists.

            var failGame = new Game(
                0,
                "Defender",
                "Awesome Space Game");

            errorMessage = "";
            result = modelAssembler.AddOrEditGame(failGame, ref errorMessage);

            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(errorMessage, "A game named 'Defender' already exists.  Unable to add game.");

            errorMessage = "";

            // Fail to get game by id

            foundGame = modelAssembler.GetGameById(792, ref errorMessage);

            Assert.IsNull(foundGame);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Fail to get game by name

            foundGame = modelAssembler.GetGameByName("Bubba and Bubba''s amazing adventure", ref errorMessage);

            Assert.IsNull(foundGame);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Edit a Game

            var updateGame = new Game(
                game.Id,
                "QBert",
                "Awesome game about a penguin");

            errorMessage = "";
            result = modelAssembler.AddOrEditGame(updateGame, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(game.Id, updateGame.Id);

            // Get Game by Id

            foundGame = modelAssembler.GetGameById(updateGame.Id, ref errorMessage);

            Assert.IsNotNull(foundGame);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(game.Id, foundGame.Id);

            // Get Game by Name

            foundGame = modelAssembler.GetGameByName(updateGame.Name, ref errorMessage);

            Assert.IsNotNull(foundGame);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(updateGame.Name, foundGame.Name);

            // Delete a game

            errorMessage = "";
            result = modelAssembler.DeleteGame(updateGame.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // Get Game by Id should fail

            foundGame = modelAssembler.GetGameById(updateGame.Id, ref errorMessage);

            Assert.IsNull(foundGame);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // Get Game by Name should fail

            foundGame = modelAssembler.GetGameByName(updateGame.Name, ref errorMessage);

            Assert.IsNull(foundGame);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        #endregion Game Test Cases

        #region Genre Test Cases

        [TestMethod]
        public void ModelAssemblerConstructorTestGenre()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);
        }

        [TestMethod]
        public void ModelAssemblerGenreTest()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);

            // Get All Genre

            var genres = modelAssembler.GetGenres();

            if (genres.List.Count > 0)
            {
                Assert.IsTrue(genres.List.Count > 0);
            }

            // Add a Genre

            var genre = new Genre(0,"Bob","not really a genre");
            Assert.IsNotNull(genre);
            var errorMessage = "";
            var result = modelAssembler.AddOrEditGenre(genre, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(genre.Id > 0);

            // Get Genre by Id 

            var foundGenre = modelAssembler.GetGenreById(genre.Id, ref errorMessage);

            Assert.IsNotNull(foundGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(genre.Id, foundGenre.Id);

            // Get Genre by Name

            foundGenre = modelAssembler.GetGenreByName(genre.Name, ref errorMessage);
          
            Assert.IsNotNull(foundGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(genre.Name, foundGenre.Name);

            // Fail on genre add attempt because the genre already exists.
            
            var failedGenre = new Genre(0, "Bob", "not really a genre");
       
            errorMessage = "";
            result = modelAssembler.AddOrEditGenre(failedGenre, ref errorMessage);

            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(errorMessage, "A genre named 'Bob' already exists.  Unable to add genre.");

            errorMessage = "";

            // Fail to get genre by id

            foundGenre = modelAssembler.GetGenreById(792, ref errorMessage);

            Assert.IsNull(foundGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // Fail to get genre by name

            foundGenre = modelAssembler.GetGenreByName("Doesn''t Exist", ref errorMessage);

            Assert.IsNull(foundGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // Edit a Genre

            var updateGenre = new Genre(
                genre.Id,
                "Joe",
                "Joe is a Scrub");

            errorMessage = "";
            result = modelAssembler.AddOrEditGenre(updateGenre, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(genre.Id, updateGenre.Id);

            // Get Genre by Id

            foundGenre = modelAssembler.GetGenreById(updateGenre.Id, ref errorMessage);

            Assert.IsNotNull(foundGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(genre.Id, foundGenre.Id);

            // Get Genre by Name

            foundGenre = modelAssembler.GetGenreByName(updateGenre.Name, ref errorMessage);

            Assert.IsNotNull(foundGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(updateGenre.Name, foundGenre.Name);

            // Delete a genre

            errorMessage = "";
            result = modelAssembler.DeleteGenre(updateGenre.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // Get Genre by Id should fail

            foundGenre = modelAssembler.GetGenreById(updateGenre.Id, ref errorMessage);

            Assert.IsNull(foundGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // Get Genre by Name should fail

            foundGenre = modelAssembler.GetGenreByName(updateGenre.Name, ref errorMessage);

            Assert.IsNull(foundGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }


        #endregion Genre Test Cases

        #region Ratings Test Cases

        [TestMethod]
        public void ModelAssemblerConstructorTestReview()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);
        }

        [TestMethod]
        public void ModelAssemblerRatingTest()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);

            // get all ratings 

            var ratings = modelAssembler.GetRatings();

            if (ratings.List.Count > 0)
            {
                Assert.IsTrue(ratings.List.Count > 0);
            }

            // add a rating 

            var rating = new Rating(0, "Bobthegreat","BubbaGames", "0");

            Assert.IsNotNull(rating);

            var errorMessage = "";

            var result = modelAssembler.AddOrEditRating(rating, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(rating.Id > 0);

            // get a rating by id

            var foundRating = modelAssembler.GetRatingById(rating.Id, ref errorMessage);

            Assert.IsNotNull(foundRating);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(rating.Id, foundRating.Id);
            
            // get rating by name 

            foundRating = modelAssembler.GetRatingByName(rating.Name, ref errorMessage);

            Assert.IsNotNull(foundRating);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(rating.Name, foundRating.Name);

            // Fail on rating add attempt because the rating already exists.

            var failedRating = new Rating(0, "Bobthegreat", "BubbaGames", "0");

            errorMessage = "";
            result = modelAssembler.AddOrEditRating(failedRating, ref errorMessage);

            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(errorMessage, "A rating named 'Bobthegreat' already exists.  Unable to add rating.");

            errorMessage = "";

            // failed to get rating by id

            foundRating = modelAssembler.GetRatingById(500, ref errorMessage);

            Assert.IsNull(foundRating);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // failed to get rating by name 

            foundRating = modelAssembler.GetRatingByName("Bubba and Bubba''s amazing adventure", ref errorMessage);

            Assert.IsNull(foundRating);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // edit rating 

            var updateRating = new Rating(rating.Id,"2","Slightly better","5");

            errorMessage = "";

            result = modelAssembler.AddOrEditRating(updateRating, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(rating.Id, updateRating.Id);

            // get rating by id 

            foundRating = modelAssembler.GetRatingById(updateRating.Id, ref errorMessage);

            Assert.IsNotNull(foundRating);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(updateRating.Id, foundRating.Id);

            // get rating by name 

            foundRating = modelAssembler.GetRatingByName(updateRating.Name, ref errorMessage);

            Assert.IsNotNull(foundRating);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(updateRating.Name, foundRating.Name);

            // delete a rating 

            result = modelAssembler.DeleteRating(updateRating.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // find rating by name should fail

            foundRating = modelAssembler.GetRatingByName(updateRating.Name, ref errorMessage);

            Assert.IsNull(foundRating);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

        }

        #endregion Ratings Test Cases

        #region Review Test Cases

        [TestMethod]
        public void ModelAssemblerConstructorTestReviews()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);
        }

        [TestMethod]
        public void ModelAssemblerReviewTest()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);

            // get all Reviews 

            var platform = modelAssembler.GetReviews();

            if (platform.List.Count > 0)
            {
                Assert.IsTrue(platform.List.Count > 0);
            }

            // add a review 

            var Review = new Review(0, "Name", "Description", 5);

            Assert.IsNotNull(Review);

            var errorMessage = "";

            var result = modelAssembler.AddOrEditReview(Review, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(Review.Id > 0);

            // find review by id

            var foundReview = modelAssembler.GetReviewById(Review.Id, ref errorMessage);

            Assert.IsNotNull(foundReview);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(Review.Id, foundReview.Id);

            // find platform by name 

            foundReview = modelAssembler.GetReviewByName(Review.Name, ref errorMessage);

            Assert.IsNotNull(foundReview);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(Review.Name, foundReview.Name);

            // failed to add review

            var failedplatform = new Review(0, "Name", "Description", 5);

            errorMessage = "";

            result = modelAssembler.AddOrEditReview(failedplatform, ref errorMessage);

            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(errorMessage, "A review named 'Name' already exists.  Unable to add review.");

            errorMessage = "";

            // failed to get review by Id

            foundReview = modelAssembler.GetReviewById(789, ref errorMessage);

            Assert.IsNull(foundReview);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // failed to get by name 

            foundReview = modelAssembler.GetReviewByName("Bleh", ref errorMessage);

            Assert.IsNull(foundReview);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // update review 

            var updateReview = new Review(Review.Id, "Name2", "Description2", 8);

            errorMessage = "";

            result = modelAssembler.AddOrEditReview(updateReview, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(Review.Id, updateReview.Id);

            // get updated reivew by id 

            foundReview = modelAssembler.GetReviewById(updateReview.Id, ref errorMessage);

            Assert.IsNotNull(foundReview);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(updateReview.Id, foundReview.Id);

            // get updated reivew by name

            foundReview = modelAssembler.GetReviewByName(updateReview.Name, ref errorMessage);

            Assert.IsNotNull(foundReview);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(updateReview.Id, foundReview.Id);

            // delete review 

            result = modelAssembler.DeleteReview(updateReview.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // find review name should fail 

            foundReview = modelAssembler.GetReviewByName(updateReview.Name, ref errorMessage);

            Assert.IsNull(foundReview);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        #endregion Review Test Cases

        #region Platform Test Cases

        [TestMethod]
        public void ModelAssemblerConstructorTestPlatform()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);
        }

        [TestMethod]
        public void ModelAssemblerPlatformTest()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);

            // get all Platforms 

            var platforms = modelAssembler.GetPlatforms();

            if (platforms.List.Count > 0)
            {
                Assert.IsTrue(platforms.List.Count > 0);
            }

            // add a platform

            var platform = new Platform(0, "Playstation", "Sony");

            Assert.IsNotNull(platform);

            var errorMessage = "";

            var result = modelAssembler.AddOrEditPlatform(platform, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(platform.Id > 0);

            // find platform id 

            var foundplatform = modelAssembler.GetPlatformById(platform.Id, ref errorMessage);

            Assert.IsNotNull(foundplatform);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(platform.Id, foundplatform.Id);

            // find platform by name

            foundplatform = modelAssembler.GetPlatfomrByName(platform.Name, ref errorMessage);

            Assert.IsNotNull(foundplatform);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(platform.Name, foundplatform.Name);

            // failed to add platform 

            var failedplatform = new Platform(0, "Playstation", "Sony");

            errorMessage = "";

            result = modelAssembler.AddOrEditPlatform(failedplatform, ref errorMessage);

            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(errorMessage, "A platform named 'Playstation' already exists.  Unable to add platform.");

            errorMessage = "";

            // failed to get id 

            foundplatform = modelAssembler.GetPlatformById(500, ref errorMessage);

            Assert.IsNull(foundplatform);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // failed to get by name

            foundplatform = modelAssembler.GetPlatfomrByName("bob", ref errorMessage);

            Assert.IsNull(foundplatform);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // edit platform 

            var updatedplatform = new Platform(platform.Id, "Xbox", "Microsoft");

            errorMessage = "";

            result = modelAssembler.AddOrEditPlatform(updatedplatform, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(platform.Id, updatedplatform.Id);

            // get updated platform by id 

            foundplatform = modelAssembler.GetPlatformById(updatedplatform.Id, ref errorMessage);

            Assert.IsNotNull(foundplatform);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(updatedplatform.Id, foundplatform.Id);

            // get updated platform by name

            foundplatform = modelAssembler.GetPlatfomrByName(updatedplatform.Name, ref errorMessage);

            Assert.IsNotNull(foundplatform);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(updatedplatform.Name, foundplatform.Name);

            // delete platform 

            result = modelAssembler.DeletePlatform(updatedplatform.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // find platform by name should fail 

            foundplatform = modelAssembler.GetPlatfomrByName(updatedplatform.Name, ref errorMessage);

            Assert.IsNull(foundplatform);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

        }

        #endregion Platform Test Cases

        #region GameGenre Test Cases

        [TestMethod]
        public void ModelAssemblerConstructorTestGameGenre()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);
        }

        [TestMethod]
        public void ModelAssemblerGameGenreTest()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);

            
            // Add Games used for references in GameGenres

            var game = new Game(
                0,
                "Defender",
                "Awesome Space Game");

            var errorMessage = "";
            var result = modelAssembler.AddOrEditGame(game, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(game.Id > 0);

            var updateGame = new Game(
                0,
                "Name1",
                "Description1");

            errorMessage = "";
            result = modelAssembler.AddOrEditGame(updateGame, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(updateGame.Id > 0);


            // Add Genres used for references in GameGenres

            var genre = new Genre(0, "Bob", "not really a genre");
            Assert.IsNotNull(genre);
            errorMessage = "";
            result = modelAssembler.AddOrEditGenre(genre, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(genre.Id > 0);

            var updateGenre = new Genre(0, "Billy", "imaginary friend genre");
            Assert.IsNotNull(updateGenre);
            errorMessage = "";
            result = modelAssembler.AddOrEditGenre(updateGenre, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(updateGenre.Id > 0);

            
            // get all GameGenres 

            var gameGenres = modelAssembler.GetGameGenres();

            if (gameGenres.List.Count > 0)
            {
                Assert.IsTrue(gameGenres.List.Count > 0);
            }

            // add a gameGenre

            var gameGenre = new GameGenre(
                0,
                game.Id,
                genre.Id);

            Assert.IsNotNull(gameGenre);

            errorMessage = "";

            result = modelAssembler.AddOrEditGameGenre(gameGenre, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(gameGenre.Id > 0);

            // find gameGenre id 

            var foundgameGenre = modelAssembler.GetGameGenreById(gameGenre.Id, ref errorMessage);

            Assert.IsNotNull(foundgameGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gameGenre.Id, foundgameGenre.Id);

            // find gameGenre by ids

            foundgameGenre = modelAssembler.GetGameGenreByIds(gameGenre.GameId, gameGenre.GenreId, ref errorMessage);

            Assert.IsNotNull(foundgameGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gameGenre.Id, foundgameGenre.Id);

            // find gameGenre by gameId

            var foundgameGenres = modelAssembler.GetGenresOfGame(gameGenre.GameId);

            Assert.IsNotNull(foundgameGenre);
            Assert.IsTrue(string.IsNullOrEmpty(foundgameGenres.ErrorMessage));
            Assert.IsTrue(foundgameGenres.List.Count == 1);

            // find gameGenre by genreId

            foundgameGenres = modelAssembler.GetGamesOfGenre(gameGenre.GenreId);

            Assert.IsNotNull(foundgameGenre);
            Assert.IsTrue(string.IsNullOrEmpty(foundgameGenres.ErrorMessage));
            Assert.IsTrue(foundgameGenres.List.Count == 1);

            // failed to add gameGenre 

            foundgameGenre.Id = 0;

            errorMessage = "";

            result = modelAssembler.AddOrEditGameGenre(foundgameGenre, ref errorMessage);

            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));

            errorMessage = "";

            // failed to get id 

            foundgameGenre = modelAssembler.GetGameGenreById(500, ref errorMessage);

            Assert.IsNull(foundgameGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // edit gameGenre 

            var updatedgameGenre = new GameGenre(gameGenre.Id, updateGame.Id, updateGenre.Id);

            errorMessage = "";

            result = modelAssembler.AddOrEditGameGenre(updatedgameGenre, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gameGenre.Id, updatedgameGenre.Id);

            // get updated gameGenre by id 

            foundgameGenre = modelAssembler.GetGameGenreById(updatedgameGenre.Id, ref errorMessage);

            Assert.IsNotNull(foundgameGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(updatedgameGenre.Id, foundgameGenre.Id);

            // get updated gameGenre by name

            foundgameGenre = modelAssembler.GetGameGenreByIds(updatedgameGenre.GameId, updatedgameGenre.GenreId, ref errorMessage);

            Assert.IsNotNull(foundgameGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gameGenre.Id, foundgameGenre.Id);

            // delete gameGenre 

            result = modelAssembler.DeleteGameGenre(updatedgameGenre.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // find gameGenre by ids should fail 

            foundgameGenre = modelAssembler.GetGameGenreByIds(updatedgameGenre.GameId, updatedgameGenre.GenreId, ref errorMessage);

            Assert.IsNull(foundgameGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Delete the genres

            errorMessage = "";
            result = modelAssembler.DeleteGenre(genre.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            errorMessage = "";
            result = modelAssembler.DeleteGenre(updateGenre.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Delete the games

            errorMessage = "";
            result = modelAssembler.DeleteGame(game.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            errorMessage = "";
            result = modelAssembler.DeleteGame(updateGame.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        #endregion GameGenre Test Cases

        #region GameRating Test Cases

        [TestMethod]
        public void ModelAssemblerConstructorTestGameRating()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);
        }

        [TestMethod]
        public void ModelAssemblerGameRatingTest()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);


            // Add Games used for references in GameRatings

            var game = new Game(
                0,
                "Defender1",
                "Awesome Space Game1");

            var errorMessage = "";
            var result = modelAssembler.AddOrEditGame(game, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(game.Id > 0);

            var updateGame = new Game(
                0,
                "Name1",
                "Description1");

            errorMessage = "";
            result = modelAssembler.AddOrEditGame(updateGame, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(updateGame.Id > 0);


            // Add Ratings used for references in GameRatings

            var rating = new Rating(0, "Bob", "not really a rating", "5");
            Assert.IsNotNull(rating);
            errorMessage = "";
            result = modelAssembler.AddOrEditRating(rating, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(rating.Id > 0);

            var updateRating = new Rating(0, "Billy1", "imaginary friend rating1", "8");
            Assert.IsNotNull(updateRating);
            errorMessage = "";
            result = modelAssembler.AddOrEditRating(updateRating, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(updateRating.Id > 0);


            // get all GameRatings 

            var gameRatings = modelAssembler.GetGameRatings();

            if (gameRatings.List.Count > 0)
            {
                Assert.IsTrue(gameRatings.List.Count > 0);
            }

            // add a gameRating

            var gameRating = new GameRating(
                0,
                game.Id,
                rating.Id, "Bleh");

            Assert.IsNotNull(gameRating);

            errorMessage = "";

            result = modelAssembler.AddOrEditGameRating(gameRating, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(gameRating.Id > 0);

            // find gameRating id 

            var foundgameRating = modelAssembler.GetGameRatingById(gameRating.Id, ref errorMessage);

            Assert.IsNotNull(foundgameRating);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gameRating.Id, foundgameRating.Id);

            // find gameRating by ids

            foundgameRating = modelAssembler.GetGameRatingsByIds(gameRating.GameId, gameRating.RatingId, ref errorMessage);

            Assert.IsNotNull(foundgameRating);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gameRating.Id, foundgameRating.Id);

            // find gameRating by ratingId

            var foundgameRatings = modelAssembler.GetRatingsOfGame(gameRating.RatingId);

            Assert.IsNotNull(foundgameRating);
            Assert.IsTrue(string.IsNullOrEmpty(foundgameRatings.ErrorMessage));
            Assert.IsTrue(foundgameRatings.List.Count == 1);

            // find gameRating by gameId

            foundgameRatings = modelAssembler.GetGamesOfRating(gameRating.GameId);

            Assert.IsNotNull(foundgameRating);
            Assert.IsTrue(string.IsNullOrEmpty(foundgameRatings.ErrorMessage));
            Assert.IsTrue(foundgameRatings.List.Count == 1);

            // failed to add gameRating 

            foundgameRating.Id = 0;

            errorMessage = "";

            result = modelAssembler.AddOrEditGameRating(foundgameRating, ref errorMessage);

            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));

            errorMessage = "";

            // failed to get id 

            foundgameRating = modelAssembler.GetGameRatingById(500, ref errorMessage);

            Assert.IsNull(foundgameRating);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // edit gameRating 

            var updatedgameRating = new GameRating(gameRating.Id, updateGame.Id, updateRating.Id, "Bob");

            errorMessage = "";

            result = modelAssembler.AddOrEditGameRating(updatedgameRating, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gameRating.Id, updatedgameRating.Id);

            // get updated gameRating by id 

            foundgameRating = modelAssembler.GetGameRatingById(updatedgameRating.Id, ref errorMessage);

            Assert.IsNotNull(foundgameRating);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(updatedgameRating.Id, foundgameRating.Id);

            // get updated gameRating by name

            foundgameRating = modelAssembler.GetGameRatingsByIds(updatedgameRating.GameId, updatedgameRating.RatingId, ref errorMessage);

            Assert.IsNotNull(foundgameRating);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gameRating.Id, foundgameRating.Id);

            // delete gameRating 

            result = modelAssembler.DeleteGameRating(updatedgameRating.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // find gameRating by ids should fail 

            foundgameRating = modelAssembler.GetGameRatingsByIds(updatedgameRating.GameId, updatedgameRating.RatingId, ref errorMessage);

            Assert.IsNull(foundgameRating);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Delete the ratings

            errorMessage = "";
            result = modelAssembler.DeleteRating(rating.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            errorMessage = "";
            result = modelAssembler.DeleteRating(updateRating.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Delete the games

            errorMessage = "";
            result = modelAssembler.DeleteGame(game.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            errorMessage = "";
            result = modelAssembler.DeleteGame(updateGame.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        #endregion GameRating Test Cases

        #region GameReview Test Cases

        [TestMethod]
        public void ModelAssemblerConstructorTestGameReview()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);
        }

        [TestMethod]
        public void ModelAssemblerGameReviewTest()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);


            // Add Games used for references in GameReviews

            var game = new Game(
                0,
                "Defender2",
                "Awesome Space Game2");

            var errorMessage = "";
            var result = modelAssembler.AddOrEditGame(game, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(game.Id > 0);

            var updateGame = new Game(
                0,
                "Name2",
                "Description2");

            errorMessage = "";
            result = modelAssembler.AddOrEditGame(updateGame, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(updateGame.Id > 0);


            // Add Reviews used for references in GameReviews

            var review = new Review(0, "Bob", "not really a review", 5);
            Assert.IsNotNull(review);
            errorMessage = "";
            result = modelAssembler.AddOrEditReview(review, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(review.Id > 0);

            var updateReview = new Review(0, "Billy2", "imaginary friend review2", 5);
            Assert.IsNotNull(updateReview);
            errorMessage = "";
            result = modelAssembler.AddOrEditReview(updateReview, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(updateReview.Id > 0);


            // get all GameReviews 

            var gameReviews = modelAssembler.GetGameReviews();

            if (gameReviews.List.Count > 0)
            {
                Assert.IsTrue(gameReviews.List.Count > 0);
            }

            // add a gameReview

            var gameReview = new GameReview(
                0,
                game.Id,
                review.Id);

            Assert.IsNotNull(gameReview);

            errorMessage = "";

            result = modelAssembler.AddOrEditGameReview(gameReview, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(gameReview.Id > 0);

            // find gameReview id 

            var foundgameReview = modelAssembler.GetGameReviewById(gameReview.Id, ref errorMessage);

            Assert.IsNotNull(foundgameReview);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gameReview.Id, foundgameReview.Id);

            // find gameReview by ids

            foundgameReview = modelAssembler.GetGameReviewsByIds(gameReview.GameId, gameReview.ReviewId, ref errorMessage);

            Assert.IsNotNull(foundgameReview);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gameReview.Id, foundgameReview.Id);

            // find gameReview by reviewId

            var foundgameReviews = modelAssembler.GetReviewsOfGame(gameReview.ReviewId);

            Assert.IsNotNull(foundgameReview);
            Assert.IsTrue(string.IsNullOrEmpty(foundgameReviews.ErrorMessage));
            Assert.IsTrue(foundgameReviews.List.Count == 1);

            // find gameReview by gameId

            foundgameReviews = modelAssembler.GetGamesofReview(gameReview.GameId);

            Assert.IsNotNull(foundgameReview);
            Assert.IsTrue(string.IsNullOrEmpty(foundgameReviews.ErrorMessage));
            Assert.IsTrue(foundgameReviews.List.Count == 1);

            // failed to add gameReview 

            foundgameReview.Id = 0;

            errorMessage = "";

            result = modelAssembler.AddOrEditGameReview(foundgameReview, ref errorMessage);

            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));

            errorMessage = "";

            // failed to get id 

            foundgameReview = modelAssembler.GetGameReviewById(500, ref errorMessage);

            Assert.IsNull(foundgameReview);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // edit gameReview 

            var updatedgameReview = new GameReview(gameReview.Id, updateGame.Id, updateReview.Id);

            errorMessage = "";

            result = modelAssembler.AddOrEditGameReview(updatedgameReview, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gameReview.Id, updatedgameReview.Id);

            // get updated gameReview by id 

            foundgameReview = modelAssembler.GetGameReviewById(updatedgameReview.Id, ref errorMessage);

            Assert.IsNotNull(foundgameReview);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(updatedgameReview.Id, foundgameReview.Id);

            // get updated gameReview by name

            foundgameReview = modelAssembler.GetGameReviewsByIds(updatedgameReview.GameId, updatedgameReview.ReviewId, ref errorMessage);

            Assert.IsNotNull(foundgameReview);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gameReview.Id, foundgameReview.Id);

            // delete gameReview 

            result = modelAssembler.DeleteGameReviews(updatedgameReview.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // find gameReview by ids should fail 

            foundgameReview = modelAssembler.GetGameReviewsByIds(updatedgameReview.GameId, updatedgameReview.ReviewId, ref errorMessage);

            Assert.IsNull(foundgameReview);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Delete the reviews

            errorMessage = "";
            result = modelAssembler.DeleteReview(review.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            errorMessage = "";
            result = modelAssembler.DeleteReview(updateReview.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Delete the games

            errorMessage = "";
            result = modelAssembler.DeleteGame(game.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            errorMessage = "";
            result = modelAssembler.DeleteGame(updateGame.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        #endregion GameReview Test Cases

        #region GamePlatform Test Cases

        [TestMethod]
        public void ModelAssemblerConstructorTestGamePlatform()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);
        }

        [TestMethod]
        public void ModelAssemblerGamePlatformTest()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);


            // Add Games used for references in GamePlatforms

            var game = new Game(
                0,
                "Defender3",
                "Awesome Space Game3");

            var errorMessage = "";
            var result = modelAssembler.AddOrEditGame(game, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(game.Id > 0);

            var updateGame = new Game(
                0,
                "Name3",
                "Description3");

            errorMessage = "";
            result = modelAssembler.AddOrEditGame(updateGame, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(updateGame.Id > 0);


            // Add Platform used for references in GamePlatforms

            var platform = new Platform(0, "Bob", "not really a platform");
            Assert.IsNotNull(platform);
            errorMessage = "";
            result = modelAssembler.AddOrEditPlatform(platform, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(platform.Id > 0);

            var updatePlatform = new Platform(0, "Billy3", "imaginary friend platform3");
            Assert.IsNotNull(updatePlatform);
            errorMessage = "";
            result = modelAssembler.AddOrEditPlatform(updatePlatform, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(updatePlatform.Id > 0);


            // get all GamePlatforms 

            var gamePlatforms = modelAssembler.GetGamePlatforms();

            if (gamePlatforms.List.Count > 0)
            {
                Assert.IsTrue(gamePlatforms.List.Count > 0);
            }

            // add a gamePlatform

            var gamePlatform = new GamePlatform(
                0,
                game.Id,
                platform.Id);

            Assert.IsNotNull(gamePlatform);

            errorMessage = "";

            result = modelAssembler.AddOrEditGamePlatform(gamePlatform, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(gamePlatform.Id > 0);

            // find gamePlatform id 

            var foundgamePlatform = modelAssembler.GetGamePlatformById(gamePlatform.Id, ref errorMessage);

            Assert.IsNotNull(foundgamePlatform);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gamePlatform.Id, foundgamePlatform.Id);

            // find gamePlatform by ids

            foundgamePlatform = modelAssembler.GetGamePlatformsByIds(gamePlatform.GameId, gamePlatform.PlatformId, ref errorMessage);

            Assert.IsNotNull(foundgamePlatform);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gamePlatform.Id, foundgamePlatform.Id);

            // find gamePlatform by platformId

            var foundgamePlatforms = modelAssembler.GetPlatformsOfGame(gamePlatform.PlatformId);

            Assert.IsNotNull(foundgamePlatform);
            Assert.IsTrue(string.IsNullOrEmpty(foundgamePlatforms.ErrorMessage));
            Assert.IsTrue(foundgamePlatforms.List.Count == 1);

            // find gamePlatform by gameId

            foundgamePlatforms = modelAssembler.GetGamesofPlatform(gamePlatform.GameId);

            Assert.IsNotNull(foundgamePlatform);
            Assert.IsTrue(string.IsNullOrEmpty(foundgamePlatforms.ErrorMessage));
            Assert.IsTrue(foundgamePlatforms.List.Count == 1);

            // failed to add gamePlatform 

            foundgamePlatform.Id = 0;

            errorMessage = "";

            result = modelAssembler.AddOrEditGamePlatform(foundgamePlatform, ref errorMessage);

            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));

            errorMessage = "";

            // failed to get id 

            foundgamePlatform = modelAssembler.GetGamePlatformById(500, ref errorMessage);

            Assert.IsNull(foundgamePlatform);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // edit gamePlatform 

            var updatedgamePlatform = new GamePlatform(gamePlatform.Id, updateGame.Id, updatePlatform.Id);

            errorMessage = "";

            result = modelAssembler.AddOrEditGamePlatform(updatedgamePlatform, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gamePlatform.Id, updatedgamePlatform.Id);

            // get updated gamePlatform by id 

            foundgamePlatform = modelAssembler.GetGamePlatformById(updatedgamePlatform.Id, ref errorMessage);

            Assert.IsNotNull(foundgamePlatform);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(updatedgamePlatform.Id, foundgamePlatform.Id);

            // get updated gamePlatform by name

            foundgamePlatform = modelAssembler.GetGamePlatformsByIds(updatedgamePlatform.GameId, updatedgamePlatform.PlatformId, ref errorMessage);

            Assert.IsNotNull(foundgamePlatform);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gamePlatform.Id, foundgamePlatform.Id);

            // delete gamePlatform 

            result = modelAssembler.DeleteGamePlatforms(updatedgamePlatform.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // find gamePlatform by ids should fail 

            foundgamePlatform = modelAssembler.GetGamePlatformsByIds(updatedgamePlatform.GameId, updatedgamePlatform.PlatformId, ref errorMessage);

            Assert.IsNull(foundgamePlatform);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Delete the platforms

            errorMessage = "";
            result = modelAssembler.DeletePlatform(platform.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            errorMessage = "";
            result = modelAssembler.DeletePlatform(updatePlatform.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));


            // Delete the games

            errorMessage = "";
            result = modelAssembler.DeleteGame(game.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            errorMessage = "";
            result = modelAssembler.DeleteGame(updateGame.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        #endregion GamePlatform Test Cases

        #region GameImage Test Cases

        [TestMethod]
        public void ModelAssemblerConstructorTestGameImage()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);
        }

        [TestMethod]
        public void ModelAssemblerImageTest()
        {
            var modelAssembler = new ModelAssembler(ConnectionString);

            Assert.IsTrue(modelAssembler.IsDatabaseConnected);


            // Add Games used for references in GameImages

            var game = new Game(
                0,
                "Defender3",
                "Awesome Space Game3");

            var errorMessage = "";
            var result = modelAssembler.AddOrEditGame(game, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(game.Id > 0);

            var updateGame = new Game(
                0,
                "Name3",
                "Description3");

            errorMessage = "";
            result = modelAssembler.AddOrEditGame(updateGame, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(updateGame.Id > 0);

            // get all GameImages 

            var gameImages = modelAssembler.GetGameImages();

            if (gameImages.List.Count > 0)
            {
                Assert.IsTrue(gameImages.List.Count > 0);
            }

            // add a gameImage

            var gameImage = new GameImage(
                0,
                game.Id,
                null);

            Assert.IsNotNull(gameImage);

            errorMessage = "";

            result = modelAssembler.AddOrEditGameImage(gameImage, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(gameImage.Id > 0);

            // find gameImage id 

            var foundgameImage = modelAssembler.GetGameImageById(gameImage.Id, ref errorMessage);

            Assert.IsNotNull(foundgameImage);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gameImage.Id, foundgameImage.Id);

            // find gameImage by gameId

            var foundgameImages = modelAssembler.GetGamesOfImage(gameImage.GameId);

            Assert.IsNotNull(foundgameImage);
            Assert.IsTrue(string.IsNullOrEmpty(foundgameImages.ErrorMessage));
            Assert.IsTrue(foundgameImages.List.Count == 1);

            // failed to add gameImage 

            foundgameImage.GameId = 0;

            errorMessage = "";

            result = modelAssembler.AddOrEditGameImage(foundgameImage, ref errorMessage);

            Assert.IsFalse(result);
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));

            errorMessage = "";

            // failed to get id 

            foundgameImage = modelAssembler.GetGameImageById(500, ref errorMessage);

            Assert.IsNull(foundgameImage);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // edit gameImage 

            var updatedgameImage = new GameImage(gameImage.Id, updateGame.Id, null);

            errorMessage = "";

            result = modelAssembler.AddOrEditGameImage(updatedgameImage, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(gameImage.Id, updatedgameImage.Id);

            // get updated gameImage by id 

            foundgameImage = modelAssembler.GetGameImageById(updatedgameImage.Id, ref errorMessage);

            Assert.IsNotNull(foundgameImage);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(updatedgameImage.Id, foundgameImage.Id);

            // delete gameImage 

            result = modelAssembler.DeleteGameImage(updatedgameImage.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // find gameImage by id should fail 

            foundgameImage = modelAssembler.GetGameImageById(updatedgameImage.Id, ref errorMessage);

            Assert.IsNull(foundgameImage);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            // Delete the games

            errorMessage = "";
            result = modelAssembler.DeleteGame(game.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));

            errorMessage = "";
            result = modelAssembler.DeleteGame(updateGame.Id, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        #endregion GameImage Test Cases

    }
}
