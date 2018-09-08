
using System;
using GameLibrary.Core;
using GameLibrary.gRPC.Client;
using Grpc.Core;

namespace GameLibrarygRPCClientTest.UnitTests
{
    public class GameLibraryClientTest
    {
        #region Constants

        private const string ConnectionString = "Server=.;Database=BubbaGames;Trusted_Connection=true;";

        private const int GrpcPort = 50052;
        private const string GrpcHost = "127.0.0.1";
        private const string GrpcHostName = "localhost";

        #endregion Constants


        #region Test Cases

        [Fact]
        public void GameLibrarygRpcClientTests()
        {
            GameLibraryAgent.Startup(ConnectionString);

            var gameLibraryServer = new Server
            {
                Services = { Gamelibrary.GameLibrary.BindService(new GameLibrary.gRPC.Server.GameLibraryServer()) },
                Ports = { new ServerPort(GrpcHostName, GrpcPort, ServerCredentials.Insecure) }
            };

            gameLibraryServer.Start();

            Assert.True(true);


            // Get the session

            var channel = new Channel($"{GrpcHost}:{GrpcPort}", ChannelCredentials.Insecure);
            var client = new GameLibraryClient(new Gamelibrary.GameLibrary.GameLibraryClient(channel));

            // Get all games

            var games = client.SearchGames(0, "");

            Assert.NotNull(games);
            Assert.True(games.List.Count, 0);


            // Mark queued records as processed

            var resultQueueRecords = client.MarkQueueRecordsAsProcessed(session, queueRecords);

            Assert.NotNull(resultQueueRecords);


            // Get all queued records that are not processed

            queueRecords = client.GetQueueRecords(session, UniversalNodeId, DataSourceType, AnTypes.MinDateTimeValue, AnTypes.MinDateTimeValue);

            Assert.NotNull(queueRecords);
            Assert.False(queueRecords.List.Count > 0);


            // Mark queued records as processed

            var rawDataRecords = new RawDataRecordList();

            rawDataRecords.Add(new Daffinity.Model.RawDataRecord(
                1,
                CustomerId,
                SourceSystemCompanyId,
                UniversalNodeId, 
                "DataSourceType",
                "Organization",
                "Attachment",
                "ADD",
                "ErrorMessage",
                "RecordData",
                new DateTime(2015, 11, 29, 1, 1, 1),
                AnTypes.MinDateTimeValue,
                false,
                false));

            var resultRawDataRecords = client.UploadDiscoveredRecords(session, rawDataRecords);

            Assert.NotNull(resultRawDataRecords);


            gameLibraryServer.ShutdownAsync().Wait();
        }

        #endregion Test Cases
    }
}
