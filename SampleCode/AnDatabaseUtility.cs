
using System;
using System.Linq;
using System.Text;
using ActiveNet.Core.Base;
using ActiveNet.Core.Cache;
using ActiveNet.Core.Log;
using ActiveNet.Types.Base;
using ActiveNet.Types.Database;

namespace ActiveNet.Core.Database
{
    public static class AnDatabaseUtility
    {
        #region Private Members

        private const int DefaultStringSize = 200;
        private const int MaxNonBlobNVarCharSize = 4000;
        private const int MaxNonBlobVarCharSize = 8000;
        private const int MaxNonBlobBinarySize = 8000;

        #endregion Private Members


        #region Public Members

        public static string GenerateCreateInsertTriggerScript(
            string dataChangesTableName,
            string triggerName,
            string hostName, 
            string hostId, 
            string scriptPrefix, 
	        int hostMaxNumberOfCharacters, 
            int hostMaxNumberOfKeys, 
            int hostMaxNumberOfRetries, 
            bool logDataChangesRecords, 
            bool logSyncDbQueueRecords, 
	        bool dataChangesHasIdentityId, 
            bool dontAllowMoreThanOneInstanceOfARecord, 
            bool bidirectionalSyncMode, 
            int hostNumberOfProcesses, 
            int hostNumberOfGuests,
            AnDriverProperties processingDriverProperties, 
	        AnDriverProperties dataChangesDriverProperties, 
            AnDriverProperties syncDbQueueDriverProperties, 
	        AnTableObject tableObject, 
	        bool after)
        {
            var result = new StringBuilder();

            try
            {
	            if ((tableObject != null) && 
	                !string.IsNullOrEmpty(hostId) && 
				    !string.IsNullOrEmpty(hostName) &&
                    !string.IsNullOrEmpty(triggerName) &&
                    !string.IsNullOrEmpty(tableObject.Name))
                {
                    var driverType = AnDatabaseTypes.DriverTypeFromString(processingDriverProperties.GetStringSettingValue(AnDriverProperties.AdapterTypeKey));

                    if (AnDatabaseTypes.IsMsSqlServerDriver(driverType))
                    {
						// Drop trigger if it already exists.

						result.Append("\r\n");
						result.Append("GO\r\n");
						result.Append("\r\n");
						result.Append($"IF EXISTS (SELECT name FROM sysobjects WHERE name = N'{triggerName}' AND type = 'TR')\r\n");
						result.Append($"\tDROP TRIGGER {triggerName}\r\n");
						result.Append("\r\n");
						result.Append("GO\r\n");
						result.Append("\r\n");

						// Create trigger header.

						result.Append($"CREATE TRIGGER {triggerName} ON [{processingDriverProperties.DatabaseName}].[dbo].[{tableObject.Name}]\r\n");
						result.Append(after ? "AFTER INSERT\r\n" : "FOR INSERT\r\n");
						result.Append("AS\r\n");
						result.Append("BEGIN\r\n");
						result.Append("\r\n");

						// Declare instance related variables.

						result.Append("\tDECLARE @InstanceID INT\r\n");
						result.Append("\tDECLARE @NumberOfInstances INT\r\n");

						// Delcare process related variables.

						if (hostNumberOfProcesses > 0)
						{
							result.Append("\tDECLARE @LastDataChangesID INT\r\n");
							result.Append("\tDECLARE @ProcessingID INT\r\n");
							result.Append("\tDECLARE @NumberOfProcesses INT\r\n");
						}

						result.Append("\r\n");

						// Set instance and process related variables.

						result.Append($"\tSET @NumberOfInstances = {hostNumberOfGuests}\r\n");

						if (hostNumberOfProcesses > 0)
						{
							result.Append($"\tSET @NumberOfProcesses = {hostNumberOfProcesses}\r\n");
						}

						// Set instance ID to 1 and loop through all instances.

						result.Append("\tSET @InstanceID = 1\r\n");
						result.Append("\r\n");

						// Set the processing id if necessary.

						if (hostNumberOfProcesses > 0)
						{
							if (dataChangesHasIdentityId)
							{
                                result.Append($"\tSELECT @LastDataChangesID = IDENT_CURRENT('[{dataChangesDriverProperties.DatabaseName}].[dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}]')\r\n");
                                result.Append("\r\n");
								result.Append("\tSET @ProcessingID = (@LastDataChangesID % @NumberOfProcesses) + 1\r\n");
								result.Append("\r\n");
							}

							else
							{
								result.Append($"\tSET @LastDataChangesID = (SELECT VALUE FROM [{syncDbQueueDriverProperties.DatabaseName}].[dbo].[GLOBALVALUES] WHERE HOSTID = '{hostId}' AND NAME = 'NEXTUNIQUEID')\r\n");
								result.Append("\r\n");
								result.Append("\tSET @ProcessingID = (@LastDataChangesID % @NumberOfProcesses) + 1\r\n");
								result.Append("\r\n");
								result.Append($"\tUPDATE [{syncDbQueueDriverProperties.DatabaseName}].[dbo].[GLOBALVALUES] SET VALUE = @LastDataChangesID + 1 WHERE HOSTID = '{hostId}' AND NAME = 'NEXTUNIQUEID'\r\n");
								result.Append("\r\n");
							}
						}

						// Cycle throug instances.

						result.Append("\tWHILE (@InstanceID <= @NumberOfInstances)\r\n");
						result.Append("\tBEGIN\r\n");
						result.Append("\r\n");

						// Construct 'Insert' statement for Data Changes table.

                        result.Append($"\t\tINSERT INTO [{dataChangesDriverProperties.DatabaseName}].[dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}]\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append("\t\t\t\tHOSTID,\r\n");
						result.Append("\t\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\t\tSELECT \r\n");
						result.Append($"\t\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t\t'ADD',\r\n");
						result.Append("\t\t\t\t'',\r\n");
						result.Append("\t\t\t\t0,\r\n");
						result.Append("\t\t\t\tGETDATE(),\r\n");
						result.Append("\t\t\t\t@InstanceID,\r\n");
						result.Append(hostNumberOfProcesses > 0 ? "\t\t\t\t@ProcessingID,\r\n" : "\t\t\t\t1,\r\n");

						var keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
                            result.Append($"\t\t\t\t'{columnObject.ColumnName}',\r\n");

                            if (keyCount < hostMaxNumberOfKeys)
                            {
                                result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tINSERTED.{columnObject.ColumnName},\r\n" : $"\t\t\t\tCONVERT(VARCHAR({hostMaxNumberOfCharacters}), INSERTED.{columnObject.ColumnName}),\r\n");
                            }

                            else
                            {
                                result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tINSERTED.{columnObject.ColumnName}\r\n" : $"\t\t\t\t'CONVERT(VARCHAR({hostMaxNumberOfCharacters}), INSERTED.{columnObject.ColumnName})\r\n");
                            }

                            keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						// Fill in any missing columns.

						for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t\t'',\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? "\t\t\t\t'',\r\n" : "\t\t\t\t''\r\n");
						}

						// Fill in insert clause.

						result.Append("\t\t\tFROM INSERTED\r\n");
						result.Append("\t\t\tWHERE \r\n");

						keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
                            result.Append(((tableObject.ColumnObjectList.PrimaryKeyColumnCount == keyCount) && !dontAllowMoreThanOneInstanceOfARecord) ? $"\t\t\t\tINSERTED.{columnObject.ColumnName} IS NOT NULL\r\n" : $"\t\t\t\tINSERTED.{columnObject.ColumnName} IS NOT NULL AND \r\n");

                            keyCount++;
                                                
                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						// Don't allow the insert if the record alread exists.

						if (dontAllowMoreThanOneInstanceOfARecord)
						{
							result.Append("\t\t\t\t(\r\n");
							result.Append("\t\t\t\t\tSELECT COUNT(*)\r\n");
                            result.Append($"\t\t\t\t\tFROM [{dataChangesDriverProperties.DatabaseName}].[dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}]\r\n");
							result.Append("\t\t\t\t\tWHERE \r\n");
							result.Append($"\t\t\t\t\t\tHOSTID = '{hostId}' AND \r\n");
							result.Append($"\t\t\t\t\t\tDATABASENAME = '{processingDriverProperties.DatabaseName}' AND \r\n");
							result.Append($"\t\t\t\t\t\tTABLENAME = '{tableObject.Name}' AND \r\n");
							result.Append("\t\t\t\t\t\tACTIONCODE = 'ADD' AND \r\n");
							result.Append("\t\t\t\t\t\tERRORCODE = '' AND \r\n");
							result.Append($"\t\t\t\t\t\tRETRYCOUNT < {hostMaxNumberOfRetries} AND \r\n");
							result.Append("\t\t\t\t\t\tINSTANCEID = @InstanceID AND \r\n");
							result.Append(hostNumberOfProcesses > 0 ? "\t\t\t\t\t\tPROCESSINGID = @ProcessingID AND \r\n" : "\t\t\t\t\t\tPROCESSINGID = 1 AND \r\n");

							keyCount = 1;

                            foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                            {
                                result.Append($"\t\t\t\t\t\tKEY{keyCount}NAME = '{columnObject.ColumnName}' AND \r\n");

								if (keyCount < hostMaxNumberOfKeys)
								{
									result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\t\t\tKEY{keyCount}VALUE = INSERTED.{columnObject.ColumnName} AND \r\n" : $"\t\t\t\t\t\tKEY{keyCount}VALUE = CONVERT(VARCHAR({hostMaxNumberOfCharacters}), INSERTED.{columnObject.ColumnName}) AND \r\n");
								}

								else
								{
									result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\t\t\tKEY{keyCount}VALUE = INSERTED.{columnObject.ColumnName}\r\n" : $"\t\t\t\t\t\tKEY{keyCount}VALUE = CONVERT(VARCHAR({hostMaxNumberOfCharacters}), INSERTED.{columnObject.ColumnName})\r\n");
								}

                                keyCount++;

                                if (keyCount > hostMaxNumberOfKeys)
                                {
                                    break;
                                }
                            }

							for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
							{
								result.Append($"\t\t\t\t\t\tKEY{keyIndex}NAME = '' AND \r\n");
                                result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\t\t\tKEY{keyIndex}VALUE = '' AND \r\n" : $"\t\t\t\t\t\tKEY{keyIndex}VALUE = ''\r\n");
							}

							result.Append("\t\t\t\t) = 0\r\n");
						}

						result.Append("\r\n");
						result.Append("\t\tSET @InstanceID = @InstanceID + 1\r\n");
						result.Append("\r\n");
						result.Append("\tEND\r\n");
						result.Append("\r\n");
						result.Append("END\r\n");
						result.Append("\r\n");
						result.Append("GO\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsIbmDb2Driver(driverType))
                    {
					    result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName} ;\r\n");
						result.Append("\r\n");
						result.Append($"CREATE TRIGGER {triggerName}\r\n");
						result.Append($"AFTER INSERT ON {tableObject.Name}\r\n");
						result.Append("REFERENCING NEW AS NEW_ROW\r\n");
						result.Append("FOR EACH ROW MODE DB2Sql\r\n");
						result.Append("\tBEGIN ATOMIC\r\n");
                        result.Append($"\t\tINSERT INTO {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append("\t\t\t\tHOSTID,\r\n");
						result.Append("\t\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\tVALUES (\r\n");
						result.Append($"\t\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t\t'ADD',\r\n");
						result.Append("\t\t\t\t'',\r\n");
						result.Append("\t\t\t\t0,\r\n");
						result.Append("\t\t\t\tCURRENT_TIMESTAMP,\r\n");
						result.Append("\t\t\t\t@InstanceID,\r\n");
						result.Append(hostNumberOfProcesses > 0 ? "\t\t\t\t@ProcessingID,\r\n" : "\t\t\t\t1,\r\n");

						var keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							result.Append($"\t\t\t\t'{columnObject.ColumnName}',\r\n");

							if (keyCount < hostMaxNumberOfKeys)
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tNEW_ROW.{columnObject.ColumnName},\r\n" : $"\t\t\t\tCHAR(NEW_ROW.{columnObject.ColumnName}),\r\n");
							}

							else
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tNEW_ROW.{columnObject.ColumnName}); END\r\n" : $"\t\t\t\tCHAR(NEW_ROW.{columnObject.ColumnName})); END\r\n");
							}

                            keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

                        for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t\t'',\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? "\t\t\t\t'',\r\n" : "\t\t\t\t''); END\r\n");
						}

						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsOracleDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER \"{triggerName}\"\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
						result.Append($"CREATE OR REPLACE TRIGGER \"{triggerName}\"\r\n");
						result.Append($"AFTER INSERT ON {tableObject.Name}\r\n");
						result.Append("REFERENCING NEW AS NEW_ROW\r\n");
						result.Append("FOR EACH ROW\r\n");
						result.Append("\r\n");
						result.Append("\tDECLARE");

						var first = true;
						var keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							if (AnTypes.IsStringDataType(columnObject.DataType))
							{
								if (first)
								{
									first = false;

									result.Append($" {columnObject.ColumnName} VARCHAR2({hostMaxNumberOfCharacters}) := :NEW_ROW.{columnObject.ColumnName};\r\n");
								}

								else
								{
									result.Append($"\t\t{columnObject.ColumnName} VARCHAR2({hostMaxNumberOfCharacters}) := :NEW_ROW.{columnObject.ColumnName};\r\n");
								}
							}

							else
							{
								if (first)
								{
									first = false;

									result.Append($" {columnObject.ColumnName} VARCHAR2({hostMaxNumberOfCharacters}) := TO_CHAR(:NEW_ROW.{columnObject.ColumnName});\r\n");
								}

								else
								{
									result.Append($"\t\t{columnObject.ColumnName} VARCHAR2({hostMaxNumberOfCharacters}) := TO_CHAR(:NEW_ROW.{columnObject.ColumnName});\r\n");
								}
							}

                            keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						result.Append("\r\n");
						result.Append("\tBEGIN\r\n");
						result.Append("\r\n");
                        result.Append($"\t\tINSERT INTO {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append("\t\t\t\tHOSTID,\r\n");
						result.Append("\t\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\tVALUES\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append($"\t\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t\t'ADD',\r\n");
						result.Append("\t\t\t\t'',\r\n");
						result.Append("\t\t\t\t0,\r\n");
						result.Append("\t\t\t\t(SELECT CURRENT_DATE FROM dual),\r\n");
						result.Append("\t\t\t\t@InstanceID,\r\n");
						result.Append(hostNumberOfProcesses > 0 ? "\t\t\t\t@ProcessingID,\r\n" : "\t\t\t\t1,\r\n");

						keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							result.Append($"\t\t\t\t'{columnObject.ColumnName}',\r\n");
                            result.Append(keyCount < hostMaxNumberOfKeys ? $"\t\t\t\t{columnObject.ColumnName},\r\n" : $"\t\t\t\t{columnObject.ColumnName}\r\n");

                            keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t\t' ',\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? "\t\t\t\t' ',\r\n" : "\t\t\t\t' '\r\n");
						}

						result.Append("\t\t\t);\r\n");
						result.Append("\tEND;\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
						result.Append($"ALTER TRIGGER \"{triggerName}\" ENABLE\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsPervasiveDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER \"{triggerName}\"\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
						result.Append($"CREATE TRIGGER \"{triggerName}\"\r\n");
						result.Append($"AFTER INSERT ON \"{tableObject.Name}\"\r\n");
						result.Append("REFERENCING NEW AS NEW_ROW\r\n");
						result.Append("FOR EACH ROW\r\n");
						result.Append("\r\n");
                        result.Append($"\t\tINSERT INTO {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append("\t\t\t\tHOSTID,\r\n");
						result.Append("\t\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\tVALUES\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append($"\t\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t\t'ADD',\r\n");
						result.Append("\t\t\t\t'',\r\n");
						result.Append("\t\t\t\t0,\r\n");
						result.Append("\t\t\t\tGETDATE(),\r\n");
						result.Append("\t\t\t\t1,\r\n");
						result.Append("\t\t\t\t1,\r\n");

						var keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							result.Append($"\t\t\t\t'{columnObject.ColumnName}',\r\n");

							if (keyCount < hostMaxNumberOfKeys)
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tNEW_ROW.{columnObject.ColumnName},\r\n" : $"\t\t\t\tCONVERT(NEW_ROW.{columnObject.ColumnName}, Sql_VARCHAR),\r\n");
							}

							else
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tNEW_ROW.{columnObject.ColumnName}\r\n" : $"\t\t\t\tCONVERT(NEW_ROW.{columnObject.ColumnName}, Sql_VARCHAR)\r\n");
							}

                            keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t\t'',\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? "\t\t\t\t'',\r\n" : "\t\t\t\t''\r\n");
						}

						result.Append("\t\t\t);\r\n");

						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsMySqlDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName}\r\n");
						result.Append("\r\n");
						result.Append("-- Seperator\r\n");
						result.Append("\r\n");
						result.Append("DELIMITER |\r\n");
						result.Append("\r\n");
						result.Append($"CREATE TRIGGER {triggerName}\r\n");
						result.Append($"AFTER INSERT ON {tableObject.Name}\r\n");
						result.Append("FOR EACH ROW\r\n");
						result.Append("BEGIN\r\n");
						result.Append("\r\n");
                        result.Append($"\t\tINSERT INTO {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append("\t\t\t\tHOSTID,\r\n");
						result.Append("\t\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\tVALUES\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append($"\t\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t\t'ADD',\r\n");
						result.Append("\t\t\t\t'',\r\n");
						result.Append("\t\t\t\t0,\r\n");
						result.Append("\t\t\t\tGETDATE(),\r\n");
						result.Append("\t\t\t\t@InstanceID,\r\n");
						result.Append(hostNumberOfProcesses > 0 ? "\t\t\t\t@ProcessingID,\r\n" : "\t\t\t\t1,\r\n");

						var keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							result.Append($"\t\t\t\t'{columnObject.ColumnName}',\r\n");

							if (keyCount < hostMaxNumberOfKeys)
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tNEW.{columnObject.ColumnName},\r\n" : $"\t\t\t\tCONVERT(NEW.{columnObject.ColumnName}, CHAR(200)),\r\n");
							}

							else
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tNEW.{columnObject.ColumnName}\r\n" : $"\t\t\t\tCONVERT(NEW.{columnObject.ColumnName}, CHAR(200)),\r\n");
							}

							keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t\t'',\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? "\t\t\t\t'',\r\n" : "\t\t\t\t''\r\n");
						}

						result.Append("\t\t\t);\r\n");
						result.Append("\r\n");
						result.Append("END;\r\n");
						result.Append("\r\n");
						result.Append("|\r\n");
						result.Append("\r\n");
						result.Append("DELIMITER ;\r\n");
					}

                    else if (AnDatabaseTypes.IsPostgresDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName} ON {tableObject.Name};\r\n");
						result.Append("\r\n");
						result.Append("-- Seperator\r\n");
						result.Append("\r\n");
						result.Append($"DROP FUNCTION add_{triggerName}();\r\n");
						result.Append("\r\n");
						result.Append("-- Seperator\r\n");
						result.Append("\r\n");
						result.Append($"CREATE FUNCTION add_{triggerName}()\r\n");
						result.Append("\tRETURNS trigger AS\r\n");
						result.Append("$BODY$\r\n");
						result.Append("\r\n");
						result.Append("BEGIN\r\n");
						result.Append("\tIF(TG_OP='INSERT') THEN\r\n");
                        result.Append($"\t\tINSERT INTO {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
						result.Append("\t\t(\r\n");
						result.Append("\t\t\tHOSTID,\r\n");
						result.Append("\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\tVALUES\r\n");
						result.Append("\t\t(\r\n");
						result.Append($"\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t'ADD',\r\n");
						result.Append("\t\t\t'',\r\n");
						result.Append("\t\t\t0,\r\n");
						result.Append("\t\t\tcurrent_timestamp,\r\n");
						result.Append("\t\t\t@InstanceID,\r\n");
						result.Append(hostNumberOfProcesses > 0 ? "\t\t\t@ProcessingID,\r\n" : "\t\t\t1,\r\n");

						var keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							result.Append($"\t\t\t'{columnObject.ColumnName}',\r\n");

							if (keyCount < hostMaxNumberOfKeys)
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\tNEW.{columnObject.ColumnName},\r\n" : $"\t\t\tNEW.{columnObject.ColumnName}::VARCHAR,\r\n");
							}

							else
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\tNEW.{columnObject.ColumnName}\r\n" : $"\t\t\tNEW.{columnObject.ColumnName}::VARCHAR\r\n");
							}

							keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

                        for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                        {
                            result.Append("\t\t\t'',\r\n");
                            result.Append(keyIndex != hostMaxNumberOfKeys ? "\t\t\t'',\r\n" : "\t\t\t''\r\n");
                        }

						result.Append("\t\t);\r\n");
						result.Append("\tEND IF;\r\n");
						result.Append("\tRETURN NEW;\r\n");
						result.Append("END;\r\n");
						result.Append("\r\n");
						result.Append("$BODY$\r\n");
						result.Append("\tLANGUAGE 'plpgsql' VOLATILE\r\n");
						result.Append("\tCOST 100;\r\n");
						result.Append($"ALTER FUNCTION add_{triggerName}() OWNER TO postgres;\r\n");
						result.Append("\r\n");
						result.Append("-- Seperator\r\n");
						result.Append("\r\n");
						result.Append($"CREATE TRIGGER {triggerName}\r\n");
						result.Append("\tAFTER INSERT\r\n");
						result.Append($"\tON {tableObject.Name}\r\n");
						result.Append("\tFOR EACH ROW\r\n");
						result.Append($"\tEXECUTE PROCEDURE add_{triggerName}();\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsSqLiteDriver(driverType))
                    {
	                    // Nothing for now.
					}
			    }
	        }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateCreateUpdateTriggerScript(
            string dataChangesTableName, 
            string triggerName,
            string hostName,
            string hostId,
            string scriptPrefix,
	        int hostMaxNumberOfCharacters,
            int hostMaxNumberOfKeys,
            int hostMaxNumberOfRetries,
            bool logDataChangesRecords,
            bool logSyncDbQueueRecords,
	        bool dataChangesHasIdentityId,
            bool dontAllowMoreThanOneInstanceOfARecord,
            bool bidirectionalSyncMode,
            int hostNumberOfProcesses,
            int hostNumberOfGuests,
            AnDriverProperties processingDriverProperties,
	        AnDriverProperties dataChangesDriverProperties,
            AnDriverProperties syncDbQueueDriverProperties,
            AnTableObject tableObject, 
	        bool after)
        {
            var result = new StringBuilder();

            try
            {
			    if ((tableObject != null) && 
                    !string.IsNullOrEmpty(hostId) && 
				    !string.IsNullOrEmpty(hostName) && 
                    !string.IsNullOrEmpty(triggerName) && 
				    !string.IsNullOrEmpty(tableObject.Name))
	            {
                    var driverType = AnDatabaseTypes.DriverTypeFromString(processingDriverProperties.GetStringSettingValue(AnDriverProperties.AdapterTypeKey));

                    if (AnDatabaseTypes.IsMsSqlServerDriver(driverType))
                    {
						// Drop trigger if it already exists.

						result.Append("\r\n");
						result.Append("GO\r\n");
						result.Append("\r\n");
						result.Append($"IF EXISTS (SELECT name FROM sysobjects WHERE name = N'{triggerName}' AND type = 'TR')\r\n");
						result.Append($"\tDROP TRIGGER {triggerName}\r\n");
						result.Append("\r\n");
						result.Append("GO\r\n");
						result.Append("\r\n");

						// Create trigger header.

						result.Append($"CREATE TRIGGER {triggerName} ON [{processingDriverProperties.DatabaseName}].[dbo].[{tableObject.Name}]\r\n");
						result.Append(after ? "AFTER UPDATE\r\n" : "FOR UPDATE\r\n");
						result.Append("AS\r\n");
						result.Append("BEGIN\r\n");
						result.Append("\r\n");

						// Declare instance related variables.

						result.Append("\tDECLARE @InstanceID INT\r\n");
						result.Append("\tDECLARE @NumberOfInstances INT\r\n");

						// Delcare process related variables.

						if (hostNumberOfProcesses > 0)
						{
							result.Append("\tDECLARE @LastDataChangesID INT\r\n");
							result.Append("\tDECLARE @ProcessingID INT\r\n");
							result.Append("\tDECLARE @NumberOfProcesses INT\r\n");
						}

						result.Append("\r\n");

						// Set instance and process related variables.

						result.Append($"\tSET @NumberOfInstances = {hostNumberOfGuests}\r\n");

						if (hostNumberOfProcesses > 0)
						{
							result.Append($"\tSET @NumberOfProcesses = {hostNumberOfProcesses}\r\n");
						}

						// Set instance ID to 1 and loop through all instances.

						result.Append("\tSET @InstanceID = 1\r\n");
						result.Append("\r\n");

						// Set the processing id if necessary.

						if (hostNumberOfProcesses > 0)
						{
							if (dataChangesHasIdentityId)
							{
                                result.Append($"\tSELECT @LastDataChangesID = IDENT_CURRENT('[{dataChangesDriverProperties.DatabaseName}].[dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}]')\r\n");
								result.Append("\r\n");
								result.Append("\tSET @ProcessingID = (@LastDataChangesID % @NumberOfProcesses) + 1\r\n");
								result.Append("\r\n");
							}

							else
							{
								result.Append($"\tSET @LastDataChangesID = (SELECT VALUE FROM [{syncDbQueueDriverProperties.DatabaseName}].[dbo].[GLOBALVALUES] WHERE HOSTID = '{hostId}' AND NAME = 'NEXTUNIQUEID')\r\n");
								result.Append("\r\n");
								result.Append("\tSET @ProcessingID = (@LastDataChangesID % @NumberOfProcesses) + 1\r\n");
								result.Append("\r\n");
								result.Append($"\tUPDATE [{syncDbQueueDriverProperties.DatabaseName}].[dbo].[GLOBALVALUES] SET VALUE = @LastDataChangesID + 1 WHERE HOSTID = '{hostId}' AND NAME = 'NEXTUNIQUEID'\r\n");
								result.Append("\r\n");
							}
						}

						// Cycle throug instances.

						result.Append("\tWHILE (@InstanceID <= @NumberOfInstances)\r\n");
						result.Append("\tBEGIN\r\n");
						result.Append("\r\n");

                        // Construct 'Insert' statement for Data Changes table.

                        result.Append($"\t\tINSERT INTO [{dataChangesDriverProperties.DatabaseName}].[dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}]\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append("\t\t\t\tHOSTID,\r\n");
						result.Append("\t\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\t\tSELECT \r\n");
						result.Append($"\t\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t\t'EDIT',\r\n");
						result.Append("\t\t\t\t'',\r\n");
						result.Append("\t\t\t\t0,\r\n");
						result.Append("\t\t\t\tGETDATE(),\r\n");
						result.Append("\t\t\t\t@InstanceID,\r\n");
						result.Append(hostNumberOfProcesses > 0 ? "\t\t\t\t@ProcessingID,\r\n" : "\t\t\t\t1,\r\n");

						var keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							result.Append($"\t\t\t\t'{columnObject.ColumnName}',\r\n");

							if (keyCount < hostMaxNumberOfKeys)
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ?
								    $"\t\t\t\tINSERTED.{columnObject.ColumnName},\r\n" : $"\t\t\t\tCONVERT(VARCHAR({hostMaxNumberOfCharacters}), INSERTED.{columnObject.ColumnName}),\r\n");
							}

							else
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tINSERTED.{columnObject.ColumnName}\r\n" : $"\t\t\t\t'CONVERT(VARCHAR({hostMaxNumberOfCharacters}), INSERTED.{columnObject.ColumnName})\r\n");
							}

							keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

                        // Fill in any missing columns.

						for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t\t'',\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? "\t\t\t\t'',\r\n" : "\t\t\t\t''\r\n");
						}

						// Fill in insert clause.

						result.Append("\t\t\tFROM INSERTED\r\n");
						result.Append("\t\t\tWHERE \r\n");

						keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							result.Append((tableObject.ColumnObjectList.PrimaryKeyColumnCount == keyCount) && !dontAllowMoreThanOneInstanceOfARecord ? $"\t\t\t\tINSERTED.{columnObject.ColumnName} IS NOT NULL\r\n" : $"\t\t\t\tINSERTED.{columnObject.ColumnName} IS NOT NULL AND \r\n");

							keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						// Don't allow the insert if the record alread exists.

						if (dontAllowMoreThanOneInstanceOfARecord)
						{
							result.Append(bidirectionalSyncMode ? "\t\t\t\t((\r\n" : "\t\t\t\t(\r\n");
							result.Append("\t\t\t\t\tSELECT COUNT(*)\r\n");
                            result.Append($"\t\t\t\t\tFROM [{dataChangesDriverProperties.DatabaseName}].[dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}]\r\n");
							result.Append("\t\t\t\t\tWHERE \r\n");
							result.Append($"\t\t\t\t\t\tHOSTID = '{hostId}' AND \r\n");
							result.Append($"\t\t\t\t\t\tDATABASENAME = '{processingDriverProperties.DatabaseName}' AND \r\n");
							result.Append($"\t\t\t\t\t\tTABLENAME = '{tableObject.Name}' AND \r\n");
							result.Append("\t\t\t\t\t\tACTIONCODE = 'EDIT' AND \r\n");
							result.Append("\t\t\t\t\t\tERRORCODE = '' AND \r\n");
							result.Append($"\t\t\t\t\t\tRETRYCOUNT < {hostMaxNumberOfRetries} AND \r\n");
							result.Append("\t\t\t\t\t\tINSTANCEID = @InstanceID AND \r\n");
							result.Append(hostNumberOfProcesses > 0 ? "\t\t\t\t\t\tPROCESSINGID = @ProcessingID AND \r\n" : "\t\t\t\t\t\tPROCESSINGID = 1 AND \r\n");

							keyCount = 1;

                            foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                            {
								result.Append($"\t\t\t\t\t\tKEY{keyCount}NAME = '{columnObject.ColumnName}' AND \r\n");

								if (keyCount < hostMaxNumberOfKeys)
								{
									result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\t\t\tKEY{keyCount}VALUE = INSERTED.{columnObject.ColumnName} AND \r\n" : $"\t\t\t\t\t\tKEY{keyCount}VALUE = CONVERT(VARCHAR({hostMaxNumberOfCharacters}), INSERTED.{columnObject.ColumnName}) AND \r\n");
								}

								else
								{
									result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\t\t\tKEY{keyCount}VALUE = INSERTED.{columnObject.ColumnName}\r\n" : $"\t\t\t\t\t\tKEY{keyCount}VALUE = CONVERT(VARCHAR({hostMaxNumberOfCharacters}), INSERTED.{columnObject.ColumnName})\r\n");
								}

								keyCount++;

                                if (keyCount > hostMaxNumberOfKeys)
                                {
                                    break;
                                }
                            }

                            for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
							{
								result.Append($"\t\t\t\t\t\tKEY{keyIndex}NAME = '' AND \r\n");
								result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\t\t\tKEY{keyIndex}VALUE = '' AND \r\n" : $"\t\t\t\t\t\tKEY{keyIndex}VALUE = ''\r\n");
							}

							result.Append(bidirectionalSyncMode ? "\t\t\t\t) = 0) AND \r\n" : "\t\t\t\t) = 0\r\n");
						}


						// Don't allow update if the record exists in the BOUNCEQUEUE and bi-directional sync.

						if (bidirectionalSyncMode)
						{
							result.Append(dontAllowMoreThanOneInstanceOfARecord ? "\t\t\t\t((\r\n" : "\t\t\t\t(\r\n");
							result.Append("\t\t\t\t\tSELECT COUNT(*)\r\n");
							result.Append($"\t\t\t\t\tFROM [{dataChangesDriverProperties.DatabaseName}].[dbo].[BOUNCEQUEUE]\r\n");
							result.Append("\t\t\t\t\tWHERE \r\n");
							result.Append($"\t\t\t\t\t\tHOSTID = '{hostId}' AND \r\n");
							result.Append($"\t\t\t\t\t\tDATABASENAME = '{processingDriverProperties.DatabaseName}' AND \r\n");
							result.Append($"\t\t\t\t\t\tTABLENAME = '{tableObject.Name}' AND \r\n");

							keyCount = 1;

                            foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                            {
								result.Append($"\t\t\t\t\t\tKEY{keyCount}NAME = '{columnObject.ColumnName}' AND \r\n");

								if (keyCount < hostMaxNumberOfKeys)
								{
									result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\t\t\tKEY{keyCount}VALUE = INSERTED.{columnObject.ColumnName} AND \r\n" : $"\t\t\t\t\t\tKEY{keyCount}VALUE = CONVERT(VARCHAR({hostMaxNumberOfCharacters}), INSERTED.{columnObject.ColumnName}) AND \r\n");
								}

								else
								{
									result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\t\t\tKEY{keyCount}VALUE = INSERTED.{columnObject.ColumnName}\r\n" : $"\t\t\t\t\t\tKEY{keyCount}VALUE = CONVERT(VARCHAR({hostMaxNumberOfCharacters}), INSERTED.{columnObject.ColumnName})\r\n");
								}

								keyCount++;

                                if (keyCount > hostMaxNumberOfKeys)
                                {
                                    break;
                                }
                            }

                            for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
							{
								result.Append($"\t\t\t\t\t\tKEY{keyIndex}NAME = '' AND \r\n");
								result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\t\t\tKEY{keyIndex}VALUE = '' AND \r\n" : $"\t\t\t\t\t\tKEY{keyIndex}VALUE = ''\r\n");
							}

							result.Append(dontAllowMoreThanOneInstanceOfARecord ? "\t\t\t\t) = 0)\r\n" : "\t\t\t\t) = 0\r\n");
						}

						result.Append("\r\n");
						result.Append("\t\tSET @InstanceID = @InstanceID + 1\r\n");
						result.Append("\r\n");
						result.Append("\tEND\r\n");
						result.Append("\r\n");
						result.Append("END\r\n");
						result.Append("\r\n");
						result.Append("GO\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsIbmDb2Driver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName} ;\r\n");
						result.Append("\r\n");
						result.Append($"CREATE TRIGGER {triggerName}\r\n");
						result.Append($"AFTER UPDATE ON {tableObject.Name}\r\n");
						result.Append("REFERENCING NEW AS NEW_ROW\r\n");
						result.Append("FOR EACH ROW MODE DB2Sql\r\n");
						result.Append("\tBEGIN ATOMIC\r\n");
                        result.Append($"\t\tINSERT INTO {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append("\t\t\t\tHOSTID,\r\n");
						result.Append("\t\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\tVALUES (\r\n");
						result.Append($"\t\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t\t'EDIT',\r\n");
						result.Append("\t\t\t\t'',\r\n");
						result.Append("\t\t\t\t0,\r\n");
						result.Append("\t\t\t\tCURRENT_TIMESTAMP,\r\n");
						result.Append("\t\t\t\t@InstanceID,\r\n");
						result.Append(hostNumberOfProcesses > 0 ? "\t\t\t\t@ProcessingID,\r\n" : "\t\t\t\t1,\r\n");

						var keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							result.Append($"\t\t\t\t'{columnObject.ColumnName}',\r\n");

							if (keyCount < hostMaxNumberOfKeys)
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tNEW_ROW.{columnObject.ColumnName},\r\n" : $"\t\t\t\tCHAR(NEW_ROW.{columnObject.ColumnName}),\r\n");
							}

							else
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tNEW_ROW.{columnObject.ColumnName}); END\r\n" : $"\t\t\t\tCHAR(NEW_ROW.{columnObject.ColumnName})); END\r\n");
							}

							keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

                        for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t\t'',\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? "\t\t\t\t'',\r\n" : "\t\t\t\t''); END\r\n");
						}

						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsOracleDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER \"{triggerName}\"\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
						result.Append($"CREATE OR REPLACE TRIGGER \"{triggerName}\"\r\n");
						result.Append($"AFTER UPDATE ON {tableObject.Name}\r\n");
						result.Append("REFERENCING NEW AS NEW_ROW\r\n");
						result.Append("FOR EACH ROW\r\n");
						result.Append("\r\n");
						result.Append("\tDECLARE");

						var first = true;
						var keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							if (AnTypes.IsStringDataType(columnObject.DataType))
							{
								if (first)
								{
									first = false;

									result.Append($" {columnObject.ColumnName} VARCHAR2({hostMaxNumberOfCharacters}) := :NEW_ROW.{columnObject.ColumnName};\r\n");
								}

								else
								{
									result.Append($"\t\t{columnObject.ColumnName} VARCHAR2({hostMaxNumberOfCharacters}) := :NEW_ROW.{columnObject.ColumnName};\r\n");
								}
							}

							else
							{
								if (first)
								{
									first = false;

									result.Append($" {columnObject.ColumnName} VARCHAR2({hostMaxNumberOfCharacters}) := TO_CHAR(:NEW_ROW.{columnObject.ColumnName});\r\n");
								}

								else
								{
									result.Append($"\t\t{columnObject.ColumnName} VARCHAR2({hostMaxNumberOfCharacters}) := TO_CHAR(:NEW_ROW.{columnObject.ColumnName});\r\n");
								}
							}

							keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						result.Append("\r\n");
						result.Append("\tBEGIN\r\n");
						result.Append("\r\n");
                        result.Append($"\t\tINSERT INTO {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append("\t\t\t\tHOSTID,\r\n");
						result.Append("\t\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\tVALUES\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append($"\t\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t\t'EDIT',\r\n");
						result.Append("\t\t\t\t'',\r\n");
						result.Append("\t\t\t\t0,\r\n");
						result.Append("\t\t\t\t(SELECT CURRENT_DATE FROM dual),\r\n");
						result.Append("\t\t\t\t@InstanceID,\r\n");
						result.Append(hostNumberOfProcesses > 0 ? "\t\t\t\t@ProcessingID,\r\n" : "\t\t\t\t1,\r\n");

						keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							result.Append($"\t\t\t\t'{columnObject.ColumnName}',\r\n");

                            result.Append(keyCount < hostMaxNumberOfKeys ? $"\t\t\t\t{columnObject.ColumnName},\r\n" : $"\t\t\t\t{columnObject.ColumnName}\r\n");

                            keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t\t' ',\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? "\t\t\t\t' ',\r\n" : "\t\t\t\t' '\r\n");
						}

						result.Append("\t\t\t);\r\n");
						result.Append("\tEND;\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
						result.Append($"ALTER TRIGGER \"{triggerName}\" ENABLE\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsPervasiveDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER \"{triggerName}\"\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
						result.Append($"CREATE TRIGGER \"{triggerName}\"\r\n");
						result.Append($"AFTER UPDATE ON \"{tableObject.Name}\"\r\n");
						result.Append("REFERENCING NEW AS NEW_ROW\r\n");
						result.Append("FOR EACH ROW\r\n");
						result.Append("\r\n");
                        result.Append($"\t\tINSERT INTO {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append("\t\t\t\tHOSTID,\r\n");
						result.Append("\t\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\tVALUES\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append($"\t\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t\t'EDIT',\r\n");
						result.Append("\t\t\t\t'',\r\n");
						result.Append("\t\t\t\t0,\r\n");
						result.Append("\t\t\t\tGETDATE(),\r\n");
						result.Append("\t\t\t\t1,\r\n");
						result.Append("\t\t\t\t1,\r\n");

						var keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							result.Append($"\t\t\t\t'{columnObject.ColumnName}',\r\n");

							if (keyCount < hostMaxNumberOfKeys)
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tNEW_ROW.{columnObject.ColumnName},\r\n" : $"\t\t\t\tCONVERT(NEW_ROW.{columnObject.ColumnName}, Sql_VARCHAR),\r\n");
							}

							else
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tNEW_ROW.{columnObject.ColumnName}\r\n" : $"\t\t\t\tCONVERT(NEW_ROW.{columnObject.ColumnName}, Sql_VARCHAR)\r\n");
							}

                            keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

                        for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t\t'',\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? "\t\t\t\t'',\r\n" : "\t\t\t\t''\r\n");
						}

						result.Append("\t\t\t);\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsMySqlDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName}\r\n");
						result.Append("\r\n");
						result.Append("-- Seperator\r\n");
						result.Append("\r\n");
						result.Append("DELIMITER |\r\n");
						result.Append("\r\n");
						result.Append($"CREATE TRIGGER {triggerName}\r\n");
						result.Append($"AFTER UPDATE ON {tableObject.Name}\r\n");
						result.Append("FOR EACH ROW\r\n");
						result.Append("BEGIN\r\n");
						result.Append("\r\n");
                        result.Append($"\t\tINSERT INTO {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append("\t\t\t\tHOSTID,\r\n");
						result.Append("\t\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\tVALUES\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append($"\t\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t\t'EDIT',\r\n");
						result.Append("\t\t\t\t'',\r\n");
						result.Append("\t\t\t\t0,\r\n");
						result.Append("\t\t\t\tGETDATE(),\r\n");
						result.Append("\t\t\t\t@InstanceID,\r\n");
						result.Append(hostNumberOfProcesses > 0 ? "\t\t\t\t@ProcessingID,\r\n" : "\t\t\t\t1,\r\n");

						var keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							result.Append($"\t\t\t\t'{columnObject.ColumnName}',\r\n");

							if (keyCount < hostMaxNumberOfKeys)
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tNEW.{columnObject.ColumnName},\r\n" : $"\t\t\t\tCONVERT(NEW.{columnObject.ColumnName}, CHAR(200)),\r\n");
							}

							else
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tNEW.{columnObject.ColumnName}\r\n" : $"\t\t\t\tCONVERT(NEW.{columnObject.ColumnName}, CHAR(200)),\r\n");
							}

                            keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

                        for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t\t'',\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? "\t\t\t\t'',\r\n" : "\t\t\t\t''\r\n");
						}

						result.Append("\t\t\t);\r\n");
						result.Append("\r\n");
						result.Append("END;\r\n");
						result.Append("\r\n");
						result.Append("|\r\n");
						result.Append("\r\n");
						result.Append("DELIMITER ;\r\n");
					}

                    else if (AnDatabaseTypes.IsPostgresDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName} ON {tableObject.Name};\r\n");
						result.Append("\r\n");
						result.Append("-- Seperator\r\n");
						result.Append("\r\n");
						result.Append($"DROP FUNCTION edit_{triggerName}();\r\n");
						result.Append("\r\n");
						result.Append("-- Seperator\r\n");
						result.Append("\r\n");
						result.Append($"CREATE FUNCTION edit_{triggerName}()\r\n");
						result.Append("\tRETURNS trigger AS\r\n");
						result.Append("$BODY$\r\n");
						result.Append("\r\n");
						result.Append("BEGIN\r\n");
						result.Append("\tIF(TG_OP='UPDATE') THEN\r\n");
                        result.Append($"\t\tINSERT INTO {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
						result.Append("\t\t(\r\n");
						result.Append("\t\t\tHOSTID,\r\n");
						result.Append("\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\tVALUES\r\n");
						result.Append("\t\t(\r\n");
						result.Append($"\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t'EDIT',\r\n");
						result.Append("\t\t\t'',\r\n");
						result.Append("\t\t\t0,\r\n");
						result.Append("\t\t\tcurrent_timestamp,\r\n");
						result.Append("\t\t\t@InstanceID,\r\n");
						result.Append(hostNumberOfProcesses > 0 ? "\t\t\t@ProcessingID,\r\n" : "\t\t\t1,\r\n");

						var keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							result.Append($"\t\t\t'{columnObject.ColumnName}',\r\n");

							if (keyCount < hostMaxNumberOfKeys)
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\tNEW.{columnObject.ColumnName},\r\n" : $"\t\t\tNEW.{columnObject.ColumnName}::VARCHAR,\r\n");
							}

							else
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\tNEW.{columnObject.ColumnName}\r\n" : $"\t\t\tNEW.{columnObject.ColumnName}::VARCHAR\r\n");
							}

                            keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t'',\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? "\t\t\t'',\r\n" : "\t\t\t''\r\n");
						}

						result.Append("\t\t);\r\n");
						result.Append("\tEND IF;\r\n");
						result.Append("\tRETURN NEW;\r\n");
						result.Append("END;\r\n");
						result.Append("\r\n");
						result.Append("$BODY$\r\n");
						result.Append("\tLANGUAGE 'plpgsql' VOLATILE\r\n");
						result.Append("\tCOST 100;\r\n");
						result.Append($"ALTER FUNCTION edit_{triggerName}() OWNER TO postgres;\r\n");
						result.Append("\r\n");
						result.Append("-- Seperator\r\n");
						result.Append("\r\n");
						result.Append($"CREATE TRIGGER {triggerName}\r\n");
						result.Append("\tAFTER UPDATE\r\n");
						result.Append($"\tON {tableObject.Name}\r\n");
						result.Append("\tFOR EACH ROW\r\n");
						result.Append($"\tEXECUTE PROCEDURE edit_{triggerName}();\r\n");
						result.Append("\r\n");
					}

		            else if (AnDatabaseTypes.IsSqLiteDriver(driverType))
		            {
			            // Nothing for now.
		            }
			    }
	        }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateCreateDeleteTriggerScript(
            string dataChangesTableName,
            string triggerName,
            string hostName,
            string hostId,
            string scriptPrefix,
	        int hostMaxNumberOfCharacters,
            int hostMaxNumberOfKeys,
            int hostMaxNumberOfRetries,
            bool logDataChangesRecords,
            bool logSyncDbQueueRecords,
	        bool dataChangesHasIdentityId,
            bool dontAllowMoreThanOneInstanceOfARecord,
            bool bidirectionalSyncMode,
            int hostNumberOfProcesses,
            int hostNumberOfGuests,
            AnDriverProperties processingDriverProperties,
	        AnDriverProperties dataChangesDriverProperties,
            AnDriverProperties syncDbQueueDriverProperties,
            AnTableObject tableObject, 
	        AnTableObjectList deleteTableObjectList, 
	        bool after)
        {
            var result = new StringBuilder();

            try
            {
	            if ((tableObject != null) && 
                    !string.IsNullOrEmpty(hostId) && 
				    !string.IsNullOrEmpty(hostName) && 
				    !string.IsNullOrEmpty(triggerName) && 
				    !string.IsNullOrEmpty(tableObject.Name))
                {
                    var driverType = AnDatabaseTypes.DriverTypeFromString(processingDriverProperties.GetStringSettingValue(AnDriverProperties.AdapterTypeKey));

                    if (AnDatabaseTypes.IsMsSqlServerDriver(driverType))
                    {
						// Drop trigger if it already exists.

						result.Append("\r\n");
						result.Append("GO\r\n");
						result.Append("\r\n");
						result.Append($"IF EXISTS (SELECT name FROM sysobjects WHERE name = N'{triggerName}' AND type = 'TR')\r\n");
						result.Append($"\tDROP TRIGGER {triggerName}\r\n");
						result.Append("\r\n");
						result.Append("GO\r\n");
						result.Append("\r\n");

						// Create trigger header.

						result.Append($"CREATE TRIGGER {triggerName} ON [{processingDriverProperties.DatabaseName}].[dbo].[{tableObject.Name}]\r\n");
						result.Append(after ? "AFTER DELETE\r\n" : "FOR DELETE\r\n");
						result.Append("AS\r\n");
						result.Append("BEGIN\r\n");
						result.Append("\r\n");

						// Declare instance related variables.

						result.Append("\tDECLARE @InstanceID INT\r\n");
						result.Append("\tDECLARE @NumberOfInstances INT\r\n");

						// Delcare process related variables.

						if (hostNumberOfProcesses > 0)
						{
							result.Append("\tDECLARE @LastDataChangesID INT\r\n");
							result.Append("\tDECLARE @ProcessingID INT\r\n");
							result.Append("\tDECLARE @NumberOfProcesses INT\r\n");
						}

						result.Append("\r\n");

						// Set instance and process related variables.

						result.Append($"\tSET @NumberOfInstances = {hostNumberOfGuests}\r\n");

						if (hostNumberOfProcesses > 0)
						{
							result.Append($"\tSET @NumberOfProcesses = {hostNumberOfProcesses}\r\n");
						}

						// Set instance ID to 1 and loop through all instances.

						result.Append("\tSET @InstanceID = 1\r\n");
						result.Append("\r\n");

						// Set the processing id if necessary.

						if (hostNumberOfProcesses > 0)
						{
							if (dataChangesHasIdentityId)
							{
                                result.Append($"\tSELECT @LastDataChangesID = IDENT_CURRENT('[{dataChangesDriverProperties.DatabaseName}].[dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}]')\r\n");
								result.Append("\r\n");
								result.Append("\tSET @ProcessingID = (@LastDataChangesID % @NumberOfProcesses) + 1\r\n");
								result.Append("\r\n");
							}

							else
							{
								result.Append($"\tSET @LastDataChangesID = (SELECT VALUE FROM [{syncDbQueueDriverProperties.DatabaseName}].[dbo].[GLOBALVALUES] WHERE HOSTID = '{hostId}' AND NAME = 'NEXTUNIQUEID')\r\n");
								result.Append("\r\n");
								result.Append("\tSET @ProcessingID = (@LastDataChangesID % @NumberOfProcesses) + 1\r\n");
								result.Append("\r\n");
								result.Append($"\tUPDATE [{syncDbQueueDriverProperties.DatabaseName}].[dbo].[GLOBALVALUES] SET VALUE = @LastDataChangesID + 1 WHERE HOSTID = '{hostId}' AND NAME = 'NEXTUNIQUEID'\r\n");
								result.Append("\r\n");
							}
						}


						// Cycle throug instances.

						result.Append("\tWHILE (@InstanceID <= @NumberOfInstances)\r\n");
						result.Append("\tBEGIN\r\n");
						result.Append("\r\n");

                        // Construct 'Insert' statement for Data Changes table.

                        result.Append($"\t\tINSERT INTO [{dataChangesDriverProperties.DatabaseName}].[dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}]\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append("\t\t\t\tHOSTID,\r\n");
						result.Append("\t\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\t\tSELECT \r\n");
						result.Append($"\t\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t\t'DELETE',\r\n");
						result.Append("\t\t\t\t'',\r\n");
						result.Append("\t\t\t\t0,\r\n");
						result.Append("\t\t\t\tGETDATE(),\r\n");
						result.Append("\t\t\t\t@InstanceID,\r\n");
						result.Append(hostNumberOfProcesses > 0 ? "\t\t\t\t@ProcessingID,\r\n" : "\t\t\t\t1,\r\n");

						var keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							result.Append($"\t\t\t\t'{columnObject.ColumnName}',\r\n");

							if (keyCount < hostMaxNumberOfKeys)
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tDELETED.{columnObject.ColumnName},\r\n" : $"\t\t\t\tCONVERT(VARCHAR({hostMaxNumberOfCharacters}), DELETED.{columnObject.ColumnName}),\r\n");
							}

							else
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tDELETED.{columnObject.ColumnName}\r\n" : $"\t\t\t\t'CONVERT(VARCHAR({hostMaxNumberOfCharacters}), DELETED.{columnObject.ColumnName})\r\n");
							}

                            keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

                        // Fill in any missing columns.

						for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t\t'',\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? "\t\t\t\t'',\r\n" : "\t\t\t\t''\r\n");
						}

						// Fill in insert clause.

						result.Append("\t\t\tFROM DELETED\r\n");
						result.Append("\t\t\tWHERE \r\n");

						keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							if ((tableObject.ColumnObjectList.PrimaryKeyColumnCount == keyCount) && !dontAllowMoreThanOneInstanceOfARecord)
                            {
								result.Append($"\t\t\t\tDELETED.{columnObject.ColumnName} IS NOT NULL\r\n");
                            }
									        
                            else
                            {
								result.Append($"\t\t\t\tDELETED.{columnObject.ColumnName} IS NOT NULL AND \r\n");
                            }

                            keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						// Don't allow the insert if the record alread exists.

						if (dontAllowMoreThanOneInstanceOfARecord)
						{
							result.Append("\t\t\t\t(\r\n");
							result.Append("\t\t\t\t\tSELECT COUNT(*)\r\n");
                            result.Append($"\t\t\t\t\tFROM [{dataChangesDriverProperties.DatabaseName}].[dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}]\r\n");
							result.Append("\t\t\t\t\tWHERE \r\n");
							result.Append($"\t\t\t\t\t\tHOSTID = '{hostId}' AND \r\n");
							result.Append($"\t\t\t\t\t\tDATABASENAME = '{processingDriverProperties.DatabaseName}' AND \r\n");
							result.Append($"\t\t\t\t\t\tTABLENAME = '{tableObject.Name}' AND \r\n");
							result.Append("\t\t\t\t\t\tACTIONCODE = 'DELETE' AND \r\n");
							result.Append("\t\t\t\t\t\tERRORCODE = '' AND \r\n");
							result.Append($"\t\t\t\t\t\tRETRYCOUNT < {hostMaxNumberOfRetries} AND \r\n");
							result.Append("\t\t\t\t\t\tINSTANCEID = @InstanceID AND \r\n");
							result.Append(hostNumberOfProcesses > 0 ? "\t\t\t\t\t\tPROCESSINGID = @ProcessingID AND \r\n" : "\t\t\t\t\t\tPROCESSINGID = 1 AND \r\n");

							keyCount = 1;

                            foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                            {
								result.Append($"\t\t\t\t\t\tKEY{keyCount}NAME = '{columnObject.ColumnName}' AND \r\n");

								if (keyCount < hostMaxNumberOfKeys)
								{
									result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\t\t\tKEY{keyCount}VALUE = DELETED.{columnObject.ColumnName} AND \r\n" : $"\t\t\t\t\t\tKEY{keyCount}VALUE = CONVERT(VARCHAR({hostMaxNumberOfCharacters}), DELETED.{columnObject.ColumnName}) AND \r\n");
								}

								else
								{
									result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\t\t\tKEY{keyCount}VALUE = DELETED.{columnObject.ColumnName}\r\n" : $"\t\t\t\t\t\tKEY{keyCount}VALUE = CONVERT(VARCHAR({hostMaxNumberOfCharacters}), DELETED.{columnObject.ColumnName})\r\n");
								}

                                keyCount++;

                                if (keyCount > hostMaxNumberOfKeys)
                                {
                                    break;
                                }
                            }

                            for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
							{
								result.Append($"\t\t\t\t\t\tKEY{keyIndex}NAME = '' AND \r\n");
								result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\t\t\tKEY{keyIndex}VALUE = '' AND \r\n" : $"\t\t\t\t\t\tKEY{keyIndex}VALUE = ''\r\n");
							}

							result.Append("\t\t\t\t) = 0\r\n");
						}

						result.Append("\r\n");
						result.Append("\t\tSET @InstanceID = @InstanceID + 1\r\n");
						result.Append("\r\n");
						result.Append("\tEND\r\n");
						result.Append("\r\n");
						result.Append("END\r\n");
						result.Append("\r\n");
						result.Append("GO\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsIbmDb2Driver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName} ;\r\n");
						result.Append("\r\n");
						result.Append($"CREATE TRIGGER {triggerName}\r\n");
						result.Append($"AFTER DELETE ON {tableObject.Name}\r\n");
						result.Append("REFERENCING OLD AS OLD_ROW\r\n");
						result.Append("FOR EACH ROW MODE DB2Sql\r\n");
						result.Append("\tBEGIN ATOMIC\r\n");
                        result.Append($"\t\tINSERT INTO {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append("\t\t\t\tHOSTID,\r\n");
						result.Append("\t\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\tVALUES (\r\n");
						result.Append($"\t\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t\t'DELETE',\r\n");
						result.Append("\t\t\t\t'',\r\n");
						result.Append("\t\t\t\t0,\r\n");
						result.Append("\t\t\t\tCURRENT_TIMESTAMP,\r\n");
						result.Append("\t\t\t\t@InstanceID,\r\n");
						result.Append(hostNumberOfProcesses > 0 ? "\t\t\t\t@ProcessingID,\r\n" : "\t\t\t\t1,\r\n");

						var numberOfFields = 0;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.IsSelected))
                        {
							result.Append($"\t\t\t\t'{columnObject.ColumnName}',\r\n");

							if (numberOfFields < (hostMaxNumberOfKeys - 1))
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tOLD_ROW.{columnObject.ColumnName},\r\n" : $"\t\t\t\tCHAR(OLD_ROW.{columnObject.ColumnName}),\r\n");
							}

							else
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tOLD_ROW.{columnObject.ColumnName}); END\r\n" : $"\t\t\t\tCHAR(OLD_ROW.{columnObject.ColumnName})); END\r\n");
							}

                            numberOfFields++;

                            if (numberOfFields >= hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						for (var keyIndex = numberOfFields; keyIndex < hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t\t'',\r\n");
							result.Append(keyIndex != (hostMaxNumberOfKeys - 1) ? "\t\t\t\t'',\r\n" : "\t\t\t\t''); END\r\n");
						}

						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsOracleDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER \"{triggerName}\"\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
						result.Append($"CREATE OR REPLACE TRIGGER \"{triggerName}\"\r\n");
						result.Append($"AFTER DELETE ON {tableObject.Name}\r\n");
						result.Append("REFERENCING OLD AS OLD_ROW\r\n");
						result.Append("FOR EACH ROW\r\n");
						result.Append("\r\n");
						result.Append("\tDECLARE");

						// If the delete columns have been selected then use them for the delete script.
						// Use the primary key if nothing is specified.

						var numberOfFields = 0;
						var first = true;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.IsSelected))
                        {
							if (AnTypes.IsStringDataType(columnObject.DataType))
							{
								if (first)
								{
									first = false;

									result.Append($" {columnObject.ColumnName} VARCHAR2({hostMaxNumberOfCharacters}) := :OLD_ROW.{columnObject.ColumnName};\r\n");
								}

								else
								{
									result.Append($"\t\t{columnObject.ColumnName} VARCHAR2({hostMaxNumberOfCharacters}) := :OLD_ROW.{columnObject.ColumnName};\r\n");
								}
							}

							else
							{
								if (first)
								{
									first = false;

									result.Append($" {columnObject.ColumnName} VARCHAR2({hostMaxNumberOfCharacters}) := TO_CHAR(:OLD_ROW.{columnObject.ColumnName});\r\n");
								}

								else
								{
									result.Append($"\t\t{columnObject.ColumnName} VARCHAR2({hostMaxNumberOfCharacters}) := TO_CHAR(:OLD_ROW.{columnObject.ColumnName});\r\n");
								}
							}

                            numberOfFields++;

                            if (numberOfFields >= hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						result.Append("\r\n");
						result.Append("\tBEGIN\r\n");
						result.Append("\r\n");
                        result.Append($"\t\tINSERT INTO {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append("\t\t\t\tHOSTID,\r\n");
						result.Append("\t\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\tVALUES\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append($"\t\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t\t'DELETE',\r\n");
						result.Append("\t\t\t\t'',\r\n");
						result.Append("\t\t\t\t0,\r\n");
						result.Append("\t\t\t\t(SELECT CURRENT_DATE FROM dual),\r\n");
						result.Append("\t\t\t\t@InstanceID,\r\n");
						result.Append(hostNumberOfProcesses > 0 ? "\t\t\t\t@ProcessingID,\r\n" : "\t\t\t\t1,\r\n");

						numberOfFields = 0;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.IsSelected))
                        {
							result.Append($"\t\t\t\t'{columnObject.ColumnName}',\r\n");

                            result.Append(numberOfFields < (hostMaxNumberOfKeys - 1) ? $"\t\t\t\t{columnObject.ColumnName},\r\n" : $"\t\t\t\t{columnObject.ColumnName}\r\n");

                            numberOfFields++;

                            if (numberOfFields >= hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						for (var keyIndex = numberOfFields; keyIndex < hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t\t' ',\r\n");
							result.Append(keyIndex != (hostMaxNumberOfKeys - 1) ? "\t\t\t\t' ',\r\n" : "\t\t\t\t' '\r\n");
						}

						result.Append("\t\t\t);\r\n");
						result.Append("\tEND;\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
						result.Append($"ALTER TRIGGER \"{triggerName}\" ENABLE\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsPervasiveDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER \"{triggerName}\"\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
						result.Append($"CREATE TRIGGER \"{triggerName}\"\r\n");
						result.Append($"AFTER DELETE ON \"{tableObject.Name}\"\r\n");
						result.Append("REFERENCING OLD AS OLD_ROW\r\n");
						result.Append("FOR EACH ROW\r\n");
						result.Append("\r\n");
                        result.Append($"\t\tINSERT INTO {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append("\t\t\t\tHOSTID,\r\n");
						result.Append("\t\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\tVALUES\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append($"\t\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t\t'DELETE',\r\n");
						result.Append("\t\t\t\t'',\r\n");
						result.Append("\t\t\t\t0,\r\n");
						result.Append("\t\t\t\tGETDATE(),\r\n");
						result.Append("\t\t\t\t1,\r\n");
						result.Append("\t\t\t\t1,\r\n");

						var numberOfFields = 0;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.IsSelected))
                        {
							if (numberOfFields < (hostMaxNumberOfKeys - 1))
							{
								result.Append($"\t\t\t\t'{columnObject.ColumnName}',\r\n");

								if (numberOfFields < (hostMaxNumberOfKeys - 1))
								{
									result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tOLD_ROW.{columnObject.ColumnName},\r\n" : $"\t\t\t\tCONVERT(OLD_ROW.{columnObject.ColumnName}, Sql_VARCHAR),\r\n");
								}

								else
								{
									result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tOLD_ROW.{columnObject.ColumnName}\r\n" : $"\t\t\t\tCONVERT(OLD_ROW.{columnObject.ColumnName}, Sql_VARCHAR)\r\n");
								}
							}

                            numberOfFields++;

                            if (numberOfFields >= hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						for (var keyIndex = numberOfFields; keyIndex < hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t\t'',\r\n");
							result.Append(keyIndex != (hostMaxNumberOfKeys - 1) ? "\t\t\t\t'',\r\n" : "\t\t\t\t''\r\n");
						}

						result.Append("\t\t\t);\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsMySqlDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName}\r\n");
						result.Append("\r\n");
						result.Append("-- Seperator\r\n");
						result.Append("\r\n");
						result.Append("DELIMITER |\r\n");
						result.Append("\r\n");
						result.Append($"CREATE TRIGGER {triggerName}\r\n");
						result.Append($"AFTER DELETE ON {tableObject.Name}\r\n");
						result.Append("FOR EACH ROW\r\n");
						result.Append("BEGIN\r\n");
						result.Append("\r\n");
                        result.Append($"\t\tINSERT INTO {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append("\t\t\t\tHOSTID,\r\n");
						result.Append("\t\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\tVALUES\r\n");
						result.Append("\t\t\t(\r\n");
						result.Append($"\t\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t\t'DELETE',\r\n");
						result.Append("\t\t\t\t'',\r\n");
						result.Append("\t\t\t\t0,\r\n");
						result.Append("\t\t\t\tGETDATE(),\r\n");
						result.Append("\t\t\t\t@InstanceID,\r\n");
						result.Append(hostNumberOfProcesses > 0 ? "\t\t\t\t@ProcessingID,\r\n" : "\t\t\t\t1,\r\n");

						var numberOfFields = 0;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.IsSelected))
                        {
							if (numberOfFields < (hostMaxNumberOfKeys - 1))
							{
								result.Append($"\t\t\t\t'{columnObject.ColumnName}',\r\n");

								if (numberOfFields < (hostMaxNumberOfKeys - 1))
								{
									result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tOLD.{columnObject.ColumnName},\r\n" : $"\t\t\t\tCONVERT(OLD.{columnObject.ColumnName}, CHAR(200)),\r\n");
								}

								else
								{
									result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\t\tOLD.{columnObject.ColumnName}\r\n" : $"\t\t\t\tCONVERT(OLD.{columnObject.ColumnName}, CHAR(200)),\r\n");
								}
							}

                            numberOfFields++;

                            if (numberOfFields >= hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

						for (var keyIndex = numberOfFields; keyIndex < hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t\t'',\r\n");
							result.Append(keyIndex != (hostMaxNumberOfKeys - 1) ? "\t\t\t\t'',\r\n" : "\t\t\t\t''\r\n");
						}

						result.Append("\t\t\t);\r\n");
						result.Append("\r\n");
						result.Append("END;\r\n");
						result.Append("\r\n");
						result.Append("|\r\n");
						result.Append("\r\n");
						result.Append("DELIMITER ;\r\n");
					}

                    else if (AnDatabaseTypes.IsPostgresDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName} ON {tableObject.Name};\r\n");
						result.Append("\r\n");
						result.Append("-- Seperator\r\n");
						result.Append("\r\n");
						result.Append($"DROP FUNCTION delete_{triggerName}();\r\n");
						result.Append("\r\n");
						result.Append("-- Seperator\r\n");
						result.Append("\r\n");
						result.Append($"CREATE FUNCTION delete_{triggerName}()\r\n");
						result.Append("\tRETURNS trigger AS\r\n");
						result.Append("$BODY$\r\n");
						result.Append("\r\n");
						result.Append("BEGIN\r\n");
						result.Append("\tIF(TG_OP='DELETE') THEN\r\n");
                        result.Append($"\t\tINSERT INTO {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
						result.Append("\t\t(\r\n");
						result.Append("\t\t\tHOSTID,\r\n");
						result.Append("\t\t\tDATABASENAME,\r\n");
						result.Append("\t\t\tTABLENAME,\r\n");
						result.Append("\t\t\tACTIONCODE,\r\n");
						result.Append("\t\t\tERRORCODE,\r\n");
						result.Append("\t\t\tRETRYCOUNT,\r\n");
						result.Append("\t\t\tENTEREDDATETIME,\r\n");
						result.Append("\t\t\tINSTANCEID,\r\n");
						result.Append("\t\t\tPROCESSINGID,\r\n");

						for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append($"\t\t\tKEY{keyIndex}NAME,\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? $"\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\tKEY{keyIndex}VALUE\r\n");
						}

						result.Append("\t\t)\r\n");
						result.Append("\r\n");
						result.Append("\t\tVALUES\r\n");
						result.Append("\t\t(\r\n");
						result.Append($"\t\t\t'{hostId}',\r\n");
						result.Append($"\t\t\t'{processingDriverProperties.DatabaseName}',\r\n");
						result.Append($"\t\t\t'{tableObject.Name}',\r\n");
						result.Append("\t\t\t'DELETE',\r\n");
						result.Append("\t\t\t'',\r\n");
						result.Append("\t\t\t0,\r\n");
						result.Append("\t\t\tcurrent_timestamp,\r\n");
						result.Append("\t\t\t@InstanceID,\r\n");
						result.Append(hostNumberOfProcesses > 0 ? "\t\t\t@ProcessingID,\r\n" : "\t\t\t1,\r\n");

						var keyCount = 1;

                        foreach (var columnObject in tableObject.ColumnObjectList.List.Where(columnObject => !string.IsNullOrEmpty(columnObject?.ColumnName) && columnObject.PrimaryKey))
                        {
							result.Append($"\t\t\t'{columnObject.ColumnName}',\r\n");

							if (keyCount < hostMaxNumberOfKeys)
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\tOLD.{columnObject.ColumnName},\r\n" : $"\t\t\tOLD.{columnObject.ColumnName}::VARCHAR,\r\n");
							}

							else
							{
								result.Append(AnTypes.IsStringDataType(columnObject.DataType) ? $"\t\t\tOLD.{columnObject.ColumnName}\r\n" : $"\t\t\tOLD.{columnObject.ColumnName}::VARCHAR\r\n");
							}

                            keyCount++;

                            if (keyCount > hostMaxNumberOfKeys)
                            {
                                break;
                            }
                        }

                        for (var keyIndex = keyCount; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
						{
							result.Append("\t\t\t'',\r\n");
							result.Append(keyIndex != hostMaxNumberOfKeys ? "\t\t\t'',\r\n" : "\t\t\t''\r\n");
						}

						result.Append("\t\t);\r\n");
						result.Append("\tEND IF;\r\n");
						result.Append("\tRETURN OLD;\r\n");
						result.Append("END;\r\n");
						result.Append("\r\n");
						result.Append("$BODY$\r\n");
						result.Append("\tLANGUAGE 'plpgsql' VOLATILE\r\n");
						result.Append("\tCOST 100;\r\n");
						result.Append($"ALTER FUNCTION delete_{triggerName}() OWNER TO postgres;\r\n");
						result.Append("\r\n");
						result.Append("-- Seperator\r\n");
						result.Append("\r\n");
						result.Append($"CREATE TRIGGER {triggerName}\r\n");
						result.Append("\tAFTER DELETE\r\n");
						result.Append($"\tON {tableObject.Name}\r\n");
						result.Append("\tFOR EACH ROW\r\n");
						result.Append($"\tEXECUTE PROCEDURE delete_{triggerName}();\r\n");
						result.Append("\r\n");
					}

	                else if (AnDatabaseTypes.IsSqLiteDriver(driverType))
	                {
		                // Nothing for now.
	                }
				}
	        }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateClearInsertTriggerScript(
	        string triggerName, 
            string hostName, 
            string hostId, 
            string scriptPrefix, 
	        int hostMaxNumberOfCharacters, 
            int hostMaxNumberOfKeys, 
            int hostMaxNumberOfRetries, 
            bool logDataChangesRecords, 
            bool logSyncDbQueueRecords, 
	        bool dataChangesHasIdentityId, 
            bool dontAllowMoreThanOneInstanceOfARecord, 
            bool bidirectionalSyncMode, 
            int hostNumberOfProcesses, 
            int hostNumberOfGuests, 
            AnDriverProperties processingDriverProperties, 
	        AnDriverProperties dataChangesDriverProperties,
            AnDriverProperties syncDbQueueDriverProperties, 
	        AnTableObject tableObject)
        {
            var result = new StringBuilder();

            try
            {
	            if ((tableObject != null) && 
			        !string.IsNullOrEmpty(hostId) && 
				    !string.IsNullOrEmpty(hostName) && 
				    !string.IsNullOrEmpty(triggerName) && 
				    !string.IsNullOrEmpty(tableObject.Name))
			    {
                    var driverType = AnDatabaseTypes.DriverTypeFromString(processingDriverProperties.GetStringSettingValue(AnDriverProperties.AdapterTypeKey));

                    if (AnDatabaseTypes.IsMsSqlServerDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append("GO\r\n");
						result.Append("\r\n");
						result.Append($"IF EXISTS (SELECT name FROM sysobjects WHERE name = N'{triggerName}' AND type = 'TR')\r\n");
						result.Append($"\tDROP TRIGGER {triggerName}\r\n");
						result.Append("\r\n");
						result.Append("GO\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsIbmDb2Driver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName} ;\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsOracleDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER \"{triggerName}\"\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsPervasiveDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER \"{triggerName}\"\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsMySqlDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName}\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsPostgresDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName} ON {tableObject.Name};\r\n");
						result.Append("\r\n");
						result.Append("-- Seperator\r\n");
						result.Append("\r\n");
						result.Append($"DROP FUNCTION add_{triggerName}();\r\n");
						result.Append("\r\n");
					}

				    else if (AnDatabaseTypes.IsSqLiteDriver(driverType))
				    {
					    // Nothing for now.
				    }
			    }
	        }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateClearUpdateTriggerScript(
	        string triggerName, 
            string hostName, 
            string hostId, 
            string scriptPrefix, 
	        int hostMaxNumberOfCharacters, 
            int hostMaxNumberOfKeys, 
            int hostMaxNumberOfRetries, 
            bool logDataChangesRecords, 
            bool logSyncDbQueueRecords, 
	        bool dataChangesHasIdentityId, 
            bool dontAllowMoreThanOneInstanceOfARecord, 
            bool bidirectionalSyncMode, 
            int hostNumberOfProcesses, 
            int hostNumberOfGuests, 
            AnDriverProperties processingDriverProperties, 
	        AnDriverProperties dataChangesDriverProperties,
            AnDriverProperties syncDbQueueDriverProperties, 
	        AnTableObject tableObject)
        {
            var result = new StringBuilder();

            try
            {
	            if ((tableObject != null) && 
			        !string.IsNullOrEmpty(hostId) && 
				    !string.IsNullOrEmpty(hostName) && 
				    !string.IsNullOrEmpty(triggerName) && 
				    !string.IsNullOrEmpty(tableObject.Name))
			    {
                    var driverType = AnDatabaseTypes.DriverTypeFromString(processingDriverProperties.GetStringSettingValue(AnDriverProperties.AdapterTypeKey));

                    if (AnDatabaseTypes.IsMsSqlServerDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append("GO\r\n");
						result.Append("\r\n");
						result.Append($"IF EXISTS (SELECT name FROM sysobjects WHERE name = N'{triggerName}' AND type = 'TR')\r\n");
						result.Append($"\tDROP TRIGGER {triggerName}\r\n");
						result.Append("\r\n");
						result.Append("GO\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsIbmDb2Driver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName} ;\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsOracleDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER \"{triggerName}\"\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsPervasiveDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER \"{triggerName}\"\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsMySqlDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName}\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsPostgresDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName} ON {tableObject.Name};\r\n");
						result.Append("\r\n");
						result.Append("-- Seperator\r\n");
						result.Append("\r\n");
						result.Append($"DROP FUNCTION edit_{triggerName}();\r\n");
						result.Append("\r\n");
					}

				    else if (AnDatabaseTypes.IsSqLiteDriver(driverType))
				    {
					    // Nothing for now.
				    }
			    }
	        }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateClearDeleteTriggerScript(
            string triggerName, 
            string hostName, 
            string hostId, 
            string scriptPrefix, 
	        int hostMaxNumberOfCharacters, 
            int hostMaxNumberOfKeys, 
            int hostMaxNumberOfRetries, 
            bool logDataChangesRecords, 
            bool logSyncDbQueueRecords, 
	        bool dataChangesHasIdentityId, 
            bool dontAllowMoreThanOneInstanceOfARecord, 
            bool bidirectionalSyncMode, 
            int hostNumberOfProcesses, 
            int hostNumberOfGuests, 
            AnDriverProperties processingDriverProperties, 
	        AnDriverProperties dataChangesDriverProperties, 
            AnDriverProperties syncDbQueueDriverProperties, 
	        AnTableObject tableObject)
        {
            var result = new StringBuilder();

            try
            {
	            if ((tableObject != null) && 
			        !string.IsNullOrEmpty(hostId) && 
				    !string.IsNullOrEmpty(hostName) && 
				    !string.IsNullOrEmpty(triggerName) && 
				    !string.IsNullOrEmpty(tableObject.Name))
			    {
                    var driverType = AnDatabaseTypes.DriverTypeFromString(processingDriverProperties.GetStringSettingValue(AnDriverProperties.AdapterTypeKey));

                    if (AnDatabaseTypes.IsMsSqlServerDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append("GO\r\n");
						result.Append("\r\n");
						result.Append($"IF EXISTS (SELECT name FROM sysobjects WHERE name = N'{triggerName}' AND type = 'TR')\r\n");
						result.Append($"\tDROP TRIGGER {triggerName}\r\n");
						result.Append("\r\n");
						result.Append("GO\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsIbmDb2Driver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName} ;\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsOracleDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER \"{triggerName}\"\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsPervasiveDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER \"{triggerName}\"\r\n");
						result.Append("\r\n");
						result.Append("/\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsMySqlDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName}\r\n");
						result.Append("\r\n");
					}

                    else if (AnDatabaseTypes.IsPostgresDriver(driverType))
                    {
						result.Append("\r\n");
						result.Append($"DROP TRIGGER {triggerName} ON {tableObject.Name};\r\n");
						result.Append("\r\n");
						result.Append("-- Seperator\r\n");
						result.Append("\r\n");
						result.Append($"DROP FUNCTION delete_{triggerName}();\r\n");
						result.Append("\r\n");
					}

				    else if (AnDatabaseTypes.IsSqLiteDriver(driverType))
				    {
					    // Nothing for now.
				    }
			    }
	        }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateCreateSyncDbQueueTablesScript(
            int dataHostId,
            string hostId,
            string syncDbQueueTableName,
            AnDriverProperties syncDbQueueDriverProperties,
            int hostMaxNumberOfCharacters,
            int hostMaxNumberOfKeys,
            int maxNumberOfRetries,
            bool logSyncDbQueueRecords,
            bool dataChangesHasIdentityId,
            bool autoGeneratedId)
        {
            var result = new StringBuilder();

            try
            {
                if ((dataHostId > 0) &&
                    !string.IsNullOrEmpty(hostId) &&
                    (syncDbQueueDriverProperties != null))
                {
                    var driverType = AnDatabaseTypes.DriverTypeFromString(syncDbQueueDriverProperties.GetStringSettingValue(AnDriverProperties.AdapterTypeKey));

                    if (AnDatabaseTypes.IsMsSqlServerDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append($"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}]') AND type in (N'U'))\r\n");
                        result.Append($"\tDROP TABLE [dbo].[{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}]\r\n");
                        result.Append("\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append("SET ANSI_NULLS ON\r\n");
                        result.Append("\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append("SET QUOTED_IDENTIFIER ON\r\n");
                        result.Append("\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append($"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}]') AND type in (N'U'))\r\n");
                        result.Append("BEGIN\r\n");
                        result.Append($"CREATE TABLE [dbo].[{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}](\r\n");
                        result.Append(autoGeneratedId && !dataChangesHasIdentityId ? "\t[ID] [int] IDENTITY(1,1) NOT NULL,\r\n" : "\t[ID] [int] NOT NULL,\r\n");
                        result.Append("\t[HOSTID] [varchar](20) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                        result.Append("\t[ACTIONTYPE] [smallint] NOT NULL,\r\n");
                        result.Append("\t[INSTANCEID] [smallint] NOT NULL,\r\n");
                        result.Append("\t[PROCESSINGID] [smallint] NOT NULL,\r\n");
                        result.Append("\t[SENDSTATUS] [smallint] NOT NULL,\r\n");
                        result.Append($"\t[TABLENAME] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                        result.Append("\t[HASHCODE] [varchar](50) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                        result.Append("\t[ENTEREDDATETIME] [datetime] NOT NULL,\r\n");
                        result.Append("\t[PROCESSEDDATETIME] [datetime] NOT NULL,\r\n");

                        for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                        {
                            result.Append($"\t[KEY{keyIndex}NAME] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_{AnTypes.FormatDataChangesTableName(syncDbQueueTableName)}_KEY{keyIndex}NAME] DEFAULT (''),\r\n");
                            result.Append($"\t[KEY{keyIndex}VALUE] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_{AnTypes.FormatDataChangesTableName(syncDbQueueTableName)}_KEY{keyIndex}VALUE] DEFAULT (''),\r\n");
                        }

                        result.Append("\t[OBJECTDATA] image NULL,\r\n");
                        result.Append($"\t[ERRORCODE] [varchar](200) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}_ERRORCODE] DEFAULT (''),\r\n");
                        result.Append($"\tCONSTRAINT [PK_{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}] PRIMARY KEY CLUSTERED\r\n");
                        result.Append("\t(\r\n");
                        result.Append("\t\t[ID] ASC\r\n");
                        result.Append("\t) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\n");
                        result.Append(") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]\r\n");
                        result.Append("END\r\n");
                        result.Append("\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");

                        if (logSyncDbQueueRecords)
                        {
                            // Create the log table.

                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append($"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG]') AND type in (N'U'))\r\n");
                            result.Append($"\tDROP TABLE [dbo].[{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG]\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append("SET ANSI_NULLS ON\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append("SET QUOTED_IDENTIFIER ON\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append($"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG]') AND type in (N'U'))\r\n");
                            result.Append("BEGIN\r\n");
                            result.Append($"CREATE TABLE [dbo].[{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG](\r\n");
                            result.Append("\t[ID] [int] NOT NULL,\r\n");
                            result.Append("\t[HOSTID] [varchar](20) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                            result.Append("\t[ACTIONTYPE] [smallint] NOT NULL,\r\n");
                            result.Append("\t[INSTANCEID] [smallint] NOT NULL,\r\n");
                            result.Append("\t[PROCESSINGID] [smallint] NOT NULL,\r\n");
                            result.Append("\t[SENDSTATUS] [smallint] NOT NULL,\r\n");
                            result.Append($"\t[TABLENAME] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                            result.Append("\t[HASHCODE] [varchar](50) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                            result.Append("\t[ENTEREDDATETIME] [datetime] NOT NULL,\r\n");
                            result.Append("\t[PROCESSEDDATETIME] [datetime] NOT NULL,\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\t[KEY{keyIndex}NAME] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_{AnTypes.FormatDataChangesTableName(syncDbQueueTableName)}LOG_KEY{keyIndex}NAME] DEFAULT (''),\r\n");
                                result.Append($"\t[KEY{keyIndex}VALUE] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_{AnTypes.FormatDataChangesTableName(syncDbQueueTableName)}LOG_KEY{keyIndex}VALUE] DEFAULT (''),\r\n");
                            }

                            result.Append("\t[OBJECTDATA] image NULL,\r\n");
                            result.Append($"\t[ERRORCODE] [varchar](200) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG_ERRORCODE] DEFAULT (''),\r\n");
                            result.Append($"\tCONSTRAINT [PK_{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG] PRIMARY KEY CLUSTERED\r\n");
                            result.Append("\t(\r\n");
                            result.Append("\t\t[ID] ASC\r\n");
                            result.Append("\t) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\n");
                            result.Append(") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]\r\n");
                            result.Append("END\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");

                            // No trigger script used because of difficulty with BLOB column.
                            // The log file is populated programatically.
                        }


                        if (!autoGeneratedId && !dataChangesHasIdentityId)
                        {
                            // Add the global variables table and stored procedures.

                            result.Append("\r\n");
                            result.Append("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GLOBALVALUES]') AND type in (N'U'))\r\n");
                            result.Append("\tDROP TABLE [dbo].[GLOBALVALUES]\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append("SET ANSI_NULLS ON\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append("SET QUOTED_IDENTIFIER ON\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append("IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GLOBALVALUES]') AND type in (N'U'))\r\n");
                            result.Append("BEGIN\r\n");
                            result.Append("CREATE TABLE [dbo].[GLOBALVALUES](\r\n");
                            result.Append("\t[HOSTID] [varchar](20) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL, \r\n");
                            result.Append("\t[NAME] [varchar](20) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL, \r\n");
                            result.Append("\t[VALUE] [int] NOT NULL\r\n");
                            result.Append(")\r\n");
                            result.Append("END\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append("\r\n");
                            result.Append($"INSERT INTO [{syncDbQueueDriverProperties.DatabaseName}].[dbo].[GLOBALVALUES] (HOSTID, NAME, VALUE) VALUES ('{hostId}', 'NEXTUNIQUEID', 1)\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                        }
                    }

                    else if (AnDatabaseTypes.IsIbmDb2Driver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)} ;\r\n");
                        result.Append("\r\n");
                        result.Append($"CREATE TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)} (\r\n");

                        if (autoGeneratedId && !dataChangesHasIdentityId)
                        {
                            result.Append("\tID INTEGER NOT NULL\r\n");
                            result.Append("\t\tGENERATED ALWAYS AS IDENTITY (\r\n");
                            result.Append("\t\t\tSTART WITH 1 INCREMENT BY 1\r\n");
                            result.Append("\t\t\tCACHE 20 NO ORDER),\r\n");
                        }

                        else
                        {
                            result.Append("\tID INTEGER NOT NULL, \r\n");
                        }

                        result.Append("\tHOSTID VARCHAR (100) DEFAULT '' NOT NULL,\r\n");
                        result.Append("\tACTIONTYPE SMALLINT DEFAULT 0 NOT NULL,\r\n");
                        result.Append("\tINSTANCEID SMALLINT DEFAULT 0 NOT NULL,\r\n");
                        result.Append("\tPROCESSINGID SMALLINT DEFAULT 0 NOT NULL,\r\n");
                        result.Append("\tSENDSTATUS SMALLINT DEFAULT 0 NOT NULL,\r\n");
                        result.Append($"\tTABLENAME VARCHAR ({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                        result.Append("\tHASHCODE VARCHAR (50) DEFAULT '' NOT NULL,\r\n");
                        result.Append("\tENTEREDDATETIME TIMESTAMP NOT NULL,\r\n");
                        result.Append("\tPROCESSEDDATETIME TIMESTAMP NOT NULL,\r\n");

                        for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                        {
                            result.Append($"\tKEY{keyIndex}NAME VARCHAR({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                            result.Append($"\tKEY{keyIndex}VALUE VARCHAR({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                        }

                        result.Append("\tOBJECTDATA BLOB (2 G) NOT LOGGED NOT COMPACT,\r\n");
                        result.Append("\tERRORCODE VARCHAR (200) NOT NULL WITH DEFAULT '',\r\n");
                        result.Append($"\tCONSTRAINT {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}_PK PRIMARY KEY (ID)\r\n");
                        result.Append(");\r\n");
                        result.Append("\r\n");

                        if (logSyncDbQueueRecords)
                        {
                            result.Append("\r\n");
                            result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG ;\r\n");
                            result.Append("\r\n");
                            result.Append($"CREATE TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG (\r\n");
                            result.Append("\tID INTEGER NOT NULL, \r\n");
                            result.Append("\tHOSTID VARCHAR (100) DEFAULT '' NOT NULL,\r\n");
                            result.Append("\tACTIONTYPE SMALLINT DEFAULT 0 NOT NULL,\r\n");
                            result.Append("\tINSTANCEID SMALLINT DEFAULT 0 NOT NULL,\r\n");
                            result.Append("\tPROCESSINGID SMALLINT DEFAULT 0 NOT NULL,\r\n");
                            result.Append("\tSENDSTATUS SMALLINT DEFAULT 0 NOT NULL,\r\n");
                            result.Append($"\tTABLENAME VARCHAR ({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                            result.Append("\tHASHCODE VARCHAR (50) DEFAULT '' NOT NULL,\r\n");
                            result.Append("\tENTEREDDATETIME TIMESTAMP NOT NULL,\r\n");
                            result.Append("\tPROCESSEDDATETIME TIMESTAMP NOT NULL,\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\tKEY{keyIndex}NAME VARCHAR({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                                result.Append($"\tKEY{keyIndex}VALUE VARCHAR({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                            }

                            result.Append("\tOBJECTDATA BLOB (2 G) NOT LOGGED NOT COMPACT,\r\n");
                            result.Append("\tERRORCODE VARCHAR (200) NOT NULL WITH DEFAULT '',\r\n");
                            result.Append($"\tCONSTRAINT {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG_PK PRIMARY KEY (ID)\r\n");
                            result.Append(");\r\n");
                            result.Append("\r\n");
                        }
                    }

                    else if (AnDatabaseTypes.IsOracleDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");

                        if (autoGeneratedId && !dataChangesHasIdentityId)
                        {
                            result.Append($"DROP SEQUENCE \"{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}_SEQ\"\r\n");
                            result.Append("/\r\n");
                            result.Append("\r\n");
                        }

                        result.Append($"CREATE TABLE \"{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}\"\r\n");
                        result.Append("(\r\n");
                        result.Append("\t\"ID\" NUMBER,\r\n");
                        result.Append("\t\"HOSTID\" VARCHAR2(20) DEFAULT '' NOT NULL,\r\n");
                        result.Append("\t\"ACTIONTYPE\" NUMBER DEFAULT 0 NOT NULL,\r\n");
                        result.Append("\t\"INSTANCEID\" NUMBER DEFAULT 0 NOT NULL,\r\n");
                        result.Append("\t\"PROCESSINGID\" NUMBER DEFAULT 0 NOT NULL,\r\n");
                        result.Append("\t\"SENDSTATUS\" NUMBER DEFAULT 0 NOT NULL,\r\n");
                        result.Append($"\t\"TABLENAME\" VARCHAR2({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                        result.Append("\t\"HASHCODE\" VARCHAR2(50) DEFAULT '' NOT NULL,\r\n");
                        result.Append("\t\"ENTEREDDATETIME\" DATE NOT NULL,\r\n");
                        result.Append("\t\"PROCESSEDDATETIME\" DATE NOT NULL,\r\n");

                        for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                        {
                            result.Append($"\t\"KEY{keyIndex}NAME\" VARCHAR2({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                            result.Append($"\t\"KEY{keyIndex}VALUE\" VARCHAR2({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                        }

                        result.Append("\t\"OBJECTDATA\" BLOB,\r\n");
                        result.Append("\t\"ERRORCODE\" VARCHAR2(200) DEFAULT '' NOT NULL,\r\n");
                        result.Append($"\tCONSTRAINT \"{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}_PK\" PRIMARY KEY (\"ID\") ENABLE\r\n");
                        result.Append(")\r\n");
                        result.Append("\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");

                        if (autoGeneratedId && !dataChangesHasIdentityId)
                        {
                            result.Append($"CREATE SEQUENCE \"{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}_SEQ\" MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 11901 CACHE 20 NOORDER NOCYCLE\r\n");
                            result.Append("\r\n");
                            result.Append("/\r\n");
                            result.Append("\r\n");
                            result.Append($"CREATE OR REPLACE TRIGGER \"BI_{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}\"\r\n");
                            result.Append($"\tBEFORE INSERT ON \"{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}\"\r\n");
                            result.Append("\tFOR EACH ROW\r\n");
                            result.Append("BEGIN\r\n");
                            result.Append($"\tSELECT \"{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}_SEQ\".nextval INTO :NEW.ID FROM dual;\r\n");
                            result.Append("END;\r\n");
                            result.Append("\r\n");
                            result.Append("/\r\n");
                            result.Append("\r\n");
                            result.Append($"ALTER TRIGGER \"BI_{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}\" ENABLE\r\n");
                            result.Append("\r\n");
                            result.Append("/\r\n");
                            result.Append("\r\n");
                        }

                        if (logSyncDbQueueRecords)
                        {
                            result.Append("\r\n");
                            result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG\r\n");
                            result.Append("/\r\n");
                            result.Append("\r\n");
                            result.Append($"CREATE TABLE \"{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG\"\r\n");
                            result.Append("(\r\n");
                            result.Append("\t\"ID\" NUMBER,\r\n");
                            result.Append("\t\"HOSTID\" VARCHAR2(20) DEFAULT '' NOT NULL,\r\n");
                            result.Append("\t\"ACTIONTYPE\" NUMBER DEFAULT 0 NOT NULL,\r\n");
                            result.Append("\t\"INSTANCEID\" NUMBER DEFAULT 0 NOT NULL,\r\n");
                            result.Append("\t\"PROCESSINGID\" NUMBER DEFAULT 0 NOT NULL,\r\n");
                            result.Append("\t\"SENDSTATUS\" NUMBER DEFAULT 0 NOT NULL,\r\n");
                            result.Append($"\t\"TABLENAME\" VARCHAR2({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                            result.Append("\t\"HASHCODE\" VARCHAR2(50) DEFAULT '' NOT NULL,\r\n");
                            result.Append("\t\"ENTEREDDATETIME\" DATE NOT NULL,\r\n");
                            result.Append("\t\"PROCESSEDDATETIME\" DATE NOT NULL,\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\t\"KEY{keyIndex}NAME\" VARCHAR2({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                                result.Append($"\t\"KEY{keyIndex}VALUE\" VARCHAR2({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                            }

                            result.Append("\t\"OBJECTDATA\" BLOB,\r\n");
                            result.Append("\t\"ERRORCODE\" VARCHAR2(200) DEFAULT '' NOT NULL,\r\n");
                            result.Append($"\tCONSTRAINT \"{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG_PK\" PRIMARY KEY (\"ID\") ENABLE\r\n");
                            result.Append(")\r\n");
                            result.Append("\r\n");
                            result.Append("/\r\n");
                            result.Append("\r\n");
                        }
                    }

                    else if (AnDatabaseTypes.IsPervasiveDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");
                        result.Append($"CREATE TABLE \"{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}\"(\r\n");
                        result.Append(autoGeneratedId && !dataChangesHasIdentityId ? "\t\"ID\" IDENTITY DEFAULT '0',\r\n" : "\t\"ID\" INTEGER DEFAULT '0',\r\n");
                        result.Append("\t\"HOSTID\" VARCHAR(20) CASE ,\r\n");
                        result.Append("\t\"ACTIONTYPE\" SMALLINT,\r\n");
                        result.Append("\t\"INSTANCEID\" SMALLINT,\r\n");
                        result.Append("\t\"PROCESSINGID\" SMALLINT,\r\n");
                        result.Append("\t\"SENDSTATUS\" SMALLINT,\r\n");
                        result.Append($"\t\"TABLENAME\" VARCHAR({hostMaxNumberOfCharacters}) CASE ,\r\n");
                        result.Append("\t\"HASHCODE\" VARCHAR(50) CASE ,\r\n");
                        result.Append("\t\"ENTEREDDATETIME\" DATETIME,\r\n");
                        result.Append("\t\"PROCESSEDDATETIME\" DATETIME,\r\n");

                        for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                        {
                            result.Append($"\t\"KEY{keyIndex}NAME\" VARCHAR({hostMaxNumberOfCharacters}) NOT NULL  CASE ,\r\n");
                            result.Append($"\t\"KEY{keyIndex}VALUE\" VARCHAR({hostMaxNumberOfCharacters}) NOT NULL  CASE ,\r\n");
                        }

                        result.Append("\t\"OBJECTDATA\" LONGVARBINARY,\r\n");
                        result.Append("\t\"ERRORCODE\" VARCHAR(200) CASE ,\r\n");
                        result.Append("\tPRIMARY KEY (\"ID\")\r\n");
                        result.Append(");\r\n");

                        if (autoGeneratedId && !dataChangesHasIdentityId)
                        {
                            result.Append($"CREATE UNIQUE INDEX \"PK_ID\" ON \"{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}\"(\"ID\");\r\n");
                            result.Append($"CREATE UNIQUE INDEX \"UK_ID\" ON \"{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}\"(\"ID\");\r\n");
                            result.Append("\r\n");
                            result.Append("/\r\n");
                        }

                        result.Append("\r\n");

                        if (logSyncDbQueueRecords)
                        {
                            result.Append("\r\n");
                            result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG\r\n");
                            result.Append("/\r\n");
                            result.Append("\r\n");
                            result.Append($"CREATE TABLE \"{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG\"(\r\n");
                            result.Append("\t\"ID\" INTEGER DEFAULT '0',\r\n");
                            result.Append("\t\"HOSTID\" VARCHAR(20) CASE ,\r\n");
                            result.Append("\t\"ACTIONTYPE\" SMALLINT,\r\n");
                            result.Append("\t\"INSTANCEID\" SMALLINT,\r\n");
                            result.Append("\t\"PROCESSINGID\" SMALLINT,\r\n");
                            result.Append("\t\"SENDSTATUS\" SMALLINT,\r\n");
                            result.Append($"\t\"TABLENAME\" VARCHAR({hostMaxNumberOfCharacters}) CASE ,\r\n");
                            result.Append("\t\"HASHCODE\" VARCHAR(50) CASE ,\r\n");
                            result.Append("\t\"ENTEREDDATETIME\" DATETIME,\r\n");
                            result.Append("\t\"PROCESSEDDATETIME\" DATETIME,\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\t\"KEY{keyIndex}NAME\" VARCHAR({hostMaxNumberOfCharacters}) NOT NULL  CASE ,\r\n");
                                result.Append($"\t\"KEY{keyIndex}VALUE\" VARCHAR({hostMaxNumberOfCharacters}) NOT NULL  CASE ,\r\n");
                            }

                            result.Append("\t\"OBJECTDATA\" LONGVARBINARY,\r\n");
                            result.Append("\t\"ERRORCODE\" VARCHAR(200) CASE ,\r\n");
                            result.Append("\tPRIMARY KEY (\"ID\")\r\n");
                            result.Append(");\r\n");
                            result.Append("\r\n");
                        }
                    }

                    else if (AnDatabaseTypes.IsMySqlDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append("SET FOREIGN_KEY_CHECKS=0;\r\n");
                        result.Append("\r\n");
                        result.Append("-- Seperator\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP TABLE IF EXISTS `{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}`;\r\n");
                        result.Append("\r\n");
                        result.Append("-- Seperator\r\n");
                        result.Append("\r\n");
                        result.Append($"CREATE TABLE `{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}` (\r\n");
                        result.Append(autoGeneratedId && !dataChangesHasIdentityId ? "\t`ID` int(11) NOT NULL AUTO_INCREMENT,\r\n" : "\t`ID` int(11) NOT NULL,\r\n");
                        result.Append("\t`HOSTID` varchar(20) NOT NULL DEFAULT '',\r\n");
                        result.Append("\t`ACTIONTYPE` smallint(6) NOT NULL DEFAULT '0',\r\n");
                        result.Append("\t`INSTANCEID` smallint(6) NOT NULL DEFAULT '0',\r\n");
                        result.Append("\t`PROCESSINGID` smallint(6) NOT NULL DEFAULT '0',\r\n");
                        result.Append("\t`SENDSTATUS` smallint(6) NOT NULL DEFAULT '0',\r\n");
                        result.Append($"\t`TABLENAME` varchar({hostMaxNumberOfCharacters}) NOT NULL DEFAULT '',\r\n");
                        result.Append("\t`HASHCODE` varchar(50) NOT NULL DEFAULT '',\r\n");
                        result.Append("\t`ENTEREDDATETIME` datetime DEFAULT NULL,\r\n");
                        result.Append("\t`PROCESSEDDATETIME` datetime DEFAULT NULL,\r\n");

                        for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                        {
                            result.Append($"\t`KEY{keyIndex}NAME` varchar({hostMaxNumberOfCharacters}) NOT NULL DEFAULT '',\r\n");
                            result.Append($"\t`KEY{keyIndex}VALUE` varchar({hostMaxNumberOfCharacters}) NOT NULL DEFAULT '',\r\n");
                        }

                        result.Append("\t`OBJECTDATA` blob,\r\n");
                        result.Append("\t`ERRORCODE` varchar(200) NOT NULL DEFAULT '',\r\n");
                        result.Append("\tPRIMARY KEY (`ID`)\r\n");
                        result.Append(") ENGINE=InnoDB DEFAULT CHARSET=latin1;\r\n");
                        result.Append("\r\n");

                        if (logSyncDbQueueRecords)
                        {
                            result.Append("\r\n");
                            result.Append("SET FOREIGN_KEY_CHECKS=0;\r\n");
                            result.Append("\r\n");
                            result.Append("-- Seperator\r\n");
                            result.Append("\r\n");
                            result.Append($"DROP TABLE IF EXISTS `{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG`;\r\n");
                            result.Append("\r\n");
                            result.Append("-- Seperator\r\n");
                            result.Append("\r\n");
                            result.Append($"CREATE TABLE `{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG` (\r\n");
                            result.Append("\t`ID` int(11) NOT NULL,\r\n");
                            result.Append("\t`HOSTID` varchar(20) NOT NULL DEFAULT '',\r\n");
                            result.Append("\t`ACTIONTYPE` smallint(6) NOT NULL DEFAULT '0',\r\n");
                            result.Append("\t`INSTANCEID` smallint(6) NOT NULL DEFAULT '0',\r\n");
                            result.Append("\t`PROCESSINGID` smallint(6) NOT NULL DEFAULT '0',\r\n");
                            result.Append("\t`SENDSTATUS` smallint(6) NOT NULL DEFAULT '0',\r\n");
                            result.Append($"\t`TABLENAME` varchar({hostMaxNumberOfCharacters}) NOT NULL DEFAULT '',\r\n");
                            result.Append("\t`HASHCODE` varchar(50) NOT NULL DEFAULT '',\r\n");
                            result.Append("\t`ENTEREDDATETIME` datetime DEFAULT NULL,\r\n");
                            result.Append("\t`PROCESSEDDATETIME` datetime DEFAULT NULL,\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\t`KEY{keyIndex}NAME` varchar({hostMaxNumberOfCharacters}) NOT NULL DEFAULT '',\r\n");
                                result.Append($"\t`KEY{keyIndex}VALUE` varchar({hostMaxNumberOfCharacters}) NOT NULL DEFAULT '',\r\n");
                            }

                            result.Append("\t`OBJECTDATA` blob,\r\n");
                            result.Append("\t`ERRORCODE` varchar(200) NOT NULL DEFAULT '',\r\n");
                            result.Append("\tPRIMARY KEY (`ID`)\r\n");
                            result.Append(") ENGINE=InnoDB DEFAULT CHARSET=latin1;\r\n");
                            result.Append("\r\n");
                        }
                    }

                    else if (AnDatabaseTypes.IsPostgresDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)};\r\n");
                        result.Append("\r\n");
                        result.Append("-- Seperator\r\n");
                        result.Append("\r\n");
                        result.Append($"CREATE TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}\r\n");
                        result.Append("(\r\n");
                        result.Append(autoGeneratedId && !dataChangesHasIdentityId ? "\tID serial NOT NULL, \r\n" : "\tID int NOT NULL, \r\n");
                        result.Append("\tHOSTID character varying(20) NOT NULL, \r\n");
                        result.Append("\tACTIONTYPE smallint NOT NULL, \r\n");
                        result.Append("\tINSTANCEID smallint NOT NULL, \r\n");
                        result.Append("\tPROCESSINGID smallint NOT NULL, \r\n");
                        result.Append("\tSENDSTATUS smallint NOT NULL, \r\n");
                        result.Append("\tTABLENAME character varying(100) NOT NULL, \r\n");
                        result.Append("\tHASHCODE character varying(50) NOT NULL, \r\n");
                        result.Append("\tENTEREDDATETIME timestamp without time zone NOT NULL, \r\n");
                        result.Append("\tPROCESSEDDATETIME timestamp without time zone NOT NULL, \r\n");

                        for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                        {
                            result.Append($"\tKEY{keyIndex}NAME character varying({hostMaxNumberOfCharacters}) NOT NULL,\r\n");
                            result.Append($"\tKEY{keyIndex}VALUE character varying({hostMaxNumberOfCharacters}) NOT NULL,\r\n");
                        }

                        result.Append("\tOBJECTDATA bytea NULL, \r\n");
                        result.Append("\tERRORCODE character varying NOT NULL, \r\n");
                        result.Append("\tPRIMARY KEY (ID)\r\n");
                        result.Append(") WITH (OIDS=FALSE)\r\n");
                        result.Append(";\r\n");
                        result.Append("\r\n");

                        if (logSyncDbQueueRecords)
                        {
                            result.Append("\r\n");
                            result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG;\r\n");
                            result.Append("\r\n");
                            result.Append("-- Seperator\r\n");
                            result.Append("\r\n");
                            result.Append($"CREATE TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG\r\n");
                            result.Append("(\r\n");
                            result.Append("\tID int NOT NULL, \r\n");
                            result.Append("\tHOSTID character varying(20) NOT NULL, \r\n");
                            result.Append("\tACTIONTYPE smallint NOT NULL, \r\n");
                            result.Append("\tINSTANCEID smallint NOT NULL, \r\n");
                            result.Append("\tPROCESSINGID smallint NOT NULL, \r\n");
                            result.Append("\tSENDSTATUS smallint NOT NULL, \r\n");
                            result.Append("\tTABLENAME character varying(100) NOT NULL, \r\n");
                            result.Append("\tHASHCODE character varying(50) NOT NULL, \r\n");
                            result.Append("\tENTEREDDATETIME timestamp without time zone NOT NULL, \r\n");
                            result.Append("\tPROCESSEDDATETIME timestamp without time zone NOT NULL, \r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\tKEY{keyIndex}NAME character varying({hostMaxNumberOfCharacters}) NOT NULL,\r\n");
                                result.Append($"\tKEY{keyIndex}VALUE character varying({hostMaxNumberOfCharacters}) NOT NULL,\r\n");
                            }

                            result.Append("\tOBJECTDATA bytea NULL, \r\n");
                            result.Append("\tERRORCODE character varying NOT NULL, \r\n");
                            result.Append("\tPRIMARY KEY (ID)\r\n");
                            result.Append(") WITH (OIDS=FALSE)\r\n");
                            result.Append(";\r\n");
                            result.Append("\r\n");
                        }

	                    else if (AnDatabaseTypes.IsSqLiteDriver(driverType))
	                    {
		                    // Nothing for now.
	                    }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateClearSyncDbQueueTablesScript(
            string syncDbQueueTableName,
            AnDriverProperties syncDbQueueDriverProperties)
        {
            var result = new StringBuilder();

            try
            {
                if (syncDbQueueDriverProperties != null)
                {
                    var driverType = AnDatabaseTypes.DriverTypeFromString(syncDbQueueDriverProperties.GetStringSettingValue(AnDriverProperties.AdapterTypeKey));

                    if (AnDatabaseTypes.IsMsSqlServerDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append($"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}]') AND type in (N'U'))\r\n");
                        result.Append($"\tDROP TABLE [dbo].[{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}]\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append("\r\n");
                        result.Append($"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG]') AND type in (N'U'))\r\n");
                        result.Append($"\tDROP TABLE [dbo].[{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG]\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append("\r\n");
                        result.Append("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GLOBALVALUES]') AND type in (N'U'))\r\n");
                        result.Append("\tDROP TABLE [dbo].[GLOBALVALUES]\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                    }

                    else if (AnDatabaseTypes.IsIbmDb2Driver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)};\r\n");
                        result.Append("\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG;\r\n");
                        result.Append("\r\n");
                    }

                    else if (AnDatabaseTypes.IsOracleDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP SEQUENCE \"{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}_SEQ\"\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");
                    }

                    else if (AnDatabaseTypes.IsPervasiveDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");
                    }

                    else if (AnDatabaseTypes.IsMySqlDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append("SET FOREIGN_KEY_CHECKS=0;\r\n");
                        result.Append("\r\n");
                        result.Append("-- Seperator\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP TABLE IF EXISTS `{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}`;\r\n");
                        result.Append("\r\n");
                        result.Append("\r\n");
                        result.Append("SET FOREIGN_KEY_CHECKS=0;\r\n");
                        result.Append("\r\n");
                        result.Append("-- Seperator\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP TABLE IF EXISTS `{AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG`;\r\n");
                        result.Append("\r\n");
                    }

                    else if (AnDatabaseTypes.IsPostgresDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)};\r\n");
                        result.Append("\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(syncDbQueueTableName)}LOG;\r\n");
                        result.Append("\r\n");
                    }

	                else if (AnDatabaseTypes.IsSqLiteDriver(driverType))
	                {
		                // Nothing for now.
	                }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateCreateDataChangesTablesScript(
            string dataChangesTableName,
            AnDriverProperties dataChangesDriverProperties,
            int hostMaxNumberOfCharacters,
            int hostMaxNumberOfKeys,
            int hostMaxNumberOfRetries,
            bool logDataChangesRecords,
            bool bidirectionalSyncMode,
            bool autoGeneratedId)
        {
            var result = new StringBuilder();

            try
            {
                if (dataChangesDriverProperties != null)
                {
                    var driverType = AnDatabaseTypes.DriverTypeFromString(dataChangesDriverProperties.GetStringSettingValue(AnDriverProperties.AdapterTypeKey));

                    if (AnDatabaseTypes.IsMsSqlServerDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append($"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}]') AND type in (N'U'))\r\n");
                        result.Append($"\tDROP TABLE [dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}]\r\n");
                        result.Append("\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append("SET ANSI_NULLS ON\r\n");
                        result.Append("\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append("SET QUOTED_IDENTIFIER ON\r\n");
                        result.Append("\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append($"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}]') AND type in (N'U'))\r\n");
                        result.Append("BEGIN\r\n");
                        result.Append($"CREATE TABLE [dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}](\r\n");
                        result.Append(autoGeneratedId ? "\t[ID] [int] IDENTITY(1,1) NOT NULL,\r\n" : "\t[ID] [int] NOT NULL DEFAULT (0),\r\n");
                        result.Append("\t[HOSTID] [varchar](20) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                        result.Append($"\t[DATABASENAME] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                        result.Append($"\t[TABLENAME] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_{AnTypes.FormatDataChangesTableName(dataChangesTableName)}_TABLENAME] DEFAULT ((0)),\r\n");
                        result.Append("\t[ACTIONCODE] [varchar](10) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                        result.Append("\t[ERRORCODE] [varchar](200) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                        result.Append("\t[RETRYCOUNT] [smallint] NOT NULL DEFAULT (0),\r\n");
                        result.Append("\t[ENTEREDDATETIME] [datetime] NOT NULL,\r\n");
                        result.Append("\t[INSTANCEID] [int] NOT NULL,\r\n");
                        result.Append("\t[PROCESSINGID] [int] NOT NULL,\r\n");

                        for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                        {
                            result.Append($"\t[KEY{keyIndex}NAME] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_{AnTypes.FormatDataChangesTableName(dataChangesTableName)}_KEY{keyIndex}NAME] DEFAULT (''),\r\n");
                            result.Append($"\t[KEY{keyIndex}VALUE] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_{AnTypes.FormatDataChangesTableName(dataChangesTableName)}_KEY{keyIndex}VALUE] DEFAULT (''),\r\n");
                        }

                        result.Append($"\tCONSTRAINT [PK_{AnTypes.FormatDataChangesTableName(dataChangesTableName)}] PRIMARY KEY CLUSTERED\r\n");
                        result.Append("\t(\r\n");
                        result.Append("\t\t[ID] ASC\r\n");
                        result.Append("\t) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\n");
                        result.Append(") ON [PRIMARY]\r\n");
                        result.Append("END\r\n");
                        result.Append("\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");

                        if (logDataChangesRecords)
                        {
                            // Create the log table

                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append($"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG]') AND type in (N'U'))\r\n");
                            result.Append($"\tDROP TABLE [dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG]\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append("SET ANSI_NULLS ON\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append("SET QUOTED_IDENTIFIER ON\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append($"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG]') AND type in (N'U'))\r\n");
                            result.Append("BEGIN\r\n");
                            result.Append($"CREATE TABLE [dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG](\r\n");
                            result.Append("\t[ID] [int] NOT NULL DEFAULT (0),\r\n");
                            result.Append("\t[HOSTID] [varchar](20) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                            result.Append($"\t[DATABASENAME] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                            result.Append($"\t[TABLENAME] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG_TABLENAME] DEFAULT ((0)),\r\n");
                            result.Append("\t[ACTIONCODE] [varchar](10) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                            result.Append("\t[ERRORCODE] [varchar](200) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                            result.Append("\t[RETRYCOUNT] [smallint] NOT NULL DEFAULT (0),\r\n");
                            result.Append("\t[ENTEREDDATETIME] [datetime] NOT NULL,\r\n");
                            result.Append("\t[PROCESSEDDATETIME] [datetime] NOT NULL,\r\n");
                            result.Append("\t[INSTANCEID] [int] NOT NULL,\r\n");
                            result.Append("\t[PROCESSINGID] [int] NOT NULL,\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\t[KEY{keyIndex}NAME] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG_KEY{keyIndex}NAME] DEFAULT (''),\r\n");
                                result.Append($"\t[KEY{keyIndex}VALUE] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG_KEY{keyIndex}VALUE] DEFAULT (''),\r\n");
                            }

                            result.Append($"\tCONSTRAINT [PK_{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG] PRIMARY KEY CLUSTERED\r\n");
                            result.Append("\t(\r\n");
                            result.Append("\t\t[ID] ASC\r\n");
                            result.Append("\t) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\n");
                            result.Append(") ON [PRIMARY]\r\n");
                            result.Append("END\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");

                            // Create the log Delete Trigger

                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append("IF EXISTS (SELECT name FROM sysobjects WHERE name = N'DataSync_DeleteDataChanges' AND type = 'TR')\r\n");
                            result.Append("\tDROP TRIGGER DataSync_DeleteDataChanges\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append($"CREATE TRIGGER DataSync_DeleteDataChanges ON [dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}]\r\n");
                            result.Append("FOR DELETE\r\n");
                            result.Append("AS\r\n");
                            result.Append("BEGIN\r\n");
                            result.Append("\r\n");
                            result.Append("\tDECLARE @ID int\r\n");
                            result.Append("\tDECLARE @HOSTID varchar(20)\r\n");
                            result.Append($"\tDECLARE @DATABASENAME varchar({hostMaxNumberOfCharacters})\r\n");
                            result.Append($"\tDECLARE @TABLENAME varchar({hostMaxNumberOfCharacters})\r\n");
                            result.Append("\tDECLARE @ACTIONCODE varchar(10)\r\n");
                            result.Append("\tDECLARE @ERRORCODE varchar(200)\r\n");
                            result.Append("\tDECLARE @RETRYCOUNT smallint\r\n");
                            result.Append("\tDECLARE @ENTEREDDATETIME datetime\r\n");
                            result.Append("\tDECLARE @INSTANCEID int\r\n");
                            result.Append("\tDECLARE @PROCESSINGID int\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\tDECLARE @KEY{keyIndex}NAME varchar({hostMaxNumberOfCharacters})\r\n");
                                result.Append($"\tDECLARE @KEY{keyIndex}VALUE varchar({hostMaxNumberOfCharacters})\r\n");
                            }

                            result.Append("\r\n");
                            result.Append("\tSELECT TOP 1\r\n");
                            result.Append("\t\t@ID = D.[ID],\r\n");
                            result.Append("\t\t@HOSTID = D.[HOSTID],\r\n");
                            result.Append("\t\t@DATABASENAME = D.[DATABASENAME],\r\n");
                            result.Append("\t\t@TABLENAME = D.[TABLENAME],\r\n");
                            result.Append("\t\t@ACTIONCODE = D.[ACTIONCODE],\r\n");
                            result.Append("\t\t@ERRORCODE = D.[ERRORCODE],\r\n");
                            result.Append("\t\t@RETRYCOUNT = D.[RETRYCOUNT],\r\n");
                            result.Append("\t\t@ENTEREDDATETIME = D.[ENTEREDDATETIME],\r\n");
                            result.Append("\t\t@INSTANCEID = D.[INSTANCEID],\r\n");
                            result.Append("\t\t@PROCESSINGID = D.[PROCESSINGID],\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\t\t@KEY{keyIndex}NAME = D.[KEY{keyIndex}NAME],\r\n");
                                result.Append(keyIndex < hostMaxNumberOfKeys ? $"\t\t@KEY{keyIndex}VALUE = D.[KEY{keyIndex}VALUE],\r\n" : $"\t\t@KEY{keyIndex}VALUE = D.[KEY{keyIndex}VALUE]\r\n");
                            }

                            result.Append("\tFROM DELETED AS D\r\n");
                            result.Append("\r\n");
                            result.Append($"\tINSERT INTO [dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG]\r\n");
                            result.Append("\t\t(\r\n");
                            result.Append("\t\t\tID,\r\n");
                            result.Append("\t\t\tHOSTID,\r\n");
                            result.Append("\t\t\tDATABASENAME,\r\n");
                            result.Append("\t\t\tTABLENAME,\r\n");
                            result.Append("\t\t\tACTIONCODE,\r\n");
                            result.Append("\t\t\tERRORCODE,\r\n");
                            result.Append("\t\t\tRETRYCOUNT,\r\n");
                            result.Append("\t\t\tENTEREDDATETIME,\r\n");
                            result.Append("\t\t\tPROCESSEDDATETIME,\r\n");
                            result.Append("\t\t\tINSTANCEID,\r\n");
                            result.Append("\t\t\tPROCESSINGID,\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\t\t\tKEY{keyIndex}NAME,\r\n");
                                result.Append(keyIndex < hostMaxNumberOfKeys ? $"\t\t\tKEY{keyIndex}VALUE,\r\n" : $"\t\t\tKEY{keyIndex}VALUE\r\n");
                            }

                            result.Append("\t\t)\r\n");
                            result.Append("\r\n");
                            result.Append("\tVALUES\r\n");
                            result.Append("\t\t(\r\n");
                            result.Append("\t\t\t@ID,\r\n");
                            result.Append("\t\t\t@HOSTID,\r\n");
                            result.Append("\t\t\t@DATABASENAME,\r\n");
                            result.Append("\t\t\t@TABLENAME,\r\n");
                            result.Append("\t\t\t@ACTIONCODE,\r\n");
                            result.Append("\t\t\t@ERRORCODE,\r\n");
                            result.Append("\t\t\t@RETRYCOUNT,\r\n");
                            result.Append("\t\t\t@ENTEREDDATETIME,\r\n");
                            result.Append("\t\t\tGETDATE(),\r\n");
                            result.Append("\t\t\t@INSTANCEID,\r\n");
                            result.Append("\t\t\t@PROCESSINGID,\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\t\t\t@KEY{keyIndex}NAME,\r\n");
                                result.Append(keyIndex < hostMaxNumberOfKeys ? $"\t\t\t@KEY{keyIndex}VALUE,\r\n" : $"\t\t\t@KEY{keyIndex}VALUE\r\n");
                            }

                            result.Append("\t\t)\r\n");
                            result.Append("\r\n");
                            result.Append("END\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                        }

                        if (bidirectionalSyncMode)
                        {
                            // Create BOUNCEQUEUE table.

                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BOUNCEQUEUE]') AND type in (N'U'))\r\n");
                            result.Append("\tDROP TABLE [dbo].[BOUNCEQUEUE]\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append("SET ANSI_NULLS ON\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append("SET QUOTED_IDENTIFIER ON\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                            result.Append("IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BOUNCEQUEUE]') AND type in (N'U'))\r\n");
                            result.Append("BEGIN\r\n");
                            result.Append("CREATE TABLE [dbo].[BOUNCEQUEUE](\r\n");
                            result.Append(autoGeneratedId ? "\t[ID] [int] IDENTITY(1,1) NOT NULL,\r\n" : "\t[ID] [int] NOT NULL DEFAULT (0),\r\n");
                            result.Append("\t[HOSTID] [varchar](20) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                            result.Append("\t[GUESTNAME] [varchar](200) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                            result.Append($"\t[DATABASENAME] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL,\r\n");
                            result.Append($"\t[TABLENAME] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_BOUNCEQUEUE_TABLENAME] DEFAULT ((0)),\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\t[KEY{keyIndex}NAME] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_BOUNCEQUEUE_KEY{keyIndex}NAME] DEFAULT (''),\r\n");
                                result.Append($"\t[KEY{keyIndex}VALUE] [varchar]({hostMaxNumberOfCharacters}) COLLATE Sql_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_BOUNCEQUEUE_KEY{keyIndex}VALUE] DEFAULT (''),\r\n");
                            }

                            result.Append("\tCONSTRAINT [PK_BOUNCEQUEUE] PRIMARY KEY CLUSTERED\r\n");
                            result.Append("\t(\r\n");
                            result.Append("\t\t[ID] ASC\r\n");
                            result.Append("\t) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]\r\n");
                            result.Append(") ON [PRIMARY]\r\n");
                            result.Append("END\r\n");
                            result.Append("\r\n");
                            result.Append("GO\r\n");
                            result.Append("\r\n");
                        }
                    }

                    else if (AnDatabaseTypes.IsIbmDb2Driver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)} ;\r\n");
                        result.Append("\r\n");
                        result.Append($"CREATE TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)} (\r\n");

                        if (autoGeneratedId)
                        {
                            result.Append("\tID INTEGER NOT NULL\r\n");
                            result.Append("\t\tGENERATED ALWAYS AS IDENTITY (\r\n");
                            result.Append("\t\t\tSTART WITH 1 INCREMENT BY 1\r\n");
                            result.Append("\t\t\tCACHE 20 NO ORDER),\r\n");
                        }

                        else
                        {
                            result.Append("\tID INTEGER NOT NULL DEFAULT 0, \r\n");
                        }

                        result.Append("\tHOSTID VARCHAR(20) DEFAULT '' NOT NULL,\r\n");
                        result.Append($"\tDATABASENAME VARCHAR({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                        result.Append($"\tTABLENAME VARCHAR({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                        result.Append("\tACTIONCODE VARCHAR(10) DEFAULT '' NOT NULL,\r\n");
                        result.Append("\tERRORCODE VARCHAR(200) DEFAULT '' NOT NULL,\r\n");
                        result.Append("\tRETRYCOUNT INTEGER NOT NULL DEFAULT 0, \r\n");
                        result.Append("\tENTEREDDATETIME TIMESTAMP NOT NULL,\r\n");
                        result.Append("\tINSTANCEID INTEGER NOT NULL,\r\n");
                        result.Append("\tPROCESSINGID INTEGER NOT NULL,\r\n");

                        for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                        {
                            result.Append($"\tKEY{keyIndex}NAME VARCHAR({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                            result.Append($"\tKEY{keyIndex}VALUE VARCHAR({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                        }

                        if (autoGeneratedId)
                        {
                            result.Append($"\tCONSTRAINT {AnTypes.FormatDataChangesTableName(dataChangesTableName)}_PK PRIMARY KEY (ID)\r\n");
                        }

                        else
                        {
                            var tempResult = result.ToString();
                            tempResult = tempResult.TrimEnd('\n');
                            tempResult = tempResult.TrimEnd('\r');
                            result = new StringBuilder(tempResult.TrimEnd(','));
                        }

                        result.Append(");\r\n");
                        result.Append("\r\n");

                        if (logDataChangesRecords)
                        {
                            result.Append("\r\n");
                            result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG ;\r\n");
                            result.Append("\r\n");
                            result.Append($"CREATE TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG (\r\n");
                            result.Append("\tID INTEGER NOT NULL DEFAULT 0, \r\n");
                            result.Append("\tHOSTID VARCHAR(20) DEFAULT '' NOT NULL,\r\n");
                            result.Append($"\tDATABASENAME VARCHAR({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                            result.Append($"\tTABLENAME VARCHAR({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                            result.Append("\tACTIONCODE VARCHAR(10) DEFAULT '' NOT NULL,\r\n");
                            result.Append("\tERRORCODE VARCHAR(200) DEFAULT '' NOT NULL,\r\n");
                            result.Append("\tRETRYCOUNT INTEGER NOT NULL DEFAULT 0, \r\n");
                            result.Append("\tENTEREDDATETIME TIMESTAMP NOT NULL,\r\n");
                            result.Append("\tINSTANCEID INTEGER NOT NULL,\r\n");
                            result.Append("\tPROCESSINGID INTEGER NOT NULL,\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\tKEY{keyIndex}NAME VARCHAR({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                                result.Append($"\tKEY{keyIndex}VALUE VARCHAR({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                            }

                            var tempResult = result.ToString();
                            tempResult = tempResult.TrimEnd('\n');
                            tempResult = tempResult.TrimEnd('\r');
                            result = new StringBuilder(tempResult.TrimEnd(','));

                            result.Append("\r\n);\r\n");
                            result.Append("\r\n");
                        }
                    }

                    else if (AnDatabaseTypes.IsOracleDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");

                        if (autoGeneratedId)
                        {
                            result.Append($"DROP SEQUENCE \"{AnTypes.FormatDataChangesTableName(dataChangesTableName)}_SEQ\"\r\n");
                            result.Append("/\r\n");
                            result.Append("\r\n");
                        }

                        result.Append($"CREATE TABLE \"{AnTypes.FormatDataChangesTableName(dataChangesTableName)}\"\r\n");
                        result.Append("(\r\n");
                        result.Append(autoGeneratedId ? "\t\"ID\" NUMBER,\r\n" : "\t\"ID\" NUMBER DEFAULT 0,\r\n");
                        result.Append("\t\"HOSTID\" VARCHAR2(20) DEFAULT '' NOT NULL,\r\n");
                        result.Append($"\t\"DATABASENAME\" VARCHAR2({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                        result.Append($"\t\"TABLENAME\" VARCHAR2({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                        result.Append("\t\"ACTIONCODE\" VARCHAR2(10) DEFAULT '' NOT NULL,\r\n");
                        result.Append("\t\"ERRORCODE\" VARCHAR2(200) DEFAULT '' NULL,\r\n");
                        result.Append("\t\"RETRYCOUNT\" NUMBER DEFAULT 0,\r\n");
                        result.Append("\t\"ENTEREDDATETIME\" DATE NOT NULL,\r\n");
                        result.Append("\t\"INSTANCEID\" NUMBER,\r\n");
                        result.Append("\t\"PROCESSINGID\" NUMBER,\r\n");

                        for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                        {
                            result.Append($"\t\"KEY{keyIndex}NAME\" VARCHAR2({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                            result.Append($"\t\"KEY{keyIndex}VALUE\" VARCHAR2({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                        }

                        if (autoGeneratedId)
                        {
                            result.Append($"\tCONSTRAINT \"{AnTypes.FormatDataChangesTableName(dataChangesTableName)}_PK\" PRIMARY KEY (\"ID\") ENABLE\r\n");
                        }

                        else
                        {
                            var tempResult = result.ToString();
                            tempResult = tempResult.TrimEnd('\n');
                            tempResult = tempResult.TrimEnd('\r');
                            result = new StringBuilder(tempResult.TrimEnd(','));
                        }

                        result.Append(")\r\n");
                        result.Append("\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");

                        if (autoGeneratedId)
                        {
                            result.Append($"CREATE SEQUENCE \"{AnTypes.FormatDataChangesTableName(dataChangesTableName)}_SEQ\" MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 11901 CACHE 20 NOORDER NOCYCLE\r\n");
                            result.Append("\r\n");
                            result.Append("/\r\n");
                            result.Append("\r\n");
                            result.Append($"CREATE OR REPLACE TRIGGER \"BI_{AnTypes.FormatDataChangesTableName(dataChangesTableName)}\"\r\n");
                            result.Append($"\tBEFORE INSERT ON \"{AnTypes.FormatDataChangesTableName(dataChangesTableName)}\"\r\n");
                            result.Append("\tFOR EACH ROW\r\n");
                            result.Append("BEGIN\r\n");
                            result.Append($"\tSELECT \"{AnTypes.FormatDataChangesTableName(dataChangesTableName)}_SEQ\".nextval INTO :NEW.ID FROM dual;\r\n");
                            result.Append("END;\r\n");
                            result.Append("\r\n");
                            result.Append("/\r\n");
                            result.Append("\r\n");
                            result.Append($"ALTER TRIGGER \"BI_{AnTypes.FormatDataChangesTableName(dataChangesTableName)}\" ENABLE\r\n");
                            result.Append("\r\n");
                            result.Append("/\r\n");
                            result.Append("\r\n");
                        }

                        if (logDataChangesRecords)
                        {
                            result.Append("\r\n");
                            result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG\r\n");
                            result.Append("/\r\n");
                            result.Append("\r\n");
                            result.Append($"CREATE TABLE \"{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG\"\r\n");
                            result.Append("(\r\n");
                            result.Append("\t\"ID\" NUMBER DEFAULT 0,\r\n");
                            result.Append("\t\"HOSTID\" VARCHAR2(20) DEFAULT '' NOT NULL,\r\n");
                            result.Append($"\t\"DATABASENAME\" VARCHAR2({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                            result.Append($"\t\"TABLENAME\" VARCHAR2({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                            result.Append("\t\"ACTIONCODE\" VARCHAR2(10) DEFAULT '' NOT NULL,\r\n");
                            result.Append("\t\"ERRORCODE\" VARCHAR2(200) DEFAULT '' NOT NULL,\r\n");
                            result.Append("\t\"RETRYCOUNT\" NUMBER DEFAULT 0,\r\n");
                            result.Append($"\t\"ENTEREDDATETIME\" VARCHAR2({hostMaxNumberOfCharacters}),\r\n");
                            result.Append("\t\"INSTANCEID\" NUMBER,\r\n");
                            result.Append("\t\"PROCESSINGID\" NUMBER,\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\t\"KEY{keyIndex}NAME\" VARCHAR2({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                                result.Append($"\t\"KEY{keyIndex}VALUE\" VARCHAR2({hostMaxNumberOfCharacters}) DEFAULT '' NOT NULL,\r\n");
                            }

                            var tempResult = result.ToString();
                            tempResult = tempResult.TrimEnd('\n');
                            tempResult = tempResult.TrimEnd('\r');
                            result = new StringBuilder(tempResult.TrimEnd(','));
                                    
                            result.Append("\r\n)\r\n");
                            result.Append("\r\n");
                            result.Append("/\r\n");
                            result.Append("\r\n");
                        }
                    }

                    else if (AnDatabaseTypes.IsPervasiveDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");
                        result.Append($"CREATE TABLE \"{AnTypes.FormatDataChangesTableName(dataChangesTableName)}\"(\r\n");
                        result.Append(autoGeneratedId ? "\t\"ID\" IDENTITY,\r\n" : "\t\"ID\" INTEGER DEFAULT '0',\r\n");
                        result.Append("\t\"HOSTID\" VARCHAR(20) NOT NULL  CASE ,\r\n");
                        result.Append($"\t\"DATABASENAME\" VARCHAR({hostMaxNumberOfCharacters}) NOT NULL  CASE ,\r\n");
                        result.Append($"\t\"TABLENAME\" VARCHAR({hostMaxNumberOfCharacters}) NOT NULL  CASE ,\r\n");
                        result.Append("\t\"ACTIONCODE\" VARCHAR(10) NOT NULL  CASE ,\r\n");
                        result.Append("\t\"ERRORCODE\" VARCHAR(200) NOT NULL  CASE ,\r\n");
                        result.Append("\t\"RETRYCOUNT\" SMALLINTEGER DEFAULT '0',\r\n");
                        result.Append("\t\"ENTEREDDATETIME\" DATETIME,\r\n");
                        result.Append("\t\"INSTANCEID\" INTEGER NOT NULL,\r\n");
                        result.Append("\t\"PROCESSINGID\" INTEGER NOT NULL,\r\n");

                        for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                        {
                            result.Append($"\t\"KEY{keyIndex}NAME\" VARCHAR({hostMaxNumberOfCharacters}) NOT NULL  CASE ,\r\n");
                            result.Append($"\t\"KEY{keyIndex}VALUE\" VARCHAR({hostMaxNumberOfCharacters}) NOT NULL  CASE ,\r\n");
                        }

                        if (autoGeneratedId)
                        {
                            result.Append("\tPRIMARY KEY (\"ID\")\r\n");
                        }

                        else
                        {
                            var tempResult = result.ToString();
                            tempResult = tempResult.TrimEnd('\n');
                            tempResult = tempResult.TrimEnd('\r');
                            result = new StringBuilder(tempResult.TrimEnd(','));
                        }

                        result.Append(");\r\n");

                        if (autoGeneratedId)
                        {
                            result.Append($"CREATE UNIQUE INDEX \"PK_ID\" ON \"{AnTypes.FormatDataChangesTableName(dataChangesTableName)}\"(\"ID\");\r\n");
                            result.Append($"CREATE UNIQUE INDEX \"UK_ID\" ON \"{AnTypes.FormatDataChangesTableName(dataChangesTableName)}\"(\"ID\");\r\n");
                            result.Append("\r\n");
                            result.Append("/\r\n");
                        }

                        result.Append("\r\n");

                        if (logDataChangesRecords)
                        {
                            result.Append("\r\n");
                            result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG\r\n");
                            result.Append("/\r\n");
                            result.Append("\r\n");
                            result.Append($"CREATE TABLE \"{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG\"(\r\n");
                            result.Append("\t\"ID\" INTEGER DEFAULT '0',\r\n");
                            result.Append("\t\"HOSTID\" VARCHAR(20) NOT NULL  CASE ,\r\n");
                            result.Append($"\t\"DATABASENAME\" VARCHAR({hostMaxNumberOfCharacters}) NOT NULL  CASE ,\r\n");
                            result.Append($"\t\"TABLENAME\" VARCHAR({hostMaxNumberOfCharacters}) NOT NULL  CASE ,\r\n");
                            result.Append("\t\"ACTIONCODE\" VARCHAR(10) NOT NULL  CASE ,\r\n");
                            result.Append("\t\"ERRORCODE\" VARCHAR(200) NOT NULL  CASE ,\r\n");
                            result.Append("\t\"RETRYCOUNT\" SMALLINTEGER DEFAULT '0',\r\n");
                            result.Append("\t\"ENTEREDDATETIME\" DATETIME,\r\n");
                            result.Append("\t\"INSTANCEID\" INTEGER NOT NULL,\r\n");
                            result.Append("\t\"PROCESSINGID\" INTEGER NOT NULL,\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\t\"KEY{keyIndex}NAME\" VARCHAR({hostMaxNumberOfCharacters}) NOT NULL  CASE ,\r\n");
                                result.Append($"\t\"KEY{keyIndex}VALUE\" VARCHAR({hostMaxNumberOfCharacters}) NOT NULL  CASE ,\r\n");
                            }

                            var tempResult = result.ToString();
                            tempResult = tempResult.TrimEnd('\n');
                            tempResult = tempResult.TrimEnd('\r');
                            result = new StringBuilder(tempResult.TrimEnd(','));
                                    
                            result.Append("\r\n);\r\n");
                            result.Append("\r\n");
                        }
                    }

                    else if (AnDatabaseTypes.IsMySqlDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append("SET FOREIGN_KEY_CHECKS=0;\r\n");
                        result.Append("\r\n");
                        result.Append("-- Seperator\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP TABLE IF EXISTS `{AnTypes.FormatDataChangesTableName(dataChangesTableName)}`;\r\n");
                        result.Append("\r\n");
                        result.Append("-- Seperator\r\n");
                        result.Append("\r\n");
                        result.Append($"CREATE TABLE `{AnTypes.FormatDataChangesTableName(dataChangesTableName)}` (\r\n");
                        result.Append(autoGeneratedId ? "\t`ID` int(11) NOT NULL AUTO_INCREMENT,\r\n" : "\t`ID` int(11) NOT NULL DEFAULT 0,\r\n");
                        result.Append("\t`HOSTID` varchar(20) NOT NULL DEFAULT '',\r\n");
                        result.Append($"\t`DATABASENAME` varchar({hostMaxNumberOfCharacters}) NOT NULL DEFAULT '',\r\n");
                        result.Append($"\t`TABLENAME` varchar({hostMaxNumberOfCharacters}) NOT NULL DEFAULT '',\r\n");
                        result.Append("\t`ACTIONCODE` varchar(10) NOT NULL DEFAULT '',\r\n");
                        result.Append("\t`ERRORCODE` varchar(200) NOT NULL DEFAULT '',\r\n");
                        result.Append("\t`RETRYCOUNT` int(8) NOT NULL DEFAULT 0,\r\n");
                        result.Append("\t`ENTEREDDATETIME` datetime DEFAULT NULL,\r\n");
                        result.Append("\t`INSTANCEID` int(11) NOT NULL,\r\n");
                        result.Append("\t`PROCESSINGID` int(11) NOT NULL,\r\n");

                        for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                        {
                            result.Append($"\t`KEY{keyIndex}NAME` varchar({hostMaxNumberOfCharacters}) NOT NULL DEFAULT '',\r\n");
                            result.Append($"\t`KEY{keyIndex}VALUE` varchar({hostMaxNumberOfCharacters}) NOT NULL DEFAULT '',\r\n");
                        }

                        if (autoGeneratedId)
                        {
                            result.Append("\tPRIMARY KEY (`ID`)\r\n");
                        }

                        else
                        {
                            var tempResult = result.ToString();
                            tempResult = tempResult.TrimEnd('\n');
                            tempResult = tempResult.TrimEnd('\r');
                            result = new StringBuilder(tempResult.TrimEnd(','));
                        }

                        result.Append(") ENGINE=InnoDB DEFAULT CHARSET=latin1;\r\n");
                        result.Append("\r\n");

                        if (logDataChangesRecords)
                        {
                            result.Append("\r\n");
                            result.Append("SET FOREIGN_KEY_CHECKS=0;\r\n");
                            result.Append("\r\n");
                            result.Append("-- Seperator\r\n");
                            result.Append("\r\n");
                            result.Append($"DROP TABLE IF EXISTS `{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG`;\r\n");
                            result.Append("\r\n");
                            result.Append("-- Seperator\r\n");
                            result.Append("\r\n");
                            result.Append($"CREATE TABLE `{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG` (\r\n");
                            result.Append("\t`ID` int(11) NOT NULL DEFAULT 0,\r\n");
                            result.Append("\t`HOSTID` varchar(20) NOT NULL DEFAULT '',\r\n");
                            result.Append($"\t`DATABASENAME` varchar({hostMaxNumberOfCharacters}) NOT NULL DEFAULT '',\r\n");
                            result.Append($"\t`TABLENAME` varchar({hostMaxNumberOfCharacters}) NOT NULL DEFAULT '',\r\n");
                            result.Append("\t`ACTIONCODE` varchar(10) NOT NULL DEFAULT '',\r\n");
                            result.Append("\t`ERRORCODE` varchar(200) NOT NULL DEFAULT '',\r\n");
                            result.Append("\t`RETRYCOUNT` int(8) NOT NULL,\r\n");
                            result.Append("\t`ENTEREDDATETIME` datetime DEFAULT NULL,\r\n");
                            result.Append("\t`INSTANCEID` int(11) NOT NULL,\r\n");
                            result.Append("\t`PROCESSINGID` int(11) NOT NULL,\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\t`KEY{keyIndex}NAME` varchar({hostMaxNumberOfCharacters}) NOT NULL DEFAULT '',\r\n");
                                result.Append($"\t`KEY{keyIndex}VALUE` varchar({hostMaxNumberOfCharacters}) NOT NULL DEFAULT '',\r\n");
                            }

                            var tempResult = result.ToString();
                            tempResult = tempResult.TrimEnd('\n');
                            tempResult = tempResult.TrimEnd('\r');
                            result = new StringBuilder(tempResult.TrimEnd(','));
                                    
                            result.Append("\r\n) ENGINE=InnoDB DEFAULT CHARSET=latin1;\r\n");
                            result.Append("\r\n");
                        }
                    }

                    else if (AnDatabaseTypes.IsPostgresDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)};\r\n");
                        result.Append("\r\n");
                        result.Append("-- Seperator\r\n");
                        result.Append("\r\n");
                        result.Append($"CREATE TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
                        result.Append("(\r\n");
                        result.Append(autoGeneratedId ? "\tID serial NOT NULL, \r\n" : "\tID int NOT NULL, \r\n");
                        result.Append("\tHOSTID character varying(20) NOT NULL, \r\n");
                        result.Append($"\tDATABASENAME character varying({hostMaxNumberOfCharacters}) NOT NULL,\r\n");
                        result.Append($"\tTABLENAME character varying({hostMaxNumberOfCharacters}) NOT NULL,\r\n");
                        result.Append("\tACTIONCODE character varying(10) NOT NULL,\r\n");
                        result.Append("\tERRORCODE character varying(200) NOT NULL,\r\n");
                        result.Append("\tRETRYCOUNT smallint NOT NULL,\r\n");
                        result.Append("\tENTEREDDATETIME timestamp without time zone NOT NULL, \r\n");
                        result.Append("\tINSTANCEID smallint NOT NULL,\r\n");
                        result.Append("\tPROCESSINGID smallint NOT NULL,\r\n");

                        for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                        {
                            result.Append($"\tKEY{keyIndex}NAME character varying({hostMaxNumberOfCharacters}) NOT NULL,\r\n");
                            result.Append($"\tKEY{keyIndex}VALUE character varying({hostMaxNumberOfCharacters}) NOT NULL,\r\n");
                        }

                        if (autoGeneratedId)
                        {
                            result.Append("\tPRIMARY KEY (ID)\r\n");
                        }

                        else
                        {
                            var tempResult = result.ToString();
                            tempResult = tempResult.TrimEnd('\n');
                            tempResult = tempResult.TrimEnd('\r');
                            result = new StringBuilder(tempResult.TrimEnd(','));
                        }

                        result.Append(") WITH (OIDS=FALSE)\r\n");
                        result.Append(";\r\n");
                        result.Append("\r\n");

                        if (logDataChangesRecords)
                        {
                            result.Append("\r\n");
                            result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG;\r\n");
                            result.Append("\r\n");
                            result.Append("-- Seperator\r\n");
                            result.Append("\r\n");
                            result.Append($"CREATE TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG\r\n");
                            result.Append("(\r\n");
                            result.Append("\tID int NOT NULL, \r\n");
                            result.Append("\tHOSTID character varying(20) NOT NULL, \r\n");
                            result.Append($"\tDATABASENAME character varying({hostMaxNumberOfCharacters}) NOT NULL,\r\n");
                            result.Append($"\tTABLENAME character varying({hostMaxNumberOfCharacters}) NOT NULL,\r\n");
                            result.Append("\tACTIONCODE character varying(10) NOT NULL,\r\n");
                            result.Append("\tERRORCODE character varying(200) NOT NULL,\r\n");
                            result.Append("\tRETRYCOUNT smallint NOT NULL,\r\n");
                            result.Append("\tENTEREDDATETIME timestamp without time zone NOT NULL, \r\n");
                            result.Append("\tINSTANCEID smallint NOT NULL,\r\n");
                            result.Append("\tPROCESSINGID smallint NOT NULL,\r\n");

                            for (var keyIndex = 1; keyIndex <= hostMaxNumberOfKeys; keyIndex++)
                            {
                                result.Append($"\tKEY{keyIndex}NAME character varying({hostMaxNumberOfCharacters}) NOT NULL,\r\n");
                                result.Append($"\tKEY{keyIndex}VALUE character varying({hostMaxNumberOfCharacters}) NOT NULL,\r\n");
                            }

                            var tempResult = result.ToString();
                            tempResult = tempResult.TrimEnd('\n');
                            tempResult = tempResult.TrimEnd('\r');
                            result = new StringBuilder(tempResult.TrimEnd(','));

                            result.Append("\r\n) WITH (OIDS=FALSE)\r\n");
                            result.Append(";\r\n");
                            result.Append("\r\n");
                        }
                    }

	                else if (AnDatabaseTypes.IsSqLiteDriver(driverType))
	                {
		                // Nothing for now.
	                }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateClearDataChangesTablesScript(
            string dataChangesTableName,
            AnDriverProperties dataChangesDriverProperties)
        {
            var result = new StringBuilder();

            try
            {
                if (dataChangesDriverProperties != null)
                {
                    var driverType = AnDatabaseTypes.DriverTypeFromString(dataChangesDriverProperties.GetStringSettingValue(AnDriverProperties.AdapterTypeKey));

                    if (AnDatabaseTypes.IsMsSqlServerDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append($"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}]') AND type in (N'U'))\r\n");
                        result.Append($"\tDROP TABLE [dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}]\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append("\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append($"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG]') AND type in (N'U'))\r\n");
                        result.Append($"\tDROP TABLE [dbo].[{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG]\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append("\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                        result.Append("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BOUNCEQUEUE]') AND type in (N'U'))\r\n");
                        result.Append("\tDROP TABLE [dbo].[BOUNCEQUEUE]\r\n");
                        result.Append("GO\r\n");
                        result.Append("\r\n");
                    }

                    else if (AnDatabaseTypes.IsIbmDb2Driver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)};\r\n");
                        result.Append("\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG;\r\n");
                        result.Append("\r\n");
                    }

                    else if (AnDatabaseTypes.IsOracleDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP SEQUENCE \"{AnTypes.FormatDataChangesTableName(dataChangesTableName)}_SEQ\"\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");
                    }

                    else if (AnDatabaseTypes.IsPervasiveDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)}\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");
                    }

                    else if (AnDatabaseTypes.IsMySqlDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append("SET FOREIGN_KEY_CHECKS=0;\r\n");
                        result.Append("\r\n");
                        result.Append("-- Seperator\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP TABLE IF EXISTS `{AnTypes.FormatDataChangesTableName(dataChangesTableName)}`;\r\n");
                        result.Append("\r\n");
                        result.Append("\r\n");
                        result.Append("SET FOREIGN_KEY_CHECKS=0;\r\n");
                        result.Append("\r\n");
                        result.Append("-- Seperator\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP TABLE IF EXISTS `{AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG`;\r\n");
                        result.Append("\r\n");
                    }

                    else if (AnDatabaseTypes.IsPostgresDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)};\r\n");
                        result.Append("\r\n");
                        result.Append("\r\n");
                        result.Append($"DROP TABLE {AnTypes.FormatDataChangesTableName(dataChangesTableName)}LOG;\r\n");
                        result.Append("\r\n");
                    }

	                else if (AnDatabaseTypes.IsSqLiteDriver(driverType))
	                {
		                // Nothing for now.
	                }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateCreateLogTableScript(AnColumnObjectList columnObjectList, AnDatabaseTypes.DriverType driverType, bool createDropTableScripts, out string errorMessage)
        {
            var result = new StringBuilder();

            errorMessage = "";

            try
            {
                if (columnObjectList != null)
                {
                    if (!string.IsNullOrEmpty(columnObjectList.Name))
                    {
                        result.Append(GenerateSqlTableScript(columnObjectList, driverType, createDropTableScripts));
                    }

                    else
                    {
                        errorMessage = "Invalid table name.";
                    }
                }

                else
                {
                    errorMessage = "Invalid table definition.";
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateClearLogTableScript(string tableName, AnDatabaseTypes.DriverType driverType, out string errorMessage)
        {
            var result = new StringBuilder();

            errorMessage = "";

            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    result.Append(GenerateSqlClearTableScript(tableName, driverType));
                }

                else
                {
                    errorMessage = "Invalid table name.";
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateSqlTableSequenceScript(
            AnColumnObjectList columnObjectList,
            AnDatabaseTypes.DriverType driverType,
            bool after)
        {
            var result = new StringBuilder();

            try
            {
                if ((columnObjectList != null) &&
                    (columnObjectList.IsATable) && 
                    !string.IsNullOrEmpty(columnObjectList.Name) && 
                    after)
                {
                    if (AnDatabaseTypes.IsOracleDriver(driverType))
                    {
                        result.Append("\r\n");
                        result.Append($"CREATE SEQUENCE \"{columnObjectList.Name}_SEQ\" MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 11901 CACHE 20 NOORDER NOCYCLE\r\n");
                        result.Append("\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");
                        result.Append($"CREATE OR REPLACE TRIGGER \"BI_{columnObjectList.Name}\"\r\n");
                        result.Append($"\tBEFORE INSERT ON \"{columnObjectList.Name}\"\r\n");
                        result.Append("\tFOR EACH ROW\r\n");
                        result.Append("BEGIN\r\n");
                        result.Append($"\tSELECT \"{columnObjectList.Name}_SEQ\".nextval INTO :NEW.{columnObjectList.AutoGeneratedColumnName} FROM dual;\r\n");
                        result.Append("END;\r\n");
                        result.Append("\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");
                        result.Append($"ALTER TRIGGER \"BI_{columnObjectList.Name}\" ENABLE\r\n");
                        result.Append("\r\n");
                        result.Append("/\r\n");
                        result.Append("\r\n");
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateSqlTableScripts(
            string tableDefinitionText, 
            AnDatabaseTypes.DriverType driverType,
            bool createDropTableScripts = true)
        {
            var result = new StringBuilder();

            try
            {
	            if (!string.IsNullOrEmpty(tableDefinitionText))
	            {
	                var cacheList = new AnCacheList("TableDefParser");
                    cacheList.LoadCachesAndJoinsFromFileText(tableDefinitionText);

                    result.Append(GenerateSqlTableScripts(cacheList.GetCopyOfColumnObjectLists(), driverType, createDropTableScripts));
	            }
	        }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateSqlTableScripts(
            AnTableObjectList tableObjectList, 
            AnDatabaseTypes.DriverType driverType,
            bool createDropTableScripts = true)
        {
            var result = new StringBuilder();

            try
            {
                if (tableObjectList != null)
	            {
                    foreach (var tableObject in tableObjectList.Where(tableObject => tableObject != null).Where(tableObject => tableObject.ColumnObjectList.IsATable))
                    {
                        if (!string.IsNullOrEmpty(tableObject.Name) &&
                            (tableObject.Name != tableObject.ColumnObjectList.Name))
                        {
                            tableObject.ColumnObjectList.Name = tableObject.Name;
                        }

                        if (tableObject.ColumnObjectList.HasAutoGeneratedColumn)
                        {
                            result.Append(GenerateSqlTableSequenceScript(tableObject.ColumnObjectList, driverType, false));
                        }

                        result.Append(GenerateSqlTableScript(tableObject.ColumnObjectList, driverType, createDropTableScripts));

                        if (tableObject.ColumnObjectList.HasAutoGeneratedColumn)
                        {
                            result.Append(GenerateSqlTableSequenceScript(tableObject.ColumnObjectList, driverType, true));
                        }
                    }
	            }
	        }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateSqlTableScripts(
            AnColumnObjectListManager columnObjectListManager,
            AnDatabaseTypes.DriverType driverType,
            bool createDropTableScripts = true)
        {
            var result = new StringBuilder();

            try
            {
                if (columnObjectListManager != null)
                {
                    foreach (var columnObjectList in columnObjectListManager.Where(columnObjectList => columnObjectList != null).Where(columnObjectList => columnObjectList.IsATable))
                    {
                        if (columnObjectList.HasAutoGeneratedColumn)
                        {
                            result.Append(GenerateSqlTableSequenceScript(columnObjectList, driverType, false));
                        }

                        result.Append(GenerateSqlTableScript(columnObjectList, driverType, createDropTableScripts));

                        if (columnObjectList.HasAutoGeneratedColumn)
                        {
                            result.Append(GenerateSqlTableSequenceScript(columnObjectList, driverType, true));
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateSqlTableScripts(
            string databaseName, 
            AnTableObjectList tableObjectList,
            AnDatabaseTypes.DriverType driverType,
            bool createDropTableScripts = true)
        {
            var result = new StringBuilder();

            try
            {
                if (AnDatabaseTypes.IsMsSqlServerDriver(driverType))
                {
			        if (!string.IsNullOrEmpty(databaseName))
			        {
			            result.Append("\r\nUSE [");
			            result.Append(databaseName);
			            result.Append("]\r\nGO\r\n");
			            result.Append("\r\n");
			        }
			    }

                result.Append(GenerateSqlTableScripts(tableObjectList, driverType, createDropTableScripts));
	        }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateSqlTableScript(
            AnColumnObjectList columnObjectList, 
            AnDatabaseTypes.DriverType driverType,
            bool createDropTableScripts = true)
        {
            var result = new StringBuilder();

            try
            {
	            if ((columnObjectList != null) &&
                    (columnObjectList.IsATable))
	            {
		            if (!string.IsNullOrEmpty(columnObjectList.Name))
		            {
                        if (AnDatabaseTypes.IsMsSqlServerDriver(driverType))
		                {
		                    result.Append(GenerateSqlServerTableScript(columnObjectList, driverType, createDropTableScripts));
		                }
		                
                        else if (AnDatabaseTypes.IsIbmDb2Driver(driverType))
		                {
                            result.Append(GenerateIbmDb2TableScript(columnObjectList, driverType));
		                }
		                
                        else if (AnDatabaseTypes.IsOracleDriver(driverType))
		                {
                            result.Append(GenerateOracleTableScript(columnObjectList, driverType));
		                }
		                
                        else if (AnDatabaseTypes.IsPervasiveDriver(driverType))
		                {
                            result.Append(GeneratePervasiveTableScript(columnObjectList, driverType));
		                }
		                
                        else if (AnDatabaseTypes.IsMySqlDriver(driverType))
		                {
                            result.Append(GenerateMySqlTableScript(columnObjectList, driverType));
		                }
		                
                        else if (AnDatabaseTypes.IsPostgresDriver(driverType))
		                {
                            result.Append(GeneratePostgresTableScript(columnObjectList, driverType));
		                }

			            else if (AnDatabaseTypes.IsSqLiteDriver(driverType))
			            {
				            result.Append(GenerateSqLiteTableScript(columnObjectList, driverType));
			            }
		            }
	            }
	        }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateSqlDropTableScript(
            string tableName,
            AnDatabaseTypes.DriverType driverType)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    if (AnDatabaseTypes.IsMsSqlServerDriver(driverType))
                    {
                        result.Append(GenerateSqlServerDropTableScript(tableName));
                    }

                    else if (AnDatabaseTypes.IsIbmDb2Driver(driverType))
                    {
                        result.Append(GenerateIbmDb2DropTableScript(tableName));
                    }

                    else if (AnDatabaseTypes.IsOracleDriver(driverType))
                    {
                        result.Append(GenerateOracleDropTableScript(tableName));
                    }

                    else if (AnDatabaseTypes.IsPervasiveDriver(driverType))
                    {
                        result.Append(GeneratePervasiveDropTableScript(tableName));
                    }

                    else if (AnDatabaseTypes.IsMySqlDriver(driverType))
                    {
                        result.Append(GenerateMySqlDropTableScript(tableName));
                    }

                    else if (AnDatabaseTypes.IsPostgresDriver(driverType))
                    {
                        result.Append(GeneratePostgresDropTableScript(tableName));
                    }

	                else if (AnDatabaseTypes.IsSqLiteDriver(driverType))
	                {
		                result.Append(GenerateSqLiteDropTableScript(tableName));
	                }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateSqlClearTableScript(
            string tableName,
            AnDatabaseTypes.DriverType driverType)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    if (AnDatabaseTypes.IsMsSqlServerDriver(driverType))
                    {
                        result.Append(GenerateSqlServerClearTableScript(tableName));
                    }

                    else if (AnDatabaseTypes.IsIbmDb2Driver(driverType))
                    {
                        result.Append(GenerateIbmDb2ClearTableScript(tableName));
                    }

                    else if (AnDatabaseTypes.IsOracleDriver(driverType))
                    {
                        result.Append(GenerateOracleClearTableScript(tableName));
                    }

                    else if (AnDatabaseTypes.IsPervasiveDriver(driverType))
                    {
                        result.Append(GeneratePervasiveClearTableScript(tableName));
                    }

                    else if (AnDatabaseTypes.IsMySqlDriver(driverType))
                    {
                        result.Append(GenerateMySqlClearTableScript(tableName));
                    }

                    else if (AnDatabaseTypes.IsPostgresDriver(driverType))
                    {
                        result.Append(GeneratePostgresClearTableScript(tableName));
                    }

	                else if (AnDatabaseTypes.IsSqLiteDriver(driverType))
	                {
		                result.Append(GenerateSqLiteClearTableScript(tableName));
	                }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateSqlColumnScript(
            AnColumnObject columnObject,
            AnDatabaseTypes.DriverType driverType)
        {
            var result = new StringBuilder();

            try
            {
                if (columnObject != null)
                {
                    if (AnDatabaseTypes.IsMsSqlServerDriver(driverType))
                    {
                        result.Append(GenerateSqlServerColumnScript(columnObject));
                    }

                    else if (AnDatabaseTypes.IsIbmDb2Driver(driverType))
                    {
                        result.Append(GenerateIbmDb2ColumnScript(columnObject));
                    }

                    else if (AnDatabaseTypes.IsOracleDriver(driverType))
                    {
                        result.Append(GenerateOracleColumnScript(columnObject));
                    }

                    else if (AnDatabaseTypes.IsPervasiveDriver(driverType))
                    {
                        result.Append(GeneratePervasiveColumnScript(columnObject));
                    }

                    else if (AnDatabaseTypes.IsMySqlDriver(driverType))
                    {
                        result.Append(GenerateMySqlColumnScript(columnObject));
                    }

                    else if (AnDatabaseTypes.IsPostgresDriver(driverType))
                    {
                        result.Append(GeneratePostgresColumnScript(columnObject));
                    }

	                else if (AnDatabaseTypes.IsSqLiteDriver(driverType))
	                {
		                result.Append(GenerateSqLiteColumnScript(columnObject));
	                }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        public static string GenerateTableScriptsFromXml(string xmlFilePath, AnDatabaseTypes.DriverType driverType, bool createDropTableScripts = true)
        {
            var result = new StringBuilder();

            try
            {
                var tableObjectList = AnDefinitionFileUtility.ParseXmlStructureFile(xmlFilePath);

                if ((tableObjectList != null) &&
                    (tableObjectList.Count > 0))
                {
                    foreach (var table in tableObjectList.Where(table => !string.IsNullOrEmpty(table?.Name)))
                    {
                        var dropSqlCommand = GenerateSqlDropTableScript(table.ColumnObjectList.Name, driverType);
                        var createSqlCommand = GenerateSqlTableScript(table.ColumnObjectList, driverType, createDropTableScripts);

                        if (!string.IsNullOrEmpty(dropSqlCommand) &&
                            !string.IsNullOrEmpty(createSqlCommand))
                        {
                            result.Append(dropSqlCommand);
                            result.Append(createSqlCommand);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        #endregion Public Members


        #region Private Methods

        private static string GenerateIbmDb2TableScript(
            AnColumnObjectList columnObjectList,
            AnDatabaseTypes.DriverType driverType)
        {
	        var result = new StringBuilder();

            try
            {
	            if (!string.IsNullOrEmpty(columnObjectList?.Name))
	            {
					result.Append("\r\n");
					result.Append($"CREATE TABLE {columnObjectList.Name} (\r\n");

	                var primaryKeyCount = GenerateSqlTableColumnsScript(columnObjectList, driverType, out var tableColumnsScript);

                    result.Append(tableColumnsScript);

                    if (primaryKeyCount > 0)
					{
						result.Append($"\tCONSTRAINT {columnObjectList.Name}_PK PRIMARY KEY (");

						var primaryKeysFound = 0;

						var primaryKeyColumnNames = new StringBuilder();

                        foreach (var columnObject in columnObjectList.List.Where(columnObject => (columnObject != null) && columnObject.PrimaryKey))
                        {
                            if (!AnTypes.IsNestedDataType(columnObject.DataType))
                            {
                                if (primaryKeyColumnNames.Length > 0)
                                {
                                    primaryKeyColumnNames.Append(", ");
                                }

                                primaryKeyColumnNames.Append(columnObject.ColumnName);
                            }

                            primaryKeysFound++;
                                             
                            if (primaryKeysFound >= primaryKeyCount)
                            {
                                break;
                            }
                        }

                        result.Append(primaryKeyColumnNames);
						result.Append(")\r\n");
					}

					result.Append(");\r\n");
					result.Append("\r\n");
				}
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GenerateMySqlTableScript(
            AnColumnObjectList columnObjectList,
            AnDatabaseTypes.DriverType driverType)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(columnObjectList?.Name))
                {
                    result.Append("\r\n");
                    result.Append("set foreign_key_checks=0;\r\n");
                    result.Append("\r\n");
                    result.Append($"create table `{columnObjectList.Name}` (\r\n");

                    var primaryKeyCount = GenerateSqlTableColumnsScript(columnObjectList, driverType, out var tableColumnsScript);

                    result.Append(tableColumnsScript);

                    if (primaryKeyCount > 0)
                    {
                        result.Append("\tprimary key (");

                        var primaryKeysFound = 0;

                        var primaryKeyColumnNames = new StringBuilder();

                        foreach (var columnObject in columnObjectList.List.Where(columnObject => (columnObject != null) && columnObject.PrimaryKey))
                        {
                            if (!AnTypes.IsNestedDataType(columnObject.DataType))
                            {
                                if (primaryKeyColumnNames.Length > 0)
                                {
                                    primaryKeyColumnNames.Append(", ");
                                }

                                primaryKeyColumnNames.Append(columnObject.ColumnName);
                            }

                            primaryKeysFound++;

                            if (primaryKeysFound >= primaryKeyCount)
                            {
                                break;
                            }
                        }

                        result.Append(primaryKeyColumnNames);
                        result.Append(")\r\n");
                    }

                    result.Append(") engine=innodb default charset=latin1;\r\n");
                    result.Append("\r\n");
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GenerateOracleTableScript(
            AnColumnObjectList columnObjectList,
            AnDatabaseTypes.DriverType driverType)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(columnObjectList?.Name))
                {
                    result.Append("\r\n");
                    result.Append($"CREATE TABLE \"{columnObjectList.Name}\"\r\n");
                    result.Append("(\r\n");

                    var primaryKeyCount = GenerateSqlTableColumnsScript(columnObjectList, driverType, out var tableColumnsScript);

                    result.Append(tableColumnsScript);

                    if (primaryKeyCount > 0)
                    {
                        result.Append($"\tCONSTRAINT \"{columnObjectList.Name}_PK\" PRIMARY KEY (");

                        var primaryKeysFound = 0;

                        var primaryKeyColumnNames = new StringBuilder();

                        foreach (var columnObject in columnObjectList.List.Where(columnObject => (columnObject != null) && columnObject.PrimaryKey))
                        {
                            if (!AnTypes.IsNestedDataType(columnObject.DataType))
                            {
                                if (primaryKeyColumnNames.Length > 0)
                                {
                                    primaryKeyColumnNames.Append(", ");
                                }

                                primaryKeyColumnNames.Append(columnObject.ColumnName);
                            }

                            primaryKeysFound++;

                            if (primaryKeysFound >= primaryKeyCount)
                            {
                                break;
                            }
                        }

                        result.Append(primaryKeyColumnNames);
                        result.Append(") ENABLE\r\n");
                    }

                    result.Append(")\r\n");
                    result.Append("/\r\n");
                    result.Append("\r\n");
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GeneratePostgresTableScript(
            AnColumnObjectList columnObjectList,
            AnDatabaseTypes.DriverType driverType)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(columnObjectList?.Name))
                {
                    result.Append("\r\n");
                    result.Append($"create table {columnObjectList.Name}(\r\n");

                    var primaryKeyCount = GenerateSqlTableColumnsScript(columnObjectList, driverType, out var tableColumnsScript);

                    result.Append(tableColumnsScript);

                    if (primaryKeyCount > 0)
                    {
                        result.Append("\tprimary key (");

                        var primaryKeysFound = 0;

                        var primaryKeyColumnNames = new StringBuilder();

                        foreach (var columnObject in columnObjectList.List.Where(columnObject => (columnObject != null) && columnObject.PrimaryKey))
                        {
                            if (!AnTypes.IsNestedDataType(columnObject.DataType))
                            {
                                if (primaryKeyColumnNames.Length > 0)
                                {
                                    primaryKeyColumnNames.Append(", ");
                                }

                                primaryKeyColumnNames.Append(columnObject.ColumnName);
                            }

                            primaryKeysFound++;

                            if (primaryKeysFound >= primaryKeyCount)
                            {
                                break;
                            }
                        }

                        result.Append(primaryKeyColumnNames);
                        result.Append(")\r\n");
                    }

                    else
                    {
                        // Remove the trailing comma if there is no primary key.

                        if (result.Length > 0)
                        {
                            var tempResult = result.ToString();
                            tempResult = tempResult.TrimEnd(' ');
                            result = new StringBuilder(tempResult.TrimEnd(','));
                            result.Append("\r\n");
                        }
                    }

                    result.Append(") with (oids=false)\r\n");
                    result.Append(";\r\n");
                    result.Append("\r\n");
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GenerateSqLiteTableScript(
            AnColumnObjectList columnObjectList,
            AnDatabaseTypes.DriverType driverType)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(columnObjectList?.Name))
                {
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GenerateSqlServerTableScript(
            AnColumnObjectList columnObjectList,
            AnDatabaseTypes.DriverType driverType,
            bool createDropTableScripts = true)
        {
	        var result = new StringBuilder();

            try
            {
	            if (!string.IsNullOrEmpty(columnObjectList?.Name))
	            {
	                if (createDropTableScripts)
	                {
	                    result.Append("\r\n");
	                    result.Append("\r\n");
	                    result.Append($"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{columnObjectList.Name}]') AND type in (N'U'))\r\n");
	                    result.Append($"\tDROP TABLE [dbo].[{columnObjectList.Name}]\r\n");
	                    result.Append("GO\r\n");
	                    result.Append("\r\n");
	                }

                    result.Append("\r\n");
					result.Append("SET ANSI_NULLS ON\r\n");
					result.Append("GO\r\n");
					result.Append("\r\n");
					result.Append("SET QUOTED_IDENTIFIER ON\r\n");
					result.Append("GO\r\n");
					result.Append("\r\n");
					result.Append($"CREATE TABLE [dbo].[{columnObjectList.Name}](\r\n");

	                var primaryKeyCount = GenerateSqlTableColumnsScript(columnObjectList, driverType, out var tableColumnsScript);

                    result.Append(tableColumnsScript);

					if (primaryKeyCount > 0)
					{
						result.Append($"\tCONSTRAINT [PK_{columnObjectList.Name}] PRIMARY KEY CLUSTERED\r\n");
						result.Append("\t(\r\n");

						var primaryKeysFound = 0;

                        foreach (var columnObject in columnObjectList.List.Where(columnObject => (columnObject != null) && columnObject.PrimaryKey))
						{
                            if (!AnTypes.IsNestedDataType(columnObject.DataType))
                            {
                                result.Append($"\t\t[{columnObject.ColumnName}] ASC,\r\n");
                            }

                            primaryKeysFound++;

                            if (primaryKeysFound >= primaryKeyCount)
						    {
						        break;
						    }
						}

                        result = new StringBuilder(AnTypes.TrimEnd(result.ToString(), ",\r\n"));
					    result.Append("\r\n");

                        result.Append("\t)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)\r\n");
						result.Append(") ON [PRIMARY]\r\n");
					}

					else
					{
                        result.Append(")\r\n");
                    }

					result.Append("\r\n");
					result.Append("GO\r\n");
					result.Append("\r\n");
	            }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GeneratePervasiveTableScript(
            AnColumnObjectList columnObjectList,
            AnDatabaseTypes.DriverType driverType)
        {
	        var result = new StringBuilder();

            try
            {
	            if (!string.IsNullOrEmpty(columnObjectList?.Name))
	            {
					result.Append("\r\n");
					result.Append($"create table \"{columnObjectList.Name}\"(\r\n");

	                var primaryKeyCount = GenerateSqlTableColumnsScript(columnObjectList, driverType, out var tableColumnsScript);

                    result.Append(tableColumnsScript);

                    if (primaryKeyCount > 0)
					{
						result.Append("\tprimary key (");

						var primaryKeysFound = 0;

						var primaryKeyColumnNames = new StringBuilder();

                        foreach (var columnObject in columnObjectList.List.Where(columnObject => (columnObject != null) && columnObject.PrimaryKey))
                        {
                            if (!AnTypes.IsNestedDataType(columnObject.DataType))
                            {
                                if (primaryKeyColumnNames.Length > 0)
                                {
                                    primaryKeyColumnNames.Append(", ");
                                }

                                primaryKeyColumnNames.Append(columnObject.ColumnName);
                            }

                            primaryKeysFound++;
                                             
                            if (primaryKeysFound >= primaryKeyCount)
                            {
                                break;
                            }
                        }

                        result.Append(primaryKeyColumnNames);
						result.Append(")\r\n");
					}

					result.Append(");\r\n");
					result.Append("\r\n");
	            }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GenerateIbmDb2DropTableScript(string tableName)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    result.Append("\r\n");
                    result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(tableName)} ;\r\n");
                    result.Append("\r\n");
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GenerateMySqlDropTableScript(string tableName)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    result.Append("\r\n");
                    result.Append("SET FOREIGN_KEY_CHECKS=0;\r\n");
                    result.Append("\r\n");
                    result.Append($"DROP TABLE IF EXISTS `{AnTypes.FormatSyncDbQueueTableName(tableName)}`;\r\n");
                    result.Append("\r\n");
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GenerateOracleDropTableScript(string tableName)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    result.Append("\r\n");
                    result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(tableName)}\r\n");
                    result.Append("/\r\n");
                    result.Append("\r\n");
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GeneratePostgresDropTableScript(string tableName)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    result.Append("\r\n");
                    result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(tableName)};\r\n");
                    result.Append("\r\n");
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

	    private static string GenerateSqLiteDropTableScript(string tableName)
	    {
		    var result = new StringBuilder();

		    try
		    {
			    if (!string.IsNullOrEmpty(tableName))
			    {
			    }
		    }

		    catch (Exception ex)
		    {
			    AnLog.Error(ex);
		    }

		    return result.ToString();
	    }

        private static string GenerateSqlServerDropTableScript(string tableName)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    result.Append("\r\n");
                    result.Append("GO\r\n");
                    result.Append("\r\n");
                    result.Append($"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{AnTypes.FormatSyncDbQueueTableName(tableName)}]') AND type in (N'U'))\r\n");
                    result.Append($"\tDROP TABLE [dbo].[{AnTypes.FormatSyncDbQueueTableName(tableName)}]\r\n");
                    result.Append("\r\n");
                    result.Append("GO\r\n");
                    result.Append("\r\n");
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GeneratePervasiveDropTableScript(string tableName)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    result.Append("\r\n");
                    result.Append($"DROP TABLE {AnTypes.FormatSyncDbQueueTableName(tableName)}\r\n");
                    result.Append("/\r\n");
                    result.Append("\r\n");
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GenerateIbmDb2ClearTableScript(string tableName)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    result.Append("\r\n");
                    result.Append($"DELETE FROM {tableName}\r\n");
                    result.Append("\r\n");
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GenerateMySqlClearTableScript(string tableName)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    result.Append("\r\n");
                    result.Append($"DELETE FROM {tableName}\r\n");
                    result.Append("\r\n");
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GenerateOracleClearTableScript(string tableName)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    result.Append("\r\n");
                    result.Append($"DELETE FROM {tableName}\r\n");
                    result.Append("\r\n");
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GeneratePostgresClearTableScript(string tableName)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    result.Append("\r\n");
                    result.Append($"delete from {tableName}\r\n");
                    result.Append("\r\n");
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

	    private static string GenerateSqLiteClearTableScript(string tableName)
	    {
		    var result = new StringBuilder();

		    try
		    {
			    if (!string.IsNullOrEmpty(tableName))
			    {
			    }
		    }

		    catch (Exception ex)
		    {
			    AnLog.Error(ex);
		    }

		    return result.ToString();
	    }

        private static string GenerateSqlServerClearTableScript(string tableName)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    result.Append("\r\n");
                    result.Append($"DELETE {tableName}\r\n");
                    result.Append("\r\n");
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GeneratePervasiveClearTableScript(string tableName)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    result.Append("\r\n");
                    result.Append($"DELETE FROM {tableName}\r\n");
                    result.Append("\r\n");
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static int GenerateSqlTableColumnsScript(
            AnColumnObjectList columnObjectList,
            AnDatabaseTypes.DriverType driverType,
            out string tableColumnsScript)
        {
            var result = 0;

            tableColumnsScript = "";

            try
            {
                if (columnObjectList != null)
                {
                    tableColumnsScript = columnObjectList.List.Where(columnObject => columnObject != null).Aggregate(tableColumnsScript, (current, columnObject) => current + GenerateSqlColumnScript(columnObject, driverType));

                    result = columnObjectList.PrimaryKeyColumnCount;
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        private static string GenerateIbmDb2ColumnScript(AnColumnObject columnObject)
        {
	        var result = new StringBuilder();

            try
            {
	            if (columnObject != null)
	            {
                    var nullOrNotNull = (columnObject.AllowNulls && !columnObject.PrimaryKey && !columnObject.AutoGenerated) ? "NULL" : "NOT NULL";

                    switch (columnObject.DataType)
                    {
                        case AnTypes.DataType.Null:
                            break;

                        case AnTypes.DataType.Character:
                            result.Append($"\t{columnObject.ColumnName} CHARACTER(1) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Byte:
                        case AnTypes.DataType.Short:
                        case AnTypes.DataType.UShort:

                            if (columnObject.AutoGenerated)
                            {
                                result.Append($"\t{columnObject.ColumnName} SMALLINT NOT NULL GENERATED ALWAYS AS IDENTITY (");
                                result.Append("START WITH +1 ");
                                result.Append("INCREMENT BY +1 ");
                                result.Append("MINVALUE +1 ");
                                result.Append("MAXVALUE +32767 ");
                                result.Append("NO CYCLE ");
                                result.Append("NO CACHE ");
                                result.Append("NO ORDER),\r\n");
                            }

                            else
                            {
                                result.Append($"\t{columnObject.ColumnName} SMALLINT {nullOrNotNull},\r\n");
                            }

                            break;

                        case AnTypes.DataType.Int:
                        case AnTypes.DataType.UInt:

                            if (columnObject.AutoGenerated)
                            {
                                result.Append($"\t{columnObject.ColumnName} INTEGER NOT NULL GENERATED ALWAYS AS IDENTITY (");
                                result.Append("START WITH +1 ");
                                result.Append("INCREMENT BY +1 ");
                                result.Append("MINVALUE +1 ");
                                result.Append("MAXVALUE +32767 ");
                                result.Append("NO CYCLE ");
                                result.Append("NO CACHE ");
                                result.Append("NO ORDER),\r\n");
                            }

                            else
                            {
                                result.Append($"\t{columnObject.ColumnName} INTEGER {nullOrNotNull},\r\n");
                            }

                            break;

                        case AnTypes.DataType.Long:
                        case AnTypes.DataType.ULong:

                            if (columnObject.AutoGenerated)
                            {
                                result.Append($"\t{columnObject.ColumnName} BIGINT NOT NULL GENERATED ALWAYS AS IDENTITY (");
                                result.Append("START WITH +1 ");
                                result.Append("INCREMENT BY +1 ");
                                result.Append("MINVALUE +1 ");
                                result.Append("MAXVALUE +32767 ");
                                result.Append("NO CYCLE ");
                                result.Append("NO CACHE ");
                                result.Append("NO ORDER),\r\n");
                            }

                            else
                            {
                                result.Append($"\t{columnObject.ColumnName} BIGINT {nullOrNotNull},\r\n");
                            }

                            break;

                        case AnTypes.DataType.Single:
                            result.Append($"\t{columnObject.ColumnName} REAL {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Double:
                            result.Append($"\t{columnObject.ColumnName} DOUBLE {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Decimal:

                            if (columnObject.AutoGenerated)
                            {
                                result.Append($"\t{columnObject.ColumnName} DECIMAL(11,0) NOT NULL GENERATED ALWAYS AS IDENTITY (");
                                result.Append("START WITH +1 ");
                                result.Append("INCREMENT BY +1 ");
                                result.Append("MINVALUE +1 ");
                                result.Append("MAXVALUE +32767 ");
                                result.Append("NO CYCLE ");
                                result.Append("NO CACHE ");
                                result.Append("NO ORDER),\r\n");
                            }

                            else
                            {
                                result.Append($"\t{columnObject.ColumnName} DECIMAL(11,0) {nullOrNotNull},\r\n");
                            }

                            break;

                        case AnTypes.DataType.Boolean:
                            result.Append($"\t{columnObject.ColumnName} SMALLINT {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Currency:
                            result.Append($"\t{columnObject.ColumnName} DECIMAL(11,0) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.DateTime:
                        case AnTypes.DataType.FileTime:
                            result.Append($"\t{columnObject.ColumnName} TIMESTAMP {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Guid:
                            result.Append($"\t{columnObject.ColumnName} VARCHAR(100) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Char:
                        case AnTypes.DataType.NChar:
                            result.Append($"\t{columnObject.ColumnName} CHAR({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.VarChar:
                        case AnTypes.DataType.NVarChar:
                            result.Append($"\t{columnObject.ColumnName} VARCHAR({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Text:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobVarCharSize)) ? $"\t{columnObject.ColumnName} CLOB({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t{columnObject.ColumnName} CLOB {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.NText:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobNVarCharSize)) ? $"\t{columnObject.ColumnName} CLOB({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t{columnObject.ColumnName} CLOB {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Image:
                        case AnTypes.DataType.Binary:
                        case AnTypes.DataType.VarBinary:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobBinarySize)) ? $"\t{columnObject.ColumnName} BLOB({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t{columnObject.ColumnName} BLOB {nullOrNotNull},\r\n");
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GenerateMySqlColumnScript(AnColumnObject columnObject)
        {
	        var result = new StringBuilder();

            try
            {
	            if (columnObject != null)
	            {
                    var nullOrNotNull = (columnObject.AllowNulls && !columnObject.PrimaryKey && !columnObject.AutoGenerated) ? "null" : "not null";

                    switch (columnObject.DataType)
                    {
                        case AnTypes.DataType.Null:
                            break;

                        case AnTypes.DataType.Character:
                            result.Append($"\t`{columnObject.ColumnName}` char(1) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Byte:
                            result.Append(columnObject.AutoGenerated ? $"\t`{columnObject.ColumnName}` tinyint {nullOrNotNull} auto_increment,\r\n" : $"\t`{columnObject.ColumnName}` tinyint {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Short:
                        case AnTypes.DataType.UShort:
                            result.Append(columnObject.AutoGenerated ? $"\t`{columnObject.ColumnName}` smallint {nullOrNotNull} auto_increment,\r\n" : $"\t`{columnObject.ColumnName}` smallint {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Int:
                        case AnTypes.DataType.UInt:
                            result.Append(columnObject.AutoGenerated ? $"\t`{columnObject.ColumnName}` int {nullOrNotNull} auto_increment,\r\n" : $"\t`{columnObject.ColumnName}` int {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Long:
                        case AnTypes.DataType.ULong:
                            result.Append(columnObject.AutoGenerated ? $"\t`{columnObject.ColumnName}` bigint {nullOrNotNull} auto_increment,\r\n" : $"\t`{columnObject.ColumnName}` bigint {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Single:
                            result.Append($"\t`{columnObject.ColumnName}` float(5,2) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Double:
                            result.Append($"\t`{columnObject.ColumnName}` double precision(7,4) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Decimal:
                            result.Append($"\t`{columnObject.ColumnName}` decimal(19,4) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Boolean:
                            result.Append($"\t`{columnObject.ColumnName}` tinyint {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Currency:
                            result.Append($"\t`{columnObject.ColumnName}` decimal(19,4) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.DateTime:
                        case AnTypes.DataType.FileTime:
                            result.Append($"\t`{columnObject.ColumnName}` datetime {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Guid:
                            result.Append($"\t`{columnObject.ColumnName}` varchar(100) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Text:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobVarCharSize)) ? $"\t`{columnObject.ColumnName}` varchar({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t`{columnObject.ColumnName}` text {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.NText:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobNVarCharSize)) ? $"\t`{columnObject.ColumnName}` varchar({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t`{columnObject.ColumnName}` text {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Char:
                        case AnTypes.DataType.NChar:
                            result.Append($"\t`{columnObject.ColumnName}` char({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.VarChar:
                        case AnTypes.DataType.NVarChar:
                            result.Append($"\t`{columnObject.ColumnName}` varchar({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Image:
                        case AnTypes.DataType.Binary:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobBinarySize)) ? $"\t`{columnObject.ColumnName}` binary({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t`{columnObject.ColumnName}` blob {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.VarBinary:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobBinarySize)) ? $"\t`{columnObject.ColumnName}` varbinary({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t`{columnObject.ColumnName}` blob {nullOrNotNull},\r\n");
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GenerateOracleColumnScript(AnColumnObject columnObject)
        {
	        var result = new StringBuilder();

            try
            {
	            if (columnObject != null)
	            {
                    var nullOrNotNull = (columnObject.AllowNulls && !columnObject.PrimaryKey && !columnObject.AutoGenerated) ? "NULL" : "NOT NULL";

                    switch (columnObject.DataType)
                    {
                        case AnTypes.DataType.Null:
                            break;

                        case AnTypes.DataType.Character:
                            result.Append($"\t\"{columnObject.ColumnName}\" CHAR(1) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Byte:
                        case AnTypes.DataType.Short:
                        case AnTypes.DataType.UShort:
                            result.Append($"\t\"{columnObject.ColumnName}\" SMALLINT {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Int:
                        case AnTypes.DataType.UInt:
                        case AnTypes.DataType.Long:
                        case AnTypes.DataType.ULong:
                            result.Append($"\t\"{columnObject.ColumnName}\" INTEGER {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Single:
                            result.Append($"\t\"{columnObject.ColumnName}\" REAL {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Double:
                            result.Append($"\t\"{columnObject.ColumnName}\" FLOAT(128) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Decimal:
                        case AnTypes.DataType.Currency:
                            result.Append($"\t\"{columnObject.ColumnName}\" DECIMAL(38) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Boolean:
                            result.Append($"\t\"{columnObject.ColumnName}\" SMALLINT {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.DateTime:
                            result.Append($"\t\"{columnObject.ColumnName}\" DATE {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.FileTime:
                            result.Append($"\t\"{columnObject.ColumnName}\" TIMESTAMP {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Guid:
                            result.Append($"\t\"{columnObject.ColumnName}\" NVARCHAR2(100) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Text:
                            result.Append($"\t\"{columnObject.ColumnName}\" CLOB {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Char:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobVarCharSize)) ? $"\t\"{columnObject.ColumnName}\" CHAR({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t\"{columnObject.ColumnName}\" CLOB {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.VarChar:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobVarCharSize)) ? $"\t\"{columnObject.ColumnName}\" VARCHAR2({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t\"{columnObject.ColumnName}\" CLOB {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.NText:
                            result.Append($"\t\"{columnObject.ColumnName}\" NCLOB {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.NChar:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobNVarCharSize)) ? $"\t\"{columnObject.ColumnName}\" NCHAR({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t\"{columnObject.ColumnName}\" NCLOB {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.NVarChar:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobNVarCharSize)) ? $"\t\"{columnObject.ColumnName}\" NVARCHAR2({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t\"{columnObject.ColumnName}\" NCLOB {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Image:
                        case AnTypes.DataType.Binary:
                        case AnTypes.DataType.VarBinary:
                            result.Append($"\t\"{columnObject.ColumnName}\" BLOB {nullOrNotNull},\r\n");
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GeneratePostgresColumnScript(AnColumnObject columnObject)
        {
	        var result = new StringBuilder();

            try
            {
	            if (columnObject != null)
	            {
                    var nullOrNotNull = (columnObject.AllowNulls && !columnObject.PrimaryKey && !columnObject.AutoGenerated) ? "null" : "not null";

                    switch (columnObject.DataType)
                    {
                        case AnTypes.DataType.Null:
                            break;

                        case AnTypes.DataType.Character:
                            result.Append($"\t{columnObject.ColumnName} character(1) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Byte:
                        case AnTypes.DataType.Short:
                        case AnTypes.DataType.UShort:
                            result.Append(columnObject.AutoGenerated ? $"\t{columnObject.ColumnName} serial not null,\r\n" : $"\t{columnObject.ColumnName} smallint {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Int:
                        case AnTypes.DataType.UInt:
                            result.Append(columnObject.AutoGenerated ? $"\t{columnObject.ColumnName} serial not null,\r\n" : $"\t{columnObject.ColumnName} int {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Long:
                        case AnTypes.DataType.ULong:
                            result.Append(columnObject.AutoGenerated ? $"\t{columnObject.ColumnName} bigserial not null,\r\n" : $"\t{columnObject.ColumnName} bigint {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Single:
                            result.Append($"\t{columnObject.ColumnName} real {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Double:
                            result.Append($"\t{columnObject.ColumnName} double precision {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Decimal:
                            result.Append($"\t{columnObject.ColumnName} numeric(5,0) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Boolean:
                            result.Append($"\t{columnObject.ColumnName} boolean {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Currency:
                            result.Append($"\t{columnObject.ColumnName} money {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.DateTime:
                        case AnTypes.DataType.FileTime:
                            result.Append($"\t{columnObject.ColumnName} timestamp without time zone {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Guid:
                            result.Append($"\t{columnObject.ColumnName} character varying(100) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Text:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobVarCharSize)) ? $"\t{columnObject.ColumnName} character varying({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t{columnObject.ColumnName} text {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.NText:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobNVarCharSize)) ? $"\t{columnObject.ColumnName} character varying({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t{columnObject.ColumnName} text {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Char:
                        case AnTypes.DataType.NChar:
                            result.Append($"\t{columnObject.ColumnName} character({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.VarChar:
                        case AnTypes.DataType.NVarChar:
                            result.Append($"\t{columnObject.ColumnName} character varying({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Image:
                        case AnTypes.DataType.Binary:
                        case AnTypes.DataType.VarBinary:
                            result.Append($"\t{columnObject.ColumnName} bytea {nullOrNotNull},\r\n");
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GenerateSqLiteColumnScript(AnColumnObject columnObject)
        {
	        var result = new StringBuilder();

            try
            {
	            if (columnObject != null)
	            {
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GenerateSqlServerColumnScript(AnColumnObject columnObject)
        {
	        var result = new StringBuilder();

            try
            {
	            if (columnObject != null)
	            {
                    var nullOrNotNull = (columnObject.AllowNulls && !columnObject.PrimaryKey && !columnObject.AutoGenerated) ? "NULL" : "NOT NULL";

                    switch (columnObject.DataType)
                    {
                        case AnTypes.DataType.Null:
                            break;

                        case AnTypes.DataType.Character:
                            result.Append($"\t[{columnObject.ColumnName}] [char](1) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Byte:
                            result.Append($"\t[{columnObject.ColumnName}] [tinyint] {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Short:
                        case AnTypes.DataType.UShort:
                            result.Append(columnObject.AutoGenerated ? $"\t[{columnObject.ColumnName}] [smallint] IDENTITY(1,1) NOT NULL,\r\n" : $"\t[{columnObject.ColumnName}] [smallint] {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Int:
                        case AnTypes.DataType.UInt:
                            result.Append(columnObject.AutoGenerated ? $"\t[{columnObject.ColumnName}] [int] IDENTITY(1,1) NOT NULL,\r\n" : $"\t[{columnObject.ColumnName}] [int] {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Long:
                        case AnTypes.DataType.ULong:
                            result.Append(columnObject.AutoGenerated ? $"\t[{columnObject.ColumnName}] [bigint] IDENTITY(1,1) NOT NULL,\r\n" : $"\t[{columnObject.ColumnName}] [bigint] {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Single:
                            result.Append($"\t[{columnObject.ColumnName}] [real] {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Double:
                            result.Append($"\t[{columnObject.ColumnName}] [float] {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Decimal:
                            result.Append($"\t[{columnObject.ColumnName}] [decimal](19, 4) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Boolean:
                            result.Append($"\t[{columnObject.ColumnName}] [bit] {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Currency:
                            result.Append($"\t[{columnObject.ColumnName}] [money] {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.DateTime:
                            result.Append($"\t[{columnObject.ColumnName}] [datetime] {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.FileTime:
                            result.Append($"\t[{columnObject.ColumnName}] [timestamp] {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Guid:
                            result.Append($"\t[{columnObject.ColumnName}] [uniqueidentifier] {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Text:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobVarCharSize)) ? $"\t[{columnObject.ColumnName}] [varchar]({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t[{columnObject.ColumnName}] [text] {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Char:
                            result.Append($"\t[{columnObject.ColumnName}] [char]({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.VarChar:
                            result.Append(((columnObject.ColumnSize < 0) || (columnObject.ColumnSize > MaxNonBlobVarCharSize)) ? $"\t[{columnObject.ColumnName}] [varchar](max) NULL,\r\n" : $"\t[{columnObject.ColumnName}] [varchar]({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.NText:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobNVarCharSize)) ? $"\t[{columnObject.ColumnName}] [nvarchar]({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t[{columnObject.ColumnName}] [ntext] {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.NChar:
                            result.Append($"\t[{columnObject.ColumnName}] [nchar]({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.NVarChar:
                            result.Append(((columnObject.ColumnSize < 0) || (columnObject.ColumnSize > MaxNonBlobNVarCharSize)) ? $"\t[{columnObject.ColumnName}] [nvarchar](max) NULL,\r\n" : $"\t[{columnObject.ColumnName}] [nvarchar]({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Image:
                            result.Append($"\t[{columnObject.ColumnName}] [image] {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Binary:
                            result.Append(((columnObject.ColumnSize < 0) || (columnObject.ColumnSize > MaxNonBlobBinarySize)) ? $"\t[{columnObject.ColumnName}] [varbinary](max) NULL,\r\n" : $"\t[{columnObject.ColumnName}] [binary]({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.VarBinary:
                            result.Append(((columnObject.ColumnSize < 0) || (columnObject.ColumnSize > MaxNonBlobBinarySize)) ? $"\t[{columnObject.ColumnName}] [varbinary](max) NULL,\r\n" : $"\t[{columnObject.ColumnName}] [varbinary]({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static string GeneratePervasiveColumnScript(AnColumnObject columnObject)
        {
	        var result = new StringBuilder();

            try
            {
	            if (columnObject != null)
	            {
                    var nullOrNotNull = (columnObject.AllowNulls && !columnObject.PrimaryKey && !columnObject.AutoGenerated) ? "null" : "not null";

                    switch (columnObject.DataType)
                    {
                        case AnTypes.DataType.Null:
                            break;

                        case AnTypes.DataType.Character:
                            result.Append($"\t\"{columnObject.ColumnName}\" char(1) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Byte:
                            result.Append($"\t\"{columnObject.ColumnName}\" utinyint {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Short:
                            result.Append(columnObject.AutoGenerated ? $"\t\"{columnObject.ColumnName}\" identity not null,\r\n" : $"\t\"{columnObject.ColumnName}\" smallint {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.UShort:
                            result.Append(columnObject.AutoGenerated ? $"\t\"{columnObject.ColumnName}\" identity not null,\r\n" : $"\t\"{columnObject.ColumnName}\" usmallint {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Int:
                            result.Append(columnObject.AutoGenerated ? $"\t\"{columnObject.ColumnName}\" identity not null,\r\n" : $"\t\"{columnObject.ColumnName}\" integer {nullOrNotNull},\r\n");
                            break;
                        
                        case AnTypes.DataType.UInt:
                            result.Append(columnObject.AutoGenerated ? $"\t\"{columnObject.ColumnName}\" identity not null,\r\n" : $"\t\"{columnObject.ColumnName}\" uinteger {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Long:
                            result.Append(columnObject.AutoGenerated ? $"\t\"{columnObject.ColumnName}\" identity not null,\r\n" : $"\t\"{columnObject.ColumnName}\" bigint {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.ULong:
                            result.Append(columnObject.AutoGenerated ? $"\t\"{columnObject.ColumnName}\" identity not null,\r\n" : $"\t\"{columnObject.ColumnName}\" ubigint {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Single:
                            result.Append($"\t\"{columnObject.ColumnName}\" real {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Double:
                            result.Append($"\t\"{columnObject.ColumnName}\" double {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Decimal:
                            result.Append($"\t\"{columnObject.ColumnName}\" decimal(15, 0) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Boolean:
                            result.Append($"\t\"{columnObject.ColumnName}\" bit {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Currency:
                            result.Append($"\t\"{columnObject.ColumnName}\" money {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.DateTime:
                        case AnTypes.DataType.FileTime:
                            result.Append($"\t\"{columnObject.ColumnName}\" date {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Guid:
                            result.Append($"\t\"{columnObject.ColumnName}\" varchar(100) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Text:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobVarCharSize)) ? $"\t\"{columnObject.ColumnName}\" varchar({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t\"{columnObject.ColumnName}\" longvarchar {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Char:
                            result.Append($"\t\"{columnObject.ColumnName}\" char({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.VarChar:
                            result.Append($"\t\"{columnObject.ColumnName}\" varchar({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.NText:
                            result.Append(((columnObject.ColumnSize > 0) && (columnObject.ColumnSize <= MaxNonBlobNVarCharSize)) ? $"\t\"{columnObject.ColumnName}\" varchar({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n" : $"\t\"{columnObject.ColumnName}\" longvarchar {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.NChar:
                            result.Append($"\t\"{columnObject.ColumnName}\" char({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.NVarChar:
                            result.Append($"\t\"{columnObject.ColumnName}\" varchar({(columnObject.ColumnSize > 0 ? columnObject.ColumnSize : DefaultStringSize)}) {nullOrNotNull},\r\n");
                            break;

                        case AnTypes.DataType.Image:
                        case AnTypes.DataType.Binary:
                        case AnTypes.DataType.VarBinary:
                            result.Append($"\t\"{columnObject.ColumnName}\" binary {nullOrNotNull},\r\n");
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        #endregion Private Methods
    }
}
