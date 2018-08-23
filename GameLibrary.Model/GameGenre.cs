using Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GameLibrary.Model
{
    [Serializable]
    public class GameGenre
    {
        #region Constants

        public const string TableName = "GameGenre";
        
        #endregion Constants

        #region Properties 

        public int Id { get; set; }
        public int GameId { get; set; }
        public int GenreId { get; set; }

        #endregion Properties 

        #region Constructers 

        public GameGenre()
        {
            Id = 0;
            GameId = 0;
            GenreId = 0;
        }

        public GameGenre(int id, int Gameid, int Genreid)
        {
            Id = id;
            GameId = Gameid;
            GenreId = Genreid;
        }

        #endregion Constructers 

        #region Public Methodsdsfdsf

        public string GenerateInsertStatement()
        {
            var result = "";

            try
            {
                result = $"INSERT INTO {TableName} (GameId, GenreId) VALUES ({GameId}, {GenreId})";
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

               if (!string.IsNullOrEmpty(whereClause))
                {
                    result = $"UPDATE {TableName} SET GameId = {GameId}, GenreId = {GenreId} WHERE {whereClause}";
                }
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
        public string GenerateExistsQuery()
        {
            var result = "";
            try
            {
                var whereClause = GeneratePrimaryKeyWhereClause();

                if (!string.IsNullOrEmpty(whereClause))
                {
                    result = $"SELECT Id FROM {TableName} WHERE {whereClause}";
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

                if (!string.IsNullOrEmpty(whereClause))
                {
                    result = $"SELECT Id, GameId, GenreId FROM {TableName} WHERE {whereClause}";
                }
                else
                {
                    result = $"SELECT Id, GameId, GenreId FROM {TableName}";
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

        public static Dictionary<string, object> ToDictionary(GameGenre gamegenre)
        {
            var result = new Dictionary<string, object>();
            try
            {
                if (gamegenre != null)
                {
                    result.Add("Id", gamegenre.Id);
                    result.Add("GameId", gamegenre.GameId);
                    result.Add("GenreId", gamegenre.GenreId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }  

        public static GameGenre FromDictionary(IDictionary<string, object> dictionary)
        {
            var result = new GameGenre();

            try
            {
                if ((dictionary != null)&&(dictionary.Count > 0))
                {
                    foreach (var key in dictionary.Keys)
                    {
                        switch (key.Trim())
                        {
                            case "Id":
                                result.Id = Convert.ToInt32(dictionary[key]);
                                break;
                            case "GameId":
                                result.GameId = Convert.ToInt32(dictionary[key]);
                                break;
                            case "GenreId":
                                result.GenreId = Convert.ToInt32(dictionary[key]);
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
    }
    [Serializable]
    public class GameGenreList
    {
        #region properties

        public List<GameGenre> List { get; } = new List<GameGenre>();
        public string ErrorMessage { get; set; } = "";

        #endregion properties

        #region Public Methods

        public GameGenre GetById(int Id)
        {
            try
            {
                foreach (var gamegenre in List.Where(gamegenre => (gamegenre != null) && (gamegenre.Id == Id)))
                {
                    return gamegenre;
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
            return null;
        }
        public bool Exists(int Id)
        {
            var result = false;
            try
            {
                result = (GetById(Id) != null);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }
        public bool Add(GameGenre gamegenre)
        {
            var result = false;
            try
            {
                if (gamegenre != null)
                {
                    if (!Exists(gamegenre.Id))
                    {
                        List.Add(gamegenre);

                        result = Exists(gamegenre.Id);
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
            var removegamegenre = GetById(id);

            try
            {
                if (removegamegenre != null)
                {
                    List.Remove(removegamegenre);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        #endregion Public Methods 

        #region Dictionary Methods

        public static List<Dictionary<string, object>> ToDictionaryList(GameGenreList gameGenreList)
        {
            var result = new List<Dictionary<string, object>>();
            try
            {
                if (gameGenreList?.List?.Count > 0)
                {
                    result = (from gamegenre in gameGenreList.List where gamegenre != null select GameGenre.ToDictionary(gamegenre) into dictionary where dictionary?.Count > 0 select dictionary).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }
        public static GameGenreList FromDictionaryList(List<Dictionary<string, object>> dictionaryList)
        {
            var result = new GameGenreList();
            try
            {
                if (dictionaryList?.Count > 0)
                {
                    foreach (var dataDictionary in dictionaryList.Where(dataDictionary => dataDictionary?.Count > 0))
                    {
                        result.List.Add(GameGenre.FromDictionary(dataDictionary));
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
