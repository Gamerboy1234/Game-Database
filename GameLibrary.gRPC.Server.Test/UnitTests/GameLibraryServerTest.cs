
using System;
using System.Diagnostics;
using GameLibrary.Core;
using Grpc.Core;
using Xunit;
using Daffinity.Collector.gRPC.Server;

namespace GameLibrarygRPCServerTest.UnitTests
{
    public class GameLibraryServerTest
    {
        #region Constants

        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        private const int GrpcPort = 50052;
        private const string GrpcHostName = "localhost";

        #endregion Constants


        #region Test Cases

        [Fact]
        public void GameLibrarygRpcServerTests()
        {
            Assert.True(true);

            try
            {
                GameLibraryAgent.Startup(ConnectionString);

                var daffinityCollectorServer = new Server
                {
                    Services = { Gamelibrary.GameLibrary.BindService(new GameLibraryServer()) },
                    Ports = { new ServerPort(GrpcHostName, GrpcPort, ServerCredentials.Insecure) }
                };

                daffinityCollectorServer.Start();

                Assert.True(true);

                daffinityCollectorServer.ShutdownAsync().Wait();
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                Assert.True(false);
            }
        }

        #endregion Test Cases
    }
}
