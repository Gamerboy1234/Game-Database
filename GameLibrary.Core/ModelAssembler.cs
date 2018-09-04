using System;
using Logger;
using GameLibrary.Model;
using Sql.Server.Connection;
using Utilities;
using static GameLibrary.Model.Genre;

namespace GameLibrary.Core
{
    public class ModelAssembler
    {
        #region Private Fields

        private SqlServerConnection _databaseConnection;

        private string ConnectionString { get; }

        #endregion Private Fields

        #region Public Fields

        public bool IsDatabaseConnected => _databaseConnection?.IsConnected ?? false;

        #endregion Public Fields

        #region Constructors

        public ModelAssembler(string connectionString)
        {
            try
            {
                ConnectionString = connectionString ?? "";

                ValidateDatabaseConnection();
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        #endregion Constructors

        #region Game Public Methods

        public GameList GetGames()
        {
            var result = new GameList();

            try
            {
                if (ValidateDatabaseConnection())
                {
                    var errorMessage = "";

                    if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new Game().GenerateSelectQuery(), ref errorMessage), out var queryResults))
                    {
                        result = GameList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults)) ?? new GameList();
                    }

                    result.ErrorMessage = errorMessage;
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public Game GetGameById(int id, ref string errorMessage)
        {
            Game result = null;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new Game { Id = id }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                        {
                            var games = GameList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (games?.List?.Count > 0)
                            {
                                result = games.List[0];
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Invalid game id found";
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public Game GetGameByName(string name, ref string errorMessage)
        {
            Game result = null;

            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (ValidateDatabaseConnection())
                    {
                        if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new Game { Name = name ?? "" }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                        {
                            var games = GameList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (games?.List?.Count > 0)
                            {
                                result = games.List[0];
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Invalid game name found";
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool AddOrEditGame(Game game, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (!string.IsNullOrEmpty(game?.Name))
                {
                    if (ValidateDatabaseConnection())
                    {
                        // Edit
                        if (game.Id > 0)
                        {
                            var foundGame = GetGameById(game.Id, ref errorMessage);

                            if (foundGame != null)
                            {
                                result = _databaseConnection.ExecuteCommand(game.GenerateUpdateStatement(), ref errorMessage);
                            }

                            else
                            {
                                errorMessage = $"Unable to find game '{game.Name}' to edit";
                            }
                        }

                        // Add
                        else
                        {
                            var foundGame = GetGameByName(game.Name, ref errorMessage);

                            if (foundGame == null)
                            {
                                result = _databaseConnection.ExecuteCommand(game.GenerateInsertStatement(), ref errorMessage, out int newId);

                                if (result && 
                                    (newId > 0))
                                {
                                    game.Id = newId;
                                }
                            }

                            else
                            {
                                errorMessage = $"A game named '{game.Name}' already exists.  Unable to add game.";
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Invalid game name found";
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool DeleteGame(int id, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        result = _databaseConnection.ExecuteCommand(new Game { Id = id }.GenerateDeleteStatement(), ref errorMessage);
                    }
                }

                else
                {
                    errorMessage = "Invalid game id found";
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        #endregion Game Public Methods

        #region Genre Public Methods

        public GenreList GetGenres()
        {
            var result = new GenreList();

            try
            {
                if (ValidateDatabaseConnection())
                {
                    var errorMessage = "";

                    if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new Genre().GenerateSelectQuery(), ref errorMessage), out var queryResults))
                    {
                        result = GenreList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults)) ?? new GenreList();
                    }

                    result.ErrorMessage = errorMessage;
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public Genre GetGenreById(int id, ref string errorMessage)
        {
            Genre result = null;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new Genre { Id = id }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                        {
                            var genres = GenreList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (genres?.List?.Count > 0)
                            {
                                result = genres.List[0];
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Invalid Genre id found";
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public Genre GetGenreByName(string name, ref string errorMessage)
        {
            Genre result = null;

            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (ValidateDatabaseConnection())
                    {
                        if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new Genre { Name = name ?? "" }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                        {
                            var genres = GenreList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (genres?.List?.Count > 0)
                            {
                                result = genres.List[0];
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Invalid Genre name found";
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool AddOrEditGenre(Genre Genre, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (!string.IsNullOrEmpty(Genre?.Name))
                {
                    if (ValidateDatabaseConnection())
                    {
                        // Edit
                        if (Genre.Id > 0)
                        {
                            var foundGenre = GetGenreById(Genre.Id, ref errorMessage);

                            if (foundGenre != null)
                            {
                                result = _databaseConnection.ExecuteCommand(Genre.GenerateUpdateStatement(), ref errorMessage);
                            }

                            else
                            {
                                errorMessage = $"Unable to find Genre '{Genre.Name}' to edit";
                            }
                        }

                        // Add
                        else
                        {
                            var foundGenre = GetGenreByName(Genre.Name, ref errorMessage);

                            if (foundGenre == null)
                            {
                                result = _databaseConnection.ExecuteCommand(Genre.GenerateInsertStatment(), ref errorMessage, out int newId);

                                if (result &&
                                    (newId > 0))
                                {
                                    Genre.Id = newId;
                                }
                            }

                            else
                            {
                                errorMessage = $"A Genre named '{Genre.Name}' already exists.  Unable to add Genre.";
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Invalid Genre name found";
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool DeleteGenre(int id, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        result = _databaseConnection.ExecuteCommand(new Genre { Id = id }.GenerateDeleteStatement(), ref errorMessage);
                    }
                }

                else
                {
                    errorMessage = "Invalid Genre id found";
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        #endregion Genre Public Methods

        #region Private Methods

        private bool ValidateDatabaseConnection()
        {
            try
            {
                if (!IsDatabaseConnected &&
                    !string.IsNullOrEmpty(ConnectionString))
                {
                    _databaseConnection = new SqlServerConnection(ConnectionString, 0);
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return IsDatabaseConnected;
        }

        #endregion Private Methods
    }
}
