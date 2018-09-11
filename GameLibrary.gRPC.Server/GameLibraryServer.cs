
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gamelibrary;
using GameLibrary.Core;
using GameLibrary.Model;
using Grpc.Core;
using Logger;
using static GameLibrary.Model.Genre;

namespace GameLibrary.gRPC.Server
{
    public class GameLibraryServer : Gamelibrary.GameLibrary.GameLibraryBase
    {
        #region Game Methods

        public override async Task SearchGames(GamesSearchRequest request, IServerStreamWriter<GameRecord> responseStream, ServerCallContext context)
        {
            try
            {
                if (request != null)
                {
                    var errorMessage = "";

                    var games = new GameList();

                    if (request.GameId > 0)
                    {
                        var game = GameLibraryAgent.ModelAssembler.GetGameById((int)request.GameId, ref errorMessage);

                        if (game?.Id > 0)
                        {
                            games.Add(game);
                        }
                    }

                    else if (!string.IsNullOrEmpty(request.GameName))
                    {
                        var game = GameLibraryAgent.ModelAssembler.GetGameByName(request.GameName, ref errorMessage);

                        if (!string.IsNullOrEmpty(game?.Name))
                        {
                            games.Add(game);
                        }
                    }

                    else // Return all games
                    {
                        games = GameLibraryAgent.ModelAssembler.GetGames() ?? new GameList();
                    }

                    if (games?.List?.Count > 0)
                    {
                        foreach (var game in games.List.Where(game => !string.IsNullOrEmpty(game?.Name)))
                        {
                            await responseStream.WriteAsync(GrpcGame(game));
                        }
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
        }

        public override Task<GameResult> AddGame(GameRecord request, ServerCallContext context)
        {
            var result = new GameResult();

            try
            {
                var errorMessage = "";

                var game = GameFromGrpc(request);

                result.Success = GameLibraryAgent.ModelAssembler.AddOrEditGame(game, ref errorMessage);

                result.Game = GrpcGame(game);
                result.ErrorMessage = errorMessage ?? "";
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return Task.FromResult(result);
        }

        public override Task<GameResult> EditGame(GameRecord request, ServerCallContext context)
        {
            var result = new GameResult();

            try
            {
                var errorMessage = "";

                var game = GameFromGrpc(request);

                result.Success = GameLibraryAgent.ModelAssembler.AddOrEditGame(game, ref errorMessage);

                result.Game = GrpcGame(game);
                result.ErrorMessage = errorMessage ?? "";
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return Task.FromResult(result);
        }

        public override Task<GameResult> DeleteGame(GameRecord request, ServerCallContext context)
        {
            var result = new GameResult();

            try
            {
                var errorMessage = "";

                var game = GameFromGrpc(request);

                result.Success = GameLibraryAgent.ModelAssembler.DeleteGame(game.Id, ref errorMessage);

                result.Game = GrpcGame(game);
                result.ErrorMessage = errorMessage ?? "";
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return Task.FromResult(result);
        }

        #endregion Game Methods

        #region Genre Methods

        public override async Task SearchGenres(GenresSearchRequest request, IServerStreamWriter<GenreRecord> responseStream, ServerCallContext context)
        {
            try
            {
                if (request != null)
                {
                    var errorMessage = "";

                    var genres = new GenreList();

                    if (request.GenreId > 0)
                    {
                        var genre = GameLibraryAgent.ModelAssembler.GetGenreById((int)request.GenreId, ref errorMessage);

                        if (genre?.Id > 0)
                        {
                            genres.Add(genre);
                        }
                    }

                    else if (!string.IsNullOrEmpty(request.GenreName))
                    {
                        var genre = GameLibraryAgent.ModelAssembler.GetGenreByName(request.GenreName, ref errorMessage);

                        if (!string.IsNullOrEmpty(genre?.Name))
                        {
                            genres.Add(genre);
                        }
                    }

                    else // Return all games
                    {
                        genres = GameLibraryAgent.ModelAssembler.GetGenres() ?? new GenreList();
                    }

                    if (genres?.List?.Count > 0)
                    {
                        foreach (var genre in genres.List.Where(genre => !string.IsNullOrEmpty(genre?.Name)))
                        {
                            await responseStream.WriteAsync(GrpcGenre(genre));
                        }
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
        }

        public override Task<GenreResult> AddGenre(GenreRecord request, ServerCallContext context)
        {
            var result = new GenreResult();

            try
            {
                var errorMessage = "";

                var genre = GenreFromGrpc(request);

                result.Success = GameLibraryAgent.ModelAssembler.AddOrEditGenre(genre, ref errorMessage);

                result.Genre = GrpcGenre(genre);
                result.ErrorMessage = errorMessage ?? "";
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return Task.FromResult(result);
        }

        public override Task<GenreResult> EditGenre(GenreRecord request, ServerCallContext context)
        {
            var result = new GenreResult();

            try
            {
                var errorMessage = "";

                var genre = GenreFromGrpc(request);

                result.Success = GameLibraryAgent.ModelAssembler.AddOrEditGenre(genre, ref errorMessage);

                result.Genre = GrpcGenre(genre);
                result.ErrorMessage = errorMessage ?? "";
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return Task.FromResult(result);
        }

        public override Task<GenreResult> DeleteGenre(GenreRecord request, ServerCallContext context)
        {
            var result = new GenreResult();

            try
            {
                var errorMessage = "";

                var genre = GenreFromGrpc(request);

                result.Success = GameLibraryAgent.ModelAssembler.DeleteGenre(genre.Id, ref errorMessage);

                result.Genre = GrpcGenre(genre);
                result.ErrorMessage = errorMessage ?? "";
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return Task.FromResult(result);
        }

        #endregion Genre Methods

        #region Rating Methods

        public override async Task SearchRatings(RatingsSearchRequest request, IServerStreamWriter<RatingRecord> responseStream, ServerCallContext context)
        {
            try
            {
                if (request != null)
                {
                    var errorMessage = "";

                    var ratings = new RatingList();

                    if (request.RatingId > 0)
                    {
                        var rating = GameLibraryAgent.ModelAssembler.GetRatingById((int)request.RatingId, ref errorMessage);

                        if (rating?.Id > 0)
                        {
                            ratings.Add(rating);
                        }
                    }

                    else if (!string.IsNullOrEmpty(request.RatingName))
                    {
                        var rating = GameLibraryAgent.ModelAssembler.GetRatingByName(request.RatingName, ref errorMessage);

                        if (!string.IsNullOrEmpty(rating?.Name))
                        {
                            ratings.Add(rating);
                        }
                    }

                    else // Return all games
                    {
                        ratings = GameLibraryAgent.ModelAssembler.GetRatings() ?? new RatingList();
                    }
                    if (ratings?.List?.Count > 0)
                    {
                        foreach (var rating in ratings.List.Where(rating => !string.IsNullOrEmpty(rating.Name)))
                        {
                            await responseStream.WriteAsync(GrpcRating(rating));
                        }
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
        }

        public override Task<RatingResult> AddRating(RatingRecord request, ServerCallContext context)
        {
            var result = new RatingResult();

            try
            {
                var errorMessage = "";

                var rating = RatingFromGrpc(request);

                result.Success = GameLibraryAgent.ModelAssembler.AddOrEditRating(rating, ref errorMessage);

                result.Rating = GrpcRating(rating);
                result.ErrorMessage = errorMessage ?? "";
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return Task.FromResult(result);
        }

        public override Task<RatingResult> EditRating(RatingRecord request, ServerCallContext context)
        {
            var result = new RatingResult();

            try
            {
                var errorMessage = "";

                var rating = RatingFromGrpc(request);

                result.Success = GameLibraryAgent.ModelAssembler.AddOrEditRating(rating, ref errorMessage);

                result.Rating = GrpcRating(rating);
                result.ErrorMessage = errorMessage ?? "";
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return Task.FromResult(result);
        }

        public override Task<RatingResult> DeleteRating(RatingRecord request, ServerCallContext context)
        {
            var result = new RatingResult();

            try
            {
                var errorMessage = "";

                var rating = RatingFromGrpc(request);

                result.Success = GameLibraryAgent.ModelAssembler.DeleteRating(rating.Id, ref errorMessage);

                result.Rating = GrpcRating(rating);
                result.ErrorMessage = errorMessage ?? "";
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return Task.FromResult(result);
        }

        #endregion Rating Methods

        #region Review Methods

        public override async Task SearchReviews(ReviewsSearchRequest request, IServerStreamWriter<ReviewRecord> responseStream, ServerCallContext context)
        {
            try
            {
                if (request != null)
                {
                    var errorMessage = "";

                    var reviews = new ReviewList();

                    if (request.ReviewId > 0)
                    {
                        var review = GameLibraryAgent.ModelAssembler.GetReviewById((int)request.ReviewId, ref errorMessage);

                        if (review?.Id > 0)
                        {
                            reviews.Add(review);
                        }
                    }

                    else if (!string.IsNullOrEmpty(request.ReviewName))
                    {
                        var review = GameLibraryAgent.ModelAssembler.GetReviewByName(request.ReviewName, ref errorMessage);

                        if (!string.IsNullOrEmpty(review?.Name))
                        {
                            reviews.Add(review);
                        }
                    }

                    else // Return all games
                    {
                        reviews = GameLibraryAgent.ModelAssembler.GetReviews() ?? new ReviewList();
                    }
                    if (reviews?.List?.Count > 0)
                    {
                        foreach (var review in reviews.List.Where(review => !string.IsNullOrEmpty(review.Name)))
                        {
                            await responseStream.WriteAsync(GrpcReview(review));
                        }
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
        }

        public override Task<ReviewResult> AddReview(ReviewRecord request, ServerCallContext context)
        {
            var result = new ReviewResult();

            try
            {
                var errorMessage = "";

                var review = ReviewFromGrpc(request);

                result.Success = GameLibraryAgent.ModelAssembler.AddOrEditReview(review, ref errorMessage);

                result.Review = GrpcReview(review);
                result.ErrorMessage = errorMessage ?? "";
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return Task.FromResult(result);
        }

        public override Task<ReviewResult> EditReview(ReviewRecord request, ServerCallContext context)
        {
            var result = new ReviewResult();

            try
            {
                var errorMessage = "";

                var review = ReviewFromGrpc(request);

                result.Success = GameLibraryAgent.ModelAssembler.AddOrEditReview(review, ref errorMessage);

                result.Review = GrpcReview(review);
                result.ErrorMessage = errorMessage ?? "";
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return Task.FromResult(result);
        }

        public override Task<ReviewResult> DeleteReview(ReviewRecord request, ServerCallContext context)
        {
            var result = new ReviewResult();

            try
            {
                var errorMessage = "";

                var review = ReviewFromGrpc(request);

                result.Success = GameLibraryAgent.ModelAssembler.DeleteReview(review.Id, ref errorMessage);

                result.Review = GrpcReview(review);
                result.ErrorMessage = errorMessage ?? "";
            }

            catch (RpcException ex)
            {
                Log.Error(ex);
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return Task.FromResult(result);
        }

        #endregion Review Methods

        #region Private Methods

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

        private static Game GameFromGrpc(GameRecord gameRecord)
        {
            var result = new Game();

            try
            {
                if (gameRecord != null)
                {
                    result = new Game(
                        (int)gameRecord.GameId,
                        gameRecord.Name ?? "",
                        gameRecord.Description ?? "");
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
                        Description = genre.Description ?? ""
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

        private static Genre GenreFromGrpc(GenreRecord genreRecord)
        {
            var result = new Genre();

            try
            {
                if (genreRecord != null)
                {
                    result = new Genre(
                        (int)genreRecord.GenreId,
                        genreRecord.Name ?? "",
                        genreRecord.Description ?? "");
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
                        Symbol = rating.Symbol ?? ""
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

        private static Rating RatingFromGrpc(RatingRecord ratingRecord)
        {
            var result = new Rating();

            try
            {
                if (ratingRecord != null)
                {
                    result = new Rating(
                        (int)ratingRecord.RatingId,
                        ratingRecord.Name ?? "",
                        ratingRecord.Description ?? "",
                        ratingRecord.Symbol ?? "");
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

        private static Review ReviewFromGrpc(ReviewRecord reviewRecord)
        {
            var result = new Review();

            try
            {
                if (reviewRecord != null)
                {
                    result = new Review(
                        (int)reviewRecord.ReviewId,
                        reviewRecord.Name ?? "",
                        reviewRecord.Description ?? "",
                        reviewRecord.Rating);
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
