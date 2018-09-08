
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibrary.Model;
using Grpc.Core;
using Logger;

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
                result = AsyncHelper.RunSync(() => GetQueueRecordsAsync(session, universalNodeId, dataSourceType, beginDateTime, endDateTime));
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

        public QueueRecordList MarkQueueRecordsAsProcessed(Model.Session session, QueueRecordList queueRecords)
        {
            var result = new QueueRecordList();

            try
            {
                result = AsyncHelper.RunSync(() => MarkQueueRecordsAsProcessedAsync(session, queueRecords));
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

        public RawDataRecordList UploadDiscoveredRecords(Model.Session session, RawDataRecordList rawDataRecords)
        {
            var result = new RawDataRecordList();

            try
            {
                result = AsyncHelper.RunSync(() => UploadDiscoveredRecordsAsync(session, rawDataRecords));
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

        public RawDataRecordList GetUnprocessedRawRecords(Model.Session session, int maxRecordsToReturn, string customerId, string universalNodeId, string dataSourceType, DateTime beginDateTime, DateTime endDateTime)
        {
            var result = new RawDataRecordList();

            try
            {
                result = AsyncHelper.RunSync(() => GetUnprocessedRawRecordsAsync(session, maxRecordsToReturn, customerId, universalNodeId, dataSourceType, beginDateTime, endDateTime));
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

        public RawDataRecordList MarkRawRecordsAsProcessed(Model.Session session, RawDataRecordList rawDataRecords)
        {
            var result = new RawDataRecordList();

            try
            {
                result = AsyncHelper.RunSync(() => MarkRawRecordsAsProcessedAsync(session, rawDataRecords));
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

        #endregion Game Methods


        #region Private Methods

        private async Task<QueueRecordList> GetQueueRecordsAsync(Model.Session session, string universalNodeId, string dataSourceType, DateTime beginDateTime, DateTime endDateTime)
        {
            var result = new QueueRecordList();

            try
            {
                if (Types.ValidateObject(_client, "Unable to create gRPC client", out var errorMessage))
                {
                    using (var queueRecordResult = _client.GetQueueRecords(new QueueRecordsRequest
                    {
                        Session = GrpcSession(session),
                        CustomerId = session?.CustomerId ?? "",
                        UniversalNodeId = universalNodeId ?? "",
                        DataSourceType = dataSourceType ?? "",
                        BeginDateTime = beginDateTime.ToString("O"),
                        EndDateTime = endDateTime.ToString("O")
                    }))
                    {
                        var responseStream = queueRecordResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var queueRecord = responseStream.Current;

                            if (!string.IsNullOrEmpty(queueRecord?.RecordData))
                            {
                                result.Add(new Model.QueueRecord(
                                    (int)queueRecord.RecordId,
                                    queueRecord.CustomerId ?? "",
                                    queueRecord.SourceSystemCompanyId ?? "", 
                                    queueRecord.UniversalNodeId ?? "",
                                    queueRecord.DataSourceType ?? "",
                                    queueRecord.Organization ?? "",
                                    queueRecord.TableName ?? "",
                                    queueRecord.Action ?? "",
                                    queueRecord.ErrorMessage ?? "",
                                    queueRecord.RecordData ?? "",
                                    AnSafeConvert.ToDateTime(queueRecord.EnteredDateTime),
                                    AnSafeConvert.ToDateTime(queueRecord.ProcessedDateTime),
                                    queueRecord.Result,
                                    queueRecord.Processed));
                            }
                        }
                    }
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

        private async Task<QueueRecordList> MarkQueueRecordsAsProcessedAsync(Model.Session session, QueueRecordList queueRecords)
        {
            var result = new QueueRecordList();

            try
            {
                if (Types.ValidateObject(_client, "Unable to create gRPC client", out var errorMessage))
                {
                    if (Types.ValidateResult(queueRecords?.List?.Count > 0, "At lease 1 queue record must exist", out errorMessage))
                    {
                        var markQueueRecordsRequest = new MarkQueueRecordsRequest
                        {
                            Session = GrpcSession(session),
                        };

                        GrpcQueueRecords(markQueueRecordsRequest.QueueRecords, queueRecords);

                        using (var queueRecordResult = _client.MarkQueueRecordsAsProcessed(markQueueRecordsRequest))
                        {
                            var responseStream = queueRecordResult.ResponseStream;

                            while (await responseStream.MoveNext())
                            {
                                var queueRecord = responseStream.Current;

                                if (!string.IsNullOrEmpty(queueRecord?.RecordData))
                                {
                                    result.Add(new Model.QueueRecord(
                                        (int)queueRecord.RecordId,
                                        queueRecord.CustomerId ?? "",
                                        queueRecord.SourceSystemCompanyId ?? "",
                                        queueRecord.UniversalNodeId ?? "",
                                        queueRecord.DataSourceType ?? "",
                                        queueRecord.Organization ?? "",
                                        queueRecord.TableName ?? "",
                                        queueRecord.Action ?? "",
                                        queueRecord.ErrorMessage ?? "",
                                        queueRecord.RecordData ?? "",
                                        AnSafeConvert.ToDateTime(queueRecord.EnteredDateTime),
                                        AnSafeConvert.ToDateTime(queueRecord.ProcessedDateTime),
                                        queueRecord.Result,
                                        queueRecord.Processed));
                                }
                            }
                        }
                    }
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

        private async Task<RawDataRecordList> UploadDiscoveredRecordsAsync(Model.Session session, RawDataRecordList rawDataRecords)
        {
            var result = new RawDataRecordList();

            try
            {
                if (Types.ValidateObject(_client, "Unable to create gRPC client", out var errorMessage))
                {
                    if (Types.ValidateResult(rawDataRecords?.List?.Count > 0, "At lease 1 raw data record must exist", out errorMessage))
                    {
                        var uploadDiscoveredRecordsRequest = new UploadDiscoveredRecordsRequest
                        {
                            Session = GrpcSession(session),
                        };

                        GrpcRawDataRecords(uploadDiscoveredRecordsRequest.RawDataRecords, rawDataRecords);

                        using (var rawDataRecordResult = _client.UploadDiscoveredRecords(uploadDiscoveredRecordsRequest))
                        {
                            var responseStream = rawDataRecordResult.ResponseStream;

                            while (await responseStream.MoveNext())
                            {
                                var rawDataRecord = responseStream.Current;

                                if (!string.IsNullOrEmpty(rawDataRecord?.RecordData))
                                {
                                    result.Add(new Model.RawDataRecord(
                                        (int)rawDataRecord.RecordId,
                                        rawDataRecord.CustomerId ?? "",
                                        rawDataRecord.SourceSystemCompanyId ?? "",
                                        rawDataRecord.UniversalNodeId ?? "",
                                        rawDataRecord.DataSourceType ?? "",
                                        rawDataRecord.Organization ?? "",
                                        rawDataRecord.TableName ?? "",
                                        rawDataRecord.Action ?? "",
                                        rawDataRecord.ErrorMessage ?? "",
                                        rawDataRecord.RecordData ?? "",
                                        AnSafeConvert.ToDateTime(rawDataRecord.EnteredDateTime),
                                        AnSafeConvert.ToDateTime(rawDataRecord.ProcessedDateTime),
                                        rawDataRecord.Result,
                                        rawDataRecord.Processed));
                                }
                            }
                        }
                    }
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

        private async Task<RawDataRecordList> GetUnprocessedRawRecordsAsync(Model.Session session, int maxRecordsToReturn, string customerId, string universalNodeId, string dataSourceType, DateTime beginDateTime, DateTime endDateTime)
        {
            var result = new RawDataRecordList();

            try
            {
                if (Types.ValidateObject(_client, "Unable to create gRPC client", out var errorMessage))
                {
                    using (var rawDataRecordResult = _client.GetUnprocessedRawRecords(new UnprocessedRawRecordsRequest
                    {
                        Session = GrpcSession(session),
                        MaxRecordsToReturn = maxRecordsToReturn,
                        CustomerId = customerId ?? "",
                        UniversalNodeId = universalNodeId ?? "",
                        DataSourceType = dataSourceType ?? "",
                        BeginDateTime = beginDateTime.ToString("O"),
                        EndDateTime = endDateTime.ToString("O")
                    }))
                    {
                        var responseStream = rawDataRecordResult.ResponseStream;

                        while (await responseStream.MoveNext())
                        {
                            var rawDataRecord = responseStream.Current;

                            if (!string.IsNullOrEmpty(rawDataRecord?.RecordData))
                            {
                                result.Add(new Model.RawDataRecord(
                                    (int)rawDataRecord.RecordId,
                                    rawDataRecord.CustomerId ?? "",
                                    rawDataRecord.SourceSystemCompanyId ?? "",
                                    rawDataRecord.UniversalNodeId ?? "",
                                    rawDataRecord.DataSourceType ?? "",
                                    rawDataRecord.Organization ?? "",
                                    rawDataRecord.TableName ?? "",
                                    rawDataRecord.Action ?? "",
                                    rawDataRecord.ErrorMessage ?? "",
                                    rawDataRecord.RecordData ?? "",
                                    AnSafeConvert.ToDateTime(rawDataRecord.EnteredDateTime),
                                    AnSafeConvert.ToDateTime(rawDataRecord.ProcessedDateTime),
                                    rawDataRecord.Result,
                                    rawDataRecord.Processed));
                            }
                        }
                    }
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

        private async Task<RawDataRecordList> MarkRawRecordsAsProcessedAsync(Model.Session session, RawDataRecordList rawDataRecords)
        {
            var result = new RawDataRecordList();

            try
            {
                if (Types.ValidateObject(_client, "Unable to create gRPC client", out var errorMessage))
                {
                    if (Types.ValidateResult(rawDataRecords?.List?.Count > 0, "At lease 1 rawData record must exist", out errorMessage))
                    {
                        var markRawDataRecordsRequest = new MarkRawDataRecordsRequest
                        {
                            Session = GrpcSession(session),
                        };

                        GrpcRawDataRecords(markRawDataRecordsRequest.RawDataRecords, rawDataRecords);

                        using (var rawDataRecordResult = _client.MarkRawRecordsAsProcessed(markRawDataRecordsRequest))
                        {
                            var responseStream = rawDataRecordResult.ResponseStream;

                            while (await responseStream.MoveNext())
                            {
                                var rawDataRecord = responseStream.Current;

                                if (!string.IsNullOrEmpty(rawDataRecord?.RecordData))
                                {
                                    result.Add(new Model.RawDataRecord(
                                        (int)rawDataRecord.RecordId,
                                        rawDataRecord.CustomerId ?? "",
                                        rawDataRecord.SourceSystemCompanyId ?? "",
                                        rawDataRecord.UniversalNodeId ?? "",
                                        rawDataRecord.DataSourceType ?? "",
                                        rawDataRecord.Organization ?? "",
                                        rawDataRecord.TableName ?? "",
                                        rawDataRecord.Action ?? "",
                                        rawDataRecord.ErrorMessage ?? "",
                                        rawDataRecord.RecordData ?? "",
                                        AnSafeConvert.ToDateTime(rawDataRecord.EnteredDateTime),
                                        AnSafeConvert.ToDateTime(rawDataRecord.ProcessedDateTime),
                                        rawDataRecord.Result,
                                        rawDataRecord.Processed));
                                }
                            }
                        }
                    }
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

        private static Session GrpcSession(Model.Session session)
        {
            return new Session { SessionToken = session?.SessionToken ?? "", CustomerId = session?.CustomerId ?? ""};
        }

        private static void GrpcQueueRecords(ICollection<QueueRecord> queueRecords, QueueRecordList daffintyQueueRecords)
        {
            try
            {
                if ((queueRecords != null) && 
                    (daffintyQueueRecords?.List?.Count > 0))
                {
                    foreach (var queueRecord in daffintyQueueRecords.List.Where(queueRecord => !string.IsNullOrEmpty(queueRecord?.RecordData)))
                    {
                        queueRecords.Add(new QueueRecord
                        {
                            RecordId = queueRecord.Id,
                            CustomerId = queueRecord.CustomerId ?? "",
                            SourceSystemCompanyId = queueRecord.SourceSystemCompanyId ?? "",
                            UniversalNodeId = queueRecord.UniversalNodeId ?? "",
                            DataSourceType = queueRecord.DataSourceType ?? "",
                            Organization = queueRecord.Organization ?? "",
                            TableName = queueRecord.TableName ?? "",
                            Action = queueRecord.Action ?? "",
                            ErrorMessage = queueRecord.ErrorMessage ?? "",
                            RecordData = queueRecord.RecordData ?? "",
                            EnteredDateTime = queueRecord.EnteredDateTime.ToString("O"),
                            ProcessedDateTime = queueRecord.ProcessedDateTime.ToString("O"),
                            Result = queueRecord.Result,
                            Processed = queueRecord.Processed
                        });
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

        private static void GrpcRawDataRecords(ICollection<RawDataRecord> rawDataRecords, RawDataRecordList daffintyRawDataRecords)
        {
            try
            {
                if ((rawDataRecords != null) && 
                    (daffintyRawDataRecords?.List?.Count > 0))
                {
                    foreach (var rawDataRecord in daffintyRawDataRecords.List.Where(rawDataRecord => !string.IsNullOrEmpty(rawDataRecord?.RecordData)))
                    {
                        rawDataRecords.Add(new RawDataRecord
                        {
                            RecordId = rawDataRecord.Id,
                            CustomerId = rawDataRecord.CustomerId ?? "",
                            SourceSystemCompanyId = rawDataRecord.SourceSystemCompanyId ?? "",
                            UniversalNodeId = rawDataRecord.UniversalNodeId ?? "",
                            DataSourceType = rawDataRecord.DataSourceType ?? "",
                            Organization = rawDataRecord.Organization ?? "",
                            TableName = rawDataRecord.TableName ?? "",
                            Action = rawDataRecord.Action ?? "",
                            ErrorMessage = rawDataRecord.ErrorMessage ?? "",
                            RecordData = rawDataRecord.RecordData ?? "",
                            EnteredDateTime = rawDataRecord.EnteredDateTime.ToString("O"),
                            ProcessedDateTime = rawDataRecord.ProcessedDateTime.ToString("O"),
                            Result = rawDataRecord.Result,
                            Processed = rawDataRecord.Processed
                        });
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

        #endregion Private Methods
    }
}
