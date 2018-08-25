
using System;
using System.Linq;
using GameLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sql.Server.Connection;
using Utilities;


namespace TitleiOModelTest.UnitTests
{
    [TestClass]
    public class GameGenreTest
    {
        #region Private Members

        // connecting to server
        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        #endregion Private Members


        #region Test Cases
       
        [TestMethod]
        public void GameGenreTestConstructorTest()
        {
            var gameGenre = new GameGenre();

            Assert.AreEqual(gameGenre.Id, 0);
            Assert.AreEqual(gameGenre.GameId, 0);
            Assert.AreEqual(gameGenre.GenreId, 0);
        }

        [TestMethod]
        public void GameGenrePropertiesTest()
        {
            var gameGenre = new GameGenre();

            Assert.AreEqual(gameGenre.Id, 0);
            gameGenre.Id = 1;
            Assert.AreEqual(gameGenre.Id, 1);

            Assert.AreEqual(gameGenre.GameId, 0);
            gameGenre.GameId = 2;
            Assert.AreEqual(gameGenre.GameId, 2);

            Assert.AreEqual(gameGenre.GenreId, 0);
            gameGenre.GenreId = 3;
            Assert.AreEqual(gameGenre.GenreId, 3);
        }

        [TestMethod]
        public void GameGenreGenerateInsertStatementTest()
        {
            var gameGenre = new GameGenre(
                1,
                2,
                3);

            Assert.AreEqual(gameGenre.GenerateInsertStatement(), "INSERT INTO GameGenre (GameId, GenreId) VALUES (2, 3)");
        }

        [TestMethod]
        public void GameGenreGenerateUpdateStatementTest()
        {
            var gameGenre = new GameGenre(
                1,
                2,
                3);

            Assert.AreEqual(gameGenre.GenerateUpdateStatement(), "UPDATE GameGenre SET GameId = 2, GenreId = 3 WHERE Id = 1");
        }

        [TestMethod]
        public void GameGenreGenerateDeleteStatementTest()
        {
            var gameGenre = new GameGenre(
                1,
                2,
                3);

            Assert.AreEqual(gameGenre.GenerateDeleteStatement(), "DELETE FROM GameGenre WHERE Id = 1");
        }

        [TestMethod]
        public void GameGenreGenerateExistsQueryTest()
        {
            var gameGenre = new GameGenre(
                1,
                2,
                3);

            Assert.AreEqual(gameGenre.GenerateExistsQuery(), "SELECT Id FROM GameGenre WHERE Id = 1");
        }

        [TestMethod]
        public void GameGenreGenerateSelectQueryTest()
        {
            var gameGenre = new GameGenre();

            Assert.AreEqual(gameGenre.GenerateSelectQuery(), "SELECT Id, GameId, GenreId FROM GameGenre");

            gameGenre = new GameGenre(
                1,
                2,
                3);

            Assert.AreEqual(gameGenre.GenerateSelectQuery(), "SELECT Id, GameId, GenreId FROM GameGenre WHERE Id = 1");
        }

        [TestMethod]
        public void GameGenreGeneratePrimaryKeyWhereClauseTest()
        {
            var gameGenre = new GameGenre();

            Assert.AreEqual(gameGenre.GeneratePrimaryKeyWhereClause(), "");

            gameGenre = new GameGenre(
                1,
                2,
                3);

            Assert.AreEqual(gameGenre.GeneratePrimaryKeyWhereClause(), "Id = 1");
        }

        [TestMethod]
        public void GameGenreDatabaseCommandsTest()
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


            // Add a Genres

            var genre = new Genre(0, "Name", "Description");

            insertCommand = genre.GenerateInsertStatment();
            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";
            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            genre.Id = newId;

            var updateGenre = new Genre(0, "Name", "Description");

            insertCommand = updateGenre.GenerateInsertStatment();
            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";
            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            updateGenre.Id = newId;


            // Select All

            var gameGenre = new GameGenre();

            var selectQuery = gameGenre.GenerateSelectQuery();

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

            gameGenre = new GameGenre(
                0,
                game.Id,
                genre.Id);

            insertCommand = gameGenre.GenerateInsertStatement();

            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";
            
            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            gameGenre.Id = newId;


            // Exists

            var existsQuery = gameGenre.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            var existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            var existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            var recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);


            // Select

            selectQuery = gameGenre.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            GameGenre foundGameGenre = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGameGenre = GameGenre.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundGameGenre);

            Assert.AreNotSame(gameGenre, foundGameGenre);

            Assert.AreEqual(gameGenre.Id, foundGameGenre.Id);
            Assert.AreEqual(gameGenre.GameId, foundGameGenre.GameId);
            Assert.AreEqual(gameGenre.GenreId, foundGameGenre.GenreId);


            // Update

            var updateGameGenre = new GameGenre(
                newId,
                updateGame.Id,
                updateGenre.Id);

            var updateCommand = updateGameGenre.GenerateUpdateStatement();

            Assert.IsFalse(string.IsNullOrEmpty(updateCommand));

            errorMessage = "";
            var updateResult = connection.ExecuteCommand(updateCommand, ref errorMessage);

            Assert.AreEqual(updateResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);


            // Exists

            existsQuery = updateGameGenre.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);


            // Select

            selectQuery = updateGameGenre.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundGameGenre = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGameGenre = GameGenre.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundGameGenre);

            Assert.AreNotSame(updateGameGenre, foundGameGenre);

            Assert.AreEqual(updateGameGenre.Id, foundGameGenre.Id);
            Assert.AreEqual(updateGameGenre.GameId, foundGameGenre.GameId);
            Assert.AreEqual(updateGameGenre.GenreId, foundGameGenre.GenreId);


            // Delete

            var deleteCommand = gameGenre.GenerateDeleteStatement();

            var deleteGenre = genre.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            var deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);


            // Exists

            existsQuery = gameGenre.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.IsFalse(recordExists);


            // Select

            selectQuery = gameGenre.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundGameGenre = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGameGenre = GameGenre.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNull(foundGameGenre);


            // Delete the genres

            deleteCommand = genre.GenerateDeleteStatement();

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
        public void GameGenreBinaryCloneTest()
        {
            var gameGenre1 = new GameGenre(
                1,
                2,
                3);

            var gameGenre2 = CloneUtility.BinaryClone(gameGenre1);

            Assert.AreNotSame(gameGenre1, gameGenre2);

            Assert.AreEqual(gameGenre1.Id, gameGenre2.Id);
            Assert.AreEqual(gameGenre1.GameId, gameGenre2.GameId);
            Assert.AreEqual(gameGenre1.GenreId, gameGenre2.GenreId);
        }

        [TestMethod]
        public void GameGenreXmlCloneTest()
        {
            var gameGenre1 = new GameGenre(
                1,
                2,
                3);

            var gameGenre2 = CloneUtility.XmlClone(gameGenre1, null);

            Assert.AreNotSame(gameGenre1, gameGenre2);

            Assert.AreEqual(gameGenre1.Id, gameGenre2.Id);
            Assert.AreEqual(gameGenre1.GameId, gameGenre2.GameId);
            Assert.AreEqual(gameGenre1.GenreId, gameGenre2.GenreId);
        }

        [TestMethod]
        public void GameGenreJsonTest()
        {
            var gameGenre1 = new GameGenre(
                1,
                2,
                3);

            var jsonText = CloneUtility.ToJson(gameGenre1);

            Assert.IsFalse(string.IsNullOrEmpty(jsonText));

            var gameGenre2 = CloneUtility.FromJson<GameGenre>(jsonText);

            Assert.AreNotSame(gameGenre1, gameGenre2);

            Assert.AreEqual(gameGenre1.Id, gameGenre2.Id);
            Assert.AreEqual(gameGenre1.GameId, gameGenre2.GameId);
            Assert.AreEqual(gameGenre1.GenreId, gameGenre2.GenreId);
        }

        [TestMethod]
        public void GameGenreDictionaryTest()
        {
            var gameGenre1 = new GameGenre(
                1,
                2,
                3);

            var dictionary = GameGenre.ToDictionary(gameGenre1);

            Assert.IsNotNull(dictionary);

            var gameGenre2 = GameGenre.FromDictionary(dictionary);

            Assert.AreNotSame(gameGenre1, gameGenre2);

            Assert.AreEqual(gameGenre1.Id, gameGenre2.Id);
            Assert.AreEqual(gameGenre1.GameId, gameGenre2.GameId);
            Assert.AreEqual(gameGenre1.GenreId, gameGenre2.GenreId);
        }

        [TestMethod]
        public void GameGenreListTestConstructorTest()
        {
            var gameGenreList = new GameGenreList();

            Assert.IsNotNull(gameGenreList);
            Assert.AreEqual(gameGenreList.List.Count, 0);
        }

        [TestMethod]
        public void GameGenreListGetByIdTest()
        {
            var gameGenreList = new GameGenreList();

            gameGenreList.Add(new GameGenre(
                1,
                2,
                3));

            gameGenreList.Add(new GameGenre(
                2,
                3,
                4));

            gameGenreList.Add(new GameGenre(
                3,
                4,
                5));

            var gameGenre = gameGenreList.GetById(0);
            Assert.AreEqual(gameGenre, null);

            gameGenre = gameGenreList.GetById(-1);
            Assert.AreEqual(gameGenre, null);

            gameGenre = gameGenreList.GetById(1);
            Assert.AreEqual(gameGenre.GameId, 2);

            gameGenre = gameGenreList.GetById(2);
            Assert.AreEqual(gameGenre.GameId, 3);

            gameGenre = gameGenreList.GetById(3);
            Assert.AreEqual(gameGenre.GameId, 4);
        }

        [TestMethod]
        public void GameGenreListExistsTest()
        {
            var gameGenreList = new GameGenreList();

            gameGenreList.Add(new GameGenre(
                1,
                2,
                3));

            gameGenreList.Add(new GameGenre(
                2,
                3,
                4));

            gameGenreList.Add(new GameGenre(
                3,
                4,
                5));

            Assert.IsFalse(gameGenreList.Exists(0));
            Assert.IsFalse(gameGenreList.Exists(-1));
            Assert.AreEqual(gameGenreList.Exists(1), true);
            Assert.AreEqual(gameGenreList.Exists(2), true);
            Assert.AreEqual(gameGenreList.Exists(3), true);
        }

        [TestMethod]
        public void GameGenreListAddandRemoveTest()
        {
            var gameGenreList = new GameGenreList();

            Assert.AreEqual(gameGenreList.List.Count, 0);

            gameGenreList.Add(new GameGenre(
                1,
                2,
                3));

            Assert.AreEqual(gameGenreList.List.Count, 1);

            gameGenreList.Add(new GameGenre(
                2,
                3,
                4));

            Assert.AreEqual(gameGenreList.List.Count, 2);

            gameGenreList.Add(new GameGenre(
                3,
                4,
                5));

            Assert.AreEqual(gameGenreList.List.Count, 3);

            gameGenreList.Remove(1);

            Assert.AreEqual(gameGenreList.List.Count, 2);

            gameGenreList.Remove(3);

            Assert.AreEqual(gameGenreList.List.Count, 1);

            gameGenreList.Remove(2);

            Assert.AreEqual(gameGenreList.List.Count, 0);
        }

        [TestMethod]
        public void GameGenreListJsonTest()
        {
            var gameGenreList1 = new GameGenreList();

            gameGenreList1.Add(new GameGenre(
                1,
                2,
                3));

            gameGenreList1.Add(new GameGenre(
                2,
                3,
                4));

            gameGenreList1.Add(new GameGenre(
                3,
                4,
                5));

            var jsonText = CloneUtility.ToJson(gameGenreList1);

            Assert.IsFalse(string.IsNullOrEmpty(jsonText));

            var gameGenreList2 = CloneUtility.FromJson<GameGenreList>(jsonText);

            Assert.AreNotSame(gameGenreList1, gameGenreList2);
            Assert.AreEqual(gameGenreList1.List.Count, gameGenreList2.List.Count);

            for (var index = 0; index < gameGenreList1.List.Count; index++)
            {
                Assert.AreEqual(gameGenreList1.List[index].Id, gameGenreList2.List[index].Id);
                Assert.AreEqual(gameGenreList1.List[index].GameId, gameGenreList2.List[index].GameId);
                Assert.AreEqual(gameGenreList1.List[index].GenreId, gameGenreList2.List[index].GenreId);
            }
        }

        [TestMethod]
        public void GameGenreListBinaryCloneTest()
        {
            var gameGenreList1 = new GameGenreList();

            gameGenreList1.Add(new GameGenre(
                1,
                2,
                3));

            gameGenreList1.Add(new GameGenre(
                2,
                3,
                4));

            gameGenreList1.Add(new GameGenre(
                3,
                4,
                5));

            var gameGenreList2 = CloneUtility.BinaryClone(gameGenreList1);

            Assert.AreNotSame(gameGenreList1, gameGenreList2);
            Assert.AreEqual(gameGenreList1.List.Count, gameGenreList2.List.Count);

            for (var index = 0; index < gameGenreList1.List.Count; index++)
            {
                Assert.AreEqual(gameGenreList1.List[index].Id, gameGenreList2.List[index].Id);
                Assert.AreEqual(gameGenreList1.List[index].GameId, gameGenreList2.List[index].GameId);
                Assert.AreEqual(gameGenreList1.List[index].GenreId, gameGenreList2.List[index].GenreId);
            }
        }

        [TestMethod]
        public void GameGenreListXmlCloneTest()
        {
            var gameGenreList1 = new GameGenreList();

            gameGenreList1.Add(new GameGenre(
                1,
                2,
                3));

            gameGenreList1.Add(new GameGenre(
                2,
                3,
                4));

            gameGenreList1.Add(new GameGenre(
                3,
                4,
                5));

            var gameGenreList2 = CloneUtility.XmlClone(gameGenreList1, null);

            Assert.AreNotSame(gameGenreList1, gameGenreList2);
            Assert.AreEqual(gameGenreList1.List.Count, gameGenreList2.List.Count);

            for (var index = 0; index < gameGenreList1.List.Count; index++)
            {
                Assert.AreEqual(gameGenreList1.List[index].Id, gameGenreList2.List[index].Id);
                Assert.AreEqual(gameGenreList1.List[index].GameId, gameGenreList2.List[index].GameId);
                Assert.AreEqual(gameGenreList1.List[index].GenreId, gameGenreList2.List[index].GenreId);
            }
        }

        [TestMethod]
        public void GameGenreListDictionaryTest()
        {
            var gameGenreList1 = new GameGenreList();

            gameGenreList1.Add(new GameGenre(
                1,
                2,
                3));

            gameGenreList1.Add(new GameGenre(
                2,
                3,
                4));

            gameGenreList1.Add(new GameGenre(
                3,
                4,
                5));

            var dictionaryList = GameGenreList.ToDictionaryList(gameGenreList1);

            Assert.IsNotNull(dictionaryList);

            var gameGenreList2 = GameGenreList.FromDictionaryList(dictionaryList);

            Assert.AreNotSame(gameGenreList1, gameGenreList2);
            Assert.AreEqual(gameGenreList1.List.Count, gameGenreList2.List.Count);

            for (var index = 0; index < gameGenreList1.List.Count; index++)
            {
                Assert.AreEqual(gameGenreList1.List[index].Id, gameGenreList2.List[index].Id);
                Assert.AreEqual(gameGenreList1.List[index].GameId, gameGenreList2.List[index].GameId);
                Assert.AreEqual(gameGenreList1.List[index].GenreId, gameGenreList2.List[index].GenreId);
            }
        }

        #endregion Test Cases
    }
}
