using Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GameImageLibrary.Model
{
    [Serializable]
    public class GameImage
    {
        #region Constants

        public const string TableName = "GameImage";

        #endregion Constants

        #region Properties 

        public int Id { get; set; }
        public int GameId { get; set; }
        

        #endregion Properties 

        #region Constructers 

        public GameImage()
        {
            Id = 0;
            GameId = 0;
            
        }

        public GameImage(int id, int GameImageid)
        {
            Id = id;
            GameId = GameImageid;
            
        }

        #endregion Constructers 

        #region Public Methodsds

        public string GenerateInsertStatment()
        {
            var result = "";

            try
            {
                result = $"INSERT INTO {TableName} (GameId) VALUES ({GameId})";
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
                    result = $"UPDATE {TableName} SET GameId = {GameId} WHERE {whereClause}";
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
                    result = $"SELECT Id, GameId FROM {TableName} WHERE {whereClause}";
                }
                else
                {
                    result = $"SELECT Id, GameId FROM {TableName}";
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

        public static Dictionary<string, object> ToDictionary(GameImage gameImage)
        {
            var result = new Dictionary<string, object>();
            try
            {
                if (gameImage != null)
                {
                    result.Add("Id", gameImage.Id);
                    result.Add("GameId", gameImage.GameId);
                    
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public static GameImage FromDictionary(IDictionary<string, object> dictionary)
        {
            var result = new GameImage();

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
    public class GameImageList
    {
        #region Properties

        public List<GameImage> List { get; } = new List<GameImage>();
        public string ErrorMessage { get; set; } = "";

        #endregion Properties

        #region Public Methods

        public GameImage GetById(int id)
        {
            try
            {
                foreach (var gameimage in List.Where(gameimage => (gameimage != null) && (gameimage.Id == id)))
                {
                    return gameimage;
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
                result = (GetById(id) != null);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        public bool Add(GameImage gameimage)
        {
            var result = false;

            try
            {
                if (gameimage != null)
                {
                    if (!Exists(gameimage.Id))
                    {
                        List.Add(gameimage);

                        result = Exists(gameimage.Id);
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
                var removeGameImage = GetById(id);

                if (removeGameImage != null)
                {
                    List.Remove(removeGameImage);
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        #endregion Public Methods

        #region Dictionary Methods

        public static List<Dictionary<string, object>> ToDictionaryList(GameImageList gameimageList)
        {
            var result = new List<Dictionary<string, object>>();

            try
            {
                if (gameimageList?.List?.Count > 0)
                {
                    
                    result = (from gameimage in gameimageList.List where gameimage != null select GameImage.ToDictionary(gameimage) into dictionary where dictionary?.Count > 0 select dictionary).ToList();
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        public static GameImageList FromDictionaryList(List<Dictionary<string, object>> dictionaryList)
        {
            var result = new GameImageList();

            try
            {
                if (dictionaryList?.Count > 0)
                {
                    foreach (var dataDictionary in dictionaryList.Where(dataDictionary => dataDictionary?.Count > 0))
                    {
                        result.List.Add(GameImage.FromDictionary(dataDictionary));
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
