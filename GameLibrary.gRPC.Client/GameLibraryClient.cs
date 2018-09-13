
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gamelibrary;
using GameLibrary.Model;
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
                            var gameGenreRecord = responseStream.Current;

                            if (gameGenreRecord.GamegenreId > 0)
                            {
                                result.Add(new GameGenre(
                                    (int)gameGenreRecord.GamegenreId,
                                    (int)gameGenreRecord.GameId,
                                    (int)gameGenreRecord.GenreId));
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

        #endregion Private Methods
    }
}
