
using System;
using System.Linq;
using GameLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sql.Server.Connection;
using Utilities;

namespace TitleiOModelTest.UnitTests
{
    [TestClass]
    public class GameTest
    {
        #region Private Members
        
        // connecting to server
        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        #endregion Private Members


        #region Test Cases

        [TestMethod]
        public void GameTestConstructorTest()
        {
            var game = new Game();

            Assert.AreEqual(game.Id, 0);
            Assert.AreEqual(game.Name, "");
            Assert.AreEqual(game.Description, "");
        }

        [TestMethod]
        public void GamePropertiesTest()
        {
            var game = new Game();

            Assert.AreEqual(game.Id, 0);
            game.Id = 1;
            Assert.AreEqual(game.Id, 1);
            
            Assert.AreEqual(game.Name, "");
            game.Name = "Name";
            Assert.AreEqual(game.Name, "Name");
            
            Assert.AreEqual(game.Description, "");
            game.Description = "Description";
            Assert.AreEqual(game.Description, "Description");
        }

        [TestMethod]
        public void GameGenerateInsertStatementTest()
        {
            var game = new Game(
                1,
                "Name",
                "Description");

            Assert.AreEqual(game.GenerateInsertStatement(), "INSERT INTO Game (Name, Description) VALUES ('Name', 'Description')");
        }

        [TestMethod]
        public void GameGenerateUpdateStatementTest()
        {
            var game = new Game(
                1,
                "Name",
                "Description");

            Assert.AreEqual(game.GenerateUpdateStatement(), "UPDATE Game SET Name = 'Name', Description = 'Description' WHERE Id = 1");
        }

        [TestMethod]
        public void GameGenerateDeleteStatementTest()
        {
            var game = new Game(
                1,
                "Name",
                "Description");

            Assert.AreEqual(game.GenerateDeleteStatement(), "DELETE FROM Game WHERE Id = 1");
        }

        [TestMethod]
        public void GameGenerateExistsQueryTest()
        {
            var game = new Game(
                1,
                "Name",
                "Description");

            Assert.AreEqual(game.GenerateExistsQuery(), "SELECT Id FROM Game WHERE Id = 1");
        }

        [TestMethod]
        public void GameGenerateSelectQueryTest()
        {
            var game = new Game();

            Assert.AreEqual(game.GenerateSelectQuery(), "SELECT Id, Name, Description FROM Game");

            game = new Game(
                0,
                "Name",
                "Description");

            Assert.AreEqual(game.GenerateSelectQuery(), "SELECT Id, Name, Description FROM Game WHERE Name = 'Name'");

            game = new Game(
                1,
                "Name",
                "Description");

            Assert.AreEqual(game.GenerateSelectQuery(), "SELECT Id, Name, Description FROM Game WHERE Id = 1");
        }

        [TestMethod]
        public void GameGeneratePrimaryKeyWhereClauseTest()
        {
            var game = new Game();

            Assert.AreEqual(game.GeneratePrimaryKeyWhereClause(), "");

            game = new Game(
                1,
                "Name",
                "Description");

            Assert.AreEqual(game.GeneratePrimaryKeyWhereClause(), "Id = 1");
        }

        [TestMethod]
        public void GameDatabaseCommandsTest()
        {
            var connection = new SqlServerConnection();

            Assert.AreEqual(connection.IsConnected, false);

            var result = connection.Connect(ConnectionString, 0);

            Assert.AreEqual(result, true);


            // Select All

            var game = new Game();

            var selectQuery = game.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            var errorMessage = "";
            var selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            var selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            if (selectResultList.Count > 0)
            {
                Assert.IsTrue(selectResultList.Count > 0);
            }


            // Insert

            game = new Game(
                0,
                "Name",
                "Description");

            var insertCommand = game.GenerateInsertStatement();

            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";
            var insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out var newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            game.Id = newId;

                
            // Exists

            var existsQuery = game.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            var existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            var existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            var recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);


            // Select

            selectQuery = game.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            Game foundGame = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGame = Game.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundGame);

            Assert.AreNotSame(game, foundGame);

            Assert.AreEqual(game.Id, foundGame.Id);
            Assert.AreEqual(game.Name, foundGame.Name);
            Assert.AreEqual(game.Description, foundGame.Description);


            // Update

            var updateGame = new Game(
                newId,
                "Name Edited",
                "Description Edited");

            var updateCommand = updateGame.GenerateUpdateStatement();

            Assert.IsFalse(string.IsNullOrEmpty(updateCommand));

            errorMessage = "";
            var updateResult = connection.ExecuteCommand(updateCommand, ref errorMessage);

            Assert.AreEqual(updateResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

                
            // Exists

            existsQuery = updateGame.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);


            // Select

            selectQuery = updateGame.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundGame = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGame = Game.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundGame);

            Assert.AreNotSame(updateGame, foundGame);

            Assert.AreEqual(updateGame.Id, foundGame.Id);
            Assert.AreEqual(updateGame.Name, foundGame.Name);
            Assert.AreEqual(updateGame.Description, foundGame.Description);



            // Delete

            var deleteCommand = game.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            var deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

                
            // Exists

            existsQuery = game.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.IsFalse(recordExists);


            // Select

            selectQuery = game.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundGame = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGame = Game.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNull(foundGame);
        }

        [TestMethod]
        public void GameBinaryCloneTest()
        {
            var game1 = new Game(
                1,
                "Name",
                "Description");

            var game2 = CloneUtility.BinaryClone(game1);

            Assert.AreNotSame(game1, game2);

            Assert.AreEqual(game1.Id, game2.Id);
            Assert.AreEqual(game1.Name, game2.Name);
            Assert.AreEqual(game1.Description, game2.Description);
        }

        [TestMethod]
        public void GameXmlCloneTest()
        {
            var game1 = new Game(
                1,
                "Name",
                "Description");

            var game2 = CloneUtility.XmlClone(game1, null);

            Assert.AreNotSame(game1, game2);

            Assert.AreEqual(game1.Id, game2.Id);
            Assert.AreEqual(game1.Name, game2.Name);
            Assert.AreEqual(game1.Description, game2.Description);
        }

        [TestMethod]
        public void GameJsonTest()
        {
            var game1 = new Game(
                1,
                "Name",
                "Description");

            var jsonText = CloneUtility.ToJson(game1);

            Assert.IsFalse(string.IsNullOrEmpty(jsonText));

            var game2 = CloneUtility.FromJson<Game>(jsonText);

            Assert.AreNotSame(game1, game2);

            Assert.AreEqual(game1.Id, game2.Id);
            Assert.AreEqual(game1.Name, game2.Name);
            Assert.AreEqual(game1.Description, game2.Description);
        }

        [TestMethod]
        public void GameDictionaryTest()
        {
            var game1 = new Game(
                1,
                "Name",
                "Description");

            var dictionary = Game.ToDictionary(game1);

            Assert.IsNotNull(dictionary);

            var game2 = Game.FromDictionary(dictionary);

            Assert.AreNotSame(game1, game2);

            Assert.AreEqual(game1.Id, game2.Id);
            Assert.AreEqual(game1.Name, game2.Name);
            Assert.AreEqual(game1.Description, game2.Description);
        }

        [TestMethod]
        public void GameListTestConstructorTest()
        {
            var gameList = new GameList();

            Assert.IsNotNull(gameList);
            Assert.AreEqual(gameList.List.Count, 0);
        }

        [TestMethod]
        public void GameListGetByIdTest()
        {
            var gameList = new GameList();

            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();

            gameList.Add(new Game(
                1,
                "Name1",
                "Description1"));

            gameList.Add(new Game(
                2,
                "Name2",
                "Description2"));

            gameList.Add(new Game(
                3,
                "Name3",
                "Description3"));

            var game = gameList.GetById(0);
            Assert.AreEqual(game, null);

            game = gameList.GetById(-1);
            Assert.AreEqual(game, null);

            game = gameList.GetById(1);
            Assert.AreEqual(game.Name, "Name1");

            game = gameList.GetById(2);
            Assert.AreEqual(game.Name, "Name2");

            game = gameList.GetById(3);
            Assert.AreEqual(game.Name, "Name3");
        }

        [TestMethod]
        public void GameListExistsTest()
        {
            var gameList = new GameList();

            gameList.Add(new Game(
                1,
                "Name1",
                "Description1"));

            gameList.Add(new Game(
                2,
                "Name2",
                "Description2"));

            gameList.Add(new Game(
                3,
                "Name3",
                "Description3"));

            Assert.IsFalse(gameList.Exists(0));
            Assert.IsFalse(gameList.Exists(-1));
            Assert.AreEqual(gameList.Exists(1), true);
            Assert.AreEqual(gameList.Exists(2), true);
            Assert.AreEqual(gameList.Exists(3), true);
        }

        [TestMethod]
        public void GameListAddandRemoveTest()
        {
            var gameList = new GameList();

            Assert.AreEqual(gameList.List.Count, 0);

            gameList.Add(new Game(
                1,
                "Name1",
                "Description1"));

            Assert.AreEqual(gameList.List.Count, 1);

            gameList.Add(new Game(
                2,
                "Name2",
                "Description2"));

            Assert.AreEqual(gameList.List.Count, 2);

            gameList.Add(new Game(
                3,
                "Name3",
                "Description3"));

            Assert.AreEqual(gameList.List.Count, 3);

            gameList.Remove(1);

            Assert.AreEqual(gameList.List.Count, 2);

            gameList.Remove(3);

            Assert.AreEqual(gameList.List.Count, 1);

            gameList.Remove(2);

            Assert.AreEqual(gameList.List.Count, 0);
        }

        [TestMethod]
        public void GameListJsonTest()
        {
            var gameList1 = new GameList();

            gameList1.Add(new Game(
                1,
                "Name1",
                "Description1"));

            gameList1.Add(new Game(
                2,
                "Name2",
                "Description2"));

            gameList1.Add(new Game(
                3,
                "Name3",
                "Description3"));

            var jsonText = CloneUtility.ToJson(gameList1);

            Assert.IsFalse(string.IsNullOrEmpty(jsonText));

            var gameList2 = CloneUtility.FromJson<GameList>(jsonText);

            Assert.AreNotSame(gameList1, gameList2);
            Assert.AreEqual(gameList1.List.Count, gameList2.List.Count);

            for (var index = 0; index < gameList1.List.Count; index++)
            {
                Assert.AreEqual(gameList1.List[index].Id, gameList2.List[index].Id);
                Assert.AreEqual(gameList1.List[index].Name, gameList2.List[index].Name);
                Assert.AreEqual(gameList1.List[index].Description, gameList2.List[index].Description);
            }
        }

        [TestMethod]
        public void GameListBinaryCloneTest()
        {
            var gameList1 = new GameList();

            gameList1.Add(new Game(
                1,
                "Name1",
                "Description1"));

            gameList1.Add(new Game(
                2,
                "Name2",
                "Description2"));

            gameList1.Add(new Game(
                3,
                "Name3",
                "Description3"));

            var gameList2 = CloneUtility.BinaryClone(gameList1);

            Assert.AreNotSame(gameList1, gameList2);
            Assert.AreEqual(gameList1.List.Count, gameList2.List.Count);

            for (var index = 0; index < gameList1.List.Count; index++)
            {
                Assert.AreEqual(gameList1.List[index].Id, gameList2.List[index].Id);
                Assert.AreEqual(gameList1.List[index].Name, gameList2.List[index].Name);
                Assert.AreEqual(gameList1.List[index].Description, gameList2.List[index].Description);
            }
        }

        [TestMethod]
        public void GameListXmlCloneTest()
        {
            var gameList1 = new GameList();

            gameList1.Add(new Game(
                1,
                "Name1",
                "Description1"));

            gameList1.Add(new Game(
                2,
                "Name2",
                "Description2"));

            gameList1.Add(new Game(
                3,
                "Name3",
                "Description3"));

            var gameList2 = CloneUtility.XmlClone(gameList1, null);

            Assert.AreNotSame(gameList1, gameList2);
            Assert.AreEqual(gameList1.List.Count, gameList2.List.Count);

            for (var index = 0; index < gameList1.List.Count; index++)
            {
                Assert.AreEqual(gameList1.List[index].Id, gameList2.List[index].Id);
                Assert.AreEqual(gameList1.List[index].Name, gameList2.List[index].Name);
                Assert.AreEqual(gameList1.List[index].Description, gameList2.List[index].Description);
            }
        }

        [TestMethod]
        public void GameListDictionaryTest()
        {
            var gameList1 = new GameList();

            gameList1.Add(new Game(
                1,
                "Name1",
                "Description1"));

            gameList1.Add(new Game(
                2,
                "Name2",
                "Description2"));

            gameList1.Add(new Game(
                3,
                "Name3",
                "Description3"));

            var dictionaryList = GameList.ToDictionaryList(gameList1);

            Assert.IsNotNull(dictionaryList);

            var gameList2 = GameList.FromDictionaryList(dictionaryList);

            Assert.AreNotSame(gameList1, gameList2);
            Assert.AreEqual(gameList1.List.Count, gameList2.List.Count);

            for (var index = 0; index < gameList1.List.Count; index++)
            {
                Assert.AreEqual(gameList1.List[index].Id, gameList2.List[index].Id);
                Assert.AreEqual(gameList1.List[index].Name, gameList2.List[index].Name);
                Assert.AreEqual(gameList1.List[index].Description, gameList2.List[index].Description);
            }
        }

        #endregion Test Cases
    }
}
