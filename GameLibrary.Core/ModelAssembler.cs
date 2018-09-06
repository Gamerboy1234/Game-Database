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
                            var genre = GenreList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (genre?.List?.Count > 0)
                            {
                                result = genre.List[0];
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Invalid genre name found";
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool AddOrEditGenre(Genre genre, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (!string.IsNullOrEmpty(genre?.Name))
                {
                    if (ValidateDatabaseConnection())
                    {
                        // Edit
                        if (genre.Id > 0)
                        {
                            var foundgenre = GetGenreById(genre.Id, ref errorMessage);

                            if (foundgenre != null)
                            {
                                result = _databaseConnection.ExecuteCommand(genre.GenerateUpdateStatement(), ref errorMessage);
                            }

                            else
                            {
                                errorMessage = $"Unable to find game '{genre.Name}' to edit";
                            }
                        }

                        // Add
                        else
                        {
                            var foundgenre = GetGenreByName(genre.Name, ref errorMessage);

                            if (foundgenre == null)
                            {
                                result = _databaseConnection.ExecuteCommand(genre.GenerateInsertStatment(), ref errorMessage, out int newId);

                                if (result &&
                                    (newId > 0))
                                {
                                    genre.Id = newId;
                                }
                            }

                            else
                            {
                                errorMessage = $"A genre named '{genre.Name}' already exists.  Unable to add genre.";
                            }
                        }
                    }
                }
                else
                {
                    errorMessage = "Invalid genre name found";
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

        #region Rating Public Methods

        public RatingList GetRatings()
        {
            var result = new RatingList();

            try
            {
                if (ValidateDatabaseConnection())
                {
                    var errorMessage = "";

                    if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new Rating().GenerateSelectQuery(), ref errorMessage), out var queryResults))
                    {
                        result = RatingList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults)) ?? new RatingList();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public Rating GetRatingById(int id, ref string errorMessage)
        {
            Rating result = null;
            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new Rating {Id = id}.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                        {
                            var ratings = RatingList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (ratings?.List?.Count > 0)
                            {
                                result = ratings.List[0];
                            }
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

        public Rating GetRatingByName(string name, ref string errorMessage)
        {
            Rating result = null;

            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (ValidateDatabaseConnection())
                    {
                        if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new Rating { Name = name ?? "" }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                        {
                            var rating = RatingList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (rating?.List?.Count > 0)
                            {
                                result = rating.List[0];
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Invalid rating name found";
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool AddOrEditRating(Rating rating, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (!string.IsNullOrEmpty(rating?.Name))
                {
                    if (ValidateDatabaseConnection())
                    {
                        // Edit
                        if (rating.Id > 0)
                        {
                            var foundGame = GetRatingById(rating.Id, ref errorMessage);

                            if (foundGame != null)
                            {
                                result = _databaseConnection.ExecuteCommand(rating.GenerateUpdateStatement(), ref errorMessage);
                            }

                            else
                            {
                                errorMessage = $"Unable to find game '{rating.Name}' to edit";
                            }
                        }

                        // Add
                        else
                        {
                            var foundGame = GetRatingByName(rating.Name, ref errorMessage);

                            if (foundGame == null)
                            {
                                result = _databaseConnection.ExecuteCommand(rating.GenerateInsertStatment(), ref errorMessage, out int newId);

                                if (result &&
                                    (newId > 0))
                                {
                                    rating.Id = newId;
                                }
                            }

                            else
                            {
                                errorMessage = $"A rating named '{rating.Name}' already exists.  Unable to add rating.";
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

        public bool DeleteRating(int id, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        result = _databaseConnection.ExecuteCommand(new Rating { Id = id }.GenerateDeleteStatement(), ref errorMessage);
                    }
                }
                else
                {
                    errorMessage = "Invalid rating id found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }
        #endregion Rating Public Methods

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
