
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

        #region Game Methods

        public GenreList SearchGenres(long genreId, string genreName)
        {
            var result = new GenreList();

            try
            {
                result = AsyncHelper.RunSync(() => SerchGenresAsync(genreId, genreName));
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

        #endregion Game Methods

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

        private async Task<GenreList> SerchGenresAsync(long genreId, string genreName)
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

        #endregion Private Methods
    }
}
