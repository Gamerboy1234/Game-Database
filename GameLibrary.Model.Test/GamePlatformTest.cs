using System;
using System.Linq;
using GameLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sql.Server.Connection;
using Utilities;

namespace GameLibrary.Model.Test
{
    public class GamePlatformTest
    {
        #region Private Members

        // connecting to server
        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        #endregion Private Members


        #region Test Cases

        [TestMethod]
        public void GamePlatformTestConstructorTest()
        {
            var gamePlatform = new GamePlatform();

            Assert.AreEqual(gamePlatform.Id, 0);
            Assert.AreEqual(gamePlatform.GameId, 0);
            Assert.AreEqual(gamePlatform.PlatformId, 0);
        }

        [TestMethod]
        public void GamePlatformPropertiesTest()
        {
            var gamePlatform = new GamePlatform();

            Assert.AreEqual(gamePlatform.Id, 0);
            gamePlatform.Id = 1;
            Assert.AreEqual(gamePlatform.Id, 1);

            Assert.AreEqual(gamePlatform.GameId, 0);
            gamePlatform.GameId = 2;
            Assert.AreEqual(gamePlatform.GameId, 2);

            Assert.AreEqual(gamePlatform.PlatformId, 0);
            gamePlatform.PlatformId = 3;
            Assert.AreEqual(gamePlatform.PlatformId, 3);
        }

        [TestMethod]
        public void GamePlatformGenerateInsertStatementTest()
        {
            var gamePlatform = new GamePlatform(
                1,
                2,
                3);

            Assert.AreEqual(gamePlatform.GenerateInsertStatement(), "INSERT INTO GamePlatform (GameId, PlatformId) VALUES (2, 3)");
        }

        [TestMethod]
        public void GamePlatformGenerateUpdateStatementTest()
        {
            var gamePlatform = new GamePlatform(
                1,
                2,
                3);

            Assert.AreEqual(gamePlatform.GenerateUpdateStatement(), "UPDATE GamePlatform SET GameId = 2, PlatformId = 3 WHERE Id = 1");
        }

        [TestMethod]
        public void GamePlatformGenerateDeleteStatementTest()
        {
            var gamePlatform = new GamePlatform(
                1,
                2,
                3);

            Assert.AreEqual(gamePlatform.GenerateDeleteStatement(), "DELETE FROM GamePlatform WHERE Id = 1");
        }

        [TestMethod]
        public void GamePlatformGenerateExistsQueryTest()
        {
            var gamePlatform = new GamePlatform(
                1,
                2,
                3);

            Assert.AreEqual(gamePlatform.GenerateExistsQuery(), "SELECT Id FROM GamePlatform WHERE Id = 1");
        }

        [TestMethod]
        public void GamePlatformGenerateSelectQueryTest()
        {
            var gamePlatform = new GamePlatform();

            Assert.AreEqual(gamePlatform.GenerateSelectQuery(), "SELECT Id, GameId, PlatformId FROM GamePlatform");

            gamePlatform = new GamePlatform(
                1,
                2,
                3);

            Assert.AreEqual(gamePlatform.GenerateSelectQuery(), "SELECT Id, GameId, PlatformId FROM GamePlatform WHERE Id = 1");
        }

        [TestMethod]
        public void GamePlatformGeneratePrimaryKeyWhereClauseTest()
        {
            var gamePlatform = new GamePlatform();

            Assert.AreEqual(gamePlatform.GeneratePrimaryKeyWhereClause(), "");

            gamePlatform = new GamePlatform(
                1,
                2,
                3);

            Assert.AreEqual(gamePlatform.GeneratePrimaryKeyWhereClause(), "Id = 1");
        }

        [TestMethod]
        public void GamePlatformDatabaseCommandsTest()
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


            // Add Platforms

            var platform = new Platform(0, "Name", "Description");

            insertCommand = platform.GenerateInsertStatment();
            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";
            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            platform.Id = newId;

            var updatePlatform = new Platform(0, "Name", "Description");

            insertCommand = updatePlatform.GenerateInsertStatment();
            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";
            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            updatePlatform.Id = newId;


            // Select All

            var gamePlatform = new GamePlatform();

            var selectQuery = gamePlatform.GenerateSelectQuery();

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

            gamePlatform = new GamePlatform(
                0,
                game.Id,
                platform.Id);

            insertCommand = gamePlatform.GenerateInsertStatement();

            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";

            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            gamePlatform.Id = newId;


            // Exists

            var existsQuery = gamePlatform.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            var existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            var existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            var recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);


            // Select

            selectQuery = gamePlatform.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            GamePlatform foundGamePlatform = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGamePlatform = GamePlatform.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundGamePlatform);

            Assert.AreNotSame(gamePlatform, foundGamePlatform);

            Assert.AreEqual(gamePlatform.Id, foundGamePlatform.Id);
            Assert.AreEqual(gamePlatform.GameId, foundGamePlatform.GameId);
            Assert.AreEqual(gamePlatform.PlatformId, foundGamePlatform.PlatformId);


            // Update

            var updateGamePlatform = new GamePlatform(
                newId,
                updateGame.Id,
                updatePlatform.Id);

            var updateCommand = updateGamePlatform.GenerateUpdateStatement();

            Assert.IsFalse(string.IsNullOrEmpty(updateCommand));

            errorMessage = "";
            var updateResult = connection.ExecuteCommand(updateCommand, ref errorMessage);

            Assert.AreEqual(updateResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);


            // Exists

            existsQuery = updateGamePlatform.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);


            // Select

            selectQuery = updateGamePlatform.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundGamePlatform = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGamePlatform = GamePlatform.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundGamePlatform);

            Assert.AreNotSame(updateGamePlatform, foundGamePlatform);

            Assert.AreEqual(updateGamePlatform.Id, foundGamePlatform.Id);
            Assert.AreEqual(updateGamePlatform.GameId, foundGamePlatform.GameId);
            Assert.AreEqual(updateGamePlatform.PlatformId, foundGamePlatform.PlatformId);


            // Delete

            var deleteCommand = gamePlatform.GenerateDeleteStatement();

            var deleteGenre = platform.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            var deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);


            // Exists

            existsQuery = gamePlatform.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.IsFalse(recordExists);


            // Select

            selectQuery = gamePlatform.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundGamePlatform = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGamePlatform = GamePlatform.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNull(foundGamePlatform);


            // Delete the platforms

            deleteCommand = platform.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            deleteCommand = updatePlatform.GenerateDeleteStatement();

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
        public void GamePlatformBinaryCloneTest()
        {
            var gamePlatform1 = new GamePlatform(
                1,
                2,
                3);

            var gamePlatform2 = CloneUtility.BinaryClone(gamePlatform1);

            Assert.AreNotSame(gamePlatform1, gamePlatform2);

            Assert.AreEqual(gamePlatform1.Id, gamePlatform2.Id);
            Assert.AreEqual(gamePlatform1.GameId, gamePlatform2.GameId);
            Assert.AreEqual(gamePlatform1.PlatformId, gamePlatform2.PlatformId);
        }

        [TestMethod]
        public void GamePlatformXmlCloneTest()
        {
            var gamePlatform1 = new GamePlatform(
                1,
                2,
                3);

            var gamePlatform2 = CloneUtility.XmlClone(gamePlatform1, null);

            Assert.AreNotSame(gamePlatform1, gamePlatform2);

            Assert.AreEqual(gamePlatform1.Id, gamePlatform2.Id);
            Assert.AreEqual(gamePlatform1.GameId, gamePlatform2.GameId);
            Assert.AreEqual(gamePlatform1.PlatformId, gamePlatform2.PlatformId);
        }

        [TestMethod]
        public void GamePlatformJsonTest()
        {
            var gamePlatform1 = new GamePlatform(
                1,
                2,
                3);

            var jsonText = CloneUtility.ToJson(gamePlatform1);

            Assert.IsFalse(string.IsNullOrEmpty(jsonText));

            var gamePlatform2 = CloneUtility.FromJson<GamePlatform>(jsonText);

            Assert.AreNotSame(gamePlatform1, gamePlatform2);

            Assert.AreEqual(gamePlatform1.Id, gamePlatform2.Id);
            Assert.AreEqual(gamePlatform1.GameId, gamePlatform2.GameId);
            Assert.AreEqual(gamePlatform1.PlatformId, gamePlatform2.PlatformId);
        }

        [TestMethod]
        public void GamePlatformDictionaryTest()
        {
            var gamePlatform1 = new GamePlatform(
                1,
                2,
                3);

            var dictionary = GamePlatform.ToDictionary(gamePlatform1);

            Assert.IsNotNull(dictionary);

            var gamePlatform2 = GamePlatform.FromDictionary(dictionary);

            Assert.AreNotSame(gamePlatform1, gamePlatform2);

            Assert.AreEqual(gamePlatform1.Id, gamePlatform2.Id);
            Assert.AreEqual(gamePlatform1.GameId, gamePlatform2.GameId);
            Assert.AreEqual(gamePlatform1.PlatformId, gamePlatform2.PlatformId);
        }

        [TestMethod]
        public void GamePlatformListTestConstructorTest()
        {
            var gamePlatformList = new GamePlatformList();

            Assert.IsNotNull(gamePlatformList);
            Assert.AreEqual(gamePlatformList.List.Count, 0);
        }

        [TestMethod]
        public void GamePlatformListGetByIdTest()
        {
            var gamePlatformList = new GamePlatformList();

            gamePlatformList.Add(new GamePlatform(
                1,
                2,
                3));

            gamePlatformList.Add(new GamePlatform(
                2,
                3,
                4));

            gamePlatformList.Add(new GamePlatform(
                3,
                4,
                5));

            var gamePlatform = gamePlatformList.GetById(0);
            Assert.AreEqual(gamePlatform, null);

            gamePlatform = gamePlatformList.GetById(-1);
            Assert.AreEqual(gamePlatform, null);

            gamePlatform = gamePlatformList.GetById(1);
            Assert.AreEqual(gamePlatform.GameId, 2);

            gamePlatform = gamePlatformList.GetById(2);
            Assert.AreEqual(gamePlatform.GameId, 3);

            gamePlatform = gamePlatformList.GetById(3);
            Assert.AreEqual(gamePlatform.GameId, 4);
        }

        [TestMethod]
        public void GamePlatformListExistsTest()
        {
            var gamePlatformList = new GamePlatformList();

            gamePlatformList.Add(new GamePlatform(
                1,
                2,
                3));

            gamePlatformList.Add(new GamePlatform(
                2,
                3,
                4));

            gamePlatformList.Add(new GamePlatform(
                3,
                4,
                5));

            Assert.IsFalse(gamePlatformList.Exists(0));
            Assert.IsFalse(gamePlatformList.Exists(-1));
            Assert.AreEqual(gamePlatformList.Exists(1), true);
            Assert.AreEqual(gamePlatformList.Exists(2), true);
            Assert.AreEqual(gamePlatformList.Exists(3), true);
        }

        [TestMethod]
        public void GamePlatformListAddandRemoveTest()
        {
            var gamePlatformList = new GamePlatformList();

            Assert.AreEqual(gamePlatformList.List.Count, 0);

            gamePlatformList.Add(new GamePlatform(
                1,
                2,
                3));

            Assert.AreEqual(gamePlatformList.List.Count, 1);

            gamePlatformList.Add(new GamePlatform(
                2,
                3,
                4));

            Assert.AreEqual(gamePlatformList.List.Count, 2);

            gamePlatformList.Add(new GamePlatform(
                3,
                4,
                5));

            Assert.AreEqual(gamePlatformList.List.Count, 3);

            gamePlatformList.Remove(1);

            Assert.AreEqual(gamePlatformList.List.Count, 2);

            gamePlatformList.Remove(3);

            Assert.AreEqual(gamePlatformList.List.Count, 1);

            gamePlatformList.Remove(2);

            Assert.AreEqual(gamePlatformList.List.Count, 0);
        }

        [TestMethod]
        public void GamePlatformListJsonTest()
        {
            var gamePlatformList1 = new GamePlatformList();

            gamePlatformList1.Add(new GamePlatform(
                1,
                2,
                3));

            gamePlatformList1.Add(new GamePlatform(
                2,
                3,
                4));

            gamePlatformList1.Add(new GamePlatform(
                3,
                4,
                5));

            var jsonText = CloneUtility.ToJson(gamePlatformList1);

            Assert.IsFalse(string.IsNullOrEmpty(jsonText));

            var gamePlatformList2 = CloneUtility.FromJson<GamePlatformList>(jsonText);

            Assert.AreNotSame(gamePlatformList1, gamePlatformList2);
            Assert.AreEqual(gamePlatformList1.List.Count, gamePlatformList2.List.Count);

            for (var index = 0; index < gamePlatformList1.List.Count; index++)
            {
                Assert.AreEqual(gamePlatformList1.List[index].Id, gamePlatformList2.List[index].Id);
                Assert.AreEqual(gamePlatformList1.List[index].GameId, gamePlatformList2.List[index].GameId);
                Assert.AreEqual(gamePlatformList1.List[index].PlatformId, gamePlatformList2.List[index].PlatformId);
            }
        }

        [TestMethod]
        public void GamePlatformListBinaryCloneTest()
        {
            var gamePlatformList1 = new GamePlatformList();

            gamePlatformList1.Add(new GamePlatform(
                1,
                2,
                3));

            gamePlatformList1.Add(new GamePlatform(
                2,
                3,
                4));

            gamePlatformList1.Add(new GamePlatform(
                3,
                4,
                5));

            var gamePlatformList2 = CloneUtility.BinaryClone(gamePlatformList1);

            Assert.AreNotSame(gamePlatformList1, gamePlatformList2);
            Assert.AreEqual(gamePlatformList1.List.Count, gamePlatformList2.List.Count);

            for (var index = 0; index < gamePlatformList1.List.Count; index++)
            {
                Assert.AreEqual(gamePlatformList1.List[index].Id, gamePlatformList2.List[index].Id);
                Assert.AreEqual(gamePlatformList1.List[index].GameId, gamePlatformList2.List[index].GameId);
                Assert.AreEqual(gamePlatformList1.List[index].PlatformId, gamePlatformList2.List[index].PlatformId);
            }
        }

        [TestMethod]
        public void GamePlatformListXmlCloneTest()
        {
            var gamePlatformList1 = new GamePlatformList();

            gamePlatformList1.Add(new GamePlatform(
                1,
                2,
                3));

            gamePlatformList1.Add(new GamePlatform(
                2,
                3,
                4));

            gamePlatformList1.Add(new GamePlatform(
                3,
                4,
                5));

            var gamePlatformList2 = CloneUtility.XmlClone(gamePlatformList1, null);

            Assert.AreNotSame(gamePlatformList1, gamePlatformList2);
            Assert.AreEqual(gamePlatformList1.List.Count, gamePlatformList2.List.Count);

            for (var index = 0; index < gamePlatformList1.List.Count; index++)
            {
                Assert.AreEqual(gamePlatformList1.List[index].Id, gamePlatformList2.List[index].Id);
                Assert.AreEqual(gamePlatformList1.List[index].GameId, gamePlatformList2.List[index].GameId);
                Assert.AreEqual(gamePlatformList1.List[index].PlatformId, gamePlatformList2.List[index].PlatformId);
            }
        }

        [TestMethod]
        public void GamePlatformListDictionaryTest()
        {
            var gamePlatformList1 = new GamePlatformList();

            gamePlatformList1.Add(new GamePlatform(
                1,
                2,
                3));

            gamePlatformList1.Add(new GamePlatform(
                2,
                3,
                4));

            gamePlatformList1.Add(new GamePlatform(
                3,
                4,
                5));

            var dictionaryList = GamePlatformList.ToDictionaryList(gamePlatformList1);

            Assert.IsNotNull(dictionaryList);

            var gamePlatformList2 = GamePlatformList.FromDictionaryList(dictionaryList);

            Assert.AreNotSame(gamePlatformList1, gamePlatformList2);
            Assert.AreEqual(gamePlatformList1.List.Count, gamePlatformList2.List.Count);

            for (var index = 0; index < gamePlatformList1.List.Count; index++)
            {
                Assert.AreEqual(gamePlatformList1.List[index].Id, gamePlatformList2.List[index].Id);
                Assert.AreEqual(gamePlatformList1.List[index].GameId, gamePlatformList2.List[index].GameId);
                Assert.AreEqual(gamePlatformList1.List[index].PlatformId, gamePlatformList2.List[index].PlatformId);
            }
        }

        #endregion Test Cases
    }
}
