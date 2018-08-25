using System;
using System.Linq;
using GameLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sql.Server.Connection;
using Utilities;

namespace GameLibrary.Model.Test
{
    [TestClass]
    public class GameReviewTest
    {
        #region Private Members

        // connecting to server
        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        #endregion Private Members


        #region Test Cases

        [TestMethod]
        public void GameReviewTestConstructorTest()
        {
            var gameReview = new GameReview();

            Assert.AreEqual(gameReview.Id, 0);
            Assert.AreEqual(gameReview.GameId, 0);
            Assert.AreEqual(gameReview.ReviewId, 0);
        }

        [TestMethod]
        public void GameReviewPropertiesTest()
        {
            var gameReview = new GameReview();

            Assert.AreEqual(gameReview.Id, 0);
            gameReview.Id = 1;
            Assert.AreEqual(gameReview.Id, 1);

            Assert.AreEqual(gameReview.GameId, 0);
            gameReview.GameId = 2;
            Assert.AreEqual(gameReview.GameId, 2);

            Assert.AreEqual(gameReview.ReviewId, 0);
            gameReview.ReviewId = 3;
            Assert.AreEqual(gameReview.ReviewId, 3);
        }

        [TestMethod]
        public void GameReviewGenerateInsertStatementTest()
        {
            var gameReview = new GameReview(
                1,
                2,
                3);

            Assert.AreEqual(gameReview.GenerateInsertStatement(), "INSERT INTO GameReview (GameId, ReviewId) VALUES (2, 3)");
        }

        [TestMethod]
        public void GameReviewGenerateUpdateStatementTest()
        {
            var gameReview = new GameReview(
                1,
                2,
                3);

            Assert.AreEqual(gameReview.GenerateUpdateStatement(), "UPDATE GameReview SET GameId = 2, ReviewId = 3 WHERE Id = 1");
        }

        [TestMethod]
        public void GameReviewGenerateDeleteStatementTest()
        {
            var gameReview = new GameReview(
                1,
                2,
                3);

            Assert.AreEqual(gameReview.GenerateDeleteStatement(), "DELETE FROM GameReview WHERE Id = 1");
        }

        [TestMethod]
        public void GameReviewGenerateExistsQueryTest()
        {
            var gameReview = new GameReview(
                1,
                2,
                3);

            Assert.AreEqual(gameReview.GenerateExistsQuery(), "SELECT Id FROM GameReview WHERE Id = 1");
        }

        [TestMethod]
        public void GameReviewGenerateSelectQueryTest()
        {
            var gameReview = new GameReview();

            Assert.AreEqual(gameReview.GenerateSelectQuery(), "SELECT Id, GameId, ReviewId FROM GameReview");

            gameReview = new GameReview(
                1,
                2,
                3);

            Assert.AreEqual(gameReview.GenerateSelectQuery(), "SELECT Id, GameId, ReviewId FROM GameReview WHERE Id = 1");
        }

        [TestMethod]
        public void GameReviewGeneratePrimaryKeyWhereClauseTest()
        {
            var gameReview = new GameReview();

            Assert.AreEqual(gameReview.GeneratePrimaryKeyWhereClause(), "");

            gameReview = new GameReview(
                1,
                2,
                3);

            Assert.AreEqual(gameReview.GeneratePrimaryKeyWhereClause(), "Id = 1");
        }

        [TestMethod]
        public void GameReviewDatabaseCommandsTest()
        {
            var connection = new SqlServerConnection();

            Assert.AreEqual(connection.IsConnected, false);

            var result = connection.Connect(ConnectionString, 0);

            Assert.AreEqual(result, true);


            // Add a Games

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


            // Add a Reviews

            var review = new Review(0, "Name", "Description", 3);

            insertCommand = review.GenerateInsertStatment();
            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";
            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            review.Id = newId;

            var updateReview = new Review(0, "Name", "Description", 3);

            insertCommand = updateReview.GenerateInsertStatment();
            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";
            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            updateReview.Id = newId;


            // Select All

            var gameReview = new GameReview();

            var selectQuery = gameReview.GenerateSelectQuery();

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

            gameReview = new GameReview(
                0,
                game.Id,
                review.Id);

            insertCommand = gameReview.GenerateInsertStatement();

            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";

            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            gameReview.Id = newId;


            // Exists

            var existsQuery = gameReview.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            var existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            var existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            var recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);


            // Select

            selectQuery = gameReview.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            GameReview foundGameReview = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGameReview = GameReview.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundGameReview);

            Assert.AreNotSame(gameReview, foundGameReview);

            Assert.AreEqual(gameReview.Id, foundGameReview.Id);
            Assert.AreEqual(gameReview.GameId, foundGameReview.GameId);
            Assert.AreEqual(gameReview.ReviewId, foundGameReview.ReviewId);


            // Update

            var updateGameReview = new GameReview(
                newId,
                updateGame.Id,
                updateReview.Id);

            var updateCommand = updateGameReview.GenerateUpdateStatement();

            Assert.IsFalse(string.IsNullOrEmpty(updateCommand));

            errorMessage = "";
            var updateResult = connection.ExecuteCommand(updateCommand, ref errorMessage);

            Assert.AreEqual(updateResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);


            // Exists

            existsQuery = updateGameReview.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);


            // Select

            selectQuery = updateGameReview.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundGameReview = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGameReview = GameReview.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundGameReview);

            Assert.AreNotSame(updateGameReview, foundGameReview);

            Assert.AreEqual(updateGameReview.Id, foundGameReview.Id);
            Assert.AreEqual(updateGameReview.GameId, foundGameReview.GameId);
            Assert.AreEqual(updateGameReview.ReviewId, foundGameReview.ReviewId);


            // Delete

            var deleteCommand = gameReview.GenerateDeleteStatement();

            var deleteReview = review.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            var deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);


            // Exists

            existsQuery = gameReview.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.IsFalse(recordExists);


            // Select

            selectQuery = gameReview.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundGameReview = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGameReview = GameReview.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNull(foundGameReview);


            // Delete the reviews

            deleteCommand = review.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            deleteCommand = updateReview.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);


            // Delete the games

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
        public void GameReviewBinaryCloneTest()
        {
            var gameReview1 = new GameReview(
                1,
                2,
                3);

            var gameReview2 = CloneUtility.BinaryClone(gameReview1);

            Assert.AreNotSame(gameReview1, gameReview2);

            Assert.AreEqual(gameReview1.Id, gameReview2.Id);
            Assert.AreEqual(gameReview1.GameId, gameReview2.GameId);
            Assert.AreEqual(gameReview1.ReviewId, gameReview2.ReviewId);
        }

        [TestMethod]
        public void GameReviewXmlCloneTest()
        {
            var gameReview1 = new GameReview(
                1,
                2,
                3);

            var gameReview2 = CloneUtility.XmlClone(gameReview1, null);

            Assert.AreNotSame(gameReview1, gameReview2);

            Assert.AreEqual(gameReview1.Id, gameReview2.Id);
            Assert.AreEqual(gameReview1.GameId, gameReview2.GameId);
            Assert.AreEqual(gameReview1.ReviewId, gameReview2.ReviewId);
        }

        [TestMethod]
        public void GameReviewJsonTest()
        {
            var gameReview1 = new GameReview(
                1,
                2,
                3);

            var jsonText = CloneUtility.ToJson(gameReview1);

            Assert.IsFalse(string.IsNullOrEmpty(jsonText));

            var gameReview2 = CloneUtility.FromJson<GameReview>(jsonText);

            Assert.AreNotSame(gameReview1, gameReview2);

            Assert.AreEqual(gameReview1.Id, gameReview2.Id);
            Assert.AreEqual(gameReview1.GameId, gameReview2.GameId);
            Assert.AreEqual(gameReview1.ReviewId, gameReview2.ReviewId);
        }

        [TestMethod]
        public void GameReviewDictionaryTest()
        {
            var gameReview1 = new GameReview(
                1,
                2,
                3);

            var dictionary = GameReview.ToDictionary(gameReview1);

            Assert.IsNotNull(dictionary);

            var gameReview2 = GameReview.FromDictionary(dictionary);

            Assert.AreNotSame(gameReview1, gameReview2);

            Assert.AreEqual(gameReview1.Id, gameReview2.Id);
            Assert.AreEqual(gameReview1.GameId, gameReview2.GameId);
            Assert.AreEqual(gameReview1.ReviewId, gameReview2.ReviewId);
        }

        [TestMethod]
        public void GameReviewListTestConstructorTest()
        {
            var gameReviewList = new GameReviewList();

            Assert.IsNotNull(gameReviewList);
            Assert.AreEqual(gameReviewList.List.Count, 0);
        }

        [TestMethod]
        public void GameReviewListGetByIdTest()
        {
            var gameReviewList = new GameReviewList();

            gameReviewList.Add(new GameReview(
                1,
                2,
                3));

            gameReviewList.Add(new GameReview(
                2,
                3,
                4));

            gameReviewList.Add(new GameReview(
                3,
                4,
                5));

            var gameReview = gameReviewList.GetById(0);
            Assert.AreEqual(gameReview, null);

            gameReview = gameReviewList.GetById(-1);
            Assert.AreEqual(gameReview, null);

            gameReview = gameReviewList.GetById(1);
            Assert.AreEqual(gameReview.GameId, 2);

            gameReview = gameReviewList.GetById(2);
            Assert.AreEqual(gameReview.GameId, 3);

            gameReview = gameReviewList.GetById(3);
            Assert.AreEqual(gameReview.GameId, 4);
        }

        [TestMethod]
        public void GameReviewListExistsTest()
        {
            var gameReviewList = new GameReviewList();

            gameReviewList.Add(new GameReview(
                1,
                2,
                3));

            gameReviewList.Add(new GameReview(
                2,
                3,
                4));

            gameReviewList.Add(new GameReview(
                3,
                4,
                5));

            Assert.IsFalse(gameReviewList.Exists(0));
            Assert.IsFalse(gameReviewList.Exists(-1));
            Assert.AreEqual(gameReviewList.Exists(1), true);
            Assert.AreEqual(gameReviewList.Exists(2), true);
            Assert.AreEqual(gameReviewList.Exists(3), true);
        }

        [TestMethod]
        public void GameReviewListAddandRemoveTest()
        {
            var gameReviewList = new GameReviewList();

            Assert.AreEqual(gameReviewList.List.Count, 0);

            gameReviewList.Add(new GameReview(
                1,
                2,
                3));

            Assert.AreEqual(gameReviewList.List.Count, 1);

            gameReviewList.Add(new GameReview(
                2,
                3,
                4));

            Assert.AreEqual(gameReviewList.List.Count, 2);

            gameReviewList.Add(new GameReview(
                3,
                4,
                5));

            Assert.AreEqual(gameReviewList.List.Count, 3);

            gameReviewList.Remove(1);

            Assert.AreEqual(gameReviewList.List.Count, 2);

            gameReviewList.Remove(3);

            Assert.AreEqual(gameReviewList.List.Count, 1);

            gameReviewList.Remove(2);

            Assert.AreEqual(gameReviewList.List.Count, 0);
        }

        [TestMethod]
        public void GameReviewListJsonTest()
        {
            var gameReviewList1 = new GameReviewList();

            gameReviewList1.Add(new GameReview(
                1,
                2,
                3));

            gameReviewList1.Add(new GameReview(
                2,
                3,
                4));

            gameReviewList1.Add(new GameReview(
                3,
                4,
                5));

            var jsonText = CloneUtility.ToJson(gameReviewList1);

            Assert.IsFalse(string.IsNullOrEmpty(jsonText));

            var gameReviewList2 = CloneUtility.FromJson<GameReviewList>(jsonText);

            Assert.AreNotSame(gameReviewList1, gameReviewList2);
            Assert.AreEqual(gameReviewList1.List.Count, gameReviewList2.List.Count);

            for (var index = 0; index < gameReviewList1.List.Count; index++)
            {
                Assert.AreEqual(gameReviewList1.List[index].Id, gameReviewList2.List[index].Id);
                Assert.AreEqual(gameReviewList1.List[index].GameId, gameReviewList2.List[index].GameId);
                Assert.AreEqual(gameReviewList1.List[index].ReviewId, gameReviewList2.List[index].ReviewId);
            }
        }

        [TestMethod]
        public void GameReviewListBinaryCloneTest()
        {
            var gameReviewList1 = new GameReviewList();

            gameReviewList1.Add(new GameReview(
                1,
                2,
                3));

            gameReviewList1.Add(new GameReview(
                2,
                3,
                4));

            gameReviewList1.Add(new GameReview(
                3,
                4,
                5));

            var gameReviewList2 = CloneUtility.BinaryClone(gameReviewList1);

            Assert.AreNotSame(gameReviewList1, gameReviewList2);
            Assert.AreEqual(gameReviewList1.List.Count, gameReviewList2.List.Count);

            for (var index = 0; index < gameReviewList1.List.Count; index++)
            {
                Assert.AreEqual(gameReviewList1.List[index].Id, gameReviewList2.List[index].Id);
                Assert.AreEqual(gameReviewList1.List[index].GameId, gameReviewList2.List[index].GameId);
                Assert.AreEqual(gameReviewList1.List[index].ReviewId, gameReviewList2.List[index].ReviewId);
            }
        }

        [TestMethod]
        public void GameReviewListXmlCloneTest()
        {
            var gameReviewList1 = new GameReviewList();

            gameReviewList1.Add(new GameReview(
                1,
                2,
                3));

            gameReviewList1.Add(new GameReview(
                2,
                3,
                4));

            gameReviewList1.Add(new GameReview(
                3,
                4,
                5));

            var gameReviewList2 = CloneUtility.XmlClone(gameReviewList1, null);

            Assert.AreNotSame(gameReviewList1, gameReviewList2);
            Assert.AreEqual(gameReviewList1.List.Count, gameReviewList2.List.Count);

            for (var index = 0; index < gameReviewList1.List.Count; index++)
            {
                Assert.AreEqual(gameReviewList1.List[index].Id, gameReviewList2.List[index].Id);
                Assert.AreEqual(gameReviewList1.List[index].GameId, gameReviewList2.List[index].GameId);
                Assert.AreEqual(gameReviewList1.List[index].ReviewId, gameReviewList2.List[index].ReviewId);
            }
        }

        [TestMethod]
        public void GameReviewListDictionaryTest()
        {
            var gameReviewList1 = new GameReviewList();

            gameReviewList1.Add(new GameReview(
                1,
                2,
                3));

            gameReviewList1.Add(new GameReview(
                2,
                3,
                4));

            gameReviewList1.Add(new GameReview(
                3,
                4,
                5));

            var dictionaryList = GameReviewList.ToDictionaryList(gameReviewList1);

            Assert.IsNotNull(dictionaryList);

            var gameReviewList2 = GameReviewList.FromDictionaryList(dictionaryList);

            Assert.AreNotSame(gameReviewList1, gameReviewList2);
            Assert.AreEqual(gameReviewList1.List.Count, gameReviewList2.List.Count);

            for (var index = 0; index < gameReviewList1.List.Count; index++)
            {
                Assert.AreEqual(gameReviewList1.List[index].Id, gameReviewList2.List[index].Id);
                Assert.AreEqual(gameReviewList1.List[index].GameId, gameReviewList2.List[index].GameId);
                Assert.AreEqual(gameReviewList1.List[index].ReviewId, gameReviewList2.List[index].ReviewId);
            }
        }

        #endregion Test Cases
    }
}
