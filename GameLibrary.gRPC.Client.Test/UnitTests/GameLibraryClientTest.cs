
using System;
using ActiveNet.Types.Base;
using Daffinity.Collector.Core;
using Daffinity.Collector.gRPC.Client;
using Daffinity.Collector.gRPC.Server;
using Daffinity.Model;
using Daffinitycollector;
using Grpc.Core;
using Xunit;

namespace DaffinityCollectorgRPCClientTest.UnitTests
{
    public class DaffinityCollectorClientTest
    {
        #region Constants

        private const int GrpcPort = 50055;
        private const string GrpcHost = "127.0.0.1";
        private const string GrpcHostName = "localhost";
        private const string CustomerId = "502";
        private const string SourceSystemCompanyId = "ABC123";
        private const string AccessKey = "password";
        private const string DataSourceType = "SalonUltimate";
        private const string UniversalNodeId = "e3201d33-6e04-42a1-9fe2-e0ac8396873d";

        #endregion Constants


        #region Test Cases

        [Fact]
        public void DaffinityCollectorgRpcClientTests()
        {
            AnOperatingSystem.SetCachedAdaptersPath(AnOperatingSystem.GetAdaptersPath(Settings.GetSetting(Settings.AdaptersPathKey), ""));

            var adaptersPath = AnOperatingSystem.GetAdaptersPath(AnOperatingSystem.CachedAdaptersPath, "");
            var mainAssemblyPath = AnOperatingSystem.MainAssemblyPath;

            DaffinityCollectorAgent.Startup(adaptersPath, mainAssemblyPath);

            var daffinityCollectorServer = new Server
            {
                Services = { DaffinityCollector.BindService(new DaffinityCollectorServer()) },
                Ports = { new ServerPort(GrpcHostName, GrpcPort, ServerCredentials.Insecure) }
            };

            daffinityCollectorServer.Start();

            Assert.True(true);


            // Get the session

            var channel = new Channel($"{GrpcHost}:{GrpcPort}", ChannelCredentials.Insecure);
            var client = new DaffinityCollectorClient(new DaffinityCollector.DaffinityCollectorClient(channel));

            var session = client.GetSessionToken(CustomerId, AccessKey);

            Assert.NotNull(session);
            Assert.False(string.IsNullOrEmpty(session.CustomerId));


            // Get all queued records that are not processed

            var queueRecords = client.GetQueueRecords(session, UniversalNodeId, DataSourceType, AnTypes.MinDateTimeValue, AnTypes.MinDateTimeValue);

            Assert.NotNull(queueRecords);
            //Assert.True(queueRecords.List.Count > 0);


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


            daffinityCollectorServer.ShutdownAsync().Wait();
        }

        #endregion Test Cases
    }
}
