using Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace GameLibrary.Model
{
    [Serializable]
    public class GameRating
    {
        #region Constants

        public const string TableName = "GameRating";

        public const int NotesSize = 1000;

        #endregion Constants

        #region Properties 

        public int Id { get; set; }
        public int GameId { get; set; }
        public int RatingId { get; set; }
        public string Notes { get; set; }


        #endregion Properties 

        #region Constructers 

        public GameRating()
        {
            Id = 0;
            GameId = 0;
            RatingId = 0;
            Notes = "";
        }

        public GameRating(int id, int Gameid, int ratingid, string notes)
        {
            Id = id;
            GameId = Gameid;
            RatingId = ratingid;
            Notes = notes;
        }

        #endregion Constructers 

        #region Public Methodsds

        public string GenerateInsertStatement()
        {
            var result = "";

            try
            {
                result = $"INSERT INTO {TableName} (GameId, RatingId, Notes) VALUES ({GameId}, {RatingId}, '{Notes}')";
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
                    result = $"UPDATE {TableName} SET GameId = {GameId}, RatingId = {RatingId}, Notes = '{Notes}' WHERE {whereClause}";
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
                    result = $"SELECT Id, GameId, RatingId, Notes FROM {TableName} WHERE {whereClause}";
                }
                else if ((GameId > 0) && (RatingId > 0))
                {
                    result = $"SELECT Id, GameId, RatingId, Notes FROM {TableName} WHERE GameId = {GameId} AND RatingId = {RatingId}";
                }
                else
                {
                    result = $"SELECT Id, GameId, RatingId, Notes FROM {TableName}";
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

        public static Dictionary<string, object> ToDictionary(GameRating gamerating)
        {
            var result = new Dictionary<string, object>();
            try
            {
                if (gamerating != null)
                {
                    result.Add("Id", gamerating.Id);
                    result.Add("GameId", gamerating.GameId);
                    result.Add("RatingId", gamerating.RatingId);
                    result.Add("Notes", gamerating.Notes);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public static GameRating FromDictionary(IDictionary<string, object> dictionary)
        {
            var result = new GameRating();

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
                            case "RatingId":
                                result.RatingId = Convert.ToInt32(dictionary[key]);
                                break;
                            case "Notes":
                                result.Notes = dictionary[key] as string;
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
    public class GameRatingList
    {
        #region properties

        public List<GameRating> List { get; } = new List<GameRating>();
        public string ErrorMessage { get; set; } = "";

        #endregion properties

        #region Public Methods

        public GameRating GetById(int Id)
        {
            try
            {
                foreach (var gamerating in List.Where(gamerating => (gamerating != null) && (gamerating.Id == Id)))
                {
                    return gamerating;
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
        public bool Add(GameRating gamerating)
        {
            var result = false;
            try
            {
                if (gamerating != null)
                {
                    if (!Exists(gamerating.Id))
                    {
                        List.Add(gamerating);

                        result = Exists(gamerating.Id);
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
            var removegamerating = GetById(id);

            try
            {
                if (removegamerating != null)
                {
                    List.Remove(removegamerating);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public static string GenerateSelectQueryByRatingId(int ratingId)
        {
            var result = "";
            try
            {
                if (ratingId > 0)
                {
                    result = $"SELECT Id, GameId, RatingId FROM {GameRating.TableName} WHERE RatingId = {ratingId}";
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
                    result = $"SELECT Id, GameId, RatingId FROM {GameRating.TableName} WHERE GameId = {gameId}";
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

        public static List<Dictionary<string, object>> ToDictionaryList(GameRatingList gameGenreList)
        {
            var result = new List<Dictionary<string, object>>();
            try
            {
                if (gameGenreList?.List?.Count > 0)
                {
                    result = (from gamerating in gameGenreList.List where gamerating != null select GameRating.ToDictionary(gamerating) into dictionary where dictionary?.Count > 0 select dictionary).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }
        public static GameRatingList FromDictionaryList(List<Dictionary<string, object>> dictionaryList)
        {
            var result = new GameRatingList();
            try
            {
                if (dictionaryList?.Count > 0)
                {
                    foreach (var dataDictionary in dictionaryList.Where(dataDictionary => dataDictionary?.Count > 0))
                    {
                        result.List.Add(GameRating.FromDictionary(dataDictionary));
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

