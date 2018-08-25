using System;
using System.Linq;
using GameLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sql.Server.Connection;
using Utilities;
using static GameLibrary.Model.Genre;

namespace GameLibrary.Model.Test
{
    [TestClass]
    class GenreTest
    {
        #region Private Members

        // connecting to server
        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        #endregion Private Members

        #region Test Methods
        [TestMethod]
        public void GenreConstructerTest()
        {
            var genre = new Genre();

            Assert.AreEqual(genre.Id, 0);
            Assert.AreEqual(genre.Name, "");
            Assert.AreEqual(genre.Description, "");
        }

        [TestMethod]
        public void GenrePropertiesTest()
        {
            var genre = new Genre();

            Assert.AreEqual(genre.Id, 0);
            genre.Id = 1;
            Assert.AreEqual(genre.Id, 1);

            Assert.AreEqual(genre.Name, "");
            genre.Name = "Name";
            Assert.AreEqual(genre.Name, "Name");

            Assert.AreEqual(genre.Description, "");
            genre.Description = "Description";
            Assert.AreEqual(genre.Description, "Description");
        }

        [TestMethod]
        public void GenreGenerateInsertStatmentTest()
        {
            var genre = new Genre(1, "Name", "Description");

            Assert.AreEqual(genre.GenerateInsertStatment(), "INSERT INTO Genre (Name, Description) VALUES ('Name', 'Description')");
        }

        [TestMethod]
        public void GenreGenerateUpdateStatementTest()
        {
            var genre = new Genre(1, "Name", "Description");

            Assert.AreEqual(genre.GenerateUpdateStatement(), "UPDATE Genre SET Name = 'Name', Description = 'Description' WHERE Id = 1");
        }

        [TestMethod]
        public void GenreGenerateDeleteStatmentTest()
        {
            var genre = new Genre(1, "Name", "Description");

            Assert.AreEqual(genre.GenerateDeleteStatement(), "DELETE FROM Genre WHERE Id = 1");
        }

        [TestMethod]
        public void GenreGenereateExistQueryTest()
        {
            var genre = new Genre(1, "Name", "Description");

            Assert.AreEqual(genre.GenerateExistQuery(), "SELECT Id FROM Genre WHERE Id = 1");
        }

        [TestMethod]
        public void GenreGenerateSelectQueryTest()
        {
            var genre = new Genre();

            Assert.AreEqual(genre.GenerateSelectQuery(), "SELECT Id, Name, Description");

            genre = new Genre(1, "Name", "Description");

            Assert.AreEqual(genre.GenerateSelectQuery(), "SELECT Id, Name, Description FROM Genre WHERE Id = 1");
        }

        [TestMethod]
        public void GenreGeneratePrimaryKeyWhereClauseTest()
        {
            var genre = new Genre();

            Assert.AreEqual(genre.GeneratePrimaryKeyWhereClause(), "");

            genre = new Genre(1, "Name", "Desciption");

            Assert.AreEqual(genre.GeneratePrimaryKeyWhereClause(), "Id = 1");
        }

        [TestMethod]
        public void GenreDatabaseCommandTest()
        {
            var connection = new SqlServerConnection();

            Assert.AreEqual(connection.IsConnected, false);

            var result = connection.Connect(ConnectionString, 0);

            Assert.AreEqual(result, true);

            //select all 

            var genre = new Genre();

            var selectQuery = genre.GenerateSelectQuery();

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

            genre = new Genre(0, "Name", "Description");

            var insertcommand = genre.GenerateInsertStatment();
            Assert.IsFalse(string.IsNullOrEmpty(insertcommand));

            errorMessage = "";
            var insertresult = connection.ExecuteCommand(insertcommand, ref errorMessage, out var newId);

            Assert.IsTrue(insertresult);
            Assert.IsTrue(newId > 0);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            genre.Id = newId;

            // exist

            var existsQuery = genre.GenerateExistQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            var existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            var existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            var recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);

            // select 

            selectQuery = genre.GenerateSelectQuery();

            Assert.AreEqual(string.IsNullOrEmpty(selectQuery), true);

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.AreEqual(!string.IsNullOrEmpty(errorMessage), false);
            Assert.IsNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            Genre foundgenre = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundgenre = Genre.FromDictionary(dictionary);
                    break;
                }
            }
            Assert.IsNotNull(foundgenre);

            Assert.AreNotSame(genre, foundgenre);

            Assert.AreEqual(genre.Id, foundgenre.Id);
            Assert.AreEqual(genre.Name, foundgenre.Name);
            Assert.AreEqual(genre.Description, foundgenre.Description);

            // update 

            var updategenre = new Genre(1, "Name", "Description");
            var updatecommand = updategenre.GenerateUpdateStatement();

            Assert.IsFalse(string.IsNullOrEmpty(updatecommand));

            errorMessage = "";
            var updateResult = connection.ExecuteCommand(updatecommand, ref errorMessage);

            Assert.AreEqual(updateResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            // exsits

            existsQuery = updategenre.GenerateExistQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.AreEqual(recordExists, true);

            // select

            selectQuery = updategenre.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
            Assert.IsNotNull(selectResult);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundgenre = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundgenre = Genre.FromDictionary(dictionary);
                    break;
                }
            }

            Assert.IsNotNull(foundgenre);

            Assert.AreNotSame(genre, foundgenre);

            Assert.AreEqual(genre.Id, foundgenre.Id);
            Assert.AreEqual(genre.Name, foundgenre.Name);
            Assert.AreEqual(genre.Description, foundgenre.Description);

            // delete 

            var deleteCommand = genre.GenerateDeleteStatement();

            Assert.IsFalse(string.IsNullOrEmpty(deleteCommand));

            errorMessage = "";
            var deleteResult = connection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.AreEqual(deleteResult, true);
            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);

            // exists 

            existsQuery = genre.GenerateExistQuery();

            Assert.IsFalse(string.IsNullOrEmpty(existsQuery));

            errorMessage = "";
            existsResult = connection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.AreEqual(string.IsNullOrEmpty(errorMessage), true);
            Assert.IsNotNull(existsResult);

            existsResultList = DataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.IsFalse(recordExists);

            // select 

            selectQuery = genre.GenerateSelectQuery();

            Assert.IsFalse(string.IsNullOrEmpty(selectQuery));

            errorMessage = "";
            selectResult = connection.ExecuteQuery(selectQuery, ref errorMessage);

            selectResultList = DataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundgenre = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundgenre = Genre.FromDictionary(dictionary);
                    break;
                }
            }
            Assert.IsNull(foundgenre);

        }

        [TestMethod]
        public void GenreBinaryCloneTest()
        {
            var genre1 = new Genre(1, "Name", "Description");

            var genre2 = CloneUtility.BinaryClone(genre1);

            Assert.AreNotSame(genre1, genre2);

            Assert.AreEqual(genre1.Id, genre2.Id);
            Assert.AreEqual(genre1.Name, genre2.Name);
            Assert.AreEqual(genre1.Description, genre2.Description);
        }

        [TestMethod]
        public void GenreXmlCloneTest()
        {
            var genre1 = new Genre(1, "Name", "Description");

            var genre2 = CloneUtility.XmlClone(genre1, null);

            Assert.AreNotSame(genre1, genre2);

            Assert.AreEqual(genre1.Id, genre2.Id);
            Assert.AreEqual(genre1.Name, genre2.Name);
            Assert.AreEqual(genre1.Description, genre2.Description);
        }
        [TestMethod]
        public void GenreJsonTest()
        {
            var genre1 = new Genre(1, "Name", "Description");

            var jsontext = CloneUtility.ToJson(genre1);

            Assert.IsFalse(string.IsNullOrEmpty(jsontext));

            var genre2 = CloneUtility.FromJson<Genre>(jsontext);

            Assert.AreNotSame(genre1, genre2);

            Assert.AreEqual(genre1.Id, genre2.Id);
            Assert.AreEqual(genre1.Name, genre2.Name);
            Assert.AreEqual(genre1.Description, genre2.Description);

        }
        [TestMethod]
        public void GenreDictionaryTest()
        {
            var genre1 = new Genre(1, "Name", "Description");

            var Dictionary = Genre.ToDictionary(genre1);

            Assert.IsNull(Dictionary);

            var genre2 = Genre.FromDictionary(Dictionary);

            Assert.AreNotSame(genre1, genre2);

            Assert.AreEqual(genre1.Id, genre2.Id);
            Assert.AreEqual(genre1.Name, genre2.Name);
            Assert.AreEqual(genre1.Description, genre2.Description);
        }
        [TestMethod]
        public void GenreListConstructerTest()
        {
            var genreList = new GenreList();

            Assert.IsNotNull(genreList);
            Assert.AreEqual(genreList.List.Count, 0);
        }
        [TestMethod]
        public void GenreListGetByIdTest()
        {
            var genreList = new GenreList();

            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();

            genreList.Add(new Genre(1, "Name1", "Description1"));
            genreList.Add(new Genre(2, "Name2", "Description2"));
            genreList.Add(new Genre(3, "Name3", "Description3"));

            var genre = genreList.GetbyId(0);
            Assert.AreEqual(genre, null);

            genre = genreList.GetbyId(-1);
            Assert.AreEqual(genre, null);

            genre = genreList.GetbyId(1);
            Assert.AreEqual(genre.Name, "Name1");

            genre = genreList.GetbyId(2);
            Assert.AreEqual(genre.Name, "Name2");

            genre = genreList.GetbyId(3);
            Assert.AreEqual(genre.Name, "Name3");

        }
        [TestMethod]
        public void GenreListExistsTest()
        {
            var genreList = new GenreList();

            genreList.Add(new Genre(1, "Name1", "Description1"));
            genreList.Add(new Genre(2, "Name2", "Description2"));
            genreList.Add(new Genre(3, "Name3", "Description3"));

            Assert.IsFalse(genreList.Exists(0));
            Assert.IsFalse(genreList.Exists(-1));
            Assert.AreEqual(genreList.Exists(1), true);
            Assert.AreEqual(genreList.Exists(2), true);
            Assert.AreEqual(genreList.Exists(3), true);
        }
        [TestMethod]
        public void GenreListAddandRemoveTest()
        {
            var genreList = new GenreList();

            Assert.AreEqual(genreList.List.Count, 0);

            genreList.Add(new Genre(1, "Name1", "Description1"));

            Assert.AreEqual(genreList.List.Count, 1);

            genreList.Add(new Genre(2, "Name2", "Description2"));

            Assert.AreEqual(genreList.List.Count, 2);

            genreList.Add(new Genre(3, "Name3", "Description3"));

            Assert.AreEqual(genreList.List.Count, 3);

            genreList.Remove(1);

            Assert.AreEqual(genreList.List.Count, 2);

            genreList.Remove(3);

            Assert.AreEqual(genreList.List.Count, 1);

            genreList.Remove(2);

            Assert.AreEqual(genreList.List.Count, 0);
        }
        [TestMethod]
        public void GenreListJsonTest()
        {
            var genreList1 = new GenreList();

            genreList1.Add(new Genre(1, "Name1", "Description1"));
            genreList1.Add(new Genre(2, "Name2", "Description2"));
            genreList1.Add(new Genre(3, "Name3", "Description3"));

            var jsontext = CloneUtility.ToJson(genreList1);

            Assert.IsFalse(string.IsNullOrEmpty(jsontext));

            var genreList2 = CloneUtility.FromJson<GenreList>(jsontext);

            Assert.AreNotSame(genreList1, genreList2);
            Assert.AreEqual(genreList1.List.Count, genreList2.List.Count);

            for (var index = 0; index < genreList1.List.Count; index++)
            {
                Assert.AreEqual(genreList1.List[index].Id, genreList2.List[index].Id);
                Assert.AreEqual(genreList1.List[index].Name, genreList2.List[index].Name);
                Assert.AreEqual(genreList1.List[index].Description, genreList2.List[index].Description);
            }
        }
        [TestMethod]
        public void GenreListBinaryTest()
        {
            var genreList1 = new GenreList();

            genreList1.Add(new Genre(1, "Name1", "Description1"));
            genreList1.Add(new Genre(2, "Name2", "Description2"));
            genreList1.Add(new Genre(3, "Name3", "Description3"));

            var genreList2 = CloneUtility.BinaryClone(genreList1);

            Assert.AreNotSame(genreList1, genreList2);
            Assert.AreEqual(genreList1.List.Count, genreList2.List.Count);

            for (var index = 0; index < genreList1.List.Count; index++)
            {
                Assert.AreEqual(genreList1.List[index].Id, genreList2.List[index].Id);
                Assert.AreEqual(genreList1.List[index].Name, genreList2.List[index].Name);
                Assert.AreEqual(genreList1.List[index].Description, genreList2.List[index].Description);
            }
        }

        [TestMethod]
        public void GenreListXmlCloneTest()
        {
            var genreList1 = new GenreList();

            genreList1.Add(new Genre(1, "Name1", "Description1"));
            genreList1.Add(new Genre(2, "Name2", "Description2"));
            genreList1.Add(new Genre(3, "Name3", "Description3"));

            var genreList2 = CloneUtility.XmlClone(genreList1, null);

            Assert.AreNotSame(genreList1, genreList2);
            Assert.AreEqual(genreList1.List.Count, genreList2.List.Count);

            for (var index = 0; index < genreList1.List.Count; index++)
            {
                Assert.AreEqual(genreList1.List[index].Id, genreList2.List[index].Id);
                Assert.AreEqual(genreList1.List[index].Name, genreList2.List[index].Name);
                Assert.AreEqual(genreList1.List[index].Description, genreList2.List[index].Description);
            }

        }
        [TestMethod]
        public void GenreListDictionaryTest()
        {
            var genreList1 = new GenreList();

            genreList1.Add(new Genre(1, "Name1", "Description1"));
            genreList1.Add(new Genre(2, "Name2", "Description2"));
            genreList1.Add(new Genre(3, "Name3", "Description3"));

            var DictionaryList = GenreList.ToDictionaryList(genreList1);

            Assert.IsNotNull(DictionaryList);

            var genreList2 = GenreList.FromDictionaryList(DictionaryList);

            Assert.AreNotSame(genreList1, genreList2);
            Assert.AreEqual(genreList1.List.Count, genreList2.List.Count);

            for (var index = 0; index < genreList1.List.Count; index++)
            {
                Assert.AreEqual(genreList1.List[index].Id, genreList2.List[index].Id);
                Assert.AreEqual(genreList1.List[index].Name, genreList2.List[index].Name);
                Assert.AreEqual(genreList1.List[index].Description, genreList2.List[index].Description);
            }
        }

        #endregion Test Methods

    }
}
