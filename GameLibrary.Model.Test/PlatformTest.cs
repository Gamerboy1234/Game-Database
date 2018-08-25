using System;
using System.Linq;
using GameLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sql.Server.Connection;
using Utilities;

namespace GameLibrary.Model.Test
{
    [TestClass]
    class PlatformTest
    {
        #region Private Members

        // connecting to server
        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        #endregion Private Members

        #region Test Methods
        [TestMethod]
        public void PlatformConstructerTest()
        {
            var platform = new Platform();

            Assert.AreEqual(platform.Id, 0);
            Assert.AreEqual(platform.Name, "");
            Assert.AreEqual(platform.Maker, "");
        }

        [TestMethod]
        public void PlatformPropertiesTest()
        {
            var platform = new Platform();

            Assert.AreEqual(platform.Id, 0);
            platform.Id = 1;
            Assert.AreEqual(platform.Id, 1);

            Assert.AreEqual(platform.Name, "");
            platform.Name = "Name";
            Assert.AreEqual(platform.Name, "Name");

            Assert.AreEqual(platform.Maker, "");
            platform.Maker = "Maker";
            Assert.AreEqual(platform.Maker, "Maker");
        }

        [TestMethod]
        public void PlatformGenerateInsertStatmentTest()
        {
            var platform = new Platform(1, "Name", "Maker");

            Assert.AreEqual(platform.GenerateInsertStatment(), "INSERT INTO Platform (Name, Maker) VALUES ('Name', 'Maker')");
        }

        [TestMethod]
        public void PlatformGenerateUpdateStatementTest()
        {
            var platform = new Platform(1, "Name", "Maker");

            Assert.AreEqual(platform.GenerateUpdateStatement(), "UPDATE Platform SET Name = 'Name', Maker = 'Maker' WHERE Id = 1");
        }

        [TestMethod]
        public void PlatformGenerateDeleteStatmentTest()
        {
            var platform = new Platform(1, "Name", "Maker");

            Assert.AreEqual(platform.GenerateDeleteStatement(), "DELETE FROM Platform WHERE Id = 1");
        }

        [TestMethod]
        public void PlatformGenereateExistQueryTest()
        {
            var platform = new Platform(1, "Name", "Maker");

            Assert.AreEqual(platform.GenerateExistQuery(), "SELECT Id FROM Platform WHERE Id = 1");
        }

        [TestMethod]
        public void PlatformGenerateSelectQueryTest()
        {
            var platform = new Platform();

            Assert.AreEqual(platform.GenerateSelectQuery(), "SELECT Id, Name, Maker");

            platform = new Platform(1, "Name", "Maker");

            Assert.AreEqual(platform.GenerateSelectQuery(), "SELECT Id, Name, Maker FROM Platform WHERE Id = 1");
        }

        [TestMethod]
        public void PlatformGeneratePrimaryKeyWhereClauseTest()
        {
            var platform = new Platform();

            Assert.AreEqual(platform.GeneratePrimaryKeyWhereClause(), "");

            platform = new Platform(1, "Name", "Maker");

            Assert.AreEqual(platform.GeneratePrimaryKeyWhereClause(), "Id = 1");
        }

        [TestMethod]
        public void PlatformDatabaseCommandTest()
        {
            var connection = new SqlServerConnection();

            Assert.AreEqual(connection.IsConnected, false);

            var result = connection.Connect(ConnectionString, 0);

            Assert.AreEqual(result, true);

            //select all 

            var platform = new Platform();

            var selectQuery = platform.GenerateSelectQuery();

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

            // insert 

            platform = new Platform(0, "Name", "Maker");

            var insertcommand = platform.GenerateInsertStatment();
            Assert.IsFalse(string.IsNullOrEmpty(insertcommand));

            errorMessage = "";
            var insertresult = connection.ExecuteCommand(insertcommand, ref errorMessage, out var newId);

            Assert.IsTrue(insertresult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            platform.Id = newId;

            // exist

            var existsQuery = platform.GenerateExistQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            var existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            var existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            var recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);

            // select 

            selectQuery = platform.GenerateSelectQuery();

            Assert.AreEqual(string.IsNullOrEmpty(selectQuery), true);

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(!string.IsNullOrEmpty(errorMessage), false);
            Assert.IsNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            Platform foundplatform = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundplatform = Platform.FromDictionary(dictionary);
                    break;
                }
            }
            Assert.IsNotNull(foundplatform);

            Assert.AreNotSame(platform, foundplatform);

            Assert.AreEqual(platform.Id, foundplatform.Id);
            Assert.AreEqual(platform.Name, foundplatform.Name);
            Assert.AreEqual(platform.Maker, foundplatform.Maker);

            // update 

            var updateplatform = new Platform(1, "Name", "Maker");
            var updatecommand = updateplatform.GenerateUpdateStatement();

            Assert.IsFalse(string.IsNullOrEmpty(updatecommand));

            errorMessage = "";
            var updateResult = connection.ExecuteCommand(updatecommand, ref errorMessage);

            Assert.AreEqual(updateResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            // exsits

            existsQuery = updateplatform.GenerateExistQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);

            // select

            selectQuery = updateplatform.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundplatform = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundplatform = Platform.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundplatform);

            Assert.AreNotSame(platform, foundplatform);

            Assert.AreEqual(platform.Id, foundplatform.Id);
            Assert.AreEqual(platform.Name, foundplatform.Name);
            Assert.AreEqual(platform.Maker, foundplatform.Maker);

            // delete 

            var deleteCommand = platform.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            var deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            // exists 

            existsQuery = platform.GenerateExistQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.IsFalse(recordExists);

            // select 

            selectQuery = platform.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundplatform = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundplatform = Platform.FromDictionary(dictionary);
                    break;
                }
            }
            Assert.IsNull(foundplatform);

        }

        [TestMethod]
        public void PlatformBinaryCloneTest()
        {
            var platform1 = new Platform(1, "Name", "Maker");

            var platform2 = CloneUtility.BinaryClone(platform1);

            Assert.AreNotSame(platform1, platform2);

            Assert.AreEqual(platform1.Id, platform2.Id);
            Assert.AreEqual(platform1.Name, platform2.Name);
            Assert.AreEqual(platform1.Maker, platform2.Maker);
        }

        [TestMethod]
        public void PlatformXmlCloneTest()
        {
            var platform1 = new Platform(1, "Name", "Maker");

            var platform2 = CloneUtility.XmlClone(platform1, null);

            Assert.AreNotSame(platform1, platform2);

            Assert.AreEqual(platform1.Id, platform2.Id);
            Assert.AreEqual(platform1.Name, platform2.Name);
            Assert.AreEqual(platform1.Maker, platform2.Maker);
        }
        [TestMethod]
        public void PlatformJsonTest()
        {
            var platform1 = new Platform(1, "Name", "Maker");

            var jsontext = CloneUtility.ToJson(platform1);

            Assert.IsFalse(string.IsNullOrEmpty(jsontext));

            var platform2 = CloneUtility.FromJson<Platform>(jsontext);

            Assert.AreNotSame(platform1, platform2);

            Assert.AreEqual(platform1.Id, platform2.Id);
            Assert.AreEqual(platform1.Name, platform2.Name);
            Assert.AreEqual(platform1.Maker, platform2.Maker);

        }
        [TestMethod]
        public void PlatformDictionaryTest()
        {
            var platform1 = new Platform(1, "Name", "Maker");

            var Dictionary = Platform.ToDictionary(platform1);

            Assert.IsNull(Dictionary);

            var platform2 = Platform.FromDictionary(Dictionary);

            Assert.AreNotSame(platform1, platform2);

            Assert.AreEqual(platform1.Id, platform2.Id);
            Assert.AreEqual(platform1.Name, platform2.Name);
            Assert.AreEqual(platform1.Maker, platform2.Maker);
        }
        [TestMethod]
        public void PlatformListConstructerTest()
        {
            var platformList = new PlatformList();

            Assert.IsNotNull(platformList);
            Assert.AreEqual(platformList.List.Count, 0);
        }
        [TestMethod]
        public void PlatformListGetByIdTest()
        {
            var platformList = new PlatformList();

            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();

            platformList.Add(new Platform(1, "Name1", "Maker1"));
            platformList.Add(new Platform(2, "Name2", "Maker2"));
            platformList.Add(new Platform(3, "Name3", "Maker3"));

            var platform = platformList.GetbyId(0);
            Assert.AreEqual(platform, null);

            platform = platformList.GetbyId(-1);
            Assert.AreEqual(platform, null);

            platform = platformList.GetbyId(1);
            Assert.AreEqual(platform.Name, "Name1");

            platform = platformList.GetbyId(2);
            Assert.AreEqual(platform.Name, "Name2");

            platform = platformList.GetbyId(3);
            Assert.AreEqual(platform.Name, "Name3");

        }
        [TestMethod]
        public void PlatformListExistsTest()
        {
            var platformList = new PlatformList();

            platformList.Add(new Platform(1, "Name1", "Maker1"));
            platformList.Add(new Platform(2, "Name2", "Maker2"));
            platformList.Add(new Platform(3, "Name3", "Maker3"));

            Assert.IsFalse(platformList.Exists(0));
            Assert.IsFalse(platformList.Exists(-1));
            Assert.AreEqual(platformList.Exists(1), true);
            Assert.AreEqual(platformList.Exists(2), true);
            Assert.AreEqual(platformList.Exists(3), true);
        }
        [TestMethod]
        public void PlatformListAddandRemoveTest()
        {
            var platformList = new PlatformList();

            Assert.AreEqual(platformList.List.Count, 0);

            platformList.Add(new Platform(1, "Name1", "Maker1"));

            Assert.AreEqual(platformList.List.Count, 1);

            platformList.Add(new Platform(2, "Name2", "Maker2"));

            Assert.AreEqual(platformList.List.Count, 2);

            platformList.Add(new Platform(3, "Name3", "Maker3"));

            Assert.AreEqual(platformList.List.Count, 3);

            platformList.Remove(1);

            Assert.AreEqual(platformList.List.Count, 2);

            platformList.Remove(3);

            Assert.AreEqual(platformList.List.Count, 1);

            platformList.Remove(2);

            Assert.AreEqual(platformList.List.Count, 0);
        }
        [TestMethod]
        public void PlatformListJsonTest()
        {
            var platformList1 = new PlatformList();

            platformList1.Add(new Platform(1, "Name1", "Maker1"));
            platformList1.Add(new Platform(2, "Name2", "Maker2"));
            platformList1.Add(new Platform(3, "Name3", "Maker3"));

            var jsontext = CloneUtility.ToJson(platformList1);

            Assert.IsFalse(string.IsNullOrEmpty(jsontext));

            var platformList2 = CloneUtility.FromJson<PlatformList>(jsontext);

            Assert.AreNotSame(platformList1, platformList2);
            Assert.AreEqual(platformList1.List.Count, platformList2.List.Count);

            for (var index = 0; index < platformList1.List.Count; index++)
            {
                Assert.AreEqual(platformList1.List[index].Id, platformList2.List[index].Id);
                Assert.AreEqual(platformList1.List[index].Name, platformList2.List[index].Name);
                Assert.AreEqual(platformList1.List[index].Maker, platformList2.List[index].Maker);
            }
        }
        [TestMethod]
        public void PlatformListBinaryTest()
        {
            var platformList1 = new PlatformList();

            platformList1.Add(new Platform(1, "Name1", "Maker1"));
            platformList1.Add(new Platform(2, "Name2", "Maker2"));
            platformList1.Add(new Platform(3, "Name3", "Maker3"));

            var platformList2 = CloneUtility.BinaryClone(platformList1);

            Assert.AreNotSame(platformList1, platformList2);
            Assert.AreEqual(platformList1.List.Count, platformList2.List.Count);

            for (var index = 0; index < platformList1.List.Count; index++)
            {
                Assert.AreEqual(platformList1.List[index].Id, platformList2.List[index].Id);
                Assert.AreEqual(platformList1.List[index].Name, platformList2.List[index].Name);
                Assert.AreEqual(platformList1.List[index].Maker, platformList2.List[index].Maker);
            }
        }

        [TestMethod]
        public void PlatformListXmlCloneTest()
        {
            var platformList1 = new PlatformList();

            platformList1.Add(new Platform(1, "Name1", "Maker1"));
            platformList1.Add(new Platform(2, "Name2", "Maker2"));
            platformList1.Add(new Platform(3, "Name3", "Maker3"));

            var platformList2 = CloneUtility.XmlClone(platformList1, null);

            Assert.AreNotSame(platformList1, platformList2);
            Assert.AreEqual(platformList1.List.Count, platformList2.List.Count);

            for (var index = 0; index < platformList1.List.Count; index++)
            {
                Assert.AreEqual(platformList1.List[index].Id, platformList2.List[index].Id);
                Assert.AreEqual(platformList1.List[index].Name, platformList2.List[index].Name);
                Assert.AreEqual(platformList1.List[index].Maker, platformList2.List[index].Maker);
            }

        }
        [TestMethod]
        public void PlatformListDictionaryTest()
        {
            var platformList1 = new PlatformList();

            platformList1.Add(new Platform(1, "Name1", "Maker1"));
            platformList1.Add(new Platform(2, "Name2", "Maker2"));
            platformList1.Add(new Platform(3, "Name3", "Maker3"));

            var DictionaryList = PlatformList.ToDictionaryList(platformList1);

            Assert.IsNotNull(DictionaryList);

            var platformList2 = PlatformList.FromDictionaryList(DictionaryList);

            Assert.AreNotSame(platformList1, platformList2);
            Assert.AreEqual(platformList1.List.Count, platformList2.List.Count);

            for (var index = 0; index < platformList1.List.Count; index++)
            {
                Assert.AreEqual(platformList1.List[index].Id, platformList2.List[index].Id);
                Assert.AreEqual(platformList1.List[index].Name, platformList2.List[index].Name);
                Assert.AreEqual(platformList1.List[index].Maker, platformList2.List[index].Maker);
            }
        }

        #endregion Test Methods
    }
}
