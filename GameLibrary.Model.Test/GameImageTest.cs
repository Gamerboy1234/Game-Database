using System.IO;
using System.Linq;
using GameImageLibrary.Model;
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
        private const string TestImageFilePath = @"D:\Programing Projects\GitHub\Game-Database\TestData\Elf.jpg";
        private const string TestUpdateImageFilePath = @"D:\Programing Projects\GitHub\Game-Database\TestData\Necromancer.jpg";

        private byte[] _imageData;
        private byte[] _updateImageData;

        #endregion Private Members


        #region Setup and Teardown

        [TestInitialize]
        public void GameImageTestSetup()
        {
            if (File.Exists(TestImageFilePath))
            {
                _imageData = File.ReadAllBytes(TestImageFilePath);
            }

            if (File.Exists(TestUpdateImageFilePath))
            {
                _updateImageData = File.ReadAllBytes(TestUpdateImageFilePath);
            }
        }

        [TestCleanup]
        public void GameImageTestCleanup()
        {
            // Nothing needed here.
        }

        #endregion Setup and Teardown


        #region Test Cases

        [TestMethod]
        public void GameImageTestConstructorTest()
        {
            var gameImage = new GameImage();

            Assert.AreEqual(gameImage.Id, 0);
            Assert.AreEqual(gameImage.GameId, 0);
            Assert.IsNull(gameImage.Image);
        }

        [TestMethod]
        public void GameImagePropertiesTest()
        {
            _imageData = File.ReadAllBytes(TestImageFilePath);

            var gameImage = new GameImage();

            Assert.AreEqual(gameImage.Id, 0);
            gameImage.Id = 1;
            Assert.AreEqual(gameImage.Id, 1);

            Assert.AreEqual(gameImage.GameId, 0);
            gameImage.GameId = 2;
            Assert.AreEqual(gameImage.GameId, 2);

            Assert.IsNull(gameImage.Image);
            gameImage.Image = _imageData;
            Assert.IsTrue(gameImage.Image.SequenceEqual(_imageData));
        }

        [TestMethod]
        public void GameImageGenerateInsertStatementTest()
        {
            var gameImage = new GameImage(1, 1, _imageData);
                
            Assert.AreEqual(gameImage.GenerateInsertStatment(), "INSERT INTO GameImage (GameId) VALUES (1)");
        }

        [TestMethod]
        public void GameImageGenerateUpdateStatementTest()
        {
            var gameImage = new GameImage(1, 1, _imageData);

            Assert.AreEqual(gameImage.GenerateUpdateStatement(), "UPDATE GameImage SET GameId = 1 WHERE Id = 1");
        }

        [TestMethod]
        public void GameImageGenerateDeleteStatementTest()
        {
            var gameImage = new GameImage(1, 1, _imageData);

            Assert.AreEqual(gameImage.GenerateDeleteStatement(), "DELETE FROM GameImage WHERE Id = 1");
        }

        [TestMethod]
        public void GameImageGenerateExistsQueryTest()
        {
            var gameImage = new GameImage(1, 1, _imageData);

            Assert.AreEqual(gameImage.GenerateExistsQuery(), "SELECT Id FROM GameImage WHERE Id = 1");
        }

        [TestMethod]
        public void GameImageGenerateSelectQueryTest()
        {
            var gameImage = new GameImage();

            Assert.AreEqual(gameImage.GenerateSelectQuery(), "SELECT Id, GameId FROM GameImage");

            gameImage = new GameImage(1, 1, _imageData);

            Assert.AreEqual(gameImage.GenerateSelectQuery(), "SELECT Id, GameId FROM GameImage WHERE Id = 1");
        }

        [TestMethod]
        public void GameImageGeneratePrimaryKeyWhereClauseTest()
        {
            var gameImage = new GameImage();

            Assert.AreEqual(gameImage.GeneratePrimaryKeyWhereClause(), "");

            gameImage = new GameImage(1, 1, _imageData);

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


            // Select All

            var gameImage = new GameImage();

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

            gameImage = new GameImage(0, game.Id, _imageData);


            insertCommand = gameImage.GenerateInsertStatment();

            Assert.IsFalse(string.IsNullOrEmpty(insertCommand));

            errorMessage = "";

            insertResult = connection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.IsTrue(insertResult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            gameImage.Id = newId;

            // Add image BLOB

            var blobUpdateCommand = connection.CreateBlobUpdateStatement(GameImage.TableName, "Image", gameImage.GeneratePrimaryKeyWhereClause());
            var blobResult = connection.WriteBlobData(blobUpdateCommand, "Image", "varbinary", null, _imageData);

            Assert.IsTrue(blobResult);


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

            // Read BLOB Data
            foundGameImage.Image = connection.ReadBlobData($"SELECT Image FROM {GameImage.TableName} WHERE Id = {foundGameImage.Id}", 0);

            Assert.AreNotSame(gameImage, foundGameImage);

            Assert.AreEqual(gameImage.Id, foundGameImage.Id);
            Assert.AreEqual(gameImage.GameId, foundGameImage.GameId);

            if (gameImage.Image != null)
            {
                Assert.IsTrue(gameImage.Image.SequenceEqual(foundGameImage.Image));
            }

            else
            {
                Assert.IsNull(gameImage.Image);
                Assert.IsNull(foundGameImage.Image);
            }


            // Update

            var updateGameImage = new GameImage(
                newId,
                updateGame.Id,
                _updateImageData);

            var updateCommand = updateGameImage.GenerateUpdateStatement();

            Assert.IsFalse(string.IsNullOrEmpty(updateCommand));

            errorMessage = "";
            var updateResult = connection.ExecuteCommand(updateCommand, ref errorMessage);

            Assert.AreEqual(updateResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            // Update image BLOB

            blobUpdateCommand = connection.CreateBlobUpdateStatement(GameImage.TableName, "Image", updateGameImage.GeneratePrimaryKeyWhereClause());
            blobResult = connection.WriteBlobData(blobUpdateCommand, "Image", "varbinary", null, _updateImageData);

            Assert.IsTrue(blobResult);


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

            // Read BLOB Data
            foundGameImage.Image = connection.ReadBlobData($"SELECT Image FROM {GameImage.TableName} WHERE Id = {foundGameImage.Id}", 0);

            Assert.AreEqual(updateGameImage.Id, foundGameImage.Id);
            Assert.AreEqual(updateGameImage.GameId, foundGameImage.GameId);

            if (updateGameImage.Image != null)
            {
                Assert.IsTrue(updateGameImage.Image.SequenceEqual(foundGameImage.Image));
            }

            else
            {
                Assert.IsNull(updateGameImage.Image);
                Assert.IsNull(foundGameImage.Image);
            }


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
                2,
                _imageData);

            var gameImage2 = CloneUtility.BinaryClone(gameImage1);

            Assert.AreNotSame(gameImage1, gameImage2);

            Assert.AreEqual(gameImage1.Id, gameImage2.Id);
            Assert.AreEqual(gameImage1.GameId, gameImage2.GameId);

            if (gameImage1.Image != null)
            {
                Assert.IsTrue(gameImage1.Image.SequenceEqual(gameImage2.Image));
            }

            else
            {
                Assert.IsNull(gameImage1.Image);
                Assert.IsNull(gameImage2.Image);
            }
        }

        [TestMethod]
        public void GameImageXmlCloneTest()
        {
            var gameImage1 = new GameImage(
                1,
                2,
                _imageData);

            var gameImage2 = CloneUtility.XmlClone(gameImage1, null);

            Assert.AreNotSame(gameImage1, gameImage2);

            Assert.AreEqual(gameImage1.Id, gameImage2.Id);
            Assert.AreEqual(gameImage1.GameId, gameImage2.GameId);

            if (gameImage1.Image != null)
            {
                Assert.IsTrue(gameImage1.Image.SequenceEqual(gameImage2.Image));
            }

            else
            {
                Assert.IsNull(gameImage1.Image);
                Assert.IsNull(gameImage2.Image);
            }
        }

        [TestMethod]
        public void GameImageJsonTest()
        {
            var gameImage1 = new GameImage(
                1,
                2,
                _imageData);

            var jsonText = CloneUtility.ToJson(gameImage1);

            Assert.IsFalse(string.IsNullOrEmpty(jsonText));

            var gameImage2 = CloneUtility.FromJson<GameImage>(jsonText);

            Assert.AreNotSame(gameImage1, gameImage2);

            Assert.AreEqual(gameImage1.Id, gameImage2.Id);
            Assert.AreEqual(gameImage1.GameId, gameImage2.GameId);

            if (gameImage1.Image != null)
            {
                Assert.IsTrue(gameImage1.Image.SequenceEqual(gameImage2.Image));
            }

            else
            {
                Assert.IsNull(gameImage1.Image);
                Assert.IsNull(gameImage2.Image);
            }
        }

        [TestMethod]
        public void GameImageDictionaryTest()
        {
            var gameImage1 = new GameImage(
                1,
                2,
                _imageData);

            var dictionary = GameImage.ToDictionary(gameImage1);

            Assert.IsNotNull(dictionary);

            var gameImage2 = GameImage.FromDictionary(dictionary);

            Assert.AreNotSame(gameImage1, gameImage2);

            Assert.AreEqual(gameImage1.Id, gameImage2.Id);
            Assert.AreEqual(gameImage1.GameId, gameImage2.GameId);

            if (gameImage1.Image != null)
            {
                Assert.IsTrue(gameImage1.Image.SequenceEqual(gameImage2.Image));
            }

            else
            {
                Assert.IsNull(gameImage1.Image);
                Assert.IsNull(gameImage2.Image);
            }
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

            gameImageList.Add(new GameImage(1, 2, _imageData));

            gameImageList.Add(new GameImage(3, 4, null));

            gameImageList.Add(new GameImage(5, 6, _imageData));

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

            gameImageList.Add(new GameImage(1, 2, _imageData));

            gameImageList.Add(new GameImage(3, 4, null));

            gameImageList.Add(new GameImage(5, 6, _imageData));

            Assert.IsFalse(gameImageList.Exists(0));
            Assert.IsFalse(gameImageList.Exists(-1));
            Assert.AreEqual(gameImageList.Exists(1), true);
            Assert.AreEqual(gameImageList.Exists(3), true);
            Assert.AreEqual(gameImageList.Exists(5), true);
        }

        [TestMethod]
        public void GameImageListAddandRemoveTest()
        {
            var gameImageList = new GameImageList();

            Assert.AreEqual(gameImageList.List.Count, 0);

            gameImageList.Add(new GameImage(1, 2, _imageData));

            Assert.AreEqual(gameImageList.List.Count, 1);

            gameImageList.Add(new GameImage(3, 4, null));

            Assert.AreEqual(gameImageList.List.Count, 2);

            gameImageList.Add(new GameImage(5, 6, _imageData));

            Assert.AreEqual(gameImageList.List.Count, 3);

            gameImageList.Remove(1);

            Assert.AreEqual(gameImageList.List.Count, 2);

            gameImageList.Remove(3);

            Assert.AreEqual(gameImageList.List.Count, 1);

            gameImageList.Remove(5);

            Assert.AreEqual(gameImageList.List.Count, 0);
        }

        [TestMethod]
        public void GameImageListJsonTest()
        {
            var gameImageList1 = new GameImageList();

            gameImageList1.Add(new GameImage(1, 2, _imageData));

            gameImageList1.Add(new GameImage(3, 4, null));

            gameImageList1.Add(new GameImage(5, 6, _imageData));

            var jsonText = CloneUtility.ToJson(gameImageList1);

            Assert.IsFalse(string.IsNullOrEmpty(jsonText));

            var gameImageList2 = CloneUtility.FromJson<GameImageList>(jsonText);

            Assert.AreNotSame(gameImageList1, gameImageList2);
            Assert.AreEqual(gameImageList1.List.Count, gameImageList2.List.Count);

            for (var index = 0; index < gameImageList1.List.Count; index++)
            {
                Assert.AreEqual(gameImageList1.List[index].Id, gameImageList2.List[index].Id);
                Assert.AreEqual(gameImageList1.List[index].GameId, gameImageList2.List[index].GameId);

                if (gameImageList1.List[index].Image != null)
                {
                    Assert.IsTrue(gameImageList1.List[index].Image.SequenceEqual(gameImageList2.List[index].Image));
                }

                else
                {
                    Assert.IsNull(gameImageList1.List[index].Image);
                    Assert.IsNull(gameImageList2.List[index].Image);
                }
            }
        }

        [TestMethod]
        public void GameImageListBinaryCloneTest()
        {
            var gameImageList1 = new GameImageList();

            gameImageList1.Add(new GameImage(1, 2, _imageData));

            gameImageList1.Add(new GameImage(3, 4, null));

            gameImageList1.Add(new GameImage(5, 6, _imageData));

            var gameImageList2 = CloneUtility.BinaryClone(gameImageList1);

            Assert.AreNotSame(gameImageList1, gameImageList2);
            Assert.AreEqual(gameImageList1.List.Count, gameImageList2.List.Count);

            for (var index = 0; index < gameImageList1.List.Count; index++)
            {
                Assert.AreEqual(gameImageList1.List[index].Id, gameImageList2.List[index].Id);
                Assert.AreEqual(gameImageList1.List[index].GameId, gameImageList2.List[index].GameId);

                if (gameImageList1.List[index].Image != null)
                {
                    Assert.IsTrue(gameImageList1.List[index].Image.SequenceEqual(gameImageList2.List[index].Image));
                }

                else
                {
                    Assert.IsNull(gameImageList1.List[index].Image);
                    Assert.IsNull(gameImageList2.List[index].Image);
                }
            }
        }

        [TestMethod]
        public void GameImageListXmlCloneTest()
        {
            var gameImageList1 = new GameImageList();

            gameImageList1.Add(new GameImage(1, 2, _imageData));

            gameImageList1.Add(new GameImage(3, 4, null));

            gameImageList1.Add(new GameImage(5, 6, _imageData));

            var gameImageList2 = CloneUtility.XmlClone(gameImageList1, null);

            Assert.AreNotSame(gameImageList1, gameImageList2);
            Assert.AreEqual(gameImageList1.List.Count, gameImageList2.List.Count);

            for (var index = 0; index < gameImageList1.List.Count; index++)
            {
                Assert.AreEqual(gameImageList1.List[index].Id, gameImageList2.List[index].Id);
                Assert.AreEqual(gameImageList1.List[index].GameId, gameImageList2.List[index].GameId);

                if (gameImageList1.List[index].Image != null)
                {
                    Assert.IsTrue(gameImageList1.List[index].Image.SequenceEqual(gameImageList2.List[index].Image));
                }

                else
                {
                    Assert.IsNull(gameImageList1.List[index].Image);
                    Assert.IsNull(gameImageList2.List[index].Image);
                }
            }
        }

        [TestMethod]
        public void GameImageListDictionaryTest()
        {
            var gameImageList1 = new GameImageList();

            gameImageList1.Add(new GameImage(1, 2, _imageData));

            gameImageList1.Add(new GameImage(3, 4, null));

            gameImageList1.Add(new GameImage(5, 6, _imageData));

            var dictionaryList = GameImageList.ToDictionaryList(gameImageList1);

            Assert.IsNotNull(dictionaryList);

            var gameImageList2 = GameImageList.FromDictionaryList(dictionaryList);

            Assert.AreNotSame(gameImageList1, gameImageList2);
            Assert.AreEqual(gameImageList1.List.Count, gameImageList2.List.Count);

            for (var index = 0; index < gameImageList1.List.Count; index++)
            {
                Assert.AreEqual(gameImageList1.List[index].Id, gameImageList2.List[index].Id);
                Assert.AreEqual(gameImageList1.List[index].GameId, gameImageList2.List[index].GameId);

                if (gameImageList1.List[index].Image != null)
                {
                    Assert.IsTrue(gameImageList1.List[index].Image.SequenceEqual(gameImageList2.List[index].Image));
                }

                else
                {
                    Assert.IsNull(gameImageList1.List[index].Image);
                    Assert.IsNull(gameImageList2.List[index].Image);
                }
            }
        }

        #endregion Test Cases
    }
}
