
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameImageLibrary.Model;
using Gamelibrary;
using GameLibrary.Model;
using Google.Protobuf;
using Grpc.Core;
using Logger;
using Utilities;
using static GameLibrary.Model.Genre;

namespace GameLibrary.gRPC.Client
{
    public class GameLibraryClient
    {
        #region Private Fields

        private readonly Gamelibrary.GameLibrary.GameLibraryClient _client;

        #endregion Private Fields

        #region Constructors

        public GameLibraryClient(Gamelibrary.GameLibrary.GameLibraryClient client)
        {
            _client = client;
        }
        
        #endregion Constructors

        #region Game Methods

        public GameList SearchGames(long gameId, string gameName)
        {
            var result = new GameList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGamesAsync(gameId, gameName));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public bool AddGame(Game game, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameResult = _client.AddGame(GrpcGame(game));

                    if (gameResult != null)
                    {
                        result = gameResult.Success;
                        errorMessage = gameResult.ErrorMessage;
                        game.Id = (int)(gameResult.Game?.GameId ?? 0);
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool EditGame(Game game, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameResult = _client.EditGame(GrpcGame(game));

                    if (gameResult != null)
                    {
                        result = gameResult.Success;
                        errorMessage = gameResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool DeleteGame(long gameId, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameResult = _client.DeleteGame(GrpcGame(new Game { Id = (int)gameId }));

                    if (gameResult != null)
                    {
                        result = gameResult.Success;
                        errorMessage = gameResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        #endregion Game Methods

        #region Genre Methods
        public GenreList SearchGenres(long genreId, string genreName)
        {
            var result = new GenreList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGenreAsync(genreId, genreName));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public bool AddGenre(Genre genre, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameResult = _client.AddGenre(GrpcGenre(genre));

                    if (gameResult != null)
                    {
                        result = gameResult.Success;
                        errorMessage = gameResult.ErrorMessage;
                        genre.Id = (int)(gameResult.Genre?.GenreId ?? 0);
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool EditGenre(Genre genre, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var genreResult = _client.EditGenre(GrpcGenre(genre));

                    if (genreResult != null)
                    {
                        result = genreResult.Success;
                        errorMessage = genreResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool DeleteGenre(long genreId, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var genreResult = _client.DeleteGenre(GrpcGenre(new Genre { Id = (int)genreId }));

                    if (genreResult != null)
                    {
                        result = genreResult.Success;
                        errorMessage = genreResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }
        #endregion Genre Methods

        #region Rating Methods

        public RatingList SearchRatings(long ratingId, string ratingName)
        {
            var result = new RatingList();

            try
            {
                result = AsyncHelper.RunSync(() => SerachRatingAsync(ratingId, ratingName));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public bool AddRating(Rating rating, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var ratingResult = _client.AddRating(GrpcRating(rating));

                    if (ratingResult != null)
                    {
                        result = ratingResult.Success;
                        errorMessage = ratingResult.ErrorMessage;
                        rating.Id = (int)(ratingResult.Rating?.RatingId ?? 0);
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool EditRating(Rating rating, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var ratingResult = _client.EditRating(GrpcRating(rating));

                    if (ratingResult != null)
                    {
                        result = ratingResult.Success;
                        errorMessage = ratingResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool DeleteRating(long ratingId, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var ratingResult = _client.DeleteRating(GrpcRating(new Rating { Id = (int)ratingId }));

                    if (ratingResult != null)
                    {
                        result = ratingResult.Success;
                        errorMessage = ratingResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        #endregion Rating Methods

        #region Review Methods

        public ReviewList SearchReview(long reviewId, string reviewName)
        {
            var result = new ReviewList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchReviewAsync(reviewId, reviewName));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public bool AddReview(Review review, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var reviewResult = _client.AddReview(GrpcReview(review));

                    if (reviewResult != null)
                    {
                        result = reviewResult.Success;
                        errorMessage = reviewResult.ErrorMessage;
                        review.Id = (int)(reviewResult.Review?.ReviewId ?? 0);
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool EditReview(Review review, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var reviewResult = _client.EditReview(GrpcReview(review));

                    if (reviewResult != null)
                    {
                        result = reviewResult.Success;
                        errorMessage = reviewResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool DeleteReview(long reviewId, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var reviewResult = _client.DeleteReview(GrpcReview(new Review { Id = (int)reviewId}));

                    if (reviewResult != null)
                    {
                        result = reviewResult.Success;
                        errorMessage = reviewResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        #endregion Review Methods

        #region Platform Methods

        public PlatformList SearchPlatforms(long platformId, string platformName)
        {
            var result = new PlatformList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchPlatformAsync(platformId, platformName));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public bool AddPlatform(Platform platform, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var platformResult = _client.AddPlatform(GrpcPlatform(platform));

                    if (platformResult != null)
                    {
                        result = platformResult.Success;
                        errorMessage = platformResult.ErrorMessage;
                        platform.Id = (int)(platformResult.Platform?.PlatformId ?? 0);
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool EditPlatform(Platform platform, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var platformResult = _client.EditPlatform(GrpcPlatform(platform));

                    if (platformResult != null)
                    {
                        result = platformResult.Success;
                        errorMessage = platformResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool DeletePlatform(long platformeId, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var platformResult = _client.DeletePlatform(GrpcPlatform(new Platform { Id = (int)platformeId }));

                    if (platformResult != null)
                    {
                        result = platformResult.Success;
                        errorMessage = platformResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        #endregion Platform Methods

        #region GameGenre Methods

        public GameGenreList SearchGameGenreByGenreId(long genreId)
        {
            var result = new GameGenreList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGameGenreByGenreIdAsync(genreId));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public GameGenreList SearchGameGenreByGameId(long gameId)
        {
            var result = new GameGenreList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGameGenreByGameIdAsync(gameId));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public GameGenreList SearchGameGenres(long gameGenreId)
        {
            var result = new GameGenreList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGameGenresAsync(gameGenreId));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public bool AddGameGenre(GameGenre gameGenre, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gamegenreResult = _client.AddGameGenre(GrpcGameGenre(gameGenre));

                    if (gamegenreResult != null)
                    {
                        result = gamegenreResult.Success;
                        errorMessage = gamegenreResult.ErrorMessage;
                        gameGenre.Id = (int)(gamegenreResult.Gamegenre?.GamegenreId ?? 0);
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool EditGameGenre(GameGenre gameGenre, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gamegenreResult = _client.EditGameGenre(GrpcGameGenre(gameGenre));

                    if (gamegenreResult != null)
                    {
                        result = gamegenreResult.Success;
                        errorMessage = gamegenreResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool DeleteGameGenre(long gamegenreId, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gamegenreResult = _client.DeleteGameGenre(GrpcGameGenre(new GameGenre { Id = (int)gamegenreId}));

                    if (gamegenreResult != null)
                    {
                        result = gamegenreResult.Success;
                        errorMessage = gamegenreResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        #endregion GameGenre Methods

        #region GamePlatform Methods

        public GamePlatformList SearchGamePlatformByPlatformId(long platformId)
        {
            var result = new GamePlatformList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGamePlatformByPlatformIdAsync(platformId));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public GamePlatformList SearchGamePlatformByGameId(long gameId)
        {
            var result = new GamePlatformList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGamePlatformByGameIdAsync(gameId));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public GamePlatformList SearchGamePlatforms(long gamePlatformId)
        {
            var result = new GamePlatformList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGamePlatformsAsync(gamePlatformId));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public bool AddGamePlatform(GamePlatform gamePlatform, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameplatformResult = _client.AddGamePlatform(GrpcGamePlatform(gamePlatform));

                    if (gameplatformResult != null)
                    {
                        result = gameplatformResult.Success;
                        errorMessage = gameplatformResult.ErrorMessage;
                        gamePlatform.Id = (int)(gameplatformResult.Gameplatform?.GameplatformId ?? 0);
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool EditGamePlatform(GamePlatform gamePlatform, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameplatformResult = _client.EditGamePlatform(GrpcGamePlatform(gamePlatform));

                    if (gameplatformResult != null)
                    {
                        result = gameplatformResult.Success;
                        errorMessage = gameplatformResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool DeleteGamePlatform(long gameplatformId, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameplatformResult = _client.DeleteGamePlatform(GrpcGamePlatform(new GamePlatform { Id = (int)gameplatformId }));

                    if (gameplatformResult != null)
                    {
                        result = gameplatformResult.Success;
                        errorMessage = gameplatformResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        #endregion GamePlatform Methods

        #region GameReview Methods

        public GameReviewList SearchGameReviews(long gameReview)
        {
            var result = new GameReviewList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGameReviewsAsync(gameReview));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public GameReviewList SearchGameReviewsByGameId(long gameReview)
        {
            var result = new GameReviewList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGameReviewByGameId(gameReview));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public GameReviewList SearchGameReviewsByReviewId(long gameReview)
        {
            var result = new GameReviewList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGameReviewByReviewId(gameReview));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public bool AddGameReview(GameReview gameReview, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameplatformResult = _client.AddGameReview(GrpcGameReview(gameReview));

                    if (gameplatformResult != null)
                    {
                        result = gameplatformResult.Success;
                        errorMessage = gameplatformResult.ErrorMessage;
                        gameReview.Id = (int)(gameplatformResult.Gamereview?.GamereviewId ?? 0);
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool EditGameReview(GameReview gameReview, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameplatformResult = _client.EditGameReview(GrpcGameReview(gameReview));

                    if (gameplatformResult != null)
                    {
                        result = gameplatformResult.Success;
                        errorMessage = gameplatformResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool DeleteGameReview(long gameplatformId, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameplatformResult = _client.DeleteGameReview(GrpcGameReview(new GameReview { Id = (int)gameplatformId }));

                    if (gameplatformResult != null)
                    {
                        result = gameplatformResult.Success;
                        errorMessage = gameplatformResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        #endregion GameReview Methods

        #region GameRating Methods

        public GameRatingList SearchGameRatingByRatingId(long ratingId)
        {
            var result = new GameRatingList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGameRatingByRatingIdAsync(ratingId));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public GameRatingList SearchGameRatingByGameId(long gameId)
        {
            var result = new GameRatingList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGameRatingByGameIdAsync(gameId));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public GameRatingList SearchGameRatings(long gameRating)
        {
            var result = new GameRatingList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGameRatingsAsync(gameRating));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public bool AddGameRating(GameRating gameRating, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameplatformResult = _client.AddGameRating(GrpcGameRating(gameRating));

                    if (gameplatformResult != null)
                    {
                        result = gameplatformResult.Success;
                        errorMessage = gameplatformResult.ErrorMessage;
                        gameRating.Id = (int)(gameplatformResult.Gamerating?.GameratingId ?? 0);
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool EditGameRating(GameRating gameRating, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameplatformResult = _client.EditGameRating(GrpcGameRating(gameRating));

                    if (gameplatformResult != null)
                    {
                        result = gameplatformResult.Success;
                        errorMessage = gameplatformResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool DeleteGameRating(long gameplatformId, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameplatformResult = _client.DeleteGameRating(GrpcGameRating(new GameRating { Id = (int)gameplatformId }));

                    if (gameplatformResult != null)
                    {
                        result = gameplatformResult.Success;
                        errorMessage = gameplatformResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        #endregion GameRating Methods

        #region GameImage Methods

        public GameImageList SearchGameImages(long gameImageId)
        {
            var result = new GameImageList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGameImagesAsync(gameImageId));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public GameImageList SearchGameImageByGameId(long gameId)
        {
            var result = new GameImageList();

            try
            {
                result = AsyncHelper.RunSync(() => SearchGameImagesByGameIdAsync(gameId));
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public bool AddGameImage(GameImage gameImage, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameImageResult = _client.AddGameImage(GrpcGameImage(gameImage));

                    if (gameImageResult != null)
                    {
                        result = gameImageResult.Success;
                        errorMessage = gameImageResult.ErrorMessage;
                        gameImage.Id = (int)(gameImageResult.Gameimage?.GameimageId ?? 0);
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool EditGameImage(GameImage gameImage, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameImageResult = _client.EditGameImage(GrpcGameImage(gameImage));

                    if (gameImageResult != null)
                    {
                        result = gameImageResult.Success;
                        errorMessage = gameImageResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        public bool DeleteGameImage(long gameImageId, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_client != null)
                {
                    var gameImageResult = _client.DeleteGameImage(GrpcGameImage(new GameImage { Id = (int)gameImageId }));

                    if (gameImageResult != null)
                    {
                        result = gameImageResult.Success;
                        errorMessage = gameImageResult.ErrorMessage;
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                errorMessage = ex.Message;
            }

            return result;
        }

        #endregion GameImage Methods

        #region Private Methods

        private async Task<GameList> SearchGamesAsync(long gameId, string gameName)
        {
            var result = new GameList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var gameResult = _client.SearchGames(new GamesSearchRequest
                    {
                        GameId = gameId,
                        GameName = gameName ?? ""
                    }))
                    {
                        var responseStream = gameResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var gameRecord = responseStream.Current;

                            if (!string.IsNullOrEmpty(gameRecord?.Name))
                            {
                                result.Add(new Game(
                                    (int)gameRecord.GameId,
                                    gameRecord.Name ?? "",
                                    gameRecord.Description ?? ""));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private static GameRecord GrpcGame(Game game)
        {
            var result = new GameRecord();

            try
            {
                if (game != null)
                {
                    result = new GameRecord
                    {
                        GameId = game.Id,
                        Name = game.Name ?? "",
                        Description = game.Description ?? ""
                    };
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }
       
        private static RatingRecord GrpcRating(Rating rating)
        {
            var result = new RatingRecord();

            try
            {
                if (rating != null)
                {
                    result = new RatingRecord
                    {
                        RatingId = rating.Id,
                        Name = rating.Name ?? "",
                        Description = rating.Description ?? "",
                        Symbol = rating.Symbol 
                    };
                }
            }
            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        private static GenreRecord GrpcGenre(Genre genre)
        {
            var result = new GenreRecord();

            try
            {
                if (genre != null)
                {
                    result = new GenreRecord
                    {
                        GenreId = genre.Id,
                        Name = genre.Name ?? "",
                        Description = genre.Description ?? "",
                    };
                }
            }
            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        private async Task<RatingList> SerachRatingAsync(long ratingId, string ratingName)
        {
            var result = new RatingList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var reviewResult = _client.SearchRatings(new RatingsSearchRequest
                    {
                        RatingId = ratingId,
                        RatingName = ratingName ?? ""
                    }))
                    {
                        var responseStream = reviewResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var ratingRecord = responseStream.Current;

                            if (!string.IsNullOrEmpty(ratingRecord?.Name))
                            {
                                result.Add(new Rating(
                                    (int)ratingRecord.RatingId,
                                    ratingRecord.Name ?? "",
                                    ratingRecord.Description ?? "", ratingRecord.Symbol));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }
     
        private async Task<GenreList> SearchGenreAsync(long genreId, string genreName)
        {
            var result = new GenreList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var genreResult = _client.SearchGenres(new GenresSearchRequest
                    {
                        GenreId = genreId,
                        GenreName = genreName ?? ""
                    }))
                    {
                        var responseStream = genreResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var genreRecord = responseStream.Current;

                            if (!string.IsNullOrEmpty(genreRecord?.Name))
                            {
                                result.Add(new Genre(
                                    (int)genreRecord.GenreId,
                                    genreRecord.Name ?? "",
                                    genreRecord.Description ?? ""));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private async Task<ReviewList> SearchReviewAsync(long reviewId, string reviewName)
        {
            var result = new ReviewList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var reviewResult = _client.SearchReviews(new ReviewsSearchRequest
                    {
                        ReviewId = reviewId,
                        ReviewName = reviewName ?? ""
                    }))
                    {
                        var responseStream = reviewResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var reviewRecord = responseStream.Current;

                            if (!string.IsNullOrEmpty(reviewRecord?.Name))
                            {
                                result.Add(new Review(
                                    (int)reviewRecord.ReviewId,
                                    reviewRecord.Name ?? "",
                                    reviewRecord.Description ?? "",
                                    reviewRecord.Rating));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private static ReviewRecord GrpcReview(Review review)
        {
            var result = new ReviewRecord();

            try
            {
                if (review != null)
                {
                    result = new ReviewRecord
                    {
                        ReviewId = review.Id,
                        Name = review.Name ?? "",
                        Description = review.Description ?? "",
                        Rating = review.Reviewrating
                    };
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        private async Task<PlatformList> SearchPlatformAsync(long platformId, string platformName)
        {
            var result = new PlatformList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var platformResult = _client.SearchPlatforms(new PlatformsSearchRequest
                    {
                        PlatformId = platformId,
                        PlatformName = platformName ?? ""
                    }))
                    {
                        var responseStream = platformResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var platformRecord = responseStream.Current;

                            if (!string.IsNullOrEmpty(platformRecord?.Name))
                            {
                                result.Add(new Platform(
                                    (int)platformRecord.PlatformId,
                                    platformRecord.Name ?? "",
                                    platformRecord.Maker ?? ""));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private static PlatformRecord GrpcPlatform(Platform platform)
        {
            var result = new PlatformRecord();

            try
            {
                if (platform != null)
                {
                    result = new PlatformRecord
                    {
                        PlatformId = platform.Id,
                        Name = platform.Name ?? "",
                        Maker = platform.Maker ?? ""
                    };
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        private async Task<GameGenreList> SearchGameGenresAsync(long gamegenreId)
        {
            var result = new GameGenreList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var gameResult = _client.SearchGameGenres(new GameGenresSearchRequest
                    {
                        GamegenreId = gamegenreId,
                        }))
                    {
                        var responseStream = gameResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var gameRatingRecord = responseStream.Current;

                            if (gameRatingRecord.GamegenreId > 0)
                            {
                                result.Add(new GameGenre(
                                    (int)gameRatingRecord.GamegenreId,
                                    (int)gameRatingRecord.GameId,
                                    (int)gameRatingRecord.GenreId));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private static GameGenreRecord GrpcGameGenre(GameGenre gameGenre)
        {
            var result = new GameGenreRecord();

            try
            {
                if (gameGenre != null)
                {
                    result = new GameGenreRecord
                    {
                        GamegenreId = gameGenre.Id,
                        GameId = gameGenre.GameId,
                        GenreId = gameGenre.GenreId
                    };
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        private static GamePlatformRecord GrpcGamePlatform(GamePlatform gamePlatform)
        {
            var result = new GamePlatformRecord();

            try
            {
                if (gamePlatform != null)
                {
                    result = new GamePlatformRecord
                    {
                        GameplatformId = gamePlatform.Id,
                        GameId = gamePlatform.GameId,
                        PlatformId = gamePlatform.PlatformId
                    };
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        private async Task<GamePlatformList> SearchGamePlatformsAsync(long gameplatformId)
        {
            var result = new GamePlatformList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var gameplatformResult = _client.SearchGamePlatforms(new GamePlatformsSearchRequest
                    {
                        GameplatformId = gameplatformId,
                    }))
                    {
                        var responseStream = gameplatformResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var gamePlatformRecord = responseStream.Current;

                            if (gamePlatformRecord.GameplatformId > 0)
                            {
                                result.Add(new GamePlatform(
                                    (int)gamePlatformRecord.GameplatformId,
                                    (int)gamePlatformRecord.GameId,
                                    (int)gamePlatformRecord.PlatformId));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private static GameReviewRecord GrpcGameReview(GameReview gameReview)
        {
            var result = new GameReviewRecord();

            try
            {
                if (gameReview != null)
                {
                    result = new GameReviewRecord
                    {
                        GamereviewId = gameReview.Id,
                        GameId = gameReview.GameId,
                        ReviewId = gameReview.ReviewId
                    };
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        private async Task<GameReviewList> SearchGameReviewsAsync(long gamereviewId)
        {
            var result = new GameReviewList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var gameplatformResult = _client.SearchGameReviews(new SearchGameReviewsRequest
                    {
                        GamereviewId = gamereviewId,
                    }))
                    {
                        var responseStream = gameplatformResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var gameReviewRecord = responseStream.Current;

                            if (gameReviewRecord.GamereviewId > 0)
                            {
                                result.Add(new GameReview(
                                    (int)gameReviewRecord.GamereviewId,
                                    (int)gameReviewRecord.GameId,
                                    (int)gameReviewRecord.ReviewId));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private async Task<GameReviewList> SearchGameReviewByGameId(long gameId)
        {
            var result = new GameReviewList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var gameplatformResult = _client.SearchGameReviewByGameId(new SearchGameReviewByGameIdRequest
                    {
                        GameId = gameId
                    }))
                    {
                        var responseStream = gameplatformResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var gameReviewRecord = responseStream.Current;

                            if (gameReviewRecord.GamereviewId > 0)
                            {
                                result.Add(new GameReview(
                                    (int)gameReviewRecord.GamereviewId,
                                    (int)gameReviewRecord.GameId,
                                    (int)gameReviewRecord.ReviewId));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private async Task<GameReviewList> SearchGameReviewByReviewId(long reviewId)
        {
            var result = new GameReviewList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var gameplatformResult = _client.SearchGameReviewByReviewId(new SearchGameReviewByReviewIdRequest
                    {
                        ReviewId = reviewId
                    }))
                    {
                        var responseStream = gameplatformResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var gameReviewRecord = responseStream.Current;

                            if (gameReviewRecord.GamereviewId > 0)
                            {
                                result.Add(new GameReview(
                                    (int)gameReviewRecord.GamereviewId,
                                    (int)gameReviewRecord.GameId,
                                    (int)gameReviewRecord.ReviewId));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private async Task<GameRatingList> SearchGameRatingsAsync(long gameratingId)
        {
            var result = new GameRatingList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var gameResult = _client.SearchGameRatings(new GameRatingsSearchRequest
                    {
                        GameratingId = gameratingId,
                        
                    }))
                    {
                        var responseStream = gameResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var gameratingRecord = responseStream.Current;

                            if (gameratingRecord.GameratingId > 0)
                            {
                                result.Add(new GameRating(
                                    (int)gameratingRecord.GameratingId,
                                    (int)gameratingRecord.GameId,
                                    (int)gameratingRecord.RatingId,
                                    gameratingRecord.Notes ?? ""));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private async Task<GameRatingList> SearchGameRatingByGameIdAsync(long gameId)
        {
            var result = new GameRatingList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var gameResult = _client.SearchGameRatingByGameId(new GameRatingsSearchRequestByGameId
                    {
                        GameId = gameId,

                    }))
                    {
                        var responseStream = gameResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var gameratingRecord = responseStream.Current;

                            if (gameratingRecord.GameratingId > 0)
                            {
                                result.Add(new GameRating(
                                    (int)gameratingRecord.GameratingId,
                                    (int)gameratingRecord.GameId,
                                    (int)gameratingRecord.RatingId,
                                    gameratingRecord.Notes ?? ""));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

         private async Task<GameGenreList> SearchGameGenreByGameIdAsync(long gameId)
        {
            var result = new GameGenreList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var gameResult = _client.SearchGameGenresByGameId(new GameGenresSearchRequestByGameId
                    {
                        GameId = gameId,

                    }))
                    {
                        var responseStream = gameResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var gamegenreRecord = responseStream.Current;

                            if (gamegenreRecord.GamegenreId > 0)
                            {
                                result.Add(new GameGenre(
                                    (int)gamegenreRecord.GamegenreId,
                                    (int)gamegenreRecord.GameId,
                                    (int)gamegenreRecord.GenreId
                                    ));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private async Task<GameGenreList> SearchGameGenreByGenreIdAsync(long genreId)
        {
            var result = new GameGenreList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var gameResult = _client.SearchGameGenresByGenreId(new GameGenresSearchRequestByGenreId
                    {
                        GenreId = genreId,

                    }))
                    {
                        var responseStream = gameResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var gamegenreRecord = responseStream.Current;

                            if (gamegenreRecord.GamegenreId > 0)
                            {
                                result.Add(new GameGenre(
                                    (int)gamegenreRecord.GamegenreId,
                                    (int)gamegenreRecord.GameId,
                                    (int)gamegenreRecord.GenreId
                                    ));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private async Task<GameRatingList> SearchGameRatingByRatingIdAsync(long ratingId)
        {
            var result = new GameRatingList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var gameResult = _client.SearchGameRatingByRatingId(new GameRatingsSearchRequestByRatingId
                    {
                        RatingId = ratingId,

                    }))
                    {
                        var responseStream = gameResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var gameratingRecord = responseStream.Current;

                            if (gameratingRecord.GameratingId > 0)
                            {
                                result.Add(new GameRating(
                                    (int)gameratingRecord.GameratingId,
                                    (int)gameratingRecord.GameId,
                                    (int)gameratingRecord.RatingId,
                                    gameratingRecord.Notes ?? ""));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private async Task<GamePlatformList> SearchGamePlatformByGameIdAsync(long gameId)
        {
            var result = new GamePlatformList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var gameResult = _client.SearchGamePlatformsByGameId(new GamePlatformsSearchRequestByGameId
                    {
                        GameId = gameId,

                    }))
                    {
                        var responseStream = gameResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var gameplatformRecord = responseStream.Current;

                            if (gameplatformRecord.GameplatformId > 0)
                            {
                                result.Add(new GamePlatform(
                                    (int)gameplatformRecord.GameplatformId,
                                    (int)gameplatformRecord.GameId,
                                    (int)gameplatformRecord.PlatformId
                                    ));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private async Task<GamePlatformList> SearchGamePlatformByPlatformIdAsync(long platformId)
        {
            var result = new GamePlatformList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var gameResult = _client.SearchGamePlatformsByPlatformId(new GamePlatformsSearchRequestByPlatformId
                    {
                        PlatformId = platformId,

                    }))
                    {
                        var responseStream = gameResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var gameplatformRecord = responseStream.Current;

                            if (gameplatformRecord.GameplatformId > 0)
                            {
                                result.Add(new GamePlatform(
                                    (int)gameplatformRecord.GameplatformId,
                                    (int)gameplatformRecord.GameId,
                                    (int)gameplatformRecord.PlatformId
                                    ));
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private static GameRatingRecord GrpcGameRating(GameRating gameRating)
        {
            var result = new GameRatingRecord();

            try
            {
                if (gameRating != null)
                {
                    result = new GameRatingRecord
                    {
                        GameratingId = gameRating.Id,
                        GameId = gameRating.GameId,
                        RatingId = gameRating.RatingId,
                        Notes = gameRating.Notes

                    };
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        private static GameImageRecord GrpcGameImage(GameImage gameImage)
        {
            var result = new GameImageRecord();

            try
            {
                if (gameImage != null)
                {
                    result = new GameImageRecord
                    {
                        GameimageId = gameImage.Id,
                        GameId = gameImage.GameId
                    };

                    if (gameImage.Image != null)
                    {
                        result.Image = ByteString.CopyFrom(gameImage.Image);
                    }
                }
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        private async Task<GameImageList> SearchGameImagesByGameIdAsync(long gameId)
        {
            var result = new GameImageList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var gameimageResult = _client.SearchGameImageByGameId(new GameImagesSearchRequestByGameId
                    {
                        GameId = gameId
                    }))
                    {
                        var responseStream = gameimageResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var gameImageRecord = responseStream.Current;

                            if (gameImageRecord.GameimageId > 0)
                            {
                                if (gameImageRecord != null)
                                {
                                    result.Add(new GameImage((int)gameImageRecord.GameimageId, (int)gameImageRecord.GameId, null));
                                        
                                }

                                if (gameImageRecord.Image != null)
                                {

                                    result.Add(new GameImage((int)gameImageRecord.GameimageId, (int)gameImageRecord.GameId, gameImageRecord.Image.ToByteArray()));

                                }
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private async Task<GameImageList> SearchGameImagesAsync(long gameimageId)
        {
            var result = new GameImageList();

            try
            {
                var errorMessage = "";

                if (_client != null)
                {
                    using (var gameimageResult = _client.SearchGameImages(new GameImagesSearchRequest
                    {
                        GameimageId = gameimageId
                    }))
                    {
                        var responseStream = gameimageResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var gameImageRecord = responseStream.Current;

                            if (gameImageRecord.GameimageId > 0)
                            {
                                if (gameImageRecord != null)
                                {
                                    result.Add(new GameImage((int)gameImageRecord.GameimageId, (int)gameImageRecord.GameimageId, null));

                                }

                                if (gameImageRecord.Image != null)
                                {

                                    result.Add(new GameImage((int)gameImageRecord.GameimageId, (int)gameImageRecord.GameimageId, gameImageRecord.Image.ToByteArray()));

                                }
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Unable to create gRPC client";
                }

                result.ErrorMessage = errorMessage;
            }

            catch (RpcException ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            catch (Exception ex)
            {
                Log.Error(ex);

                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        #endregion Private Methods
    }
}
