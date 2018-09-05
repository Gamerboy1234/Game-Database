using System;
using GameLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameLibrary.Core.Test
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
        public void ModelAssemblerConstructorTest()
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
    }
}
