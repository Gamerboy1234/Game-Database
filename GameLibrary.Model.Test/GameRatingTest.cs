using System;
using System.Linq;
using GameLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sql.Server.Connection;
using Utilities;


namespace GameLibrary.Model.Test
{
    [TestClass]
    class GameRatingTest
    {
        #region Private Members

        // connecting to server
        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        #endregion Private Members


        #region Test Cases

        [TestMethod]
        public void GameRatingTestConstructorTest()
        {
            var gameRating = new GameRating();

            Assert.AreEqual(gameRating.Id, 0);
            Assert.AreEqual(gameRating.GameId, 0);
            Assert.AreEqual(gameRating.RatingId, 0);
        }

        [TestMethod]
        public void GameRatingPropertiesTest()
        {
            var gameRating = new GameRating();

            Assert.AreEqual(gameRating.Id, 0);
            gameRating.Id = 1;
            Assert.AreEqual(gameRating.Id, 1);

            Assert.AreEqual(gameRating.GameId, 0);
            gameRating.GameId = 2;
            Assert.AreEqual(gameRating.GameId, 2);

            Assert.AreEqual(gameRating.RatingId, 0);
            gameRating.RatingId = 3;
            Assert.AreEqual(gameRating.RatingId, 3);
        }

        [TestMethod]
        public void GameRatingGenerateInsertStatementTest()
        {
            var gameRating = new GameRating(
                1,
                2,
                3,
                "Notes");

            Assert.AreEqual(gameRating.GenerateInsertStatement(), "INSERT INTO GameRating (GameId, RatingId) VALUES (2, 3)");
        }

        [TestMethod]
        public void GameRatingGenerateUpdateStatementTest()
        {
            var gameRating = new GameRating(
                1,
                2,
                3,"Notes");

            Assert.AreEqual(gameRating.GenerateUpdateStatement(), "UPDATE GameRating SET GameId = 2, RatingId = 3 WHERE Id = 1");
        }

        [TestMethod]
        public void GameRatingGenerateDeleteStatementTest()
        {
            var gameRating = new GameRating(
                1,
                2,
                3, "Notes");

            Assert.AreEqual(gameRating.GenerateDeleteStatement(), "DELETE FROM GameRating WHERE Id = 1");
        }

        [TestMethod]
        public void GameRatingGenerateExistsQueryTest()
        {
            var gameRating = new GameRating(
                1,
                2,
                3, "Notes");

            Assert.AreEqual(gameRating.GenerateExistsQuery(), "SELECT Id FROM GameRating WHERE Id = 1");
        }

        [TestMethod]
        public void GameRatingGenerateSelectQueryTest()
        {
            var gameRating = new GameRating();

            Assert.AreEqual(gameRating.GenerateSelectQuery(), "SELECT Id, GameId, RatingId FROM GameRating");

            gameRating = new GameRating(
                1,
                2,
                3, "Notes");

            Assert.AreEqual(gameRating.GenerateSelectQuery(), "SELECT Id, GameId, RatingId FROM GameRating WHERE Id = 1");
        }

        [TestMethod]
        public void GameRatingGeneratePrimaryKeyWhereClauseTest()
        {
            var gameRating = new GameRating();

            Assert.AreEqual(gameRating.GeneratePrimaryKeyWhereClause(), "");

            gameRating = new GameRating(
                1,
                2,
                3, "Notes");

            Assert.AreEqual(gameRating.GeneratePrimaryKeyWhereClause(), "Id = 1");
        }

        [TestMethod]
        public void GameRatingDatabaseCommandsTest()
        {
            var connection = new SqlServerConnection();

            Assert.AreEqual(connection.IsConnected, false);

            var result = connection.Connect(ConnectionString, 0);

            Assert.AreEqual(result, true);


            // Add Games

            var game = new Game(
                0,
                "Name",
                "Description");

            var insertCommand = game.GenerateInsertStatement();

            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            var errorMessage = "";
            var insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out var newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            game.Id = newId;

            var updateGame = new Game(
                0,
                "Name1",
                "Description1");

            insertCommand = updateGame.GenerateInsertStatement();

            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";
            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            updateGame.Id = newId;


            // Add Ratings

            var rating = new Rating(0, "Name", "Description", "Symbol");

            insertCommand = rating.GenerateInsertStatment();
            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";
            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            rating.Id = newId;

            var updateGenre = new Rating(0, "Name", "Description", "Symbol");

            insertCommand = updateGenre.GenerateInsertStatment();
            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";
            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            updateGenre.Id = newId;


            // Select All

            var gameRating = new GameRating();

            var selectQuery = gameRating.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            var selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            var selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            if (selectResultList.Count > 0)
            {
                Assert.IsTrue(selectResultList.Count > 0);
            }


            // Insert

            gameRating = new GameRating(
                0,
                game.Id,
                rating.Id,
                "Notes");

            insertCommand = gameRating.GenerateInsertStatement();

            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";

            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            gameRating.Id = newId;


            // Exists

            var existsQuery = gameRating.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            var existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            var existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            var recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);


            // Select

            selectQuery = gameRating.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            GameRating foundGameRating = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGameRating = GameRating.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundGameRating);

            Assert.AreNotSame(gameRating, foundGameRating);

            Assert.AreEqual(gameRating.Id, foundGameRating.Id);
            Assert.AreEqual(gameRating.GameId, foundGameRating.GameId);
            Assert.AreEqual(gameRating.RatingId, foundGameRating.RatingId);


            // Update

            var updateGameRating = new GameRating(
                newId,
                updateGame.Id,
                updateGenre.Id, "Notes");

            var updateCommand = updateGameRating.GenerateUpdateStatement();

            Assert.IsFalse(string.IsNullOrEmpty(updateCommand));

            errorMessage = "";
            var updateResult = connection.ExecuteCommand(updateCommand, ref errorMessage);

            Assert.AreEqual(updateResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);


            // Exists

            existsQuery = updateGameRating.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);


            // Select

            selectQuery = updateGameRating.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundGameRating = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGameRating = GameRating.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundGameRating);

            Assert.AreNotSame(updateGameRating, foundGameRating);

            Assert.AreEqual(updateGameRating.Id, foundGameRating.Id);
            Assert.AreEqual(updateGameRating.GameId, foundGameRating.GameId);
            Assert.AreEqual(updateGameRating.RatingId, foundGameRating.RatingId);


            // Delete

            var deleteCommand = gameRating.GenerateDeleteStatement();

            var deleteGenre = rating.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            var deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);


            // Exists

            existsQuery = gameRating.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.IsFalse(recordExists);


            // Select

            selectQuery = gameRating.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundGameRating = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGameRating = GameRating.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNull(foundGameRating);


            // Delete Ratings

            deleteCommand = rating.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            deleteCommand = updateGenre.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);


            // Delete Games

            deleteCommand = game.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            deleteCommand = updateGame.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
        }

        [TestMethod]
        public void GameRatingBinaryCloneTest()
        {
            var gameRating1 = new GameRating(
                1,
                2,
                3, "Notes");

            var gameRating2 = CloneUtility.BinaryClone(gameRating1);

            Assert.AreNotSame(gameRating1, gameRating2);

            Assert.AreEqual(gameRating1.Id, gameRating2.Id);
            Assert.AreEqual(gameRating1.GameId, gameRating2.GameId);
            Assert.AreEqual(gameRating1.RatingId, gameRating2.RatingId);
        }

        [TestMethod]
        public void GameRatingXmlCloneTest()
        {
            var gameRating1 = new GameRating(
                1,
                2,
                3, "Notes");

            var gameRating2 = CloneUtility.XmlClone(gameRating1, null);

            Assert.AreNotSame(gameRating1, gameRating2);

            Assert.AreEqual(gameRating1.Id, gameRating2.Id);
            Assert.AreEqual(gameRating1.GameId, gameRating2.GameId);
            Assert.AreEqual(gameRating1.RatingId, gameRating2.RatingId);
        }

        [TestMethod]
        public void GameRatingJsonTest()
        {
            var gameRating1 = new GameRating(
                1,
                2,
                3, "Notes");

            var jsonText = CloneUtility.ToJson(gameRating1);

            Assert.IsFalse(string.IsNullOrEmpty(jsonText));

            var gameRating2 = CloneUtility.FromJson<GameRating>(jsonText);

            Assert.AreNotSame(gameRating1, gameRating2);

            Assert.AreEqual(gameRating1.Id, gameRating2.Id);
            Assert.AreEqual(gameRating1.GameId, gameRating2.GameId);
            Assert.AreEqual(gameRating1.RatingId, gameRating2.RatingId);
        }

        [TestMethod]
        public void GameRatingDictionaryTest()
        {
            var gameRating1 = new GameRating(
                1,
                2,
                3, "Notes");

            var dictionary = GameRating.ToDictionary(gameRating1);

            Assert.IsNotNull(dictionary);

            var gameRating2 = GameRating.FromDictionary(dictionary);

            Assert.AreNotSame(gameRating1, gameRating2);

            Assert.AreEqual(gameRating1.Id, gameRating2.Id);
            Assert.AreEqual(gameRating1.GameId, gameRating2.GameId);
            Assert.AreEqual(gameRating1.RatingId, gameRating2.RatingId);
        }

        [TestMethod]
        public void GameRatingListTestConstructorTest()
        {
            var gameRatingList = new GameRatingList();

            Assert.IsNotNull(gameRatingList);
            Assert.AreEqual(gameRatingList.List.Count, 0);
        }

        [TestMethod]
        public void GameRatingListGetByIdTest()
        {
            var gameRatingList = new GameRatingList();

            gameRatingList.Add(new GameRating(
                1,
                2,
                3, "Notes1"));

            gameRatingList.Add(new GameRating(
                2,
                3,
                4, "Notes2"));

            gameRatingList.Add(new GameRating(
                3,
                4,
                5, "Notes3"));

            var gameRating = gameRatingList.GetById(0);
            Assert.AreEqual(gameRating, null);

            gameRating = gameRatingList.GetById(-1);
            Assert.AreEqual(gameRating, null);

            gameRating = gameRatingList.GetById(1);
            Assert.AreEqual(gameRating.GameId, 2);

            gameRating = gameRatingList.GetById(2);
            Assert.AreEqual(gameRating.GameId, 3);

            gameRating = gameRatingList.GetById(3);
            Assert.AreEqual(gameRating.GameId, 4);
        }

        [TestMethod]
        public void GameRatingListExistsTest()
        {
            var gameRatingList = new GameRatingList();

            gameRatingList.Add(new GameRating(
                1,
                2,
                3, "Notes1"));

            gameRatingList.Add(new GameRating(
                2,
                3,
                4, "Notes2"));

            gameRatingList.Add(new GameRating(
                3,
                4,
                5, "Notes3"));

            Assert.IsFalse(gameRatingList.Exists(0));
            Assert.IsFalse(gameRatingList.Exists(-1));
            Assert.AreEqual(gameRatingList.Exists(1), true);
            Assert.AreEqual(gameRatingList.Exists(2), true);
            Assert.AreEqual(gameRatingList.Exists(3), true);
        }

        [TestMethod]
        public void GameRatingListAddandRemoveTest()
        {
            var gameRatingList = new GameRatingList();

            Assert.AreEqual(gameRatingList.List.Count, 0);

            gameRatingList.Add(new GameRating(
                1,
                2,
                3, "Notes1"));

            Assert.AreEqual(gameRatingList.List.Count, 1);

            gameRatingList.Add(new GameRating(
                2,
                3,
                4, "Notes2"));

            Assert.AreEqual(gameRatingList.List.Count, 2);

            gameRatingList.Add(new GameRating(
                3,
                4,
                5, "Notes3"));

            Assert.AreEqual(gameRatingList.List.Count, 3);

            gameRatingList.Remove(1);

            Assert.AreEqual(gameRatingList.List.Count, 2);

            gameRatingList.Remove(3);

            Assert.AreEqual(gameRatingList.List.Count, 1);

            gameRatingList.Remove(2);

            Assert.AreEqual(gameRatingList.List.Count, 0);
        }

        [TestMethod]
        public void GameRatingListJsonTest()
        {
            var gameRatingList1 = new GameRatingList();

            gameRatingList1.Add(new GameRating(
                1,
                2,
                3, "Notes1"));

            gameRatingList1.Add(new GameRating(
                2,
                3,
                4, "Notes2"));

            gameRatingList1.Add(new GameRating(
                3,
                4,
                5, "Notes3"));

            var jsonText = CloneUtility.ToJson(gameRatingList1);

            Assert.IsFalse(string.IsNullOrEmpty(jsonText));

            var gameRatingList2 = CloneUtility.FromJson<GameRatingList>(jsonText);

            Assert.AreNotSame(gameRatingList1, gameRatingList2);
            Assert.AreEqual(gameRatingList1.List.Count, gameRatingList2.List.Count);

            for (var index = 0; index < gameRatingList1.List.Count; index++)
            {
                Assert.AreEqual(gameRatingList1.List[index].Id, gameRatingList2.List[index].Id);
                Assert.AreEqual(gameRatingList1.List[index].GameId, gameRatingList2.List[index].GameId);
                Assert.AreEqual(gameRatingList1.List[index].RatingId, gameRatingList2.List[index].RatingId);
            }
        }

        [TestMethod]
        public void GameRatingListBinaryCloneTest()
        {
            var gameRatingList1 = new GameRatingList();

            gameRatingList1.Add(new GameRating(
                1,
                2,
                3, "Notes1"));

            gameRatingList1.Add(new GameRating(
                2,
                3,
                4, "Notes3"));

            gameRatingList1.Add(new GameRating(
                3,
                4,
                5, "Notes4"));

            var gameRatingList2 = CloneUtility.BinaryClone(gameRatingList1);

            Assert.AreNotSame(gameRatingList1, gameRatingList2);
            Assert.AreEqual(gameRatingList1.List.Count, gameRatingList2.List.Count);

            for (var index = 0; index < gameRatingList1.List.Count; index++)
            {
                Assert.AreEqual(gameRatingList1.List[index].Id, gameRatingList2.List[index].Id);
                Assert.AreEqual(gameRatingList1.List[index].GameId, gameRatingList2.List[index].GameId);
                Assert.AreEqual(gameRatingList1.List[index].RatingId, gameRatingList2.List[index].RatingId);
            }
        }

        [TestMethod]
        public void GameRatingListXmlCloneTest()
        {
            var gameRatingList1 = new GameRatingList();

            gameRatingList1.Add(new GameRating(
                1,
                2,
                3, "Notes1"));

            gameRatingList1.Add(new GameRating(
                2,
                3,
                4, "Notes2"));

            gameRatingList1.Add(new GameRating(
                3,
                4,
                5, "Notes3"));

            var gameRatingList2 = CloneUtility.XmlClone(gameRatingList1, null);

            Assert.AreNotSame(gameRatingList1, gameRatingList2);
            Assert.AreEqual(gameRatingList1.List.Count, gameRatingList2.List.Count);

            for (var index = 0; index < gameRatingList1.List.Count; index++)
            {
                Assert.AreEqual(gameRatingList1.List[index].Id, gameRatingList2.List[index].Id);
                Assert.AreEqual(gameRatingList1.List[index].GameId, gameRatingList2.List[index].GameId);
                Assert.AreEqual(gameRatingList1.List[index].RatingId, gameRatingList2.List[index].RatingId);
            }
        }

        [TestMethod]
        public void GameRatingListDictionaryTest()
        {
            var gameRatingList1 = new GameRatingList();

            gameRatingList1.Add(new GameRating(
                1,
                2,
                3, "Notes4"));

            gameRatingList1.Add(new GameRating(
                2,
                3,
                4, "Notes1"));

            gameRatingList1.Add(new GameRating(
                3,
                4,
                5, "Notes2"));

            var dictionaryList = GameRatingList.ToDictionaryList(gameRatingList1);

            Assert.IsNotNull(dictionaryList);

            var gameRatingList2 = GameRatingList.FromDictionaryList(dictionaryList);

            Assert.AreNotSame(gameRatingList1, gameRatingList2);
            Assert.AreEqual(gameRatingList1.List.Count, gameRatingList2.List.Count);

            for (var index = 0; index < gameRatingList1.List.Count; index++)
            {
                Assert.AreEqual(gameRatingList1.List[index].Id, gameRatingList2.List[index].Id);
                Assert.AreEqual(gameRatingList1.List[index].GameId, gameRatingList2.List[index].GameId);
                Assert.AreEqual(gameRatingList1.List[index].RatingId, gameRatingList2.List[index].RatingId);
            }
        }

        #endregion Test Cases
    }
}
