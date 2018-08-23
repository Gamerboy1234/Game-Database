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

        #endregion Test Methods

    }
}
