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
    }
}
