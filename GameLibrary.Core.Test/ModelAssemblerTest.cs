using System;
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

            var genre = modelAssembler.GetGenres();

            if (genre.List.Count > 0)
            {
                Assert.IsTrue(genre.List.Count > 0);
            }

            // Add a Genre

            var Genre = new Genre(0,"Bob","not really a genre");
            Assert.IsNotNull(Genre);
            var errorMessage = "";
            var result = modelAssembler.AddOrEditGenre(Genre, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(Genre.Id > 0);

            // Get Genre by Id 

            var foundGenre = modelAssembler.GetGenreById(Genre.Id, ref errorMessage);

            Assert.IsNotNull(foundGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(Genre.Id, foundGenre.Id);

            // Get Genre by Name

            foundGenre = modelAssembler.GetGenreByName(Genre.Name, ref errorMessage);
          
            Assert.IsNotNull(foundGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(Genre.Name, foundGenre.Name);

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
                Genre.Id,
                "Joe",
                "Joe is a Scrub");

            errorMessage = "";
            result = modelAssembler.AddOrEditGenre(updateGenre, ref errorMessage);

            Assert.IsTrue(result);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(Genre.Id, updateGenre.Id);

            // Get Genre by Id

            foundGenre = modelAssembler.GetGenreById(updateGenre.Id, ref errorMessage);

            Assert.IsNotNull(foundGenre);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(Genre.Id, foundGenre.Id);

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
        public void ModelAssemblerConstructorTestRating()
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
        public void ModelAssemblerConstructorTestReview()
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

            var review = modelAssembler.GetReviews();

            if (review.List.Count > 0)
            {
                Assert.IsTrue(review.List.Count > 0);
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

            // find review by name 

            foundReview = modelAssembler.GetReviewByName(Review.Name, ref errorMessage);

            Assert.IsNotNull(foundReview);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual(Review.Name, foundReview.Name);

            // failed to add review

            var failedreview = new Review(0, "Name", "Description", 5);

            errorMessage = "";

            result = modelAssembler.AddOrEditReview(failedreview, ref errorMessage);

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

            var platforms = modelAssembler.GetPlatfomrs();

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

    }
}
