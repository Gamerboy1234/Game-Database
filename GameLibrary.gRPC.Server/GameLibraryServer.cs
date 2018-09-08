
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gamelibrary;
using GameLibrary.Core;
using GameLibrary.Model;
using Grpc.Core;
using Logger;

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

        #endregion Private Methods
    }
}
