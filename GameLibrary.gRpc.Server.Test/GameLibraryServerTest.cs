
using System;
using System.Diagnostics;
using GameLibrary.Core;
using GameLibrary.gRPC.Server;
using Grpc.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameLibrary.gRpc.Server.Test
{
    [TestClass]
    public class GameLibraryServerTest
    {
        #region Constants

        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        private const int GrpcPort = 50052;
        private const string GrpcHostName = "localhost";

        #endregion Constants


        #region Test Cases

        [TestMethod]
        public void GameLibrarygRpcServerTests()
        {
            Assert.IsTrue(true);

            try
            {
                GameLibraryAgent.Startup(ConnectionString);

                var gameLibraryServer = new Grpc.Core.Server
                {
                    Services = { Gamelibrary.GameLibrary.BindService(new GameLibraryServer()) },
                    Ports = { new ServerPort(GrpcHostName, GrpcPort, ServerCredentials.Insecure) }
                };

                gameLibraryServer.Start();

                Assert.IsTrue(true);

                gameLibraryServer.ShutdownAsync().Wait();
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                Assert.IsTrue(false);
            }
        }

        #endregion Test Cases
    }
}
