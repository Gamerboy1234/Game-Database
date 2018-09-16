using System;
using Logger;
using GameLibrary.Model;
using Sql.Server.Connection;
using Utilities;
using static GameLibrary.Model.Genre;
using GameImageLibrary.Model;

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

        #region Review Public Methods

        public ReviewList GetReviews()
        {
            var result = new ReviewList();

            try
            {
                if (ValidateDatabaseConnection())
                {
                    var errorMessage = "";

                    if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new Review().GenerateSelectQuery(), ref errorMessage), out var queryResults))
                    {
                        result = ReviewList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults)) ?? new ReviewList();
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

        public Review GetReviewByName(string name, ref string errorMessage)
        {
            Review result = null;

            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (ValidateDatabaseConnection())
                    {
                        if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new Review { Name = name ?? "" }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                        {
                            var review = ReviewList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (review?.List?.Count > 0)
                            {
                                result = review.List[0];
                            }
                        }
                    }
                    else
                    {
                        errorMessage = "Invaild review name found";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public Review GetReviewById(int id, ref string errorMessage)
        {
            Review result = null;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        if ((DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new Review { Id = id }.GenerateSelectQuery(), ref errorMessage), out var queryResults)))
                        {
                            var review = ReviewList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (review?.List?.Count > 0)
                            {
                                result = review.List[0];
                            }
                        }
                    }
                    else
                    {
                        errorMessage = "Invalid review Id found";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public bool AddOrEditReview(Review review, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (!string.IsNullOrEmpty(review.Name))
                {
                    if (ValidateDatabaseConnection())
                    {
                        // edit
                        if (review.Id > 0)
                        {
                            var foundreview = GetReviewById(review.Id, ref errorMessage);

                            if (foundreview != null)
                            {
                                result = _databaseConnection.ExecuteCommand(review.GenerateUpdateStatement(), ref errorMessage);
                            }

                            else
                            {
                                errorMessage = $"Unable to find review '{review.Name}' to edit";
                            }
                        }
                        // add
                        else
                        {
                            var foundreview = GetReviewByName(review.Name, ref errorMessage);

                            if (foundreview == null)
                            {
                                result = _databaseConnection.ExecuteCommand(review.GenerateInsertStatment(), ref errorMessage, out int newId);

                                if (result && newId > 0)
                                {
                                    review.Id = newId;
                                }
                            }
                            else
                            {
                                errorMessage = $"A review named '{review.Name}' already exists.  Unable to add review.";
                            }
                        }
                    }
                    else
                    {
                        errorMessage = "Invaild review name found";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public bool DeleteReview(int id, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        result = _databaseConnection.ExecuteCommand(new Review { Id = id }.GenerateDeleteStatement(), ref errorMessage);
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

        #endregion Review Public Methods

        #region Platform Public Methods

        public PlatformList GetPlatforms()
        {
            var result = new PlatformList();

            try
            {

                if (ValidateDatabaseConnection())
                {
                    var errorMessage = "";

                    if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new Platform().GenerateSelectQuery(), ref errorMessage), out var queryResults))
                    {
                        result = PlatformList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults)) ?? new PlatformList();
                    }

                    result.ErrorMessage = errorMessage;
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public Platform GetPlatformById(int id, ref string errorMessage)
        {
            Platform result = null;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new Platform { Id = id }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                        {
                            var platform = PlatformList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (platform?.List?.Count > 0)
                            {
                                result = platform.List[0];
                            }
                        }
                    }
                }
                else
                {
                    errorMessage = "Invalid Platform Found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public Platform GetPlatfomrByName(string name, ref string errorMessage)
        {
            Platform result = null;
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (ValidateDatabaseConnection())
                    {
                       if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new Platform { Name = name ?? ""}.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                        {
                            var platforms = PlatformList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (platforms?.List?.Count > 0)
                            {
                                result = platforms.List[0];
                            }
                        }
                    }
                }
                else
                {
                    errorMessage = "Invalid platform name found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public bool AddOrEditPlatform(Platform platform, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (!string.IsNullOrEmpty(platform?.Name))
                {
                    if (ValidateDatabaseConnection())
                    {
                        // Edit
                        if (platform.Id > 0)
                        {
                            var foundplatform = GetPlatformById(platform.Id, ref errorMessage);

                            if (foundplatform != null)
                            {
                                result = _databaseConnection.ExecuteCommand(platform.GenerateUpdateStatement(), ref errorMessage);
                            }

                            else
                            {
                                errorMessage = $"Unable to find platform '{platform.Name}' to edit";
                            }
                        }

                        // Add
                        else
                        {
                            var foundplatform = GetPlatfomrByName(platform.Name, ref errorMessage);

                            if (foundplatform == null)
                            {
                                result = _databaseConnection.ExecuteCommand(platform.GenerateInsertStatment(), ref errorMessage, out int newId);

                                if (result &&
                                    (newId > 0))
                                {
                                    platform.Id = newId;
                                }
                            }

                            else
                            {
                                errorMessage = $"A platform named '{platform.Name}' already exists.  Unable to add platform.";
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
        
        public bool DeletePlatform(int id, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        result = _databaseConnection.ExecuteCommand(new Platform { Id = id }.GenerateDeleteStatement(), ref errorMessage);
                    }
                }
                else
                {
                    errorMessage = "Invalid platform id found";
                }
                
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        #endregion Platform Public Methods

        #region GameGenre Public Methods

        public GameGenreList GetGameGenres()
        {
            var result = new GameGenreList();

            try
            {
                if (ValidateDatabaseConnection())
                {
                    var errorMessage = "";

                    if ((DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new GameGenre().GenerateSelectQuery(), ref errorMessage), out var queryResults)))
                    {
                        result = GameGenreList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));
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

        public GameGenreList GetGenresOfGame(int gameId)
        {
            var result = new GameGenreList();

            try
            {
                if (gameId > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        var errorMessage = "";

                        if ((DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(GameGenreList.GenerateSelectQueryByGameId(gameId), ref errorMessage), out var queryResults)))
                        {
                            result = GameGenreList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));
                        }
                        result.ErrorMessage = errorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public GameGenreList GetGamesOfGenre(int genreId)
        {
            var result = new GameGenreList();

            try
            {
                if (genreId > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        var errorMessage = "";

                        if ((DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(GameGenreList.GenerateSelectQueryByGenreId(genreId), ref errorMessage), out var queryResults)))
                        {
                            result = GameGenreList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));
                        }
                        result.ErrorMessage = errorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public GameGenre GetGameGenreById(int id, ref string errorMessage)
        {
            GameGenre result = null;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new GameGenre { Id = id }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                        {
                            var gamegenre = GameGenreList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (gamegenre?.List?.Count > 0)
                            {
                               result = gamegenre.List[0];
                            }
                        }

                    }
                }
                else
                {
                    errorMessage = "Invaild GameGenre id found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public GameGenre GetGameGenreByIds(int gameId, int genreId, ref string errorMessage)
        {
            GameGenre result = null;

            try
            {
                if (gameId > 0)
                {
                    if (genreId > 0)
                    {
                        if (ValidateDatabaseConnection())
                        {
                            if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new GameGenre { GameId = gameId, GenreId = genreId }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                            {
                                var gamegenre = GameGenreList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                                if (gamegenre?.List?.Count > 0)
                                {
                                    result = gamegenre.List[0];
                                }
                            }

                        }
                    }
                    else
                    {
                        errorMessage = "Invaild Genre id found";
                    }
                }
                else
                {
                    errorMessage = "Invaild Game id found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public bool AddOrEditGameGenre(GameGenre gameGenre, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (gameGenre.GameId > 0)
                {
                    if (gameGenre.GenreId > 0)
                    {
                        if (ValidateDatabaseConnection())
                        {
                            // Edit
                            if (gameGenre.Id > 0)
                            {
                                var foundgameGenre = GetGameGenreById(gameGenre.Id, ref errorMessage);

                                if (foundgameGenre != null)
                                {
                                    result = _databaseConnection.ExecuteCommand(gameGenre.GenerateUpdateStatement(), ref errorMessage);
                                }

                                else
                                {
                                    errorMessage = $"Unable to find gameGenre '{gameGenre.Id}' to edit";
                                }
                            }

                            // Add
                            else
                            {
                                var foundgameGenre = GetGameGenreByIds(gameGenre.GameId, gameGenre.GenreId, ref errorMessage);

                                if (foundgameGenre == null)
                                {
                                    result = _databaseConnection.ExecuteCommand(gameGenre.GenerateInsertStatement(), ref errorMessage, out int newId);

                                    if (result &&
                                        (newId > 0))
                                    {
                                        gameGenre.Id = newId;
                                    }
                                }

                                else
                                {
                                    errorMessage = $"A game genre with game id '{gameGenre.GameId}' and genre id '{gameGenre.GenreId}' already exists.  Unable to add game genre.";
                                }
                            }
                        }
                    }

                    else
                    {
                        errorMessage = "Invalid genre found";
                    }
                }

                else
                {
                    errorMessage = "Invalid game found";
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool DeleteGameGenre(int id, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        result = _databaseConnection.ExecuteCommand(new GameGenre { Id = id }.GenerateDeleteStatement(), ref errorMessage);
                    }
                }
                else
                {
                    errorMessage = "Invalid GameGenre id Found";
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        #endregion GameGenre Public Methods

        #region GameRating Public Methods

        public GameRatingList GetGameRatings()
        {
            var result = new GameRatingList();

            try
            {
                if (ValidateDatabaseConnection())
                {
                    var errorMessage = "";

                    if ((DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new GameRating().GenerateSelectQuery(), ref errorMessage), out var queryResults)))
                    {
                        result = GameRatingList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));
                    }
                    result.ErrorMessage = errorMessage;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public GameRatingList GetRatingsOfGame(int ratingId)
        {
            var result = new GameRatingList();

            try
            {
                if (ratingId > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        var errorMessage = "";

                        if ((DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(GameRatingList.GenerateSelectQueryByRatingId(ratingId), ref errorMessage), out var queryResults)))
                        {
                            result = GameRatingList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));
                        }
                        result.ErrorMessage = errorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public GameRatingList GetGamesOfRating(int gameId)
        {
            var result = new GameRatingList();

            try
            {
                if (gameId > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        var errorMessage = "";

                        if ((DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(GameRatingList.GenerateSelectQueryByGameId(gameId), ref errorMessage), out var queryResults)))
                        {
                            result = GameRatingList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));
                        }
                        result.ErrorMessage = errorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public GameRating GetGameRatingById(int id, ref string errorMessage)
        {
            GameRating result = null;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new GameRating { Id = id }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                        {
                            var gamerating = GameRatingList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (gamerating?.List?.Count > 0)
                            {
                                result = gamerating.List[0];
                            }
                        }
                    }
                }
                else
                {
                    errorMessage = "Invalid GameRating id found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public GameRating GetGameRatingsByIds(int gameId, int ratingId, ref string errorMessage)
        {
            GameRating result = null;

            try
            {
                if (gameId > 0)
                {
                    if (ratingId > 0)
                    {
                        if (ValidateDatabaseConnection())
                        {
                            if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new GameRating { GameId = gameId, RatingId = ratingId }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                            {
                                var gamerating = GameRatingList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                                if (gamerating?.List?.Count > 0)
                                {
                                    result = gamerating.List[0];
                                }
                            }
                        }
                    }
                    else
                    {
                        errorMessage = "Invalid ratingId found";
                    }
                }
                else
                {
                    errorMessage = "Invalid gameId found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public bool AddOrEditGameRating(GameRating gameRating, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (gameRating.GameId > 0)
                {
                    if (gameRating.RatingId > 0)
                    {
                        if (ValidateDatabaseConnection())
                        {
                            // edit 
                            if (gameRating.Id > 0)
                            {
                                var foundgamerating = GetGameRatingById(gameRating.Id, ref errorMessage);
                                
                                if (foundgamerating != null)
                                {
                                    result = _databaseConnection.ExecuteCommand(gameRating.GenerateUpdateStatement(), ref errorMessage);
                                }
                                else
                                {
                                    errorMessage = $"Unable to find gameRating {gameRating.Id} to edit";
                                }
                            }
                            // add 
                            else
                            {
                                var foundgameratings = GetGameRatingsByIds(gameRating.GameId, gameRating.RatingId, ref errorMessage);

                                if (foundgameratings == null)
                                {
                                    result = _databaseConnection.ExecuteCommand(gameRating.GenerateInsertStatement(), ref errorMessage, out int newId);

                                    if (result && newId > 0)
                                    {
                                        gameRating.Id = newId;
                                    }
                                }
                                else
                                {
                                    errorMessage = $"A game rating with game id '{gameRating.GameId}' and rating id '{gameRating.RatingId}' already exists.  Unable to add game rating.";
                                }
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

        public bool DeleteGameRating(int id, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        result = _databaseConnection.ExecuteCommand(new GameRating { Id = id }.GenerateDeleteStatement(), ref errorMessage);
                    }
                }
                else
                {
                    errorMessage = "Invalid GameRating id Found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        #endregion GameRating Public Methods

        #region GameReview Public Methods 

        public GameReviewList GetGameReviews()
        {
            var result = new GameReviewList();

            try
            {
                if (ValidateDatabaseConnection())
                {
                    var errorMessage = "";

                    if ((DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new GameReview().GenerateSelectQuery(), ref errorMessage), out var queryResults)))
                    {
                        result = GameReviewList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));
                    }
                    result.ErrorMessage = errorMessage;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public GameReviewList GetReviewsOfGame(int reviewId)
        {
            var result = new GameReviewList();

            try
            {
                if (reviewId > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        var errorMessage = "";

                        if ((DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(GameReviewList.GenerateSelectQueryByReviewId(reviewId), ref errorMessage), out var queryResults)))
                        {
                            result = GameReviewList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));
                        }
                        result.ErrorMessage = errorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public GameReviewList GetGamesofReview(int reviewId)
        {
            var result = new GameReviewList();

            try
            {
                if (reviewId > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        var errorMessage = "";

                        if ((DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(GameReviewList.GenerateSelectQueryByGameId(reviewId), ref errorMessage), out var queryResults)))
                        {
                            result = GameReviewList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));
                        }
                        result.ErrorMessage = errorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public GameReview GetGameReviewById(int id, ref string errorMessage)
        {
            GameReview result = null;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new GameReview { Id = id }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                        {
                            var gamereview = GameReviewList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (gamereview?.List?.Count > 0)
                            {
                                result = gamereview.List[0];
                            }
                        }
                    }
                }
                else
                {
                    errorMessage = "Invalid GameReview id found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public GameReview GetGameReviewsByIds(int gameId, int reviewId, ref string errorMessage)
        {
            GameReview result = null;

            try
            {
                if (gameId > 0)
                {
                    if (reviewId > 0)
                    {
                        if (ValidateDatabaseConnection())
                        {
                            if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new GameReview { GameId = gameId, ReviewId = reviewId }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                            {
                                var gameReview = GameReviewList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                                if (gameReview?.List?.Count > 0)
                                {
                                    result = gameReview.List[0];
                                }
                            }
                        }
                    }
                    else
                    {
                        errorMessage = "Invalid reviewId found";
                    }
                }
                else
                {
                    errorMessage = "Invalid gameId found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public bool AddOrEditGameReview(GameReview gameReview, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (gameReview.GameId > 0)
                {
                    if (gameReview.ReviewId > 0)
                    {
                        if (ValidateDatabaseConnection())
                        {
                            // edit 
                            if (gameReview.Id > 0)
                            {
                                var foundgameReview = GetGameReviewById(gameReview.Id, ref errorMessage);

                                if (foundgameReview != null)
                                {
                                    result = _databaseConnection.ExecuteCommand(gameReview.GenerateUpdateStatement(), ref errorMessage);
                                }
                                else
                                {
                                    errorMessage = $"Unable to find gameReview {gameReview.Id} to edit";
                                }
                            }
                            // add 
                            else
                            {
                                var foundgameratings = GetGameReviewsByIds(gameReview.GameId, gameReview.ReviewId, ref errorMessage);

                                if (foundgameratings == null)
                                {
                                    result = _databaseConnection.ExecuteCommand(gameReview.GenerateInsertStatement(), ref errorMessage, out int newId);

                                    if (result && newId > 0)
                                    {
                                        gameReview.Id = newId;
                                    }
                                }
                                else
                                {
                                    errorMessage = $"A game rating with game id '{gameReview.GameId}' and review id '{gameReview.ReviewId}' already exists.  Unable to add game review.";
                                }
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

        public bool DeleteGameReviews(int id, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        result = _databaseConnection.ExecuteCommand(new GameReview { Id = id }.GenerateDeleteStatement(), ref errorMessage);
                    }
                }
                else
                {
                    errorMessage = "Invalid GameReview id Found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        #endregion GameReview Public Methods

        #region GamePlatform Public Methods

        public GamePlatformList GetGamePlatforms()
        {
            var result = new GamePlatformList();

            try
            {
                if (ValidateDatabaseConnection())
                {
                    var errorMessage = "";

                    if ((DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new GamePlatform().GenerateSelectQuery(), ref errorMessage), out var queryResults)))
                    {
                        result = GamePlatformList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));
                    }
                    result.ErrorMessage = errorMessage;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public GamePlatformList GetPlatformsOfGame(int platformId)
        {
            var result = new GamePlatformList();

            try
            {
                if (platformId > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        var errorMessage = "";

                        if ((DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(GamePlatformList.GenerateSelectQueryByPlatfromId(platformId), ref errorMessage), out var queryResults)))
                        {
                            result = GamePlatformList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));
                        }
                        result.ErrorMessage = errorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public GamePlatformList GetGamesofPlatform(int gameId)
        {
            var result = new GamePlatformList();

            try
            {
                if (gameId > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        var errorMessage = "";

                        if ((DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(GamePlatformList.GenerateSelectQueryByGameId(gameId), ref errorMessage), out var queryResults)))
                        {
                            result = GamePlatformList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));
                        }
                        result.ErrorMessage = errorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public GamePlatform GetGamePlatformById(int id, ref string errorMessage)
        {
            GamePlatform result = null;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new GamePlatform { Id = id }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                        {
                            var gameplatform = GamePlatformList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (gameplatform?.List?.Count > 0)
                            {
                                result = gameplatform.List[0];
                            }
                        }
                    }
                }
                else
                {
                    errorMessage = "Invalid GamePlatform id found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public GamePlatform GetGamePlatformsByIds(int gameId, int platformId, ref string errorMessage)
        {
            GamePlatform result = null;

            try
            {
                if (gameId > 0)
                {
                    if (platformId > 0)
                    {
                        if (ValidateDatabaseConnection())
                        {
                            if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new GamePlatform { GameId = gameId, PlatformId = platformId }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                            {
                                var gamePlatform = GamePlatformList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                                if (gamePlatform?.List?.Count > 0)
                                {
                                    result = gamePlatform.List[0];
                                }
                            }
                        }
                    }
                    else
                    {
                        errorMessage = "Invalid platformId found";
                    }
                }
                else
                {
                    errorMessage = "Invalid gameId found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public bool AddOrEditGamePlatform(GamePlatform gamePlatform, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (gamePlatform.GameId > 0)
                {
                    if (gamePlatform.PlatformId > 0)
                    {
                        if (ValidateDatabaseConnection())
                        {
                            // edit 
                            if (gamePlatform.Id > 0)
                            {
                                var foundgamePlatform = GetGamePlatformById(gamePlatform.Id, ref errorMessage);

                                if (foundgamePlatform != null)
                                {
                                    result = _databaseConnection.ExecuteCommand(gamePlatform.GenerateUpdateStatement(), ref errorMessage);
                                }
                                else
                                {
                                    errorMessage = $"Unable to find gamePlatform {gamePlatform.Id} to edit";
                                }
                            }
                            // add 
                            else
                            {
                                var foundgameratings = GetGamePlatformsByIds(gamePlatform.GameId, gamePlatform.PlatformId, ref errorMessage);

                                if (foundgameratings == null)
                                {
                                    result = _databaseConnection.ExecuteCommand(gamePlatform.GenerateInsertStatement(), ref errorMessage, out int newId);

                                    if (result && newId > 0)
                                    {
                                        gamePlatform.Id = newId;
                                    }
                                }
                                else
                                {
                                    errorMessage = $"A game rating with game id '{gamePlatform.GameId}' and platform id '{gamePlatform.PlatformId}' already exists.  Unable to add game review.";
                                }
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

        public bool DeleteGamePlatforms(int id, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        result = _databaseConnection.ExecuteCommand(new GamePlatform { Id = id }.GenerateDeleteStatement(), ref errorMessage);
                    }
                }
                else
                {
                    errorMessage = "Invalid GamePlatform id Found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }


        #endregion GamePlatform Public Methods 

        #region GameImage Public Methods

        public GameImageList GetGameImages()
        {
            var result = new GameImageList();

            try
            {
                if (ValidateDatabaseConnection())
                {
                    var errorMessage = "";

                    if ((DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new GameImage().GenerateSelectQuery(), ref errorMessage), out var queryResults)))
                    {
                        result = GameImageList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));
                    }
                    result.ErrorMessage = errorMessage;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }

        public GameImageList GetGamesOfImage(int gameId)
        {
            var result = new GameImageList();

            try
            {
                if (gameId > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        var errorMessage = "";

                        if ((DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(GameImageList.GenerateSelectQueryByGameId(gameId), ref errorMessage), out var queryResults)))
                        {
                            result = GameImageList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));
                        }
                        result.ErrorMessage = errorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public GameImage GetGameImageById(int id, ref string errorMessage)
        {
            GameImage result = null;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        if (DataSetUtility.ValidateQueryResults(_databaseConnection.ExecuteQuery(new GameImage { Id = id }.GenerateSelectQuery(), ref errorMessage), out var queryResults))
                        {
                            var gameImage = GameImageList.FromDictionaryList(DataSetUtility.ToDictionaryList(queryResults));

                            if (gameImage?.List?.Count > 0)
                            {
                                result = gameImage.List[0];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }
            return result;
        }

        public bool AddOrEditGameImage(GameImage gameImage, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (gameImage.GameId > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        // Edit
                        if (gameImage.Id > 0)
                        {
                            var foundGame = GetGameImageById(gameImage.Id, ref errorMessage);

                            if (foundGame != null)
                            {
                                result = _databaseConnection.ExecuteCommand(gameImage.GenerateUpdateStatement(), ref errorMessage);
                            }

                            else
                            {
                                errorMessage = $"Unable to find gameimage '{gameImage.Id}' to edit";
                            }
                        }

                        // Add
                        else
                        {
                            var foundGame = GetGameImageById(gameImage.Id, ref errorMessage);

                            if (foundGame == null)
                            {
                                result = _databaseConnection.ExecuteCommand(gameImage.GenerateInsertStatment(), ref errorMessage, out int newId);

                                if (result &&
                                    (newId > 0))
                                {
                                    gameImage.Id = newId;
                                }
                            }

                            else
                            {
                                errorMessage = $"A gameImage with id '{gameImage.Id}' already exists.  Unable to add gameImage.";
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Invalid gameImage id found";
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool DeleteGameImage(int id, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (id > 0)
                {
                    if (ValidateDatabaseConnection())
                    {
                        result = _databaseConnection.ExecuteCommand(new GameImage { Id = id }.GenerateDeleteStatement(), ref errorMessage);
                    }
                }
                else
                {
                    errorMessage = "Invalid GameImage id Found";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return result;
        }


        #endregion GameImage Public Methods 

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
