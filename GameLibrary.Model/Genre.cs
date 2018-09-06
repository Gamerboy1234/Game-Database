using Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GameLibrary.Model
{
    [Serializable]
    public class Genre
    {
        #region Constants

        public const string TableName = "Genre";

        public const int NameSize = 100;
        public const int DescriptionSize = 1000;

        #endregion Constants 

        #region Properties 

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        #endregion Properties 

        #region Constructers 

        public Genre()
        {
            Id = 0;
            Name = "";
            Description = "";
        }

        public Genre(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        #endregion Constructers

        #region Public Methods

        public string GenerateInsertStatment()
        {
            var result = "";

            try
            {
                result = $"INSERT INTO {TableName} (Name, Description) VALUES ('{Name}', '{Description}')";
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public string GenerateUpdateStatement()
        {
            var result = "";
            try
            {
                var whereClause = GeneratePrimaryKeyWhereClause();

                result = $"UPDATE {TableName} SET Name = '{Name}', Description = '{Description}' WHERE {whereClause}";
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public string GenerateDeleteStatement()
        {
            var result = "";

            try
            {
                var whereClause = GeneratePrimaryKeyWhereClause();

                if (!string.IsNullOrEmpty(whereClause))
                {
                    result = $"DELETE FROM {TableName} WHERE {whereClause}";
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        public string GenerateExistQuery()
        {
            var result = "";

            try
            {
                var whereclause = GeneratePrimaryKeyWhereClause();

                if (!string.IsNullOrEmpty(whereclause))
                {
                    result = $"Select Id FROM {TableName} WHERE {whereclause}";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        public string GenerateSelectQuery()
        {
            var result = "";

            try
            {
                var whereClause = GeneratePrimaryKeyWhereClause();

                if (string.IsNullOrEmpty(whereClause) &&
                    !string.IsNullOrEmpty(Name))
                {
                    whereClause = $"Name = '{Name}'";
                }

                if (!string.IsNullOrEmpty(whereClause))
                {
                    result = $"SELECT Id, Name, Description FROM {TableName} WHERE {whereClause}";
                }

                else
                {
                    result = $"SELECT Id, Name, Description FROM {TableName}";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

            public string GeneratePrimaryKeyWhereClause()
        {
            var result = "";

            try
            {
                if (Id > 0)
                {
                    result = $"Id = {Id}";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        #endregion Public Methods


        #region Dictionary Methods

        public static Dictionary<string, object> ToDictionary(Genre genre)
        {
            var result = new Dictionary<string, object>();

            try
            {
                if (genre != null)
                {
                    result.Add("Id", genre.Id);
                    result.Add("Name", genre.Name ?? "");
                    result.Add("Name", genre.Description ?? "");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public static Genre FromDictionary(IDictionary<string, object> dictionary)
        {
            var result = new Genre();

            try
            {
                if (dictionary?.Count > 0)
                {
                    foreach (var key in dictionary.Keys)
                    {
                        switch (key.Trim())
                        {
                            case "Id":
                                result.Id = Convert.ToInt32(dictionary[key]);
                                break;
                            case "Name":
                                result.Name = dictionary[key] as string;
                                break;
                            case "Description":
                                result.Description = dictionary[key] as string;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        #endregion Dictionary Methods

        [Serializable]
        public class GenreList
        {
            #region properties

            public List<Genre> List { get; } = new List<Genre>();
            public string ErrorMessage { get; set; } = "";

            #endregion properties

            #region Public Methods

            public Genre GetbyId(int id)
            {
                try
                {
                    foreach (var genre in List.Where(genre => (genre != null) && (genre.Id == id)))
                    {
                        return genre;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                return null;
                
            }
            public Genre GetByName(string name)
            {
                try
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        foreach (var genre in List.Where(genre => (genre != null) && string.Equals(genre.Name, name)))
                        {
                            return genre;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                return null;
            }

            public bool Exists(int id)
            {
                var result = false;
                try
                {
                    result = (GetbyId(id) != null);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                return result;
            }

            public bool Add(Genre genre)
            {
                var result = false;

                try
                {
                    if (genre != null)
                    {
                        if (!Exists(genre.Id))
                        {
                            List.Add(genre);

                            result = Exists(genre.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                return result;
            }

            public void Remove(int id)
            {
                try
                {
                    var RemoveGenre = GetbyId(id);

                    if (RemoveGenre != null)
                    {
                        List.Remove(RemoveGenre);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                
            }

            #endregion Public Methods

            #region Dictionary Methods

            public static List<Dictionary<string, object>> ToDictionaryList(GenreList genrelist)
            {
                var result = new List<Dictionary<string, object>>();

                try
                {
                    result = (from genre in genrelist.List where genre != null select Genre.ToDictionary(genre) into dictionary where dictionary?.Count > 0 select dictionary).ToList();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                return result;
            }

            public static GenreList FromDictionaryList(List<Dictionary<string, object>> dictionaryList)
            {
                var result = new GenreList();

                try
                {
                    if (dictionaryList?.Count > 0)
                    {
                        foreach (var dataDictionary in dictionaryList.Where(dataDictionary => dataDictionary.Count > 0))
                        {
                            result.List.Add(Genre.FromDictionary(dataDictionary));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                return result;
            }

            #endregion Dictionary Methods 

        }

    }
}
