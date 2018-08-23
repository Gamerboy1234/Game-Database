
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveNet.Core.Log;
using ActiveNet.Types.Base;
using ActiveNet.Types.Database;

namespace Daffinity.Model
{
    [Serializable]
    public class RawDataRecord : AnModelBase
    {
        #region Constants

        public const string DbTableName = "RawDataRecord";
        
        public const int CustomerIdSize = 50;
        public const int SourceSystemCompanyIdSize = 100;
        public const int UniversalNodeIdSize = 50;
        public const int DataSourceTypeSize = 50;
        public const int OrganizationSize = 100;
        public const int TableNameSize = 100;
        public const int ActionSize = 10;
        private const int ErrorMessageSize = 200;
        public const int RecordDataSize = -1;

        #endregion Constants


        #region Properties

        public int Id { get; set; }
        public string CustomerId { get; set; }
        public string SourceSystemCompanyId { get; set; }
        public string UniversalNodeId { get; set; }
        public string DataSourceType { get; set; }
        public string Organization { get; set; }
        public string TableName { get; set; }
        public string Action { get; set; }
        public string ErrorMessage { get; set; }
        public string RecordData { get; set; }
        public DateTime EnteredDateTime { get; set; }
        public DateTime ProcessedDateTime { get; set; }
        public bool Result { get; set; }
        public bool Processed { get; set; }
        
        public bool RecordFound { get; set; }

        #endregion Properties


        #region Constructors

        public RawDataRecord()
        {
            Clear();
        }

        public RawDataRecord(
            int id,
            string customerId,
            string sourceSystemCompanyId,
            string universalNodeId,
            string dataSourceType,
            string organization,
            string tableName,
            string action,
            string errorMessage,
            string recordData,
            DateTime enteredDateTime,
            DateTime processedDateTime,
            bool result,
            bool processed)
        {
            Clear();

            try
            {
                Id = id;
                CustomerId = customerId ?? "";
                SourceSystemCompanyId = sourceSystemCompanyId ?? "";
                UniversalNodeId = universalNodeId ?? "";
                DataSourceType = dataSourceType ?? "";
                Organization = organization ?? "";
                TableName = tableName ?? "";
                Action = action ?? "";
                ErrorMessage = errorMessage ?? "";
                RecordData = recordData ?? "";
                EnteredDateTime = enteredDateTime;
                ProcessedDateTime = processedDateTime;
                Result = result;
                Processed = processed;
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }
        }

        private void Clear()
        {
            try
            {
                Id = 0;
                CustomerId = "";
                SourceSystemCompanyId = "";
                UniversalNodeId = "";
                DataSourceType = "";
                Organization = "";
                TableName = "";
                Action = "";
                ErrorMessage = "";
                RecordData = "";
                EnteredDateTime = AnTypes.MinDateTimeValue;
                ProcessedDateTime = AnTypes.MinDateTimeValue;
                Result = false;
                Processed = false;
                
                RecordFound = false;
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }
        }

        #endregion Constructors


        #region Abstract Methods
        
        public override string GetDbTableName()
        {
            return DbTableName;
        }
        
        protected override string GetKeyColumnName()
        {
            return nameof(Id);
        }
        
        protected override List<string> GenerateColumnNames(bool includeIdColumn)
        {
            var result = new List<string>();

            try
            {
                if (includeIdColumn)
                {
                    result.Add(nameof(Id));
                }

                result.Add(nameof(CustomerId));
                result.Add(nameof(SourceSystemCompanyId));
                result.Add(nameof(UniversalNodeId));
                result.Add(nameof(DataSourceType));
                result.Add(nameof(Organization));
                result.Add(nameof(TableName));
                result.Add(nameof(Action));
                result.Add(nameof(ErrorMessage));
                result.Add(nameof(RecordData));
                result.Add(nameof(EnteredDateTime));
                result.Add(nameof(ProcessedDateTime));
                result.Add(nameof(Result));
                result.Add(nameof(Processed));
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }
        
        protected override List<AnDatabaseCommandData> GenerateDatabaseCommandDataList(bool includeIdColumn)
        {
            List<AnDatabaseCommandData> result = null;

            try
            {
                result = new List<AnDatabaseCommandData>();

                if (includeIdColumn)
                {
                    AnDatabaseCommandData.Add(result, nameof(Id), Id, AnTypes.DataType.Int, AnSerialization.IntSize);
                }

                AnDatabaseCommandData.Add(result, nameof(CustomerId), CustomerId, AnTypes.DataType.VarChar, CustomerIdSize);
                AnDatabaseCommandData.Add(result, nameof(SourceSystemCompanyId), SourceSystemCompanyId, AnTypes.DataType.VarChar, SourceSystemCompanyIdSize);
                AnDatabaseCommandData.Add(result, nameof(UniversalNodeId), UniversalNodeId, AnTypes.DataType.VarChar, UniversalNodeIdSize);
                AnDatabaseCommandData.Add(result, nameof(DataSourceType), DataSourceType, AnTypes.DataType.VarChar, DataSourceTypeSize);
                AnDatabaseCommandData.Add(result, nameof(Organization), Organization, AnTypes.DataType.VarChar, OrganizationSize);
                AnDatabaseCommandData.Add(result, nameof(TableName), TableName, AnTypes.DataType.VarChar, TableNameSize);
                AnDatabaseCommandData.Add(result, nameof(Action), Action, AnTypes.DataType.VarChar, ActionSize);
                AnDatabaseCommandData.Add(result, nameof(ErrorMessage), ErrorMessage, AnTypes.DataType.VarChar, ErrorMessageSize);
                AnDatabaseCommandData.Add(result, nameof(RecordData), RecordData, AnTypes.DataType.VarChar, RecordDataSize);
                AnDatabaseCommandData.Add(result, nameof(EnteredDateTime), EnteredDateTime, AnTypes.DataType.DateTime, AnSerialization.DateTimeSize);
                AnDatabaseCommandData.Add(result, nameof(ProcessedDateTime), ProcessedDateTime, AnTypes.DataType.DateTime, AnSerialization.DateTimeSize);
                AnDatabaseCommandData.Add(result, nameof(Result), Result, AnTypes.DataType.Boolean, AnSerialization.BooleanSize);
                AnDatabaseCommandData.Add(result, nameof(Processed), Processed, AnTypes.DataType.Boolean, AnSerialization.BooleanSize);
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result ?? new AnList<AnDatabaseCommandData>();
        }
        
        #endregion Abstract Methods


        #region Public Methods

        public string GetMarkAsProcessedCommand(AnDatabaseTypes.DriverType driverType)
        {
            var result = "";

            try
            {
                if (ProcessedDateTime <= AnTypes.MinDateTimeValue)
                {
                    ProcessedDateTime = DateTime.UtcNow;
                }

                result = AnDatabaseTypes.GenerateUpdateStatement(DbTableName, GeneratePrimaryKeyWhereClause(driverType), 
                    new List<AnDatabaseCommandData>
                    {
                        new AnDatabaseCommandData(nameof(ProcessedDateTime), ProcessedDateTime, AnTypes.DataType.DateTime, AnSerialization.DateTimeSize),
                        new AnDatabaseCommandData(nameof(ErrorMessage), ErrorMessage, AnTypes.DataType.VarChar, ErrorMessageSize),
                        new AnDatabaseCommandData(nameof(Result), Result, AnTypes.DataType.Boolean, AnSerialization.BooleanSize),
                        new AnDatabaseCommandData(nameof(Processed), true, AnTypes.DataType.Boolean, AnSerialization.BooleanSize)
                    }, 
                    new List<AnDatabaseCommandData>(), driverType, false);
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        #endregion Public Methods
    }

    [Serializable]
    public class RawDataRecordList : AnModelBaseList
    {
        #region Properties

        public List<RawDataRecord> List { get; set; } = new List<RawDataRecord>();
        
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string ErrorMessage { get; set; } = "";

        #endregion Properties


        #region Public Methods

        public RawDataRecord GetById(int id)
        {
            try
            {
                foreach (var rawDataRecord in List.Where(rawDataRecord => (rawDataRecord != null)).Where(rawDataRecord => rawDataRecord.Id == id))
                {
                    return rawDataRecord;
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return null;
        }

        private RawDataRecord GetByData(
            string customerId,
            string sourceSystemCompanyId,
            string universalNodeId,
            string dataSourceType,
            string organization,
            string tableName,
            string recordData)
        {
            try
            {
                if (!string.IsNullOrEmpty(customerId) &&
                    !string.IsNullOrEmpty(sourceSystemCompanyId) &&
                    !string.IsNullOrEmpty(universalNodeId) &&
                    !string.IsNullOrEmpty(dataSourceType) &&
                    !string.IsNullOrEmpty(organization) &&
                    !string.IsNullOrEmpty(tableName) &&
                    !string.IsNullOrEmpty(recordData))
                {
                    foreach (var rawDataRecord in List.Where(rawDataRecord =>
                        !string.IsNullOrEmpty(rawDataRecord?.CustomerId) &&
                        !string.IsNullOrEmpty(rawDataRecord.SourceSystemCompanyId) && 
                        !string.IsNullOrEmpty(rawDataRecord.UniversalNodeId) && 
                        !string.IsNullOrEmpty(rawDataRecord.DataSourceType) && 
                        !string.IsNullOrEmpty(rawDataRecord.Organization) && 
                        !string.IsNullOrEmpty(rawDataRecord.TableName) && 
                        !string.IsNullOrEmpty(rawDataRecord.RecordData) && 
                        string.Equals(rawDataRecord.CustomerId ?? "", customerId, StringComparison.OrdinalIgnoreCase) && 
                        string.Equals(rawDataRecord.SourceSystemCompanyId ?? "", sourceSystemCompanyId, StringComparison.OrdinalIgnoreCase) && 
                        string.Equals(rawDataRecord.UniversalNodeId ?? "", universalNodeId, StringComparison.OrdinalIgnoreCase) && 
                        string.Equals(rawDataRecord.DataSourceType ?? "", dataSourceType, StringComparison.OrdinalIgnoreCase) && 
                        string.Equals(rawDataRecord.Organization ?? "", organization, StringComparison.OrdinalIgnoreCase) && 
                        string.Equals(rawDataRecord.TableName ?? "", tableName, StringComparison.OrdinalIgnoreCase) && 
                        string.Equals(rawDataRecord.RecordData ?? "", recordData, StringComparison.OrdinalIgnoreCase)))
                    {
                        return rawDataRecord;
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return null;
        }

        public bool Exists(int id)
        {
            var result = false;

            try
            {
                result = (GetById(id) != null);
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        private bool Exists(
            string customerId,
            string sourceSystemCompanyId,
            string universalNodeId,
            string dataSourceType,
            string organization,
            string tableName,
            string recordData)
        {
            var result = false;

            try
            {
                result = (GetByData(customerId, sourceSystemCompanyId, universalNodeId, dataSourceType, organization, tableName, recordData) != null);
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public bool Add(RawDataRecord rawDataRecord)
        {
            var result = false;

            try
            {
                if (rawDataRecord != null)
                {
                    if (rawDataRecord.Id > 0)
                    {
                        if (!Exists(rawDataRecord.Id))
                        {
                            List.Add(rawDataRecord);

                            result = Exists(rawDataRecord.Id);
                        }
                    }

                    else
                    {
                        if (!Exists(rawDataRecord.CustomerId,
                            rawDataRecord.SourceSystemCompanyId,
                            rawDataRecord.UniversalNodeId,
                            rawDataRecord.DataSourceType,
                            rawDataRecord.Organization,
                            rawDataRecord.TableName,
                            rawDataRecord.RecordData))
                        {
                            List.Add(rawDataRecord);

                            result = Exists(rawDataRecord.CustomerId,
                                rawDataRecord.SourceSystemCompanyId,
                                rawDataRecord.UniversalNodeId,
                                rawDataRecord.DataSourceType,
                                rawDataRecord.Organization,
                                rawDataRecord.TableName,
                                rawDataRecord.RecordData);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public void Remove(int id)
        {
            try
            {
                var removeRawDataRecord = GetById(id);

                if (removeRawDataRecord != null)
                {
                    List.Remove(removeRawDataRecord);
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }
        }

        public static string GenerateSelectQuery(string customerId, string universalNodeId, string dataSourceType, DateTime beginDateTime, DateTime endDateTime, AnDatabaseTypes.DriverType driverType)
        {
            var result = "";

            try
            {
                if (!string.IsNullOrEmpty(customerId) && !string.IsNullOrEmpty(universalNodeId))
                {
                    var whereClause = new StringBuilder();

                    whereClause.Append($"{nameof(RawDataRecord.CustomerId)} = '{AnDatabaseTypes.FormatSingleQuote(AnDatabaseTypes.ValidStringLength(customerId, RawDataRecord.CustomerIdSize))}'");
                    whereClause.Append(" AND ");
                    whereClause.Append($"{nameof(RawDataRecord.UniversalNodeId)} = '{AnDatabaseTypes.FormatSingleQuote(AnDatabaseTypes.ValidStringLength(universalNodeId, RawDataRecord.UniversalNodeIdSize))}'");

                    if (!string.IsNullOrEmpty(dataSourceType))
                    {
                        whereClause.Append(" AND ");
                        whereClause.Append($"{nameof(RawDataRecord.DataSourceType)} = '{AnDatabaseTypes.FormatSingleQuote(AnDatabaseTypes.ValidStringLength(dataSourceType, RawDataRecord.DataSourceTypeSize))}'");
                    }

                    if ((DateTime.Compare(beginDateTime, AnTypes.MinDateTimeValue) > 0) &&
                        (DateTime.Compare(endDateTime, beginDateTime) > 0))
                    {
                        whereClause.Append(" AND ");
                        whereClause.Append($"{nameof(RawDataRecord.EnteredDateTime)} > '{AnTypes.SqlDateTimeString(beginDateTime)}'");
                        whereClause.Append(" AND ");
                        whereClause.Append($"{nameof(RawDataRecord.EnteredDateTime)} <= '{AnTypes.SqlDateTimeString(endDateTime)}'");
                    }

                    whereClause.Append(" AND ");
                    whereClause.Append($"{nameof(RawDataRecord.Processed)} = {AnDatabaseTypes.FormatBooleanValue(false, driverType)} ");
                    whereClause.Append("ORDER BY ");
                    whereClause.Append(AnDatabaseTypes.FormatDatabaseColumnName(nameof(RawDataRecord.Id), RawDataRecord.DbTableName, AnDatabaseTypes.ExecutionType.Query, driverType));

                    result = AnDatabaseTypes.GenerateSelectQuery(driverType, RawDataRecord.DbTableName, AnDatabaseTypes.FormatColumnNames(RawDataRecord.DbTableName, new RawDataRecord().GetColumnNames(true), driverType), whereClause.ToString());
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public static string GenerateSelectQuery(string customerId, string universalNodeId, string dataSourceType, DateTime beginDateTime, DateTime endDateTime, bool processed, AnDatabaseTypes.DriverType driverType)
        {
            var result = "";

            try
            {
                var whereClause = new StringBuilder();

                whereClause.Append($"{nameof(RawDataRecord.Processed)} = {AnDatabaseTypes.FormatBooleanValue(processed, driverType)}");

                if (!string.IsNullOrEmpty(customerId))
                {
                    whereClause.Append(" AND ");
                    whereClause.Append($"{nameof(RawDataRecord.CustomerId)} = '{AnDatabaseTypes.FormatSingleQuote(AnDatabaseTypes.ValidStringLength(customerId, RawDataRecord.CustomerIdSize))}'");
                }

                if (!string.IsNullOrEmpty(universalNodeId))
                {
                    whereClause.Append(" AND ");
                    whereClause.Append($"{nameof(RawDataRecord.UniversalNodeId)} = '{AnDatabaseTypes.FormatSingleQuote(AnDatabaseTypes.ValidStringLength(universalNodeId, RawDataRecord.UniversalNodeIdSize))}'");
                }

                if (!string.IsNullOrEmpty(dataSourceType))
                {
                    whereClause.Append(" AND ");
                    whereClause.Append($"{nameof(RawDataRecord.DataSourceType)} = '{AnDatabaseTypes.FormatSingleQuote(AnDatabaseTypes.ValidStringLength(dataSourceType, RawDataRecord.DataSourceTypeSize))}'");
                }

                if ((DateTime.Compare(beginDateTime, AnTypes.MinDateTimeValue) > 0) &&
                    (DateTime.Compare(endDateTime, beginDateTime) > 0))
                {
                    whereClause.Append(" AND ");
                    whereClause.Append($"{nameof(RawDataRecord.EnteredDateTime)} > '{AnTypes.SqlDateTimeString(beginDateTime)}'");
                    whereClause.Append(" AND ");
                    whereClause.Append($"{nameof(RawDataRecord.EnteredDateTime)} <= '{AnTypes.SqlDateTimeString(endDateTime)}'");
                }

                whereClause.Append(" ORDER BY ");
                whereClause.Append(AnDatabaseTypes.FormatDatabaseColumnName(nameof(RawDataRecord.Id), RawDataRecord.DbTableName, AnDatabaseTypes.ExecutionType.Query, driverType));

                result = AnDatabaseTypes.GenerateSelectQuery(driverType, RawDataRecord.DbTableName, AnDatabaseTypes.FormatColumnNames(RawDataRecord.DbTableName, new RawDataRecord().GetColumnNames(true), driverType), whereClause.ToString());
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public static string GenerateSelectQueryForNotProcessedRecords(AnDatabaseTypes.DriverType driverType)
        {
            var result = "";

            try
            {
                var whereClause = new StringBuilder();

                whereClause.Append($"{nameof(RawDataRecord.Processed)} = {AnDatabaseTypes.FormatBooleanValue(false, driverType)} ");
                whereClause.Append("ORDER BY ");
                whereClause.Append(AnDatabaseTypes.FormatDatabaseColumnName(nameof(RawDataRecord.Id), RawDataRecord.DbTableName, AnDatabaseTypes.ExecutionType.Query, driverType));

                result = AnDatabaseTypes.GenerateSelectQuery(driverType, RawDataRecord.DbTableName, AnDatabaseTypes.FormatColumnNames(RawDataRecord.DbTableName, new RawDataRecord().GetColumnNames(true), driverType), whereClause.ToString());
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public static string GenerateSelectQuery(string customerId, string sourceSystemCompanyId, string universalNodeId, string dataSourceType, string organization, string tableName, string action, AnDatabaseTypes.DriverType driverType)
        {
            var result = "";

            try
            {
                var whereClause = new StringBuilder();

                if (!string.IsNullOrEmpty(customerId) &&
                    !string.IsNullOrEmpty(sourceSystemCompanyId) &&
                    !string.IsNullOrEmpty(universalNodeId) &&
                    !string.IsNullOrEmpty(dataSourceType) &&
                    !string.IsNullOrEmpty(tableName) &&
                    !string.IsNullOrEmpty(action))
                {
                    whereClause.Append($"{nameof(RawDataRecord.CustomerId)} = '{AnDatabaseTypes.FormatSingleQuote(AnDatabaseTypes.ValidStringLength(customerId, RawDataRecord.CustomerIdSize))}'");
                    whereClause.Append(" AND ");
                    whereClause.Append($"{nameof(RawDataRecord.SourceSystemCompanyId)} = '{AnDatabaseTypes.FormatSingleQuote(AnDatabaseTypes.ValidStringLength(sourceSystemCompanyId, RawDataRecord.SourceSystemCompanyIdSize))}'");
                    whereClause.Append(" AND ");
                    whereClause.Append($"{nameof(RawDataRecord.UniversalNodeId)} = '{AnDatabaseTypes.FormatSingleQuote(AnDatabaseTypes.ValidStringLength(universalNodeId, RawDataRecord.UniversalNodeIdSize))}'");
                    whereClause.Append(" AND ");
                    whereClause.Append($"{nameof(RawDataRecord.DataSourceType)} = '{AnDatabaseTypes.FormatSingleQuote(AnDatabaseTypes.ValidStringLength(dataSourceType, RawDataRecord.DataSourceTypeSize))}'");
                    whereClause.Append(" AND ");
                    whereClause.Append($"{nameof(RawDataRecord.Organization)} = '{AnDatabaseTypes.FormatSingleQuote(AnDatabaseTypes.ValidStringLength(organization ?? "", RawDataRecord.OrganizationSize))}'");
                    whereClause.Append(" AND ");
                    whereClause.Append($"{nameof(RawDataRecord.TableName)} = '{AnDatabaseTypes.FormatSingleQuote(AnDatabaseTypes.ValidStringLength(tableName, RawDataRecord.TableNameSize))}'");
                    whereClause.Append(" AND ");
                    whereClause.Append($"{nameof(RawDataRecord.Action)} = '{AnDatabaseTypes.FormatSingleQuote(AnDatabaseTypes.ValidStringLength(action, RawDataRecord.ActionSize))}'");

                    result = AnDatabaseTypes.GenerateSelectQuery(driverType, RawDataRecord.DbTableName, AnDatabaseTypes.FormatColumnNames(RawDataRecord.DbTableName, new RawDataRecord().GetColumnNames(true), driverType), whereClause.ToString());
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        #endregion Public Methods
    }
}
