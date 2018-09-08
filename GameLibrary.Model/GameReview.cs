using Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GameLibrary.Model
{
    [Serializable]
    public class GameReview
    {
        #region Constants

        public const string TableName = "GameReview";

        #endregion Constants

        #region Properties 

        public int Id { get; set; }
        public int GameId { get; set; }
        public int ReviewId { get; set; }

        #endregion Properties 

        #region Constructers 

        public GameReview()
        {
            Id = 0;
            GameId = 0;
            ReviewId = 0;
        }

        public GameReview(int id, int Gameid, int Reviewid)
        {
            Id = id;
            GameId = Gameid;
            ReviewId = Reviewid;
        }

        #endregion Constructers 

        #region Public Methodsds

        public string GenerateInsertStatement()
        {
            var result = "";

            try
            {
                result = $"INSERT INTO {TableName} (GameId, ReviewId) VALUES ({GameId}, {ReviewId})";
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
                    result = $"UPDATE {TableName} SET GameId = {GameId}, ReviewId = {ReviewId} WHERE {whereClause}";
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
                    result = $"SELECT Id, GameId, ReviewId FROM {TableName} WHERE {whereClause}";
                }
                else
                {
                    result = $"SELECT Id, GameId, ReviewId FROM {TableName}";
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

        public static Dictionary<string, object> ToDictionary(GameReview gamerating)
        {
            var result = new Dictionary<string, object>();
            try
            {
                if (gamerating != null)
                {
                    result.Add("Id", gamerating.Id);
                    result.Add("GameId", gamerating.GameId);
                    result.Add("ReviewId", gamerating.ReviewId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public static GameReview FromDictionary(IDictionary<string, object> dictionary)
        {
            var result = new GameReview();

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
                            case "ReviewId":
                                result.ReviewId = Convert.ToInt32(dictionary[key]);
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
    public class GameReviewList
    {
        #region properties

        public List<GameReview> List { get; } = new List<GameReview>();
        public string ErrorMessage { get; set; } = "";

        #endregion properties

        #region Public Methods

        public GameReview GetById(int Id)
        {
            try
            {
                foreach (var gamereview in List.Where(gamereview => (gamereview != null) && (gamereview.Id == Id)))
                {
                    return gamereview;
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
        public bool Add(GameReview gamereview)
        {
            var result = false;
            try
            {
                if (gamereview != null)
                {
                    if (!Exists(gamereview.Id))
                    {
                        List.Add(gamereview);

                        result = Exists(gamereview.Id);
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
            var removegamereview = GetById(id);

            try
            {
                if (removegamereview != null)
                {
                    List.Remove(removegamereview);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        public static string GenerateSelectQueryByReviewId(int reviewId)
        {
            var result = "";
            try
            {
                if (reviewId > 0)
                {
                    result = $"SELECT Id, GameId, ReviewId FROM {GameReview.TableName} WHERE ReviewId = {reviewId}";
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
                    result = $"SELECT Id, GameId, ReviewId FROM {GameReview.TableName} WHERE GameId = {gameId}";
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

        public static List<Dictionary<string, object>> ToDictionaryList(GameReviewList gameGenreList)
        {
            var result = new List<Dictionary<string, object>>();
            try
            {
                if (gameGenreList?.List?.Count > 0)
                {
                    result = (from gamereview in gameGenreList.List where gamereview != null select GameReview.ToDictionary(gamereview) into dictionary where dictionary?.Count > 0 select dictionary).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }
        public static GameReviewList FromDictionaryList(List<Dictionary<string, object>> dictionaryList)
        {
            var result = new GameReviewList();
            try
            {
                if (dictionaryList?.Count > 0)
                {
                    foreach (var dataDictionary in dictionaryList.Where(dataDictionary => dataDictionary?.Count > 0))
                    {
                        result.List.Add(GameReview.FromDictionary(dataDictionary));
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
