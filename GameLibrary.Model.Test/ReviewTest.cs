using System;
using System.Linq;
using GameLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sql.Server.Connection;
using Utilities;

namespace GameLibrary.Model.Test
{
    [TestClass]
    class ReviewTest
    {
        #region Private Members

        // connecting to server
        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        #endregion Private Members

        #region Test Methods
        [TestMethod]
        public void ReviewConstructerTest()
        {
            var rating = new Review();

            Assert.AreEqual(rating.Id, 0);
            Assert.AreEqual(rating.Name, "");
            Assert.AreEqual(rating.Description, "");
            Assert.AreEqual(rating.Reviewrating, 0);
        }

        [TestMethod]
        public void ReviewPropertiesTest()
        {
            var rating = new Review();

            Assert.AreEqual(rating.Id, 0);
            rating.Id = 1;
            Assert.AreEqual(rating.Id, 1);

            Assert.AreEqual(rating.Name, "");
            rating.Name = "Name";
            Assert.AreEqual(rating.Name, "Name");

            Assert.AreEqual(rating.Description, "");
            rating.Description = "Description";
            Assert.AreEqual(rating.Description, "Description");

            Assert.AreEqual(rating.Reviewrating, "");
            rating.Reviewrating = 1;
            Assert.AreEqual(rating.Reviewrating, 1);
        }

        [TestMethod]
        public void ReviewGenerateInsertStatmentTest()
        {
            var rating = new Review(1, "Name", "Description", 2);

            Assert.AreEqual(rating.GenerateInsertStatment(), "INSERT INTO Review (Name, Description, Rating) VALUES ('Name', 'Description', 2)");
        }

        [TestMethod]
        public void ReviewGenerateUpdateStatementTest()
        {
            var rating = new Review(1, "Name", "Description", 2);

            Assert.AreEqual(rating.GenerateUpdateStatement(), "UPDATE Review SET Name = 'Name', Description = 'Description', Rating = 2 WHERE Id = 1");
        }

        [TestMethod]
        public void ReviewGenerateDeleteStatmentTest()
        {
            var rating = new Review(1, "Name", "Description", 2);

            Assert.AreEqual(rating.GenerateDeleteStatement(), "DELETE FROM Review WHERE Id = 1");
        }

        [TestMethod]
        public void ReviewGenereateExistQueryTest()
        {
            var rating = new Review(1, "Name", "Description", 2);

            Assert.AreEqual(rating.GenerateExistQuery(), "SELECT Id FROM Review WHERE Id = 1");
        }

        [TestMethod]
        public void ReviewGenerateSelectQueryTest()
        {
            var rating = new Review();

            Assert.AreEqual(rating.GenerateSelectQuery(), "SELECT Id, Name, Description, Rating");

            rating = new Review(1, "Name", "Description", 2);

            Assert.AreEqual(rating.GenerateSelectQuery(), "SELECT Id, Name, Description, Rating FROM Review WHERE Id = 1");
        }

        [TestMethod]
        public void ReviewGeneratePrimaryKeyWhereClauseTest()
        {
            var rating = new Review();

            Assert.AreEqual(rating.GeneratePrimaryKeyWhereClause(), "");

            rating = new Review(1, "Name", "Desciption", 2);

            Assert.AreEqual(rating.GeneratePrimaryKeyWhereClause(), "Id = 1");
        }

        [TestMethod]
        public void ReviewDatabaseCommandTest()
        {
            var connection = new SqlServerConnection();

            Assert.AreEqual(connection.IsConnected, false);

            var result = connection.Connect(ConnectionString, 0);

            Assert.AreEqual(result, true);

            //select all 

            var rating = new Review();

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

            rating = new Review(0, "Name", "Description", 2);

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

            Review foundrating = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundrating = Review.FromDictionary(dictionary);
                    break;
                }
            }
            Assert.IsNotNull(foundrating);

            Assert.AreNotSame(rating, foundrating);

            Assert.AreEqual(rating.Id, foundrating.Id);
            Assert.AreEqual(rating.Name, foundrating.Name);
            Assert.AreEqual(rating.Description, foundrating.Description);
            Assert.AreEqual(rating.Reviewrating, foundrating.Reviewrating);

            // update 

            var updaterating = new Review(1, "Name", "Description", 2);
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
                    foundrating = Review.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundrating);

            Assert.AreNotSame(rating, foundrating);

            Assert.AreEqual(rating.Id, foundrating.Id);
            Assert.AreEqual(rating.Name, foundrating.Name);
            Assert.AreEqual(rating.Description, foundrating.Description);
            Assert.AreEqual(rating.Reviewrating, foundrating.Reviewrating);

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
                    foundrating = Review.FromDictionary(dictionary);
                    break;
                }
            }
            Assert.IsNull(foundrating);

        }

        [TestMethod]
        public void ReviewBinaryCloneTest()
        {
            var rating1 = new Review(1, "Name", "Description", 2);

            var rating2 = CloneUtility.BinaryClone(rating1);

            Assert.AreNotSame(rating1, rating2);

            Assert.AreEqual(rating1.Id, rating2.Id);
            Assert.AreEqual(rating1.Name, rating2.Name);
            Assert.AreEqual(rating1.Description, rating2.Description);
            Assert.AreEqual(rating1.Reviewrating, rating2.Reviewrating);
        }

        [TestMethod]
        public void ReviewXmlCloneTest()
        {
            var rating1 = new Review(1, "Name", "Description", 2);

            var rating2 = CloneUtility.XmlClone(rating1, null);

            Assert.AreNotSame(rating1, rating2);

            Assert.AreEqual(rating1.Id, rating2.Id);
            Assert.AreEqual(rating1.Name, rating2.Name);
            Assert.AreEqual(rating1.Description, rating2.Description);
            Assert.AreEqual(rating1.Reviewrating, rating2.Reviewrating);
        }
        [TestMethod]
        public void ReviewJsonTest()
        {
            var rating1 = new Review(1, "Name", "Description", 2);

            var jsontext = CloneUtility.ToJson(rating1);

            Assert.IsFalse(string.IsNullOrEmpty(jsontext));

            var rating2 = CloneUtility.FromJson<Review>(jsontext);

            Assert.AreNotSame(rating1, rating2);

            Assert.AreEqual(rating1.Id, rating2.Id);
            Assert.AreEqual(rating1.Name, rating2.Name);
            Assert.AreEqual(rating1.Description, rating2.Description);
            Assert.AreEqual(rating1.Reviewrating, rating2.Reviewrating);

        }
        [TestMethod]
        public void ReviewDictionaryTest()
        {
            var rating1 = new Review(1, "Name", "Description", 2);

            var Dictionary = Review.ToDictionary(rating1);

            Assert.IsNull(Dictionary);

            var rating2 = Review.FromDictionary(Dictionary);

            Assert.AreNotSame(rating1, rating2);

            Assert.AreEqual(rating1.Id, rating2.Id);
            Assert.AreEqual(rating1.Name, rating2.Name);
            Assert.AreEqual(rating1.Description, rating2.Description);
            Assert.AreEqual(rating1.Reviewrating, rating2.Reviewrating);
        }
        [TestMethod]
        public void ReviewListConstructerTest()
        {
            var ratingList = new ReviewList();

            Assert.IsNotNull(ratingList);
            Assert.AreEqual(ratingList.List.Count, 0);
        }
        [TestMethod]
        public void ReviewListGetByIdTest()
        {
            var ratingList = new ReviewList();

            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();

            ratingList.Add(new Review(1, "Name1", "Description1", 2));
            ratingList.Add(new Review(2, "Name2", "Description2", 3));
            ratingList.Add(new Review(3, "Name3", "Description3", 4));

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
        public void ReviewListExistsTest()
        {
            var ratingList = new ReviewList();

            ratingList.Add(new Review(1, "Name1", "Description1", 2));
            ratingList.Add(new Review(2, "Name2", "Description2", 3));
            ratingList.Add(new Review(3, "Name3", "Description3", 4));

            Assert.IsFalse(ratingList.Exists(0));
            Assert.IsFalse(ratingList.Exists(-1));
            Assert.AreEqual(ratingList.Exists(1), true);
            Assert.AreEqual(ratingList.Exists(2), true);
            Assert.AreEqual(ratingList.Exists(3), true);
        }
        [TestMethod]
        public void ReviewListAddandRemoveTest()
        {
            var ratingList = new ReviewList();

            Assert.AreEqual(ratingList.List.Count, 0);

            ratingList.Add(new Review(1, "Name1", "Description1", 2));

            Assert.AreEqual(ratingList.List.Count, 1);

            ratingList.Add(new Review(2, "Name2", "Description2", 3));

            Assert.AreEqual(ratingList.List.Count, 2);

            ratingList.Add(new Review(3, "Name3", "Description3", 4));

            Assert.AreEqual(ratingList.List.Count, 3);

            ratingList.Remove(1);

            Assert.AreEqual(ratingList.List.Count, 2);

            ratingList.Remove(3);

            Assert.AreEqual(ratingList.List.Count, 1);

            ratingList.Remove(2);

            Assert.AreEqual(ratingList.List.Count, 0);
        }
        [TestMethod]
        public void ReviewListJsonTest()
        {
            var ratingList1 = new ReviewList();

            ratingList1.Add(new Review(1, "Name1", "Description1", 2));
            ratingList1.Add(new Review(2, "Name2", "Description2", 3));
            ratingList1.Add(new Review(3, "Name3", "Description3", 4));

            var jsontext = CloneUtility.ToJson(ratingList1);

            Assert.IsFalse(string.IsNullOrEmpty(jsontext));

            var ratingList2 = CloneUtility.FromJson<ReviewList>(jsontext);

            Assert.AreNotSame(ratingList1, ratingList2);
            Assert.AreEqual(ratingList1.List.Count, ratingList2.List.Count);

            for (var index = 0; index < ratingList1.List.Count; index++)
            {
                Assert.AreEqual(ratingList1.List[index].Id, ratingList2.List[index].Id);
                Assert.AreEqual(ratingList1.List[index].Name, ratingList2.List[index].Name);
                Assert.AreEqual(ratingList1.List[index].Description, ratingList2.List[index].Description);
                Assert.AreEqual(ratingList1.List[index].Reviewrating, ratingList2.List[index].Reviewrating);
            }
        }
        [TestMethod]
        public void ReviewListBinaryTest()
        {
            var ratingList1 = new ReviewList();

            ratingList1.Add(new Review(1, "Name1", "Description1", 2));
            ratingList1.Add(new Review(2, "Name2", "Description2", 3));
            ratingList1.Add(new Review(3, "Name3", "Description3", 4));

            var ratingList2 = CloneUtility.BinaryClone(ratingList1);

            Assert.AreNotSame(ratingList1, ratingList2);
            Assert.AreEqual(ratingList1.List.Count, ratingList2.List.Count);

            for (var index = 0; index < ratingList1.List.Count; index++)
            {
                Assert.AreEqual(ratingList1.List[index].Id, ratingList2.List[index].Id);
                Assert.AreEqual(ratingList1.List[index].Name, ratingList2.List[index].Name);
                Assert.AreEqual(ratingList1.List[index].Description, ratingList2.List[index].Description);
                Assert.AreEqual(ratingList1.List[index].Reviewrating, ratingList2.List[index].Reviewrating);
            }
        }

        [TestMethod]
        public void ReviewListXmlCloneTest()
        {
            var ratingList1 = new ReviewList();

            ratingList1.Add(new Review(1, "Name1", "Description1", 2));
            ratingList1.Add(new Review(2, "Name2", "Description2", 3));
            ratingList1.Add(new Review(3, "Name3", "Description3", 4));

            var ratingList2 = CloneUtility.XmlClone(ratingList1, null);

            Assert.AreNotSame(ratingList1, ratingList2);
            Assert.AreEqual(ratingList1.List.Count, ratingList2.List.Count);

            for (var index = 0; index < ratingList1.List.Count; index++)
            {
                Assert.AreEqual(ratingList1.List[index].Id, ratingList2.List[index].Id);
                Assert.AreEqual(ratingList1.List[index].Name, ratingList2.List[index].Name);
                Assert.AreEqual(ratingList1.List[index].Description, ratingList2.List[index].Description);
                Assert.AreEqual(ratingList1.List[index].Reviewrating, ratingList2.List[index].Reviewrating);
            }

        }
        [TestMethod]
        public void ReviewListDictionaryTest()
        {
            var ratingList1 = new ReviewList();

            ratingList1.Add(new Review(1, "Name1", "Description1", 2));
            ratingList1.Add(new Review(2, "Name2", "Description2", 3));
            ratingList1.Add(new Review(3, "Name3", "Description3", 4));

            var DictionaryList = ReviewList.ToDictionaryList(ratingList1);

            Assert.IsNotNull(DictionaryList);

            var ratingList2 = ReviewList.FromDictionaryList(DictionaryList);

            Assert.AreNotSame(ratingList1, ratingList2);
            Assert.AreEqual(ratingList1.List.Count, ratingList2.List.Count);

            for (var index = 0; index < ratingList1.List.Count; index++)
            {
                Assert.AreEqual(ratingList1.List[index].Id, ratingList2.List[index].Id);
                Assert.AreEqual(ratingList1.List[index].Name, ratingList2.List[index].Name);
                Assert.AreEqual(ratingList1.List[index].Description, ratingList2.List[index].Description);
                Assert.AreEqual(ratingList1.List[index].Reviewrating, ratingList2.List[index].Reviewrating);
            }
        }

        #endregion Test Methods
    }
}
