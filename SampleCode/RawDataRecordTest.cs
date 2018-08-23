
using System;
using System.Linq;
using ActiveNet.Types.Base;
using ActiveNet.Types.Database;
using Daffinity.Model;
using Xunit;

namespace DaffinityModelTest.UnitTests
{
    public class RawDataRecordTest : CollectorDbTestBase
    {
        #region Test Cases

        [Fact]
        public void RawDataRecordTestConstructorTest()
        {
            var rawDataRecord = new RawDataRecord();

            Assert.Equal(rawDataRecord.Id, 0);
            Assert.Equal(rawDataRecord.CustomerId, "");
            Assert.Equal(rawDataRecord.SourceSystemCompanyId, "");
            Assert.Equal(rawDataRecord.UniversalNodeId, "");
            Assert.Equal(rawDataRecord.DataSourceType, "");
            Assert.Equal(rawDataRecord.Organization, "");
            Assert.Equal(rawDataRecord.TableName, "");
            Assert.Equal(rawDataRecord.Action, "");
            Assert.Equal(rawDataRecord.ErrorMessage, "");
            Assert.Equal(rawDataRecord.RecordData, "");
            Assert.Equal(rawDataRecord.EnteredDateTime, AnTypes.MinDateTimeValue);
            Assert.Equal(rawDataRecord.ProcessedDateTime, AnTypes.MinDateTimeValue);
            Assert.Equal(rawDataRecord.Result, false);
            Assert.Equal(rawDataRecord.Processed, false);
            Assert.Equal(rawDataRecord.RecordFound, false);
        }

        [Fact]
        public void RawDataRecordPropertiesTest()
        {
            var rawDataRecord = new RawDataRecord();

            Assert.Equal(rawDataRecord.Id, 0);
            rawDataRecord.Id = 1;
            Assert.Equal(rawDataRecord.Id, 1);

            Assert.Equal(rawDataRecord.CustomerId, "");
            rawDataRecord.CustomerId = "CustomerId";
            Assert.Equal(rawDataRecord.CustomerId, "CustomerId");

            Assert.Equal(rawDataRecord.SourceSystemCompanyId, "");
            rawDataRecord.SourceSystemCompanyId = "SourceSystemCompanyId";
            Assert.Equal(rawDataRecord.SourceSystemCompanyId, "SourceSystemCompanyId");
            
            Assert.Equal(rawDataRecord.UniversalNodeId, "");
            rawDataRecord.UniversalNodeId = "UniversalNodeId";
            Assert.Equal(rawDataRecord.UniversalNodeId, "UniversalNodeId");

            Assert.Equal(rawDataRecord.DataSourceType, "");
            rawDataRecord.DataSourceType = "DataSourceType";
            Assert.Equal(rawDataRecord.DataSourceType, "DataSourceType");

            Assert.Equal(rawDataRecord.Organization, "");
            rawDataRecord.Organization = "Organization";
            Assert.Equal(rawDataRecord.Organization, "Organization");

            Assert.Equal(rawDataRecord.TableName, "");
            rawDataRecord.TableName = "TableName";
            Assert.Equal(rawDataRecord.TableName, "TableName");

            Assert.Equal(rawDataRecord.Action, "");
            rawDataRecord.Action = "Action";
            Assert.Equal(rawDataRecord.Action, "Action");

            Assert.Equal(rawDataRecord.ErrorMessage, "");
            rawDataRecord.ErrorMessage = "ErrorMessage";
            Assert.Equal(rawDataRecord.ErrorMessage, "ErrorMessage");

            Assert.Equal(rawDataRecord.RecordData, "");
            rawDataRecord.RecordData = "RecordData";
            Assert.Equal(rawDataRecord.RecordData, "RecordData");

            var now = DateTime.Now;

            Assert.Equal(rawDataRecord.EnteredDateTime, AnTypes.MinDateTimeValue);
            rawDataRecord.EnteredDateTime = now;
            Assert.Equal(rawDataRecord.EnteredDateTime, now);

            Assert.Equal(rawDataRecord.ProcessedDateTime, AnTypes.MinDateTimeValue);
            rawDataRecord.ProcessedDateTime = now;
            Assert.Equal(rawDataRecord.ProcessedDateTime, now);

            Assert.Equal(rawDataRecord.Result, false);
            rawDataRecord.Result = true;
            Assert.Equal(rawDataRecord.Result, true);

            Assert.Equal(rawDataRecord.Processed, false);
            rawDataRecord.Processed = true;
            Assert.Equal(rawDataRecord.Processed, true);

            Assert.Equal(rawDataRecord.RecordFound, false);
            rawDataRecord.RecordFound = true;
            Assert.Equal(rawDataRecord.RecordFound, true);
        }

        [Fact]
        public void RawDataRecordGenerateInsertStatementTest()
        {
            var rawDataRecord = new RawDataRecord(
                1,
                "CustomerId",
                "SourceSystemCompanyId",
                "UniversalNodeId",
                "DataSourceType",
                "Organization",
                "Attachment",
                "ADD",
                "ErrorMessage",
                "RecordData",
                new DateTime(2015, 11, 29, 1, 1, 1),
                AnTypes.MinDateTimeValue,
                false,
                false);

            Assert.Equal(rawDataRecord.GenerateInsertStatement(AnDatabaseTypes.DriverType.MsSqlServerDotNet), "INSERT INTO RawDataRecord (CustomerId, SourceSystemCompanyId, UniversalNodeId, DataSourceType, Organization, TableName, Action, ErrorMessage, RecordData, EnteredDateTime, ProcessedDateTime, Result, Processed) VALUES ('CustomerId', 'SourceSystemCompanyId', 'UniversalNodeId', 'DataSourceType', 'Organization', 'Attachment', 'ADD', 'ErrorMessage', 'RecordData', '2015-11-29 01:01:01', '1900-01-01 00:00:00', 0, 0)");
        }

        [Fact]
        public void RawDataRecordGenerateUpdateStatementTest()
        {
            var rawDataRecord = new RawDataRecord(
                1,
                "CustomerId",
                "SourceSystemCompanyId",
                "UniversalNodeId",
                "DataSourceType",
                "Organization",
                "Attachment",
                "ADD",
                "ErrorMessage",
                "RecordData",
                new DateTime(2015, 11, 29, 1, 1, 1),
                AnTypes.MinDateTimeValue,
                false,
                false);

            Assert.Equal(rawDataRecord.GenerateUpdateStatement<RawDataRecord>(AnDatabaseTypes.DriverType.MsSqlServerDotNet, null), "UPDATE RawDataRecord SET CustomerId='CustomerId', SourceSystemCompanyId='SourceSystemCompanyId', UniversalNodeId='UniversalNodeId', DataSourceType='DataSourceType', Organization='Organization', TableName='Attachment', Action='ADD', ErrorMessage='ErrorMessage', RecordData='RecordData', EnteredDateTime='2015-11-29 01:01:01', ProcessedDateTime='1900-01-01 00:00:00', Result=0, Processed=0 WHERE Id = 1");

            var updateRawDataRecord = new RawDataRecord(
                1,
                "CustomerId Edited",
                "SourceSystemCompanyId Edited",
                "UniversalNodeId Edited",
                "DataSourceType Edited",
                "Organization Edited",
                "Attachment",
                "EDIT",
                "RecordData Edited",
                "ErrorMessage Edited",
                new DateTime(2015, 11, 30, 1, 1, 1),
                new DateTime(2015, 12, 1, 1, 1, 1),
                true,
                true);

            Assert.Equal(updateRawDataRecord.GenerateUpdateStatement(AnDatabaseTypes.DriverType.MsSqlServerDotNet, rawDataRecord), "UPDATE RawDataRecord SET CustomerId='CustomerId Edited', SourceSystemCompanyId='SourceSystemCompanyId Edited', UniversalNodeId='UniversalNodeId Edited', DataSourceType='DataSourceType Edited', Organization='Organization Edited', Action='EDIT', ErrorMessage='RecordData Edited', RecordData='ErrorMessage Edited', EnteredDateTime='2015-11-30 01:01:01', ProcessedDateTime='2015-12-01 01:01:01', Result=1, Processed=1 WHERE Id = 1");
        }

        [Fact]
        public void RawDataRecordGetMarkAsProcessedCommandTest()
        {
            var rawDataRecord = new RawDataRecord(
                1,
                "CustomerId",
                "SourceSystemCompanyId",
                "UniversalNodeId",
                "DataSourceType",
                "Organization",
                "Attachment",
                "ADD",
                "ErrorMessage",
                "RecordData",
                new DateTime(2015, 11, 29, 1, 1, 1),
                new DateTime(2015, 11, 30, 3, 4, 5),
                false,
                false);

            Assert.Equal(rawDataRecord.GetMarkAsProcessedCommand(AnDatabaseTypes.DriverType.MsSqlServerDotNet), "UPDATE RawDataRecord SET ProcessedDateTime='2015-11-30 03:04:05', ErrorMessage='ErrorMessage', Result=0, Processed=1 WHERE Id = 1");
        }

        [Fact]
        public void RawDataRecordGenerateDeleteStatementTest()
        {
            var rawDataRecord = new RawDataRecord(
                1,
                "CustomerId",
                "SourceSystemCompanyId",
                "UniversalNodeId",
                "DataSourceType",
                "Organization",
                "Attachment",
                "ADD",
                "ErrorMessage",
                "RecordData",
                new DateTime(2015, 11, 29, 1, 1, 1),
                AnTypes.MinDateTimeValue,
                false,
                false);

            Assert.Equal(rawDataRecord.GenerateDeleteStatement(AnDatabaseTypes.DriverType.MsSqlServerDotNet), "DELETE FROM RawDataRecord WHERE Id = 1");
        }

        [Fact]
        public void RawDataRecordGenerateExistsQueryTest()
        {
            var rawDataRecord = new RawDataRecord(
                1,
                "CustomerId",
                "SourceSystemCompanyId",
                "UniversalNodeId",
                "DataSourceType",
                "Organization",
                "Attachment",
                "ADD",
                "ErrorMessage",
                "RecordData",
                new DateTime(2015, 11, 29, 1, 1, 1),
                AnTypes.MinDateTimeValue,
                false,
                false);

            Assert.Equal(rawDataRecord.GenerateExistsQuery(AnDatabaseTypes.DriverType.MsSqlServerDotNet), "SELECT Id FROM RawDataRecord WHERE Id = 1");
        }

        [Fact]
        public void RawDataRecordGenerateSelectQueryTest()
        {
            var rawDataRecord = new RawDataRecord();

            Assert.Equal(rawDataRecord.GenerateSelectQuery(AnDatabaseTypes.DriverType.MsSqlServerDotNet), "SELECT Id, CustomerId, SourceSystemCompanyId, UniversalNodeId, DataSourceType, Organization, TableName, Action, ErrorMessage, RecordData, EnteredDateTime, ProcessedDateTime, Result, Processed FROM RawDataRecord WHERE Id = 0");

            rawDataRecord = new RawDataRecord(
                1,
                "CustomerId",
                "SourceSystemCompanyId",
                "UniversalNodeId",
                "DataSourceType",
                "Organization",
                "Attachment",
                "ADD",
                "ErrorMessage",
                "RecordData",
                new DateTime(2015, 11, 29, 1, 1, 1),
                AnTypes.MinDateTimeValue,
                false,
                false);

            Assert.Equal(rawDataRecord.GenerateSelectQuery(AnDatabaseTypes.DriverType.MsSqlServerDotNet), "SELECT Id, CustomerId, SourceSystemCompanyId, UniversalNodeId, DataSourceType, Organization, TableName, Action, ErrorMessage, RecordData, EnteredDateTime, ProcessedDateTime, Result, Processed FROM RawDataRecord WHERE Id = 1");
        }

        [Fact]
        public void RawDataRecordGeneratePrimaryKeyWhereClauseTest()
        {
            var rawDataRecord = new RawDataRecord();

            Assert.Equal(rawDataRecord.GeneratePrimaryKeyWhereClause(AnDatabaseTypes.DriverType.MsSqlServerDotNet), "Id = 0");

            rawDataRecord = new RawDataRecord(
                1,
                "CustomerId",
                "SourceSystemCompanyId",
                "UniversalNodeId",
                "DataSourceType",
                "Organization",
                "Attachment",
                "ADD",
                "ErrorMessage",
                "RecordData",
                new DateTime(2015, 11, 29, 1, 1, 1),
                AnTypes.MinDateTimeValue,
                false,
                false);

            Assert.Equal(rawDataRecord.GeneratePrimaryKeyWhereClause(AnDatabaseTypes.DriverType.MsSqlServerDotNet), "Id = 1");
        }

        [Fact]
        public void RawDataRecordDatabaseCommandsTest()
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // SqlServer
            //////////////////////////////////////////////////////////////////////////////////////////////////////

            Assert.Equal(IsSqlServerConnected, true);


            // Select All

            var rawDataRecord = new RawDataRecord();

            var selectQuery = rawDataRecord.GenerateSelectQuery(AnDatabaseTypes.DriverType.MsSqlServerDotNet);

            Assert.Equal(string.IsNullOrEmpty(selectQuery), false);

            var errorMessage = "";
            var selectResult = SqlServerConnection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.NotNull(selectResult);

            var selectResultList = AnDataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            if (selectResultList.Count < 1)
            {
                rawDataRecord = new RawDataRecord(
                    0,
                    "CustomerId",
                    "SourceSystemCompanyId",
                    "UniversalNodeId",
                    "DataSourceType",
                    "Organization",
                    "Attachment",
                    "ADD",
                    "ErrorMessage",
                    "RecordData",
                    new DateTime(2015, 11, 29, 1, 1, 1),
                    AnTypes.MinDateTimeValue,
                    false,
                    false);

                SqlServerConnection.ExecuteCommand(rawDataRecord.GenerateInsertStatement(AnDatabaseTypes.DriverType.MsSqlServerDotNet), ref errorMessage, out _);
            }

            else
            {
                Assert.True(selectResultList.Count > 0);
            }


            // Insert

            rawDataRecord = new RawDataRecord(
                0,
                "CustomerId",
                "SourceSystemCompanyId",
                "UniversalNodeId",
                "DataSourceType",
                "Organization",
                "Attachment",
                "ADD",
                "ErrorMessage",
                "RecordData",
                new DateTime(2015, 11, 29, 1, 1, 1),
                AnTypes.MinDateTimeValue,
                false,
                false);

            var insertCommand = rawDataRecord.GenerateInsertStatement(AnDatabaseTypes.DriverType.MsSqlServerDotNet);

            Assert.Equal(string.IsNullOrEmpty(insertCommand), false);

            errorMessage = "";
            var insertResult = SqlServerConnection.ExecuteCommand(insertCommand, ref errorMessage, out var newId);

            Assert.Equal(insertResult, true);
            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.Equal(newId > 0, true);

            rawDataRecord.Id = newId;


            // Exists

            var existsQuery = rawDataRecord.GenerateExistsQuery(AnDatabaseTypes.DriverType.MsSqlServerDotNet);

            Assert.Equal(string.IsNullOrEmpty(existsQuery), false);

            errorMessage = "";
            var existsResult = SqlServerConnection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.NotNull(existsResult);

            var existsResultList = AnDataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            var recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.Equal(recordExists, true);


            // Select

            selectQuery = rawDataRecord.GenerateSelectQuery(AnDatabaseTypes.DriverType.MsSqlServerDotNet);

            Assert.Equal(string.IsNullOrEmpty(selectQuery), false);

            errorMessage = "";
            selectResult = SqlServerConnection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.NotNull(selectResult);

            selectResultList = AnDataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            RawDataRecord foundRawDataRecord = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundRawDataRecord = AnModelBase.FromDictionary<RawDataRecord>(dictionary);
                    break;
                }
            }

            Assert.NotNull(foundRawDataRecord);

            Assert.NotSame(rawDataRecord, foundRawDataRecord);

            Assert.Equal(rawDataRecord.CustomerId, foundRawDataRecord.CustomerId);
            Assert.Equal(rawDataRecord.SourceSystemCompanyId, foundRawDataRecord.SourceSystemCompanyId);
            Assert.Equal(rawDataRecord.UniversalNodeId, foundRawDataRecord.UniversalNodeId);
            Assert.Equal(rawDataRecord.DataSourceType, foundRawDataRecord.DataSourceType);
            Assert.Equal(rawDataRecord.Organization, foundRawDataRecord.Organization);
            Assert.Equal(rawDataRecord.TableName, foundRawDataRecord.TableName);
            Assert.Equal(rawDataRecord.Action, foundRawDataRecord.Action);
            Assert.Equal(rawDataRecord.ErrorMessage, foundRawDataRecord.ErrorMessage);
            Assert.Equal(rawDataRecord.RecordData, foundRawDataRecord.RecordData);
            Assert.Equal(DateTime.Compare(rawDataRecord.EnteredDateTime, foundRawDataRecord.EnteredDateTime), 0);
            Assert.Equal(DateTime.Compare(rawDataRecord.ProcessedDateTime, foundRawDataRecord.ProcessedDateTime), 0);
            Assert.Equal(rawDataRecord.Result, foundRawDataRecord.Result);
            Assert.Equal(rawDataRecord.Processed, foundRawDataRecord.Processed);
            Assert.Equal(rawDataRecord.RecordFound, foundRawDataRecord.RecordFound);


            // Update

            var updateRawDataRecord = new RawDataRecord(
                newId,
                "CustomerId Edited",
                "SourceSystemCompanyId Edited",
                "UniversalNodeId Edited",
                "DataSourceType Edited",
                "Organization Edited",
                "Attachment Edited",
                "EDIT",
                "ErrorMessage Edited",
                "RecordData Edited",
                new DateTime(2015, 12, 29, 1, 1, 1),
                new DateTime(2015, 12, 30, 2, 2, 2),
                false,
                false);

            var updateCommand = updateRawDataRecord.GenerateUpdateStatement(AnDatabaseTypes.DriverType.MsSqlServerDotNet, rawDataRecord);

            Assert.Equal(string.IsNullOrEmpty(updateCommand), false);

            errorMessage = "";
            var updateResult = SqlServerConnection.ExecuteCommand(updateCommand, ref errorMessage);

            Assert.Equal(updateResult, true);
            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);

                
            // Exists

            existsQuery = updateRawDataRecord.GenerateExistsQuery(AnDatabaseTypes.DriverType.MsSqlServerDotNet);

            Assert.Equal(string.IsNullOrEmpty(existsQuery), false);

            errorMessage = "";
            existsResult = SqlServerConnection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.NotNull(existsResult);

            existsResultList = AnDataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.Equal(recordExists, true);


            // Select

            selectQuery = updateRawDataRecord.GenerateSelectQuery(AnDatabaseTypes.DriverType.MsSqlServerDotNet);

            Assert.Equal(string.IsNullOrEmpty(selectQuery), false);

            errorMessage = "";
            selectResult = SqlServerConnection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.NotNull(selectResult);

            selectResultList = AnDataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundRawDataRecord = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundRawDataRecord = AnModelBase.FromDictionary<RawDataRecord>(dictionary);
                    break;
                }
            }

            Assert.NotNull(foundRawDataRecord);

            Assert.NotSame(updateRawDataRecord, foundRawDataRecord);

            Assert.Equal(updateRawDataRecord.Id, foundRawDataRecord.Id);
            Assert.Equal(updateRawDataRecord.CustomerId, foundRawDataRecord.CustomerId);
            Assert.Equal(updateRawDataRecord.SourceSystemCompanyId, foundRawDataRecord.SourceSystemCompanyId);
            Assert.Equal(updateRawDataRecord.UniversalNodeId, foundRawDataRecord.UniversalNodeId);
            Assert.Equal(updateRawDataRecord.DataSourceType, foundRawDataRecord.DataSourceType);
            Assert.Equal(updateRawDataRecord.Organization, foundRawDataRecord.Organization);
            Assert.Equal(updateRawDataRecord.TableName, foundRawDataRecord.TableName);
            Assert.Equal(updateRawDataRecord.Action, foundRawDataRecord.Action);
            Assert.Equal(updateRawDataRecord.ErrorMessage, foundRawDataRecord.ErrorMessage);
            Assert.Equal(updateRawDataRecord.RecordData, foundRawDataRecord.RecordData);
            Assert.Equal(DateTime.Compare(updateRawDataRecord.EnteredDateTime, foundRawDataRecord.EnteredDateTime), 0);
            Assert.Equal(DateTime.Compare(updateRawDataRecord.ProcessedDateTime, foundRawDataRecord.ProcessedDateTime), 0);
            Assert.Equal(updateRawDataRecord.Result, foundRawDataRecord.Result);
            Assert.Equal(updateRawDataRecord.Processed, foundRawDataRecord.Processed);
            Assert.Equal(updateRawDataRecord.RecordFound, foundRawDataRecord.RecordFound);


            // Delete

            var deleteCommand = rawDataRecord.GenerateDeleteStatement(AnDatabaseTypes.DriverType.MsSqlServerDotNet);

            Assert.Equal(string.IsNullOrEmpty(deleteCommand), false);

            errorMessage = "";
            var deleteResult = SqlServerConnection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.Equal(deleteResult, true);
            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);

                
            // Exists

            existsQuery = rawDataRecord.GenerateExistsQuery(AnDatabaseTypes.DriverType.MsSqlServerDotNet);

            Assert.Equal(string.IsNullOrEmpty(existsQuery), false);

            errorMessage = "";
            existsResult = SqlServerConnection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.NotNull(existsResult);

            existsResultList = AnDataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.Equal(recordExists, false);


            // Select

            selectQuery = rawDataRecord.GenerateSelectQuery(AnDatabaseTypes.DriverType.MsSqlServerDotNet);

            Assert.Equal(string.IsNullOrEmpty(selectQuery), false);

            errorMessage = "";
            selectResult = SqlServerConnection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.NotNull(selectResult);

            selectResultList = AnDataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundRawDataRecord = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundRawDataRecord = AnModelBase.FromDictionary<RawDataRecord>(dictionary);
                    break;
                }
            }

            Assert.Equal(foundRawDataRecord != null, false);


            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // Postgres
            //////////////////////////////////////////////////////////////////////////////////////////////////////

            Assert.Equal(IsPostgresConnected, true);


            // Select All

            rawDataRecord = new RawDataRecord();

            selectQuery = rawDataRecord.GenerateSelectQuery(AnDatabaseTypes.DriverType.PostgresDotNet);

            Assert.Equal(string.IsNullOrEmpty(selectQuery), false);

            errorMessage = "";
            selectResult = PostgresConnection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.NotNull(selectResult);

            selectResultList = AnDataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            if (selectResultList.Count < 1)
            {
                rawDataRecord = new RawDataRecord(
                    0,
                    "CustomerId",
                    "SourceSystemCompanyId",
                    "UniversalNodeId",
                    "DataSourceType",
                    "Organization",
                    "Attachment",
                    "ADD",
                    "ErrorMessage",
                    "RecordData",
                    new DateTime(2015, 11, 29, 1, 1, 1),
                    AnTypes.MinDateTimeValue,
                    false,
                    false);

                PostgresConnection.ExecuteCommand(rawDataRecord.GenerateInsertStatement(AnDatabaseTypes.DriverType.PostgresDotNet), ref errorMessage, out _);
            }

            else
            {
                Assert.True(selectResultList.Count > 0);
            }


            // Insert

            rawDataRecord = new RawDataRecord(
                0,
                "CustomerId",
                "SourceSystemCompanyId",
                "UniversalNodeId",
                "DataSourceType",
                "Organization",
                "Attachment",
                "ADD",
                "ErrorMessage",
                "RecordData",
                new DateTime(2015, 11, 29, 1, 1, 1),
                AnTypes.MinDateTimeValue,
                false,
                false);

            insertCommand = rawDataRecord.GenerateInsertStatement(AnDatabaseTypes.DriverType.PostgresDotNet);

            Assert.Equal(string.IsNullOrEmpty(insertCommand), false);

            errorMessage = "";
            insertResult = PostgresConnection.ExecuteCommand(insertCommand, ref errorMessage, out newId);

            Assert.Equal(insertResult, true);
            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.Equal(newId > 0, true);

            rawDataRecord.Id = newId;


            // Exists

            existsQuery = rawDataRecord.GenerateExistsQuery(AnDatabaseTypes.DriverType.PostgresDotNet);

            Assert.Equal(string.IsNullOrEmpty(existsQuery), false);

            errorMessage = "";
            existsResult = PostgresConnection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.NotNull(existsResult);

            existsResultList = AnDataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.Equal(recordExists, true);


            // Select

            selectQuery = rawDataRecord.GenerateSelectQuery(AnDatabaseTypes.DriverType.PostgresDotNet);

            Assert.Equal(string.IsNullOrEmpty(selectQuery), false);

            errorMessage = "";
            selectResult = PostgresConnection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.NotNull(selectResult);

            selectResultList = AnDataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundRawDataRecord = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundRawDataRecord = AnModelBase.FromDictionary<RawDataRecord>(dictionary);
                    break;
                }
            }

            Assert.NotNull(foundRawDataRecord);

            Assert.NotSame(rawDataRecord, foundRawDataRecord);

            Assert.Equal(rawDataRecord.CustomerId, foundRawDataRecord.CustomerId);
            Assert.Equal(rawDataRecord.SourceSystemCompanyId, foundRawDataRecord.SourceSystemCompanyId);
            Assert.Equal(rawDataRecord.UniversalNodeId, foundRawDataRecord.UniversalNodeId);
            Assert.Equal(rawDataRecord.DataSourceType, foundRawDataRecord.DataSourceType);
            Assert.Equal(rawDataRecord.Organization, foundRawDataRecord.Organization);
            Assert.Equal(rawDataRecord.TableName, foundRawDataRecord.TableName);
            Assert.Equal(rawDataRecord.Action, foundRawDataRecord.Action);
            Assert.Equal(rawDataRecord.ErrorMessage, foundRawDataRecord.ErrorMessage);
            Assert.Equal(rawDataRecord.RecordData, foundRawDataRecord.RecordData);
            Assert.Equal(DateTime.Compare(rawDataRecord.EnteredDateTime, foundRawDataRecord.EnteredDateTime), 0);
            Assert.Equal(DateTime.Compare(rawDataRecord.ProcessedDateTime, foundRawDataRecord.ProcessedDateTime), 0);
            Assert.Equal(rawDataRecord.Result, foundRawDataRecord.Result);
            Assert.Equal(rawDataRecord.Processed, foundRawDataRecord.Processed);
            Assert.Equal(rawDataRecord.RecordFound, foundRawDataRecord.RecordFound);


            // Update

            updateRawDataRecord = new RawDataRecord(
                newId,
                "CustomerId Edited",
                "SourceSystemCompanyId Edited",
                "UniversalNodeId Edited",
                "DataSourceType Edited",
                "Organization Edited",
                "Attachment Edited",
                "EDIT",
                "ErrorMessage Edited",
                "RecordData Edited",
                new DateTime(2015, 12, 29, 1, 1, 1),
                new DateTime(2015, 12, 30, 2, 2, 2),
                false,
                false);

            updateCommand = updateRawDataRecord.GenerateUpdateStatement(AnDatabaseTypes.DriverType.PostgresDotNet, rawDataRecord);

            Assert.Equal(string.IsNullOrEmpty(updateCommand), false);

            errorMessage = "";
            updateResult = PostgresConnection.ExecuteCommand(updateCommand, ref errorMessage);

            Assert.Equal(updateResult, true);
            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);


            // Exists

            existsQuery = updateRawDataRecord.GenerateExistsQuery(AnDatabaseTypes.DriverType.PostgresDotNet);

            Assert.Equal(string.IsNullOrEmpty(existsQuery), false);

            errorMessage = "";
            existsResult = PostgresConnection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.NotNull(existsResult);

            existsResultList = AnDataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.Equal(recordExists, true);


            // Select

            selectQuery = updateRawDataRecord.GenerateSelectQuery(AnDatabaseTypes.DriverType.PostgresDotNet);

            Assert.Equal(string.IsNullOrEmpty(selectQuery), false);

            errorMessage = "";
            selectResult = PostgresConnection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.NotNull(selectResult);

            selectResultList = AnDataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundRawDataRecord = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundRawDataRecord = AnModelBase.FromDictionary<RawDataRecord>(dictionary);
                    break;
                }
            }

            Assert.NotNull(foundRawDataRecord);

            Assert.NotSame(updateRawDataRecord, foundRawDataRecord);

            Assert.Equal(updateRawDataRecord.Id, foundRawDataRecord.Id);
            Assert.Equal(updateRawDataRecord.CustomerId, foundRawDataRecord.CustomerId);
            Assert.Equal(updateRawDataRecord.SourceSystemCompanyId, foundRawDataRecord.SourceSystemCompanyId);
            Assert.Equal(updateRawDataRecord.UniversalNodeId, foundRawDataRecord.UniversalNodeId);
            Assert.Equal(updateRawDataRecord.DataSourceType, foundRawDataRecord.DataSourceType);
            Assert.Equal(updateRawDataRecord.Organization, foundRawDataRecord.Organization);
            Assert.Equal(updateRawDataRecord.TableName, foundRawDataRecord.TableName);
            Assert.Equal(updateRawDataRecord.Action, foundRawDataRecord.Action);
            Assert.Equal(updateRawDataRecord.ErrorMessage, foundRawDataRecord.ErrorMessage);
            Assert.Equal(updateRawDataRecord.RecordData, foundRawDataRecord.RecordData);
            Assert.Equal(DateTime.Compare(updateRawDataRecord.EnteredDateTime, foundRawDataRecord.EnteredDateTime), 0);
            Assert.Equal(DateTime.Compare(updateRawDataRecord.ProcessedDateTime, foundRawDataRecord.ProcessedDateTime), 0);
            Assert.Equal(updateRawDataRecord.Result, foundRawDataRecord.Result);
            Assert.Equal(updateRawDataRecord.Processed, foundRawDataRecord.Processed);
            Assert.Equal(updateRawDataRecord.RecordFound, foundRawDataRecord.RecordFound);


            // Delete

            deleteCommand = rawDataRecord.GenerateDeleteStatement(AnDatabaseTypes.DriverType.PostgresDotNet);

            Assert.Equal(string.IsNullOrEmpty(deleteCommand), false);

            errorMessage = "";
            deleteResult = PostgresConnection.ExecuteCommand(deleteCommand, ref errorMessage);

            Assert.Equal(deleteResult, true);
            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);


            // Exists

            existsQuery = rawDataRecord.GenerateExistsQuery(AnDatabaseTypes.DriverType.PostgresDotNet);

            Assert.Equal(string.IsNullOrEmpty(existsQuery), false);

            errorMessage = "";
            existsResult = PostgresConnection.ExecuteQuery(existsQuery, ref errorMessage);

            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.NotNull(existsResult);

            existsResultList = AnDataSetUtility.ToDictionaryList(existsResult.Tables[0]);

            recordExists = existsResultList.Any(dictionary => (dictionary != null) && (dictionary.Count > 0));

            Assert.Equal(recordExists, false);


            // Select

            selectQuery = rawDataRecord.GenerateSelectQuery(AnDatabaseTypes.DriverType.PostgresDotNet);

            Assert.Equal(string.IsNullOrEmpty(selectQuery), false);

            errorMessage = "";
            selectResult = PostgresConnection.ExecuteQuery(selectQuery, ref errorMessage);

            Assert.Equal(string.IsNullOrEmpty(errorMessage), true);
            Assert.NotNull(selectResult);

            selectResultList = AnDataSetUtility.ToDictionaryList(selectResult.Tables[0]);

            foundRawDataRecord = null;

            if (selectResultList.Count > 0)
            {
                foreach (var dictionary in selectResultList.Where(dictionary => (dictionary != null) && (dictionary.Count > 0)))
                {
                    foundRawDataRecord = AnModelBase.FromDictionary<RawDataRecord>(dictionary);
                    break;
                }
            }

            Assert.Equal(foundRawDataRecord != null, false);
        }

        [Fact]
        public void RawDataRecordBinaryCloneTest()
        {
            var rawDataRecord1 = new RawDataRecord(
                1,
                "CustomerId",
                "SourceSystemCompanyId",
                "UniversalNodeId",
                "DataSourceType",
                "Organization",
                "Attachment",
                "ADD",
                "ErrorMessage",
                "RecordData",
                new DateTime(2015, 11, 29, 1, 1, 1),
                AnTypes.MinDateTimeValue,
                false,
                false);

            var rawDataRecord2 = AnCloneUtility.BinaryClone(rawDataRecord1);

            Assert.NotSame(rawDataRecord1, rawDataRecord2);

            Assert.Equal(rawDataRecord1.Id, rawDataRecord2.Id);
            Assert.Equal(rawDataRecord1.CustomerId, rawDataRecord2.CustomerId);
            Assert.Equal(rawDataRecord1.SourceSystemCompanyId, rawDataRecord2.SourceSystemCompanyId);
            Assert.Equal(rawDataRecord1.UniversalNodeId, rawDataRecord2.UniversalNodeId);
            Assert.Equal(rawDataRecord1.DataSourceType, rawDataRecord2.DataSourceType);
            Assert.Equal(rawDataRecord1.Organization, rawDataRecord2.Organization);
            Assert.Equal(rawDataRecord1.TableName, rawDataRecord2.TableName);
            Assert.Equal(rawDataRecord1.Action, rawDataRecord2.Action);
            Assert.Equal(rawDataRecord1.ErrorMessage, rawDataRecord2.ErrorMessage);
            Assert.Equal(rawDataRecord1.RecordData, rawDataRecord2.RecordData);
            Assert.Equal(DateTime.Compare(rawDataRecord1.EnteredDateTime, rawDataRecord2.EnteredDateTime), 0);
            Assert.Equal(DateTime.Compare(rawDataRecord1.ProcessedDateTime, rawDataRecord2.ProcessedDateTime), 0);
            Assert.Equal(rawDataRecord1.Result, rawDataRecord2.Result);
            Assert.Equal(rawDataRecord1.Processed, rawDataRecord2.Processed);
            Assert.Equal(rawDataRecord1.RecordFound, rawDataRecord2.RecordFound);
        }

        [Fact]
        public void RawDataRecordXmlCloneTest()
        {
            var rawDataRecord1 = new RawDataRecord(
                1,
                "CustomerId",
                "SourceSystemCompanyId",
                "UniversalNodeId",
                "DataSourceType",
                "Organization",
                "Attachment",
                "ADD",
                "ErrorMessage",
                "RecordData",
                new DateTime(2015, 11, 29, 1, 1, 1),
                AnTypes.MinDateTimeValue,
                false,
                false);

            var rawDataRecord2 = AnCloneUtility.XmlClone(rawDataRecord1, null);

            Assert.NotSame(rawDataRecord1, rawDataRecord2);

            Assert.Equal(rawDataRecord1.Id, rawDataRecord2.Id);
            Assert.Equal(rawDataRecord1.CustomerId, rawDataRecord2.CustomerId);
            Assert.Equal(rawDataRecord1.SourceSystemCompanyId, rawDataRecord2.SourceSystemCompanyId);
            Assert.Equal(rawDataRecord1.UniversalNodeId, rawDataRecord2.UniversalNodeId);
            Assert.Equal(rawDataRecord1.DataSourceType, rawDataRecord2.DataSourceType);
            Assert.Equal(rawDataRecord1.Organization, rawDataRecord2.Organization);
            Assert.Equal(rawDataRecord1.TableName, rawDataRecord2.TableName);
            Assert.Equal(rawDataRecord1.Action, rawDataRecord2.Action);
            Assert.Equal(rawDataRecord1.ErrorMessage, rawDataRecord2.ErrorMessage);
            Assert.Equal(rawDataRecord1.RecordData, rawDataRecord2.RecordData);
            Assert.Equal(DateTime.Compare(rawDataRecord1.EnteredDateTime, rawDataRecord2.EnteredDateTime), 0);
            Assert.Equal(DateTime.Compare(rawDataRecord1.ProcessedDateTime, rawDataRecord2.ProcessedDateTime), 0);
            Assert.Equal(rawDataRecord1.Result, rawDataRecord2.Result);
            Assert.Equal(rawDataRecord1.Processed, rawDataRecord2.Processed);
            Assert.Equal(rawDataRecord1.RecordFound, rawDataRecord2.RecordFound);
        }

        [Fact]
        public void RawDataRecordJsonTest()
        {
            var rawDataRecord1 = new RawDataRecord(
                1,
                "CustomerId",
                "SourceSystemCompanyId",
                "UniversalNodeId",
                "DataSourceType",
                "Organization",
                "Attachment",
                "ADD",
                "ErrorMessage",
                "RecordData",
                new DateTime(2015, 11, 29, 1, 1, 1),
                AnTypes.MinDateTimeValue,
                false,
                false);

            var jsonText = AnCloneUtility.ToJson(rawDataRecord1);

            Assert.Equal(string.IsNullOrEmpty(jsonText), false);

            var rawDataRecord2 = AnCloneUtility.FromJson<RawDataRecord>(jsonText);

            Assert.NotSame(rawDataRecord1, rawDataRecord2);

            Assert.Equal(rawDataRecord1.Id, rawDataRecord2.Id);
            Assert.Equal(rawDataRecord1.CustomerId, rawDataRecord2.CustomerId);
            Assert.Equal(rawDataRecord1.SourceSystemCompanyId, rawDataRecord2.SourceSystemCompanyId);
            Assert.Equal(rawDataRecord1.UniversalNodeId, rawDataRecord2.UniversalNodeId);
            Assert.Equal(rawDataRecord1.DataSourceType, rawDataRecord2.DataSourceType);
            Assert.Equal(rawDataRecord1.Organization, rawDataRecord2.Organization);
            Assert.Equal(rawDataRecord1.TableName, rawDataRecord2.TableName);
            Assert.Equal(rawDataRecord1.Action, rawDataRecord2.Action);
            Assert.Equal(rawDataRecord1.ErrorMessage, rawDataRecord2.ErrorMessage);
            Assert.Equal(rawDataRecord1.RecordData, rawDataRecord2.RecordData);
            Assert.Equal(DateTime.Compare(rawDataRecord1.EnteredDateTime, rawDataRecord2.EnteredDateTime), 0);
            Assert.Equal(DateTime.Compare(rawDataRecord1.ProcessedDateTime, rawDataRecord2.ProcessedDateTime), 0);
            Assert.Equal(rawDataRecord1.Result, rawDataRecord2.Result);
            Assert.Equal(rawDataRecord1.Processed, rawDataRecord2.Processed);
            Assert.Equal(rawDataRecord1.RecordFound, rawDataRecord2.RecordFound);
        }

        [Fact]
        public void RawDataRecordDictionaryTest()
        {
            var rawDataRecord1 = new RawDataRecord(
                1,
                "CustomerId",
                "SourceSystemCompanyId",
                "UniversalNodeId",
                "DataSourceType",
                "Organization",
                "Attachment",
                "ADD",
                "ErrorMessage",
                "RecordData",
                new DateTime(2015, 11, 29, 1, 1, 1),
                AnTypes.MinDateTimeValue,
                false,
                false);

            var dictionary = AnModelBase.ToDictionary(rawDataRecord1);

            Assert.NotNull(dictionary);

            var rawDataRecord2 = AnModelBase.FromDictionary<RawDataRecord>(dictionary);

            Assert.NotSame(rawDataRecord1, rawDataRecord2);

            Assert.Equal(rawDataRecord1.Id, rawDataRecord2.Id);
            Assert.Equal(rawDataRecord1.CustomerId, rawDataRecord2.CustomerId);
            Assert.Equal(rawDataRecord1.SourceSystemCompanyId, rawDataRecord2.SourceSystemCompanyId);
            Assert.Equal(rawDataRecord1.UniversalNodeId, rawDataRecord2.UniversalNodeId);
            Assert.Equal(rawDataRecord1.DataSourceType, rawDataRecord2.DataSourceType);
            Assert.Equal(rawDataRecord1.Organization, rawDataRecord2.Organization);
            Assert.Equal(rawDataRecord1.TableName, rawDataRecord2.TableName);
            Assert.Equal(rawDataRecord1.Action, rawDataRecord2.Action);
            Assert.Equal(rawDataRecord1.ErrorMessage, rawDataRecord2.ErrorMessage);
            Assert.Equal(rawDataRecord1.RecordData, rawDataRecord2.RecordData);
            Assert.Equal(DateTime.Compare(rawDataRecord1.EnteredDateTime, rawDataRecord2.EnteredDateTime), 0);
            Assert.Equal(DateTime.Compare(rawDataRecord1.ProcessedDateTime, rawDataRecord2.ProcessedDateTime), 0);
            Assert.Equal(rawDataRecord1.Result, rawDataRecord2.Result);
            Assert.Equal(rawDataRecord1.Processed, rawDataRecord2.Processed);
            Assert.Equal(rawDataRecord1.RecordFound, rawDataRecord2.RecordFound);
        }

        [Fact]
        public void RawDataRecordListTestConstructorTest()
        {
            var rawDataRecordList = new RawDataRecordList();

            Assert.NotNull(rawDataRecordList);
            Assert.Equal(rawDataRecordList.List.Count, 0);
        }

        [Fact]
        public void RawDataRecordListGetByIdTest()
        {
            var rawDataRecordList = new RawDataRecordList();

            rawDataRecordList.Add(new RawDataRecord(
                1,
                "CustomerId1",
                "SourceSystemCompanyId1",
                "UniversalNodeId1",
                "DataSourceType1",
                "Organization1",
                "Attachment1",
                "ADD",
                "ErrorMessage1",
                "RecordData1",
                new DateTime(2015, 11, 29, 1, 2, 3),
                AnTypes.MinDateTimeValue,
                false,
                false));

            rawDataRecordList.Add(new RawDataRecord(
                2,
                "CustomerId2",
                "SourceSystemCompanyId2",
                "UniversalNodeId2",
                "DataSourceType2",
                "Organization2",
                "Attachment2",
                "EDIT",
                "ErrorMessage2",
                "RecordData1",
                new DateTime(2015, 12, 29, 2, 3, 4),
                new DateTime(2015, 12, 30, 5, 6, 7),
                true,
                true));

            rawDataRecordList.Add(new RawDataRecord(
                3,
                "CustomerId3",
                "SourceSystemCompanyId3",
                "UniversalNodeId3",
                "DataSourceType3",
                "Organization3",
                "Attachment3",
                "DELETE",
                "ErrorMessage3",
                "RecordData3",
                new DateTime(2015, 12, 30, 2, 3, 4),
                new DateTime(2015, 12, 31, 4, 5, 6),
                false,
                true));

            var rawDataRecord = rawDataRecordList.GetById(0);
            Assert.Equal(rawDataRecord, null);

            rawDataRecord = rawDataRecordList.GetById(-1);
            Assert.Equal(rawDataRecord, null);

            rawDataRecord = rawDataRecordList.GetById(1);
            Assert.Equal(rawDataRecord.Organization, "Organization1");

            rawDataRecord = rawDataRecordList.GetById(2);
            Assert.Equal(rawDataRecord.Organization, "Organization2");

            rawDataRecord = rawDataRecordList.GetById(3);
            Assert.Equal(rawDataRecord.Organization, "Organization3");
        }

        [Fact]
        public void RawDataRecordListExistsTest()
        {
            var rawDataRecordList = new RawDataRecordList();

            rawDataRecordList.Add(new RawDataRecord(
                1,
                "CustomerId1",
                "SourceSystemCompanyId1",
                "UniversalNodeId1",
                "DataSourceType1",
                "Organization1",
                "Attachment1",
                "ADD",
                "ErrorMessage1",
                "RecordData1",
                new DateTime(2015, 11, 29, 1, 2, 3),
                AnTypes.MinDateTimeValue,
                false,
                false));

            rawDataRecordList.Add(new RawDataRecord(
                2,
                "CustomerId2",
                "SourceSystemCompanyId2",
                "UniversalNodeId2",
                "DataSourceType2",
                "Organization2",
                "Attachment2",
                "EDIT",
                "ErrorMessage2",
                "RecordData1",
                new DateTime(2015, 12, 29, 2, 3, 4),
                new DateTime(2015, 12, 30, 5, 6, 7),
                true,
                true));

            rawDataRecordList.Add(new RawDataRecord(
                3,
                "CustomerId3",
                "SourceSystemCompanyId3",
                "UniversalNodeId3",
                "DataSourceType3",
                "Organization3",
                "Attachment3",
                "DELETE",
                "ErrorMessage3",
                "RecordData3",
                new DateTime(2015, 12, 30, 2, 3, 4),
                new DateTime(2015, 12, 31, 4, 5, 6),
                false,
                true));

            Assert.Equal(rawDataRecordList.Exists(-1), false);
            Assert.Equal(rawDataRecordList.Exists(0), false);
            Assert.Equal(rawDataRecordList.Exists(1), true);
            Assert.Equal(rawDataRecordList.Exists(2), true);
            Assert.Equal(rawDataRecordList.Exists(3), true);
        }

        [Fact]
        public void RawDataRecordListAddandRemoveTest()
        {
            var rawDataRecordList = new RawDataRecordList();

            Assert.Equal(rawDataRecordList.List.Count, 0);

            rawDataRecordList.Add(new RawDataRecord(
                1,
                "CustomerId1",
                "SourceSystemCompanyId1",
                "UniversalNodeId1",
                "DataSourceType1",
                "Organization1",
                "Attachment1",
                "ADD",
                "ErrorMessage1",
                "RecordData1",
                new DateTime(2015, 11, 29, 1, 2, 3),
                AnTypes.MinDateTimeValue,
                false,
                false));

            Assert.Equal(rawDataRecordList.List.Count, 1);

            rawDataRecordList.Add(new RawDataRecord(
                2,
                "CustomerId2",
                "SourceSystemCompanyId2",
                "UniversalNodeId2",
                "DataSourceType2",
                "Organization2",
                "Attachment2",
                "EDIT",
                "ErrorMessage2",
                "RecordData1",
                new DateTime(2015, 12, 29, 2, 3, 4),
                new DateTime(2015, 12, 30, 5, 6, 7),
                true,
                true));

            Assert.Equal(rawDataRecordList.List.Count, 2);

            rawDataRecordList.Add(new RawDataRecord(
                3,
                "CustomerId3",
                "SourceSystemCompanyId3",
                "UniversalNodeId3",
                "DataSourceType3",
                "Organization3",
                "Attachment3",
                "DELETE",
                "ErrorMessage3",
                "RecordData3",
                new DateTime(2015, 12, 30, 2, 3, 4),
                new DateTime(2015, 12, 31, 4, 5, 6),
                false,
                true));

            Assert.Equal(rawDataRecordList.List.Count, 3);

            rawDataRecordList.Remove(1);

            Assert.Equal(rawDataRecordList.List.Count, 2);

            rawDataRecordList.Remove(3);

            Assert.Equal(rawDataRecordList.List.Count, 1);

            rawDataRecordList.Remove(2);

            Assert.Equal(rawDataRecordList.List.Count, 0);
        }

        [Fact]
        public void RawDataRecordListGenerateSelectQueryTest()
        {
            Assert.Equal("SELECT Id, CustomerId, SourceSystemCompanyId, UniversalNodeId, DataSourceType, Organization, TableName, Action, ErrorMessage, RecordData, EnteredDateTime, ProcessedDateTime, Result, Processed FROM RawDataRecord WHERE CustomerId = 'CustomerId' AND UniversalNodeId = 'UniversalNodeId' AND DataSourceType = 'DataSourceType' AND EnteredDateTime > '2015-11-26 02:03:04' AND EnteredDateTime <= '2015-11-28 03:04:05' AND Processed = 0 ORDER BY Id", RawDataRecordList.GenerateSelectQuery("CustomerId", "UniversalNodeId", "DataSourceType", new DateTime(2015, 11, 26, 2, 3, 4), new DateTime(2015, 11, 28, 3, 4, 5), AnDatabaseTypes.DriverType.MsSqlServerDotNet));
            Assert.Equal("SELECT Id, CustomerId, SourceSystemCompanyId, UniversalNodeId, DataSourceType, Organization, TableName, Action, ErrorMessage, RecordData, EnteredDateTime, ProcessedDateTime, Result, Processed FROM RawDataRecord WHERE Processed = 0 AND CustomerId = 'CustomerId' AND UniversalNodeId = 'UniversalNodeId' AND DataSourceType = 'DataSourceType' AND EnteredDateTime > '2015-11-26 02:03:04' AND EnteredDateTime <= '2015-11-28 03:04:05' ORDER BY Id", RawDataRecordList.GenerateSelectQuery("CustomerId", "UniversalNodeId", "DataSourceType", new DateTime(2015, 11, 26, 2, 3, 4), new DateTime(2015, 11, 28, 3, 4, 5), false, AnDatabaseTypes.DriverType.MsSqlServerDotNet));
            Assert.Equal("SELECT Id, CustomerId, SourceSystemCompanyId, UniversalNodeId, DataSourceType, Organization, TableName, Action, ErrorMessage, RecordData, EnteredDateTime, ProcessedDateTime, Result, Processed FROM RawDataRecord WHERE Processed = 0 ORDER BY Id", RawDataRecordList.GenerateSelectQueryForNotProcessedRecords(AnDatabaseTypes.DriverType.MsSqlServerDotNet));
            Assert.Equal("SELECT Id, CustomerId, SourceSystemCompanyId, UniversalNodeId, DataSourceType, Organization, TableName, Action, ErrorMessage, RecordData, EnteredDateTime, ProcessedDateTime, Result, Processed FROM RawDataRecord WHERE CustomerId = 'CustomerId' AND SourceSystemCompanyId = 'SourceSystemCompanyId' AND UniversalNodeId = 'UniversalNodeId' AND DataSourceType = 'DataSourceType' AND Organization = 'Organization' AND TableName = 'TableName' AND Action = 'Action'", RawDataRecordList.GenerateSelectQuery("CustomerId", "SourceSystemCompanyId", "UniversalNodeId", "DataSourceType", "Organization", "TableName", "Action", AnDatabaseTypes.DriverType.MsSqlServerDotNet));
        }

        [Fact]
        public void RawDataRecordListJsonTest()
        {
            var rawDataRecordList1 = new RawDataRecordList();

            rawDataRecordList1.Add(new RawDataRecord(
                1,
                "CustomerId1",
                "SourceSystemCompanyId1",
                "UniversalNodeId1",
                "DataSourceType1",
                "Organization1",
                "Attachment1",
                "ADD",
                "ErrorMessage1",
                "RecordData1",
                new DateTime(2015, 11, 29, 1, 2, 3),
                AnTypes.MinDateTimeValue,
                false,
                false));

            rawDataRecordList1.Add(new RawDataRecord(
                2,
                "CustomerId2",
                "SourceSystemCompanyId2",
                "UniversalNodeId2",
                "DataSourceType2",
                "Organization2",
                "Attachment2",
                "EDIT",
                "ErrorMessage2",
                "RecordData1",
                new DateTime(2015, 12, 29, 2, 3, 4),
                new DateTime(2015, 12, 30, 5, 6, 7),
                true,
                true));

            rawDataRecordList1.Add(new RawDataRecord(
                3,
                "CustomerId3",
                "SourceSystemCompanyId3",
                "UniversalNodeId3",
                "DataSourceType3",
                "Organization3",
                "Attachment3",
                "DELETE",
                "ErrorMessage3",
                "RecordData3",
                new DateTime(2015, 12, 30, 2, 3, 4),
                new DateTime(2015, 12, 31, 4, 5, 6),
                false,
                true));

            var jsonText = AnCloneUtility.ToJson(rawDataRecordList1);

            Assert.Equal(string.IsNullOrEmpty(jsonText), false);

            var rawDataRecordList2 = AnCloneUtility.FromJson<RawDataRecordList>(jsonText);

            Assert.NotSame(rawDataRecordList1, rawDataRecordList2);
            Assert.Equal(rawDataRecordList1.List.Count, rawDataRecordList2.List.Count);

            for (var index = 0; index < rawDataRecordList1.List.Count; index++)
            {
                Assert.Equal(rawDataRecordList1.List[index].Id, rawDataRecordList2.List[index].Id);
                Assert.Equal(rawDataRecordList1.List[index].CustomerId, rawDataRecordList2.List[index].CustomerId);
                Assert.Equal(rawDataRecordList1.List[index].SourceSystemCompanyId, rawDataRecordList2.List[index].SourceSystemCompanyId);
                Assert.Equal(rawDataRecordList1.List[index].UniversalNodeId, rawDataRecordList2.List[index].UniversalNodeId);
                Assert.Equal(rawDataRecordList1.List[index].DataSourceType, rawDataRecordList2.List[index].DataSourceType);
                Assert.Equal(rawDataRecordList1.List[index].Organization, rawDataRecordList2.List[index].Organization);
                Assert.Equal(rawDataRecordList1.List[index].TableName, rawDataRecordList2.List[index].TableName);
                Assert.Equal(rawDataRecordList1.List[index].Action, rawDataRecordList2.List[index].Action);
                Assert.Equal(rawDataRecordList1.List[index].ErrorMessage, rawDataRecordList2.List[index].ErrorMessage);
                Assert.Equal(rawDataRecordList1.List[index].RecordData, rawDataRecordList2.List[index].RecordData);
                Assert.Equal(DateTime.Compare(rawDataRecordList1.List[index].EnteredDateTime, rawDataRecordList2.List[index].EnteredDateTime), 0);
                Assert.Equal(DateTime.Compare(rawDataRecordList1.List[index].ProcessedDateTime, rawDataRecordList2.List[index].ProcessedDateTime), 0);
                Assert.Equal(rawDataRecordList1.List[index].Result, rawDataRecordList2.List[index].Result);
                Assert.Equal(rawDataRecordList1.List[index].Processed, rawDataRecordList2.List[index].Processed);
                Assert.Equal(rawDataRecordList1.List[index].RecordFound, rawDataRecordList2.List[index].RecordFound);
            }
        }

        [Fact]
        public void RawDataRecordListBinaryCloneTest()
        {
            var rawDataRecordList1 = new RawDataRecordList();

            rawDataRecordList1.Add(new RawDataRecord(
                1,
                "CustomerId1",
                "SourceSystemCompanyId1",
                "UniversalNodeId1",
                "DataSourceType1",
                "Organization1",
                "Attachment1",
                "ADD",
                "ErrorMessage1",
                "RecordData1",
                new DateTime(2015, 11, 29, 1, 2, 3),
                AnTypes.MinDateTimeValue,
                false,
                false));

            rawDataRecordList1.Add(new RawDataRecord(
                2,
                "CustomerId2",
                "SourceSystemCompanyId2",
                "UniversalNodeId2",
                "DataSourceType2",
                "Organization2",
                "Attachment2",
                "EDIT",
                "ErrorMessage2",
                "RecordData1",
                new DateTime(2015, 12, 29, 2, 3, 4),
                new DateTime(2015, 12, 30, 5, 6, 7),
                true,
                true));

            rawDataRecordList1.Add(new RawDataRecord(
                3,
                "CustomerId3",
                "SourceSystemCompanyId3",
                "UniversalNodeId3",
                "DataSourceType3",
                "Organization3",
                "Attachment3",
                "DELETE",
                "ErrorMessage3",
                "RecordData3",
                new DateTime(2015, 12, 30, 2, 3, 4),
                new DateTime(2015, 12, 31, 4, 5, 6),
                false,
                true));

            var rawDataRecordList2 = AnCloneUtility.BinaryClone(rawDataRecordList1);

            Assert.NotSame(rawDataRecordList1, rawDataRecordList2);
            Assert.Equal(rawDataRecordList1.List.Count, rawDataRecordList2.List.Count);

            for (var index = 0; index < rawDataRecordList1.List.Count; index++)
            {
                Assert.Equal(rawDataRecordList1.List[index].Id, rawDataRecordList2.List[index].Id);
                Assert.Equal(rawDataRecordList1.List[index].CustomerId, rawDataRecordList2.List[index].CustomerId);
                Assert.Equal(rawDataRecordList1.List[index].SourceSystemCompanyId, rawDataRecordList2.List[index].SourceSystemCompanyId);
                Assert.Equal(rawDataRecordList1.List[index].UniversalNodeId, rawDataRecordList2.List[index].UniversalNodeId);
                Assert.Equal(rawDataRecordList1.List[index].DataSourceType, rawDataRecordList2.List[index].DataSourceType);
                Assert.Equal(rawDataRecordList1.List[index].Organization, rawDataRecordList2.List[index].Organization);
                Assert.Equal(rawDataRecordList1.List[index].TableName, rawDataRecordList2.List[index].TableName);
                Assert.Equal(rawDataRecordList1.List[index].Action, rawDataRecordList2.List[index].Action);
                Assert.Equal(rawDataRecordList1.List[index].ErrorMessage, rawDataRecordList2.List[index].ErrorMessage);
                Assert.Equal(rawDataRecordList1.List[index].RecordData, rawDataRecordList2.List[index].RecordData);
                Assert.Equal(DateTime.Compare(rawDataRecordList1.List[index].EnteredDateTime, rawDataRecordList2.List[index].EnteredDateTime), 0);
                Assert.Equal(DateTime.Compare(rawDataRecordList1.List[index].ProcessedDateTime, rawDataRecordList2.List[index].ProcessedDateTime), 0);
                Assert.Equal(rawDataRecordList1.List[index].Result, rawDataRecordList2.List[index].Result);
                Assert.Equal(rawDataRecordList1.List[index].Processed, rawDataRecordList2.List[index].Processed);
                Assert.Equal(rawDataRecordList1.List[index].RecordFound, rawDataRecordList2.List[index].RecordFound);
            }
        }

        [Fact]
        public void RawDataRecordListXmlCloneTest()
        {
            var rawDataRecordList1 = new RawDataRecordList();

            rawDataRecordList1.Add(new RawDataRecord(
                1,
                "CustomerId1",
                "SourceSystemCompanyId1",
                "UniversalNodeId1",
                "DataSourceType1",
                "Organization1",
                "Attachment1",
                "ADD",
                "ErrorMessage1",
                "RecordData1",
                new DateTime(2015, 11, 29, 1, 2, 3),
                AnTypes.MinDateTimeValue,
                false,
                false));

            rawDataRecordList1.Add(new RawDataRecord(
                2,
                "CustomerId2",
                "SourceSystemCompanyId2",
                "UniversalNodeId2",
                "DataSourceType2",
                "Organization2",
                "Attachment2",
                "EDIT",
                "ErrorMessage2",
                "RecordData1",
                new DateTime(2015, 12, 29, 2, 3, 4),
                new DateTime(2015, 12, 30, 5, 6, 7),
                true,
                true));

            rawDataRecordList1.Add(new RawDataRecord(
                3,
                "CustomerId3",
                "SourceSystemCompanyId3",
                "UniversalNodeId3",
                "DataSourceType3",
                "Organization3",
                "Attachment3",
                "DELETE",
                "ErrorMessage3",
                "RecordData3",
                new DateTime(2015, 12, 30, 2, 3, 4),
                new DateTime(2015, 12, 31, 4, 5, 6),
                false,
                true));

            var rawDataRecordList2 = AnCloneUtility.XmlClone(rawDataRecordList1, null);

            Assert.NotSame(rawDataRecordList1, rawDataRecordList2);
            Assert.Equal(rawDataRecordList1.List.Count, rawDataRecordList2.List.Count);

            for (var index = 0; index < rawDataRecordList1.List.Count; index++)
            {
                Assert.Equal(rawDataRecordList1.List[index].Id, rawDataRecordList2.List[index].Id);
                Assert.Equal(rawDataRecordList1.List[index].CustomerId, rawDataRecordList2.List[index].CustomerId);
                Assert.Equal(rawDataRecordList1.List[index].SourceSystemCompanyId, rawDataRecordList2.List[index].SourceSystemCompanyId);
                Assert.Equal(rawDataRecordList1.List[index].UniversalNodeId, rawDataRecordList2.List[index].UniversalNodeId);
                Assert.Equal(rawDataRecordList1.List[index].DataSourceType, rawDataRecordList2.List[index].DataSourceType);
                Assert.Equal(rawDataRecordList1.List[index].Organization, rawDataRecordList2.List[index].Organization);
                Assert.Equal(rawDataRecordList1.List[index].TableName, rawDataRecordList2.List[index].TableName);
                Assert.Equal(rawDataRecordList1.List[index].Action, rawDataRecordList2.List[index].Action);
                Assert.Equal(rawDataRecordList1.List[index].ErrorMessage, rawDataRecordList2.List[index].ErrorMessage);
                Assert.Equal(rawDataRecordList1.List[index].RecordData, rawDataRecordList2.List[index].RecordData);
                Assert.Equal(DateTime.Compare(rawDataRecordList1.List[index].EnteredDateTime, rawDataRecordList2.List[index].EnteredDateTime), 0);
                Assert.Equal(DateTime.Compare(rawDataRecordList1.List[index].ProcessedDateTime, rawDataRecordList2.List[index].ProcessedDateTime), 0);
                Assert.Equal(rawDataRecordList1.List[index].Result, rawDataRecordList2.List[index].Result);
                Assert.Equal(rawDataRecordList1.List[index].Processed, rawDataRecordList2.List[index].Processed);
                Assert.Equal(rawDataRecordList1.List[index].RecordFound, rawDataRecordList2.List[index].RecordFound);
            }
        }

        [Fact]
        public void RawDataRecordListDictionaryTest()
        {
            var rawDataRecordList1 = new RawDataRecordList();

            rawDataRecordList1.Add(new RawDataRecord(
                1,
                "CustomerId1",
                "SourceSystemCompanyId1",
                "UniversalNodeId1",
                "DataSourceType1",
                "Organization1",
                "Attachment1",
                "ADD",
                "ErrorMessage1",
                "RecordData1",
                new DateTime(2015, 11, 29, 1, 2, 3),
                AnTypes.MinDateTimeValue,
                false,
                false));

            rawDataRecordList1.Add(new RawDataRecord(
                2,
                "CustomerId2",
                "SourceSystemCompanyId2",
                "UniversalNodeId2",
                "DataSourceType2",
                "Organization2",
                "Attachment2",
                "EDIT",
                "ErrorMessage2",
                "RecordData1",
                new DateTime(2015, 12, 29, 2, 3, 4),
                new DateTime(2015, 12, 30, 5, 6, 7),
                true,
                true));

            rawDataRecordList1.Add(new RawDataRecord(
                3,
                "CustomerId3",
                "SourceSystemCompanyId3",
                "UniversalNodeId3",
                "DataSourceType3",
                "Organization3",
                "Attachment3",
                "DELETE",
                "ErrorMessage3",
                "RecordData3",
                new DateTime(2015, 12, 30, 2, 3, 4),
                new DateTime(2015, 12, 31, 4, 5, 6),
                false,
                true));

            var dictionaryList = AnModelBaseList.ToDictionaryList(rawDataRecordList1.List);

            Assert.NotNull(dictionaryList);

            var rawDataRecordList2 = new RawDataRecordList {List = AnModelBaseList.FromDictionaryList<RawDataRecord>(dictionaryList)};

            Assert.NotSame(rawDataRecordList1, rawDataRecordList2);
            Assert.Equal(rawDataRecordList1.List.Count, rawDataRecordList2.List.Count);

            for (var index = 0; index < rawDataRecordList1.List.Count; index++)
            {
                Assert.Equal(rawDataRecordList1.List[index].Id, rawDataRecordList2.List[index].Id);
                Assert.Equal(rawDataRecordList1.List[index].CustomerId, rawDataRecordList2.List[index].CustomerId);
                Assert.Equal(rawDataRecordList1.List[index].SourceSystemCompanyId, rawDataRecordList2.List[index].SourceSystemCompanyId);
                Assert.Equal(rawDataRecordList1.List[index].UniversalNodeId, rawDataRecordList2.List[index].UniversalNodeId);
                Assert.Equal(rawDataRecordList1.List[index].DataSourceType, rawDataRecordList2.List[index].DataSourceType);
                Assert.Equal(rawDataRecordList1.List[index].Organization, rawDataRecordList2.List[index].Organization);
                Assert.Equal(rawDataRecordList1.List[index].TableName, rawDataRecordList2.List[index].TableName);
                Assert.Equal(rawDataRecordList1.List[index].Action, rawDataRecordList2.List[index].Action);
                Assert.Equal(rawDataRecordList1.List[index].ErrorMessage, rawDataRecordList2.List[index].ErrorMessage);
                Assert.Equal(rawDataRecordList1.List[index].RecordData, rawDataRecordList2.List[index].RecordData);
                Assert.Equal(DateTime.Compare(rawDataRecordList1.List[index].EnteredDateTime, rawDataRecordList2.List[index].EnteredDateTime), 0);
                Assert.Equal(DateTime.Compare(rawDataRecordList1.List[index].ProcessedDateTime, rawDataRecordList2.List[index].ProcessedDateTime), 0);
                Assert.Equal(rawDataRecordList1.List[index].Result, rawDataRecordList2.List[index].Result);
                Assert.Equal(rawDataRecordList1.List[index].Processed, rawDataRecordList2.List[index].Processed);
                Assert.Equal(rawDataRecordList1.List[index].RecordFound, rawDataRecordList2.List[index].RecordFound);
            }
        }

        #endregion Test Cases
    }
}
