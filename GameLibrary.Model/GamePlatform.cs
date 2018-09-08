using Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GameLibrary.Model
{
    [Serializable]
    public class GamePlatform
    {
        #region Constants

        public const string TableName = "GamePlatform";

        #endregion Constants

        #region Properties 

        public int Id { get; set; }
        public int GameId { get; set; }
        public int PlatformId { get; set; }

        #endregion Properties 

        #region Constructers 

        public GamePlatform()
        {
            Id = 0;
            GameId = 0;
            PlatformId = 0;
        }

        public GamePlatform(int id, int Gameid, int Genreid)
        {
            Id = id;
            GameId = Gameid;
            PlatformId = Genreid;
        }

        #endregion Constructers 

        #region Public Methodsds

        public string GenerateInsertStatement()
        {
            var result = "";

            try
            {
                result = $"INSERT INTO {TableName} (GameId, PlatformId) VALUES ({GameId}, {PlatformId})";
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
                    result = $"UPDATE {TableName} SET GameId = {GameId}, PlatformId = {PlatformId} WHERE {whereClause}";
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
                    result = $"SELECT Id, GameId, PlatformId FROM {TableName} WHERE {whereClause}";
                }
                else
                {
                    result = $"SELECT Id, GameId, PlatformId FROM {TableName}";
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

        public static Dictionary<string, object> ToDictionary(GamePlatform gamegenre)
        {
            var result = new Dictionary<string, object>();
            try
            {
                if (gamegenre != null)
                {
                    result.Add("Id", gamegenre.Id);
                    result.Add("GameId", gamegenre.GameId);
                    result.Add("PlatformId", gamegenre.PlatformId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public static GamePlatform FromDictionary(IDictionary<string, object> dictionary)
        {
            var result = new GamePlatform();

            try
            {
                if ((dictionary != null) && (dictionary.Count > 0))
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
                            case "PlatformId":
                                result.PlatformId = Convert.ToInt32(dictionary[key]);
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
    public class GamePlatformList
    {
        #region properties

        public List<GamePlatform> List { get; } = new List<GamePlatform>();
        public string ErrorMessage { get; set; } = "";

        #endregion properties

        #region Public Methods

        public GamePlatform GetById(int Id)
        {
            try
            {
                foreach (var gamegenre in List.Where(gamegenre => (gamegenre != null) && (gamegenre.Id == Id)))
                {
                    return gamegenre;
                }
            }
            catch (Exception ex)
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
        public bool Add(GamePlatform gamegenre)
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
        public static string GenerateSelectQueryByPlatfromId(int platformId)
        {
            var result = "";
            try
            {
                if (platformId > 0)
                {
                    result = $"SELECT Id, GameId, PlatformId FROM {GamePlatform.TableName} WHERE PlatformId = {platformId}";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public static string GenerateSelectQueryByGameId(int gameId)
        {
            var result = "";
            try
            {
                if (gameId > 0)
                {
                    result = $"SELECT Id, GameId, PlatformId FROM {GamePlatform.TableName} WHERE GameId = {gameId}";
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

            public static List<Dictionary<string, object>> ToDictionaryList(GamePlatformList gameGenreList)
            {
                var result = new List<Dictionary<string, object>>();
                try
                {
                    if (gameGenreList?.List?.Count > 0)
                    {
                        result = (from gamegenre in gameGenreList.List where gamegenre != null select GamePlatform.ToDictionary(gamegenre) into dictionary where dictionary?.Count > 0 select dictionary).ToList();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                return result;
            }
            public static GamePlatformList FromDictionaryList(List<Dictionary<string, object>> dictionaryList)
            {
                var result = new GamePlatformList();
                try
                {
                    if (dictionaryList?.Count > 0)
                    {
                        foreach (var dataDictionary in dictionaryList.Where(dataDictionary => dataDictionary?.Count > 0))
                        {
                            result.List.Add(GamePlatform.FromDictionary(dataDictionary));
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
