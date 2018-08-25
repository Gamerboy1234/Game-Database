using System;
using System.Linq;
using GameLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sql.Server.Connection;
using Utilities;

namespace GameLibrary.Model.Test
{
    [TestClass]
    class RatingTest
    {
        #region Private Members

        // connecting to server
        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        #endregion Private Members

        #region Test Methods
        [TestMethod]
        public void RatingConstructerTest()
        {
            var rating = new Rating();

            Assert.AreEqual(rating.Id, 0);
            Assert.AreEqual(rating.Name, "");
            Assert.AreEqual(rating.Description, "");
            Assert.AreEqual(rating.Symbol, "");
        }

        [TestMethod]
        public void RatingPropertiesTest()
        {
            var rating = new Rating();

            Assert.AreEqual(rating.Id, 0);
            rating.Id = 1;
            Assert.AreEqual(rating.Id, 1);

            Assert.AreEqual(rating.Name, "");
            rating.Name = "Name";
            Assert.AreEqual(rating.Name, "Name");

            Assert.AreEqual(rating.Description, "");
            rating.Description = "Description";
            Assert.AreEqual(rating.Description, "Description");

            Assert.AreEqual(rating.Symbol, "");
            rating.Symbol = "Symbol";
            Assert.AreEqual(rating.Symbol,"Symbol");
        }

        [TestMethod]
        public void RatingGenerateInsertStatmentTest()
        {
            var rating = new Rating(1, "Name", "Description", "Symbol");

            Assert.AreEqual(rating.GenerateInsertStatment(), "INSERT INTO Rating (Name, Description, Symbol) VALUES ('Name', 'Description', 'Symbol')");
        }

        [TestMethod]
        public void RatingGenerateUpdateStatementTest()
        {
            var rating = new Rating(1, "Name", "Description", "Symbol");

            Assert.AreEqual(rating.GenerateUpdateStatement(), "UPDATE Rating SET Name = 'Name', Description = 'Description', Symbol = Symbol WHERE Id = 1");
        }

        [TestMethod]
        public void RatingGenerateDeleteStatmentTest()
        {
            var rating = new Rating(1, "Name", "Description", "Symbol");

            Assert.AreEqual(rating.GenerateDeleteStatement(), "DELETE FROM Rating WHERE Id = 1");
        }

        [TestMethod]
        public void RatingGenereateExistQueryTest()
        {
            var rating = new Rating(1, "Name", "Description", "Symbol");

            Assert.AreEqual(rating.GenerateExistQuery(), "SELECT Id FROM Rating WHERE Id = 1");
        }

        [TestMethod]
        public void RatingGenerateSelectQueryTest()
        {
            var rating = new Rating();

            Assert.AreEqual(rating.GenerateSelectQuery(), "SELECT Id, Name, Description, Symbol");

            rating = new Rating(1, "Name", "Description", "Symbol");

            Assert.AreEqual(rating.GenerateSelectQuery(), "SELECT Id, Name, Description, Symbol FROM Rating WHERE Id = 1");
        }

        [TestMethod]
        public void RatingGeneratePrimaryKeyWhereClauseTest()
        {
            var rating = new Rating();

            Assert.AreEqual(rating.GeneratePrimaryKeyWhereClause(), "");

            rating = new Rating(1, "Name", "Desciption", "Symbol");

            Assert.AreEqual(rating.GeneratePrimaryKeyWhereClause(), "Id = 1");
        }

        [TestMethod]
        public void RatingDatabaseCommandTest()
        {
            var connection = new SqlServerConnection();

            Assert.AreEqual(connection.IsConnected, false);

            var result = connection.Connect(ConnectionString, 0);

            Assert.AreEqual(result, true);

            //select all 

            var rating = new Rating();

            var selectQuery = rating.GenerateSelectQuery();

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

            rating = new Rating(0, "Name", "Description", "Symbol");

            var insertcommand = rating.GenerateInsertStatment();
            Assert.IsFalse(string.IsNullOrEmpty(insertcommand));

            errorMessage = "";
            var insertresult = connection.ExecuteCommand(insertcommand, ref errorMessage, out var newId);

            Assert.IsTrue(insertresult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            rating.Id = newId;

            // exist

            var existsQuery = rating.GenerateExistQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            var existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            var existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            var recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);

            // select 

            selectQuery = rating.GenerateSelectQuery();

            Assert.AreEqual(string.IsNullOrEmpty(selectQuery), true);

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(!string.IsNullOrEmpty(errorMessage), false);
            Assert.IsNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            Rating foundrating = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundrating = Rating.FromDictionary(dictionary);
                    break;
                }
            }
            Assert.IsNotNull(foundrating);

            Assert.AreNotSame(rating, foundrating);

            Assert.AreEqual(rating.Id, foundrating.Id);
            Assert.AreEqual(rating.Name, foundrating.Name);
            Assert.AreEqual(rating.Description, foundrating.Description);
            Assert.AreEqual(rating.Symbol, foundrating.Symbol);

            // update 

            var updaterating = new Rating(1, "Name", "Description", "Symbol");
            var updatecommand = updaterating.GenerateUpdateStatement();

            Assert.IsFalse(string.IsNullOrEmpty(updatecommand));

            errorMessage = "";
            var updateResult = connection.ExecuteCommand(updatecommand, ref errorMessage);

            Assert.AreEqual(updateResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            // exsits

            existsQuery = updaterating.GenerateExistQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);

            // select

            selectQuery = updaterating.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundrating = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundrating = Rating.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundrating);

            Assert.AreNotSame(rating, foundrating);

            Assert.AreEqual(rating.Id, foundrating.Id);
            Assert.AreEqual(rating.Name, foundrating.Name);
            Assert.AreEqual(rating.Description, foundrating.Description);
            Assert.AreEqual(rating.Symbol, foundrating.Symbol);

            // delete 

            var deleteCommand = rating.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            var deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            // exists 

            existsQuery = rating.GenerateExistQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.IsFalse(recordExists);

            // select 

            selectQuery = rating.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundrating = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundrating = Rating.FromDictionary(dictionary);
                    break;
                }
            }
            Assert.IsNull(foundrating);

        }

        [TestMethod]
        public void RatingBinaryCloneTest()
        {
            var rating1 = new Rating(1, "Name", "Description", "Symbol");

            var rating2 = CloneUtility.BinaryClone(rating1);

            Assert.AreNotSame(rating1, rating2);

            Assert.AreEqual(rating1.Id, rating2.Id);
            Assert.AreEqual(rating1.Name, rating2.Name);
            Assert.AreEqual(rating1.Description, rating2.Description);
            Assert.AreEqual(rating1.Symbol, rating2.Symbol);
        }

        [TestMethod]
        public void RatingXmlCloneTest()
        {
            var rating1 = new Rating(1, "Name", "Description", "Symbol");

            var rating2 = CloneUtility.XmlClone(rating1, null);

            Assert.AreNotSame(rating1, rating2);

            Assert.AreEqual(rating1.Id, rating2.Id);
            Assert.AreEqual(rating1.Name, rating2.Name);
            Assert.AreEqual(rating1.Description, rating2.Description);
            Assert.AreEqual(rating1.Symbol, rating2.Symbol);
        }
        [TestMethod]
        public void RatingJsonTest()
        {
            var rating1 = new Rating(1, "Name", "Description", "Symbol");

            var jsontext = CloneUtility.ToJson(rating1);

            Assert.IsFalse(string.IsNullOrEmpty(jsontext));

            var rating2 = CloneUtility.FromJson<Rating>(jsontext);

            Assert.AreNotSame(rating1, rating2);

            Assert.AreEqual(rating1.Id, rating2.Id);
            Assert.AreEqual(rating1.Name, rating2.Name);
            Assert.AreEqual(rating1.Description, rating2.Description);
            Assert.AreEqual(rating1.Symbol, rating2.Symbol);

        }
        [TestMethod]
        public void RatingDictionaryTest()
        {
            var rating1 = new Rating(1, "Name", "Description", "Symbol");

            var Dictionary = Rating.ToDictionary(rating1);

            Assert.IsNull(Dictionary);

            var rating2 = Rating.FromDictionary(Dictionary);

            Assert.AreNotSame(rating1, rating2);

            Assert.AreEqual(rating1.Id, rating2.Id);
            Assert.AreEqual(rating1.Name, rating2.Name);
            Assert.AreEqual(rating1.Description, rating2.Description);
            Assert.AreEqual(rating1.Symbol, rating2.Symbol);
        }
        [TestMethod]
        public void RatingListConstructerTest()
        {
            var ratingList = new RatingList();

            Assert.IsNotNull(ratingList);
            Assert.AreEqual(ratingList.List.Count, 0);
        }
        [TestMethod]
        public void RatingListGetByIdTest()
        {
            var ratingList = new RatingList();

            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();

            ratingList.Add(new Rating(1, "Name1", "Description1", "Symbol1"));
            ratingList.Add(new Rating(2, "Name2", "Description2", "Symbol2"));
            ratingList.Add(new Rating(3, "Name3", "Description3", "Symbol3"));

            var rating = ratingList.GetbyId(0);
            Assert.AreEqual(rating, null);

            rating = ratingList.GetbyId(-1);
            Assert.AreEqual(rating, null);

            rating = ratingList.GetbyId(1);
            Assert.AreEqual(rating.Name, "Name1");

            rating = ratingList.GetbyId(2);
            Assert.AreEqual(rating.Name, "Name2");

            rating = ratingList.GetbyId(3);
            Assert.AreEqual(rating.Name, "Name3");

        }
        [TestMethod]
        public void RatingListExistsTest()
        {
            var ratingList = new RatingList();

            ratingList.Add(new Rating(1, "Name1", "Description1", "Symbol1"));
            ratingList.Add(new Rating(2, "Name2", "Description2", "Symbol2"));
            ratingList.Add(new Rating(3, "Name3", "Description3", "Symbol3"));

            Assert.IsFalse(ratingList.Exists(0));
            Assert.IsFalse(ratingList.Exists(-1));
            Assert.AreEqual(ratingList.Exists(1), true);
            Assert.AreEqual(ratingList.Exists(2), true);
            Assert.AreEqual(ratingList.Exists(3), true);
        }
        [TestMethod]
        public void RatingListAddandRemoveTest()
        {
            var ratingList = new RatingList();

            Assert.AreEqual(ratingList.List.Count, 0);

            ratingList.Add(new Rating(1, "Name1", "Description1", "Symbol1"));

            Assert.AreEqual(ratingList.List.Count, 1);

            ratingList.Add(new Rating(2, "Name2", "Description2", "Symbol2"));

            Assert.AreEqual(ratingList.List.Count, 2);

            ratingList.Add(new Rating(3, "Name3", "Description3", "Symbol3"));

            Assert.AreEqual(ratingList.List.Count, 3);

            ratingList.Remove(1);

            Assert.AreEqual(ratingList.List.Count, 2);

            ratingList.Remove(3);

            Assert.AreEqual(ratingList.List.Count, 1);

            ratingList.Remove(2);

            Assert.AreEqual(ratingList.List.Count, 0);
        }
        [TestMethod]
        public void RatingListJsonTest()
        {
            var ratingList1 = new RatingList();

            ratingList1.Add(new Rating(1, "Name1", "Description1", "Symbol1"));
            ratingList1.Add(new Rating(2, "Name2", "Description2", "Symbol2"));
            ratingList1.Add(new Rating(3, "Name3", "Description3", "Symbol3"));

            var jsontext = CloneUtility.ToJson(ratingList1);

            Assert.IsFalse(string.IsNullOrEmpty(jsontext));

            var ratingList2 = CloneUtility.FromJson<RatingList>(jsontext);

            Assert.AreNotSame(ratingList1, ratingList2);
            Assert.AreEqual(ratingList1.List.Count, ratingList2.List.Count);

            for (var index = 0; index < ratingList1.List.Count; index++)
            {
                Assert.AreEqual(ratingList1.List[index].Id, ratingList2.List[index].Id);
                Assert.AreEqual(ratingList1.List[index].Name, ratingList2.List[index].Name);
                Assert.AreEqual(ratingList1.List[index].Description, ratingList2.List[index].Description);
                Assert.AreEqual(ratingList1.List[index].Symbol, ratingList2.List[index].Symbol);
            }
        }
        [TestMethod]
        public void RatingListBinaryTest()
        {
            var ratingList1 = new RatingList();

            ratingList1.Add(new Rating(1, "Name1", "Description1", "Symbol1"));
            ratingList1.Add(new Rating(2, "Name2", "Description2", "Symbol2"));
            ratingList1.Add(new Rating(3, "Name3", "Description3", "Symbol3"));

            var ratingList2 = CloneUtility.BinaryClone(ratingList1);

            Assert.AreNotSame(ratingList1, ratingList2);
            Assert.AreEqual(ratingList1.List.Count, ratingList2.List.Count);

            for (var index = 0; index < ratingList1.List.Count; index++)
            {
                Assert.AreEqual(ratingList1.List[index].Id, ratingList2.List[index].Id);
                Assert.AreEqual(ratingList1.List[index].Name, ratingList2.List[index].Name);
                Assert.AreEqual(ratingList1.List[index].Description, ratingList2.List[index].Description);
                Assert.AreEqual(ratingList1.List[index].Symbol, ratingList2.List[index].Symbol);
            }
        }

        [TestMethod]
        public void RatingListXmlCloneTest()
        {
            var ratingList1 = new RatingList();

            ratingList1.Add(new Rating(1, "Name1", "Description1", "Symbol1"));
            ratingList1.Add(new Rating(2, "Name2", "Description2", "Symbol2"));
            ratingList1.Add(new Rating(3, "Name3", "Description3", "Symbol3"));

            var ratingList2 = CloneUtility.XmlClone(ratingList1, null);

            Assert.AreNotSame(ratingList1, ratingList2);
            Assert.AreEqual(ratingList1.List.Count, ratingList2.List.Count);

            for (var index = 0; index < ratingList1.List.Count; index++)
            {
                Assert.AreEqual(ratingList1.List[index].Id, ratingList2.List[index].Id);
                Assert.AreEqual(ratingList1.List[index].Name, ratingList2.List[index].Name);
                Assert.AreEqual(ratingList1.List[index].Description, ratingList2.List[index].Description);
                Assert.AreEqual(ratingList1.List[index].Symbol, ratingList2.List[index].Symbol);
            }

        }
        [TestMethod]
        public void RatingListDictionaryTest()
        {
            var ratingList1 = new RatingList();

            ratingList1.Add(new Rating(1, "Name1", "Description1", "Symbol1"));
            ratingList1.Add(new Rating(2, "Name2", "Description2", "Symbol2"));
            ratingList1.Add(new Rating(3, "Name3", "Description3", "Symbol3"));

            var DictionaryList = RatingList.ToDictionaryList(ratingList1);

            Assert.IsNotNull(DictionaryList);

            var ratingList2 = RatingList.FromDictionaryList(DictionaryList);

            Assert.AreNotSame(ratingList1, ratingList2);
            Assert.AreEqual(ratingList1.List.Count, ratingList2.List.Count);

            for (var index = 0; index < ratingList1.List.Count; index++)
            {
                Assert.AreEqual(ratingList1.List[index].Id, ratingList2.List[index].Id);
                Assert.AreEqual(ratingList1.List[index].Name, ratingList2.List[index].Name);
                Assert.AreEqual(ratingList1.List[index].Description, ratingList2.List[index].Description);
                Assert.AreEqual(ratingList1.List[index].Symbol, ratingList2.List[index].Symbol);
            }
        }

        #endregion Test Methods
    }
}
