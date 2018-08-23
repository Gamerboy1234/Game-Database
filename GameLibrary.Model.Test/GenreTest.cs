using System;
using System.Linq;
using GameLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sql.Server.Connection;
using Utilities;

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
        }

        #endregion Test Methods

    }
}
