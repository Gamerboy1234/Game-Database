using System;
using System.Linq;
using GameImageLibrary.Model;
using GameLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sql.Server.Connection;
using Utilities;

namespace GameLibrary.Model.Test
{
    [TestClass]
    public class GameImageTest
    {
        #region Private Members

        // connecting to server
        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        #endregion Private Members


        #region Test Cases

        [TestMethod]
        public void GameImageTestConstructorTest()
        {
            var gameImage = new GameImage();

            Assert.AreEqual(gameImage.Id, 0);
            Assert.AreEqual(gameImage.GameId, 0);
            
        }

        [TestMethod]
        public void GameImagePropertiesTest()
        {
            var gameImage = new GameImage();

            Assert.AreEqual(gameImage.Id, 0);
            gameImage.Id = 1;
            Assert.AreEqual(gameImage.Id, 1);

            Assert.AreEqual(gameImage.GameId, 0);
            gameImage.GameId = 2;
            Assert.AreEqual(gameImage.GameId, 2);

            
        }

        [TestMethod]
        public void GameImageGenerateInsertStatementTest()
        {
            var gameImage = new GameImage(1, 1);
                

            Assert.AreEqual(gameImage.GenerateInsertStatment(), "INSERT INTO GameImage (GameId) VALUES (1)");
        }

        [TestMethod]
        public void GameImageGenerateUpdateStatementTest()
        {
            var gameImage = new GameImage(1, 1);

            Assert.AreEqual(gameImage.GenerateUpdateStatement(), "UPDATE GameImage SET GameId = 1 WHERE Id = 1");
        }

        [TestMethod]
        public void GameImageGenerateDeleteStatementTest()
        {
            var gameImage = new GameImage(1, 1);

            Assert.AreEqual(gameImage.GenerateDeleteStatement(), "DELETE FROM GameImage WHERE Id = 1");
        }

        [TestMethod]
        public void GameImageGenerateExistsQueryTest()
        {
            var gameImage = new GameImage(1, 1);

            Assert.AreEqual(gameImage.GenerateExistsQuery(), "SELECT Id FROM GameImage WHERE Id = 1");
        }

        [TestMethod]
        public void GameImageGenerateSelectQueryTest()
        {
            var gameImage = new GameImage();

            Assert.AreEqual(gameImage.GenerateSelectQuery(), "SELECT Id, GameId FROM GameImage");

            gameImage = new GameImage(1, 1);

            Assert.AreEqual(gameImage.GenerateSelectQuery(), "SELECT Id, GameId FROM GameImage WHERE Id = 1");
        }

        [TestMethod]
        public void GameImageGeneratePrimaryKeyWhereClauseTest()
        {
            var gameImage = new GameImage();

            Assert.AreEqual(gameImage.GeneratePrimaryKeyWhereClause(), "");

            gameImage = new GameImage(1, 1);

            Assert.AreEqual(gameImage.GeneratePrimaryKeyWhereClause(), "Id = 1");
        }

        [TestMethod]
        public void GameImageDatabaseCommandsTest()
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


            // Add a GameImages

            var gameImage = new GameImage(0,0);

            insertCommand = gameImage.GenerateInsertStatment();
            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";
            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            gameImage.Id = newId;

            var updateGameImage = new GameImage(0, 0);

            insertCommand = updateGameImage.GenerateInsertStatment();
            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";
            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            updateGameImage.Id = newId;


            // Select All

            gameImage = new GameImage();

            var selectQuery = gameImage.GenerateSelectQuery();

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

            gameImage = new GameImage(0, game.Id);


            insertCommand = gameImage.GenerateInsertStatment();

            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";

            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            gameImage.Id = newId;


            // Exists

            var existsQuery = gameImage.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            var existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            var existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            var recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);


            // Select

            selectQuery = gameImage.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            GameImage foundGameImage = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGameImage = GameImage.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundGameImage);

            Assert.AreNotSame(gameImage, foundGameImage);

            Assert.AreEqual(gameImage.Id, foundGameImage.Id);
            Assert.AreEqual(gameImage.GameId, foundGameImage.GameId);
            


            // Update

                updateGameImage = new GameImage(
                newId,
                updateGame.Id
                );

            var updateCommand = updateGameImage.GenerateUpdateStatement();

            Assert.IsFalse(string.IsNullOrEmpty(updateCommand));

            errorMessage = "";
            var updateResult = connection.ExecuteCommand(updateCommand, ref errorMessage);

            Assert.AreEqual(updateResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);


            // Exists

            existsQuery = updateGameImage.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);


            // Select

            selectQuery = updateGameImage.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundGameImage = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGameImage = GameImage.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundGameImage);

            Assert.AreNotSame(updateGameImage, foundGameImage);

            Assert.AreEqual(updateGameImage.Id, foundGameImage.Id);
            Assert.AreEqual(updateGameImage.GameId, foundGameImage.GameId);
           


            // Delete

            var deleteCommand = gameImage.GenerateDeleteStatement();

            var deleteGameImage = gameImage.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            var deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);


            // Exists

            existsQuery = gameImage.GenerateExistsQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.IsFalse(recordExists);


            // Select

            selectQuery = gameImage.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundGameImage = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundGameImage = GameImage.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNull(foundGameImage);


            // Delete the gameImages

            deleteCommand = gameImage.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            deleteCommand = updateGameImage.GenerateDeleteStatement();

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
        public void GameImageBinaryCloneTest()
        {
            var gameImage1 = new GameImage(
                1,
                2
                );

            var gameImage2 = CloneUtility.BinaryClone(gameImage1);

            Assert.AreNotSame(gameImage1, gameImage2);

            Assert.AreEqual(gameImage1.Id, gameImage2.Id);
            Assert.AreEqual(gameImage1.GameId, gameImage2.GameId);
            
        }

        [TestMethod]
        public void GameImageXmlCloneTest()
        {
            var gameImage1 = new GameImage(
                1,
                2
                );

            var gameImage2 = CloneUtility.XmlClone(gameImage1, null);

            Assert.AreNotSame(gameImage1, gameImage2);

            Assert.AreEqual(gameImage1.Id, gameImage2.Id);
            Assert.AreEqual(gameImage1.GameId, gameImage2.GameId);
           
        }

        [TestMethod]
        public void GameImageJsonTest()
        {
            var gameImage1 = new GameImage(
                1,
                2
                );

            var jsonText = CloneUtility.ToJson(gameImage1);

            Assert.IsFalse(string.IsNullOrEmpty(jsonText));

            var gameImage2 = CloneUtility.FromJson<GameImage>(jsonText);

            Assert.AreNotSame(gameImage1, gameImage2);

            Assert.AreEqual(gameImage1.Id, gameImage2.Id);
            Assert.AreEqual(gameImage1.GameId, gameImage2.GameId);
            
        }

        [TestMethod]
        public void GameImageDictionaryTest()
        {
            var gameImage1 = new GameImage(
                1,
                2
                );

            var dictionary = GameImage.ToDictionary(gameImage1);

            Assert.IsNotNull(dictionary);

            var gameImage2 = GameImage.FromDictionary(dictionary);

            Assert.AreNotSame(gameImage1, gameImage2);

            Assert.AreEqual(gameImage1.Id, gameImage2.Id);
            Assert.AreEqual(gameImage1.GameId, gameImage2.GameId);
            
        }

        [TestMethod]
        public void GameImageListTestConstructorTest()
        {
            var gameImageList = new GameImageList();

            Assert.IsNotNull(gameImageList);
            Assert.AreEqual(gameImageList.List.Count, 0);
        }

        [TestMethod]
        public void GameImageListGetByIdTest()
        {
            var gameImageList = new GameImageList();

            gameImageList.Add(new GameImage(1, 2));

            gameImageList.Add(new GameImage(3, 4));

            gameImageList.Add(new GameImage(5, 6));

            var gameImage = gameImageList.GetById(0);
            Assert.AreEqual(gameImage, null);

            gameImage = gameImageList.GetById(-1);
            Assert.AreEqual(gameImage, null);

            gameImage = gameImageList.GetById(1);
            Assert.AreEqual(gameImage.GameId, 2);

            gameImage = gameImageList.GetById(3);
            Assert.AreEqual(gameImage.GameId, 4);

            gameImage = gameImageList.GetById(5);
            Assert.AreEqual(gameImage.GameId, 6);
        }

        [TestMethod]
        public void GameImageListExistsTest()
        {
            var gameImageList = new GameImageList();

            gameImageList.Add(new GameImage(1, 2));

            gameImageList.Add(new GameImage(2, 2));

            gameImageList.Add(new GameImage(3, 2));

            Assert.IsFalse(gameImageList.Exists(0));
            Assert.IsFalse(gameImageList.Exists(-1));
            Assert.AreEqual(gameImageList.Exists(1), true);
            Assert.AreEqual(gameImageList.Exists(2), true);
            Assert.AreEqual(gameImageList.Exists(3), true);
        }

        [TestMethod]
        public void GameImageListAddandRemoveTest()
        {
            var gameImageList = new GameImageList();

            Assert.AreEqual(gameImageList.List.Count, 0);

            gameImageList.Add(new GameImage(1, 2));

            Assert.AreEqual(gameImageList.List.Count, 1);

            gameImageList.Add(new GameImage(2, 3));

            Assert.AreEqual(gameImageList.List.Count, 2);

            gameImageList.Add(new GameImage(4, 5));

            Assert.AreEqual(gameImageList.List.Count, 3);

            gameImageList.Remove(1);

            Assert.AreEqual(gameImageList.List.Count, 2);

            gameImageList.Remove(2);

            Assert.AreEqual(gameImageList.List.Count, 1);

            gameImageList.Remove(3);

            Assert.AreEqual(gameImageList.List.Count, 1);
        }

        [TestMethod]
        public void GameImageListJsonTest()
        {
            var gameImageList1 = new GameImageList();

            gameImageList1.Add(new GameImage(1, 2));

            gameImageList1.Add(new GameImage(3, 4));

            gameImageList1.Add(new GameImage(5, 6));

            var jsonText = CloneUtility.ToJson(gameImageList1);

            Assert.IsFalse(string.IsNullOrEmpty(jsonText));

            var gameImageList2 = CloneUtility.FromJson<GameImageList>(jsonText);

            Assert.AreNotSame(gameImageList1, gameImageList2);
            Assert.AreEqual(gameImageList1.List.Count, gameImageList2.List.Count);

            for (var index = 0; index < gameImageList1.List.Count; index++)
            {
                Assert.AreEqual(gameImageList1.List[index].Id, gameImageList2.List[index].Id);
                Assert.AreEqual(gameImageList1.List[index].GameId, gameImageList2.List[index].GameId);
               
            }
        }

        [TestMethod]
        public void GameImageListBinaryCloneTest()
        {
            var gameImageList1 = new GameImageList();

            gameImageList1.Add(new GameImage(1, 2));

            gameImageList1.Add(new GameImage(3, 4));

            gameImageList1.Add(new GameImage(5, 6));

            var gameImageList2 = CloneUtility.BinaryClone(gameImageList1);

            Assert.AreNotSame(gameImageList1, gameImageList2);
            Assert.AreEqual(gameImageList1.List.Count, gameImageList2.List.Count);

            for (var index = 0; index < gameImageList1.List.Count; index++)
            {
                Assert.AreEqual(gameImageList1.List[index].Id, gameImageList2.List[index].Id);
                Assert.AreEqual(gameImageList1.List[index].GameId, gameImageList2.List[index].GameId);
               
            }
        }

        [TestMethod]
        public void GameImageListXmlCloneTest()
        {
            var gameImageList1 = new GameImageList();

            gameImageList1.Add(new GameImage(1, 2));

            gameImageList1.Add(new GameImage(3, 4));

            gameImageList1.Add(new GameImage(5, 6));

            var gameImageList2 = CloneUtility.XmlClone(gameImageList1, null);

            Assert.AreNotSame(gameImageList1, gameImageList2);
            Assert.AreEqual(gameImageList1.List.Count, gameImageList2.List.Count);

            for (var index = 0; index < gameImageList1.List.Count; index++)
            {
                Assert.AreEqual(gameImageList1.List[index].Id, gameImageList2.List[index].Id);
                Assert.AreEqual(gameImageList1.List[index].GameId, gameImageList2.List[index].GameId);
                
            }
        }

        [TestMethod]
        public void GameImageListDictionaryTest()
        {
            var gameImageList1 = new GameImageList();

            gameImageList1.Add(new GameImage(1, 2));

            gameImageList1.Add(new GameImage(3, 4));

            gameImageList1.Add(new GameImage(5, 6));

            var dictionaryList = GameImageList.ToDictionaryList(gameImageList1);

            Assert.IsNotNull(dictionaryList);

            var gameImageList2 = GameImageList.FromDictionaryList(dictionaryList);

            Assert.AreNotSame(gameImageList1, gameImageList2);
            Assert.AreEqual(gameImageList1.List.Count, gameImageList2.List.Count);

            for (var index = 0; index < gameImageList1.List.Count; index++)
            {
                Assert.AreEqual(gameImageList1.List[index].Id, gameImageList2.List[index].Id);
                Assert.AreEqual(gameImageList1.List[index].GameId, gameImageList2.List[index].GameId);
                
            }
        }

        #endregion Test Cases
    }
}
