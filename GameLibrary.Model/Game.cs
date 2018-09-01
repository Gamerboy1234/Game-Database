
using Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GameLibrary.Model
{
    [Serializable]
    public class Game
    {
        #region Constants

        public const string TableName = "Game";

        public const int NameSize = 100;
        public const int DescriptionSize = 1000;

        #endregion Constants


        #region Properties

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        #endregion Properties


        #region Constructors

        public Game()
        {
            Id = 0;
            Name = "";
            Description = "";
        }

        public Game(
            int id,
            string name,
            string description)
        {
            Id = id;
            Name = name ?? "";
            Description = description ?? "";
        }

        #endregion Constructors


        #region Public Methods

        public string GenerateInsertStatement()
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

                if (!string.IsNullOrEmpty(whereClause))
                {
                    result = $"UPDATE {TableName} SET Name = '{Name}', Description = '{Description}' WHERE {whereClause}";
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

        public static Dictionary<string, object> ToDictionary(Game game)
        {
            var result = new Dictionary<string, object>();

            try
            {
                if (game != null)
                {
                    result.Add("Id", game.Id);
                    result.Add("Name", game.Name ?? "");
                    result.Add("Description", game.Description ?? "");
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        public static Game FromDictionary(IDictionary<string, object> dictionary)
        {
            var result = new Game();

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
    }

    [Serializable]
    public class GameList
    {
        #region Properties

        public List<Game> List { get; } = new List<Game>();
        public string ErrorMessage { get; set; } = "";

        #endregion Properties


        #region Public Methods

        public Game GetById(int id)
        {
            try
            {
                foreach (var game in List.Where(game => (game != null) && (game.Id == id)))
                {
                    return game;
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return null;
        }

        public Game GetByName(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    foreach (var game in List.Where(game => (game != null) && string.Equals(game.Name, name)))
                    {
                        return game;
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
                result = (GetById(id) != null);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        public bool Add(Game game)
        {
            var result = false;

            try
            {
                if (game != null)
                {
                    if (!Exists(game.Id))
                    {
                        List.Add(game);

                        result = Exists(game.Id);
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
                var removeGame = GetById(id);

                if (removeGame != null)
                {
                    List.Remove(removeGame);
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        #endregion Public Methods


        #region Dictionary Methods

        public static List<Dictionary<string, object>> ToDictionaryList(GameList gameList)
        {
            var result = new List<Dictionary<string, object>>();

            try
            {
                if (gameList?.List?.Count > 0)
                {
                    // Slow way
                    /*foreach (var game in gameList.List)
                    {
                        if (game != null)
                        {
                            var gameDictionary = Game.ToDictionary(game);

                            if (gameDictionary?.Count > 0)
                            {
                                result.Add(gameDictionary);
                            }
                        }
                    }*/

                    // fast way
                    result = (from game in gameList.List where game != null select Game.ToDictionary(game) into dictionary where dictionary?.Count > 0 select dictionary).ToList();
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        public static GameList FromDictionaryList(List<Dictionary<string, object>> dictionaryList)
        {
            var result = new GameList();

            try
            {
                if (dictionaryList?.Count > 0)
                {
                    foreach (var dataDictionary in dictionaryList.Where(dataDictionary => dataDictionary?.Count > 0))
                    {
                        result.List.Add(Game.FromDictionary(dataDictionary));
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
