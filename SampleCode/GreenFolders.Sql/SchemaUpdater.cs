
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenFolders.Sql
{
    public class SchemaUpdater
    {
        #region Enums

        public enum CommandType
        {
            AddIdIdentity = 0,
            AddPrimaryKeyConstraint = 1,
            AddConstraint = 2,
            DropConstraint = 3,
            CreateNonClusteredIndex = 4,
            DropIdIdentity = 5,
            DropPrimaryKeyConstraint = 6,
            DropNonClusteredIndex = 7
        }

        #endregion Enums


        #region Private Fields

        private SqlServerConnection _databaseConnection;
        private readonly Dictionary<Guid, SqlServerConnection> _defaultDatabaseConnections = new Dictionary<Guid, SqlServerConnection>();

        private string _connectionString = "";
        private string _databaseServerName = "";
        private string _databaseServerPort = "";
        private string _databaseTimeout = "";
        private string _databaseName = "";
        private string _databaseUserName = "";
        private string _databasePassword = "";
        private bool _useConnectionString;

        #endregion Private Fields


        #region Constructors

        public SchemaUpdater()

        {
            try
            {
                _connectionString = "";
                _databaseServerName = "";
                _databaseServerPort = "";
                _databaseTimeout = "";
                _databaseName = "";
                _databaseUserName = "";
                _databasePassword = "";
                _useConnectionString = false;

                _databaseConnection = null;
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public SchemaUpdater(
            string connectionString,
            string databaseServerName,
            string databaseServerPort,
            string databaseTimeout,
            string databaseName,
            string databaseUserName,
            string databasePassword,
            bool useConnectionString)

        {
            try
            {
                _connectionString = connectionString ?? "";
                _databaseServerName = databaseServerName ?? "";
                _databaseServerPort = databaseServerPort ?? "";
                _databaseTimeout = databaseTimeout ?? "";
                _databaseName = databaseName ?? "";
                _databaseUserName = databaseUserName ?? "";
                _databasePassword = databasePassword ?? "";
                _useConnectionString = useConnectionString;

                _databaseConnection = null;
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion Constructors


        #region Public Methods

        public bool Connect(ref string errorMessage)

        {
            var result = false;

            try
            {
                result = Connect(
                    _connectionString,
                    _databaseServerName,
                    _databaseServerPort,
                    _databaseTimeout,
                    _databaseName,
                    _databaseUserName,
                    _databasePassword,
                    _useConnectionString,
                    ref errorMessage);
            }

            catch (Exception ex)
            {
                errorMessage = ex.Message;

                Logger.Error(ex);
            }

            return result;
        }

        public bool Connect(
            string connectionString,
            string databaseServerName,
            string databaseServerPort,
            string databaseTimeout,
            string databaseName,
            string databaseUserName,
            string databasePassword,
            bool useConnectionString,
            ref string errorMessage)

        {
            try
            {
                _connectionString = connectionString ?? "";
                _databaseServerName = databaseServerName ?? "";
                _databaseServerPort = databaseServerPort ?? "";
                _databaseTimeout = databaseTimeout ?? "";
                _databaseName = databaseName ?? "";
                _databaseUserName = databaseUserName ?? "";
                _databasePassword = databasePassword ?? "";
                _useConnectionString = useConnectionString;

                _databaseConnection = new SqlServerConnection();

                string localConnectionString;

                if (useConnectionString)
                {
                    localConnectionString = connectionString ?? "";
                }

                else
                {
                    if (!string.IsNullOrEmpty(databaseUserName) && !string.IsNullOrEmpty(databasePassword))
                    {
                        localConnectionString = GetDefaultConnectionString().
                            Replace("myServerAddress", _databaseServerName ?? "").
                            Replace("myDataBase", databaseName ?? "").
                            Replace("myUsername", databaseUserName).
                            Replace("myPassword", databasePassword);
                    }

                    else
                    {
                        localConnectionString = GetDefaultConnectionString(true).
                            Replace("myServerAddress", _databaseServerName ?? "").
                            Replace("myDataBase", _databaseName ?? "");
                    }
                }

                if (!string.IsNullOrEmpty(localConnectionString))
                {
                    _databaseConnection.Connect(localConnectionString, SafeConvert.ToInt32(databaseTimeout));
                }
            }

            catch (Exception ex)
            {
                errorMessage = ex.Message;

                Logger.Error(ex);
            }

            return _databaseConnection?.Connected() ?? false;
        }

        public SqlServerConnection ValidateDefaultDatabaseConnection(
            Guid folderTypeGuid,
            string defaultDatabaseServer,
            string defaultDatabaseName,
            string defaultDatabaseUserName,
            string defaultDatabasePassword)
        {
            SqlServerConnection result = null;

            try
            {
                if ((folderTypeGuid != Guid.Empty) &&
                    !string.IsNullOrEmpty(defaultDatabaseServer) &&
                    !string.IsNullOrEmpty(defaultDatabaseName))
                {
                    // Add key if it doesn't exist.
                    if (!_defaultDatabaseConnections.ContainsKey(folderTypeGuid))
                    {
                        _defaultDatabaseConnections.Add(folderTypeGuid, null);
                    }

                    if (_defaultDatabaseConnections.ContainsKey(folderTypeGuid))
                    {
                        // If not connected set to null.
                        if ((_defaultDatabaseConnections[folderTypeGuid] != null) && !_defaultDatabaseConnections[folderTypeGuid].Connected())
                        {
                            _defaultDatabaseConnections[folderTypeGuid] = null;
                        }

                        // If null then create.
                        if (_defaultDatabaseConnections[folderTypeGuid] == null)
                        {
                            _defaultDatabaseConnections[folderTypeGuid] = new SqlServerConnection();
                        }

                        // If not connected then connect.
                        if (!_defaultDatabaseConnections[folderTypeGuid].Connected())
                        {
                            string connectionString;

                            if (!string.IsNullOrEmpty(defaultDatabaseUserName) && !string.IsNullOrEmpty(defaultDatabasePassword))
                            {
                                connectionString = GetDefaultConnectionString().
                                    Replace("myServerAddress", defaultDatabaseServer).
                                    Replace("myDataBase", defaultDatabaseName).
                                    Replace("myUsername", defaultDatabaseUserName).
                                    Replace("myPassword", defaultDatabasePassword);
                            }

                            else
                            {
                                connectionString = GetDefaultConnectionString(true).
                                    Replace("myServerAddress", defaultDatabaseServer).
                                    Replace("myDataBase", defaultDatabaseName);
                            }

                            _defaultDatabaseConnections[folderTypeGuid]?.Connect(connectionString, SafeConvert.ToInt32(_databaseTimeout));
                        }
                    }

                    result = _defaultDatabaseConnections[folderTypeGuid];
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;
        }

        public static string GetDatabaseDisplayConnectionString(
            string connectionString,
            string serverName,
            string serverPort,
            string timeout,
            string databaseName,
            string userName,
            string password,
            bool useConnectionString)
        {
            string result;

            if (useConnectionString)
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    var displayConnectionString = new StringBuilder();

                    if (connectionString.ToLower().Contains("password"))
                    {
                        var startPos = connectionString.ToLower().IndexOf("password", StringComparison.Ordinal);

                        if (startPos != -1)
                        {
                            startPos += "password".Length;

                            displayConnectionString.Append(connectionString.Substring(0, startPos));
                            displayConnectionString.Append("=************");

                            var endPos = connectionString.IndexOf(";", startPos, StringComparison.Ordinal);

                            displayConnectionString.Append(((endPos != -1) && (connectionString.Length - endPos > 0)) ? connectionString.Substring(endPos, connectionString.Length - endPos) : ";");
                        }
                    }

                    else
                    {
                        displayConnectionString.Append(connectionString);
                    }

                    result = displayConnectionString.ToString();
                }

                else
                {
                    result = "(not set)";
                }
            }

            else
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    result = GetDefaultConnectionString().
                        Replace("myServerAddress", serverName ?? "").
                        Replace("myDataBase", databaseName ?? "").
                        Replace("myUsername", userName).
                        Replace("myPassword", "************");
                }

                else
                {
                    result = GetDefaultConnectionString(true).
                        Replace("myServerAddress", serverName ?? "").
                        Replace("myDataBase", databaseName ?? "");
                }

                if (result.Contains("myServerPort"))
                {
                    result = result.Replace("myServerPort", serverPort);
                }
            }

            return result;
        }

        public static string GetDefaultConnectionString(bool useTrusedConnection = false)
        {
            return useTrusedConnection ? "Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;" : "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;";
        }

        public bool ProcessCommand(string command, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (_databaseConnection?.Connected() ?? false)
                {
                    result = _databaseConnection.ExecuteCommand(command, ref errorMessage);
                }

                else
                {
                    errorMessage = "Not connected to database";
                }
            }

            catch (Exception ex)
            {
                errorMessage = ex.Message;

                Logger.Error(ex);
            }

            return result;
        }

        public bool ProcessCommand(SqlServerConnection databaseConnection, string command, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (databaseConnection?.Connected() ?? false)
                {
                    result = databaseConnection.ExecuteCommand(command, ref errorMessage);
                }

                else
                {
                    errorMessage = "Not connected to database";
                }
            }

            catch (Exception ex)
            {
                errorMessage = ex.Message;

                Logger.Error(ex);
            }

            return result;
        }

        public List<Dictionary<string, object>> GetFolderTypes(ref string errorMessage)
        {
            List<Dictionary<string, object>> result = null;

            try
            {
                if (_databaseConnection?.Connected() ?? false)
                {
                    using (var dataSet = _databaseConnection.ExecuteQuery("SELECT FolderTypeGuid, ApplicationGuid, [Name] FROM FolderTypes", ref errorMessage))
                    {
                        if (DataSetUtility.ValidateQueryResults(dataSet, out var queryResults))
                        {
                            result = DataSetUtility.ToDictionaryList(queryResults);

                            if (result?.Count > 0)
                            {
                                foreach (var folderType in result.Where(folderType => (folderType?.Count > 0)))
                                {
                                    var folderTypeGuid = SafeConvert.ToGuid(GetValue(folderType, "FolderTypeGuid"));
                                    var applicationGuid = SafeConvert.ToGuid(GetValue(folderType, "ApplicationGuid"));
                                    var name = SafeConvert.ToString(GetValue(folderType, "Name"));

                                    if ((folderTypeGuid != Guid.Empty) &&
                                        (applicationGuid != Guid.Empty) &&
                                        !string.IsNullOrEmpty(name))
                                    {
                                        var databaseStoreGuid = GetDatabaseStoreGuid(applicationGuid, folderTypeGuid, ref errorMessage);

                                        if (databaseStoreGuid != Guid.Empty)
                                        {
                                            if (GetDefaultDatabaseInformation(databaseStoreGuid, out string defaultDatabaseServer, out string defaultDatabaseName, ref errorMessage))
                                            {
                                                folderType.Add("DefaultDatabaseServer", defaultDatabaseServer);
                                                folderType.Add("DefaultDatabaseName", defaultDatabaseName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                else
                {
                    errorMessage = "Not connected to database";
                }
            }

            catch (Exception ex)
            {
                errorMessage = ex.Message;

                Logger.Error(ex);
            }

            return result;
        }

        public static Dictionary<string, string> GetMainDatabaseUpdateCommands()
        {
            var result = new Dictionary<string, string>();

            GetUpdateApplicationInfoCommands(result);
            GetUpdateApplicationsCommands(result);
            GetUpdateAttachmentTypesCommands(result);
            GetUpdateAuditTypesCommands(result);
            GetUpdateContactsCommands(result);
            GetUpdateCustomAnnotationsCommands(result);
            GetUpdateCustomAnnotationsShareCommands(result);
            GetUpdateDatabaseStoresCommands(result);
            GetUpdateDataPoliciesCommands(result);
            GetUpdateFavoritesCommands(result);
            GetUpdateFieldsCommands(result);
            GetUpdateFileStoresCommands(result);
            GetUpdateFolderIndexCommands(result);
            GetUpdateFolderTypesCommands(result);
            GetUpdateGroupMembersCommands(result);
            GetUpdateGroupsCommands(result);
            GetUpdateLabeledAttachmentIndexCommands(result);
            GetUpdateLabelsCommands(result);
            GetUpdateLinksCommands(result);
            GetUpdateLinkTypesCommands(result);
            GetUpdateMessageQueueCommands(result);
            GetUpdateMessageTemplatesCommands(result);
            GetUpdateNotificationTemplatesCommands(result);
            GetUpdateObjectAccessCommands(result);
            GetUpdateObjectAccessPermissionsCommands(result);
            GetUpdatePartiesCommands(result);
            GetUpdatePrioritiesCommands(result);
            GetUpdatePropertiesCommands(result);
            GetUpdateSavedSearchesCommands(result);
            GetUpdateSavedSearchesShareCommands(result);
            GetUpdateSecurityFunctionsCommands(result);
            GetUpdateSecurityInfoCommands(result);
            GetUpdateSecurityPrivilegesCommands(result);
            GetUpdateStatusesCommands(result);
            GetUpdateStoragePlansCommands(result);
            GetUpdateSystemMessagesCommands(result);
            GetUpdateTaskIndexCommands(result);
            GetUpdateTaskListsCommands(result);
            GetUpdateTaskListsTemplatesCommands(result);
            GetUpdateTemplatedObjectsCommands(result);
            GetUpdateUserCustomAnnotationsCommands(result);
            GetUpdateUsersCommands(result);
            GetUpdateUserSavedSearchesCommands(result);
            GetUpdateWorkFlowRulesCommands(result);

            return result;
        }

        public static Dictionary<Guid, Dictionary<string, string>> GetDefaultDatabaseUpdateCommands(List<Dictionary<string, object>> folderTypes)
        {
            var result = new Dictionary<Guid, Dictionary<string, string>>();

            if (folderTypes?.Count > 0)
            {
                foreach (var folderType in folderTypes.Where(folderType => (folderType?.Count > 0)))
                {
                    var folderTypeGuid = SafeConvert.ToGuid(GetValue(folderType, "FolderTypeGuid"));
                    var applicationGuid = SafeConvert.ToGuid(GetValue(folderType, "ApplicationGuid"));
                    var name = SafeConvert.ToString(GetValue(folderType, "Name"));
                    var defaultDatabaseServer = SafeConvert.ToString(GetValue(folderType, "DefaultDatabaseServer"));
                    var defaultDatabaseName = SafeConvert.ToString(GetValue(folderType, "DefaultDatabaseName"));

                    if ((folderTypeGuid != Guid.Empty) &&
                        (applicationGuid != Guid.Empty) &&
                        !string.IsNullOrEmpty(name) &&
                        !string.IsNullOrEmpty(defaultDatabaseServer) &&
                        !string.IsNullOrEmpty(defaultDatabaseName))
                    {
                        var folderTypeCommands = new Dictionary<string, string>();

                        GetUpdateAnnotationsCommands(name, folderTypeCommands);
                        GetUpdateAppliedContactsCommands(name, folderTypeCommands);
                        GetUpdateAttachmentsCommands(name, folderTypeCommands);
                        GetUpdateFoldersCommands(name, folderTypeCommands);
                        GetUpdateMessagesCommands(name, folderTypeCommands);
                        GetUpdateNotesCommands(name, folderTypeCommands);
                        GetUpdateTasksCommands(name, folderTypeCommands);

                        result.Add(folderTypeGuid, folderTypeCommands);
                    }
                }
            }

            return result;
        }

        public static Dictionary<string, string> GetMainDatabaseRollbackCommands()
        {
            var result = new Dictionary<string, string>();

            GetRollbackApplicationInfoCommands(result);
            GetRollbackApplicationsCommands(result);
            GetRollbackAttachmentTypesCommands(result);
            GetRollbackAuditTypesCommands(result);
            GetRollbackContactsCommands(result);
            GetRollbackCustomAnnotationsCommands(result);
            GetRollbackCustomAnnotationsShareCommands(result);
            GetRollbackDatabaseStoresCommands(result);
            GetRollbackDataPoliciesCommands(result);
            GetRollbackFavoritesCommands(result);
            GetRollbackFieldsCommands(result);
            GetRollbackFileStoresCommands(result);
            GetRollbackFolderIndexCommands(result);
            GetRollbackFolderTypesCommands(result);
            GetRollbackGroupMembersCommands(result);
            GetRollbackGroupsCommands(result);
            GetRollbackLabeledAttachmentIndexCommands(result);
            GetRollbackLabelsCommands(result);
            GetRollbackLinksCommands(result);
            GetRollbackLinkTypesCommands(result);
            GetRollbackMessageQueueCommands(result);
            GetRollbackMessageTemplatesCommands(result);
            GetRollbackNotificationTemplatesCommands(result);
            GetRollbackObjectAccessCommands(result);
            GetRollbackObjectAccessPermissionsCommands(result);
            GetRollbackPartiesCommands(result);
            GetRollbackPrioritiesCommands(result);
            GetRollbackPropertiesCommands(result);
            GetRollbackSavedSearchesCommands(result);
            GetRollbackSavedSearchesShareCommands(result);
            GetRollbackSecurityFunctionsCommands(result);
            GetRollbackSecurityInfoCommands(result);
            GetRollbackSecurityPrivilegesCommands(result);
            GetRollbackStatusesCommands(result);
            GetRollbackStoragePlansCommands(result);
            GetRollbackSystemMessagesCommands(result);
            GetRollbackTaskIndexCommands(result);
            GetRollbackTaskListsCommands(result);
            GetRollbackTaskListsTemplatesCommands(result);
            GetRollbackTemplatedObjectsCommands(result);
            GetRollbackUserCustomAnnotationsCommands(result);
            GetRollbackUsersCommands(result);
            GetRollbackUserSavedSearchesCommands(result);
            GetRollbackWorkFlowRulesCommands(result);

            return result;
        }

        public static Dictionary<Guid, Dictionary<string, string>> GetDefaultDatabaseRollbackCommands(List<Dictionary<string, object>> folderTypes)
        {
            var result = new Dictionary<Guid, Dictionary<string, string>>();

            if (folderTypes?.Count > 0)
            {
                foreach (var folderType in folderTypes.Where(folderType => (folderType?.Count > 0)))
                {
                    var folderTypeGuid = SafeConvert.ToGuid(GetValue(folderType, "FolderTypeGuid"));
                    var applicationGuid = SafeConvert.ToGuid(GetValue(folderType, "ApplicationGuid"));
                    var name = SafeConvert.ToString(GetValue(folderType, "Name"));
                    var defaultDatabaseServer = SafeConvert.ToString(GetValue(folderType, "DefaultDatabaseServer"));
                    var defaultDatabaseName = SafeConvert.ToString(GetValue(folderType, "DefaultDatabaseName"));

                    if ((folderTypeGuid != Guid.Empty) &&
                        (applicationGuid != Guid.Empty) &&
                        !string.IsNullOrEmpty(name) &&
                        !string.IsNullOrEmpty(defaultDatabaseServer) &&
                        !string.IsNullOrEmpty(defaultDatabaseName))
                    {
                        var folderTypeCommands = new Dictionary<string, string>();

                        GetRollbackAnnotationsCommands(name, folderTypeCommands);
                        GetRollbackAppliedContactsCommands(name, folderTypeCommands);
                        GetRollbackAttachmentsCommands(name, folderTypeCommands);
                        GetRollbackFoldersCommands(name, folderTypeCommands);
                        GetRollbackMessagesCommands(name, folderTypeCommands);
                        GetRollbackNotesCommands(name, folderTypeCommands);
                        GetRollbackTasksCommands(name, folderTypeCommands);

                        result.Add(folderTypeGuid, folderTypeCommands);
                    }
                }
            }

            return result;
        }

        public static int GetDefaultDatabaseCommandCount(Dictionary<Guid, Dictionary<string, string>> defaultDatabaseCommands)
        {
            return (defaultDatabaseCommands?.Count > 0) ? defaultDatabaseCommands.Keys.Where(folderTypeGuid => defaultDatabaseCommands[folderTypeGuid] != null).Sum(folderTypeGuid => defaultDatabaseCommands[folderTypeGuid].Count) : 0;
        }

        public static string GetValue(IDictionary<string, object> parameters, string key)
        {
            var result = "";

            try
            {
                var lowerCaseParameters = LowerCaseKeys(parameters);
                var lowerCaseKey = key?.ToLower() ?? "";

                if (!string.IsNullOrEmpty(lowerCaseKey) &&
                    (lowerCaseParameters?.Count > 0) &&
                    lowerCaseParameters.ContainsKey(lowerCaseKey) &&
                    (lowerCaseParameters[lowerCaseKey] != null))
                {
                    result = SafeConvert.ToString(lowerCaseParameters[lowerCaseKey]);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;
        }

        public static Dictionary<string, object> LowerCaseKeys(IDictionary<string, object> dictionary)
        {
            var result = new Dictionary<string, object>();

            try
            {
                if ((dictionary != null) &&
                    (dictionary.Count > 0))
                {
                    foreach (var item in dictionary)
                    {
                        result.Add(item.Key.ToLower(), item.Value);
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;
        }

        #endregion Public Methods


        #region Schema Update Content Methods

        private static void GetUpdateApplicationInfoCommands(IDictionary<string, string> commands)
        {
            const string tableName = "ApplicationInfo";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
        }

        private static void GetUpdateApplicationsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Applications";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
        }

        private static void GetUpdateAttachmentTypesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "AttachmentTypes";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKAttachmentGuidParentGuid", GetDropConstraintCommand(tableName, "PKAttachmentGuidParentGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"AttachmentTypeGuid", "ParentGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"AttachmentTypeGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdateAuditTypesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "AuditTypes";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKAuditTypeGuid", GetDropConstraintCommand(tableName, "PKAuditTypeGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"AuditTypeGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"ApplicationGuid"}));
        }

        private static void GetUpdateContactsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Contacts";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKContactGuid", GetDropConstraintCommand(tableName, "PKContactGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"ContactGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdateCustomAnnotationsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "CustomAnnotations";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKCustomAnnotationGuid", GetDropConstraintCommand(tableName, "PKCustomAnnotationGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"CustomAnnotationGuid"}));
        }

        private static void GetUpdateCustomAnnotationsShareCommands(IDictionary<string, string> commands)
        {
            const string tableName = "CustomAnnotationsShare";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"CustomAnnotationGuid", "UserGuid"}));
        }

        private static void GetUpdateDatabaseStoresCommands(IDictionary<string, string> commands)
        {
            const string tableName = "DatabaseStores";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKDatabaseStoreGuid", GetDropConstraintCommand(tableName, "PKDatabaseStoreGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"DatabaseStoreGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"ApplicationGuid"}));
        }

        private static void GetUpdateDataPoliciesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "DataPolicies";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"DataPolicyGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"ApplicationGuid"}));
        }

        private static void GetUpdateFavoritesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Favorites";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"UserGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdateFieldsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Fields";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKFieldGuidParentGuid", GetDropConstraintCommand(tableName, "PKFieldGuidParentGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"FieldGuid", "ParentGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"FieldGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdateFileStoresCommands(IDictionary<string, string> commands)
        {
            const string tableName = "FileStores";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKFileStoreGuid", GetDropConstraintCommand(tableName, "PKFileStoreGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> { "FileStoreGuid", "ApplicationGuid" }));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> { "FileStoreGuid" }));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> { "ApplicationGuid" }));
        }

        private static void GetUpdateFolderIndexCommands(IDictionary<string, string> commands)
        {
            const string tableName = "FolderIndex";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKFolderIndex", GetDropConstraintCommand(tableName, "PKFolderIndex"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"FolderGuid", "ApplicationGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"FolderGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ApplicationGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 4, new List<string> {"FCN"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 5, new List<string> {"FolderGuid", "FCN"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 6, new List<string> {"FolderGuid", "ApplicationGuid", "FolderTypeGuid", "StatusGuid", "PriorityGuid", "FCN"}));
        }

        private static void GetUpdateFolderTypesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "FolderTypes";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKFolderTypeGuid", GetDropConstraintCommand(tableName, "PKFolderTypeGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"FolderTypeGuid", "ApplicationGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"FolderTypeGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ApplicationGuid"}));
        }

        private static void GetUpdateGroupMembersCommands(IDictionary<string, string> commands)
        {
            const string tableName = "GroupMembers";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKGroupMemberGuid", GetDropConstraintCommand(tableName, "PKGroupMemberGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"MemberGuid", "GroupGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"MemberGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"GroupGuid"}));
        }

        private static void GetUpdateGroupsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Groups";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKGroupGuid", GetDropConstraintCommand(tableName, "PKGroupGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"GroupGuid"}));
        }

        private static void GetUpdateLabeledAttachmentIndexCommands(IDictionary<string, string> commands)
        {
            const string tableName = "LabeledAttachmentIndex";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKLabeledAttachIndexGuids", GetDropConstraintCommand(tableName, "PKLabeledAttachIndexGuids"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"LabelGuid", "AttachmentGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"AttachmentGuid", "FCN"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"FCN"}));
        }

        private static void GetUpdateLabelsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Labels";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKLabelGuidParentGuid", GetDropConstraintCommand(tableName, "PKLabelGuidParentGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"LabelGuid", "ParentGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"LabelGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdateLinksCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Links";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKLinkGuid", GetDropConstraintCommand(tableName, "PKLinkGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"LinkGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"SourceFolderGuid"}));
        }

        private static void GetUpdateLinkTypesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "LinkTypes";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKLinkTypeGuid", GetDropConstraintCommand(tableName, "PKLinkTypeGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"LinkTypeGuid"}));
        }

        private static void GetUpdateMessageQueueCommands(IDictionary<string, string> commands)
        {
            const string tableName = "MessageQueue";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKMessageQueue", GetDropConstraintCommand(tableName, "PKMessageQueue"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"MessageGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdateMessageTemplatesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "MessageTemplates";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKMessageTemplateGuid", GetDropConstraintCommand(tableName, "PKMessageTemplateGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"MessageTemplateGuid", "ParentGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"MessageTemplateGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdateNotificationTemplatesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "NotificationTemplates";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKNotificationTemplates", GetDropConstraintCommand(tableName, "PKNotificationTemplates"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"TemplateGuid", "ParentGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"TemplateGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdateObjectAccessCommands(IDictionary<string, string> commands)
        {
            const string tableName = "ObjectAccess";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKObjectAccessParentGuid", GetDropConstraintCommand(tableName, "PKObjectAccessParentGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdateObjectAccessPermissionsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "ObjectAccessPermissions";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKAccessPermissionsGuidsAndType", GetDropConstraintCommand(tableName, "PKAccessPermissionsGuidsAndType"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"ParentGuid", "MemberGuid", "AccessType"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"MemberGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdatePartiesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Parties";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKPartyGuid", GetDropConstraintCommand(tableName, "PKPartyGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"PartyGuid", "ParentGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"PartyGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdatePrioritiesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Priorities";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKPriorityGuid", GetDropConstraintCommand(tableName, "PKPriorityGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"PriorityGuid", "ParentGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"PriorityGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdatePropertiesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Properties";

            AddCommand(commands, CommandType.DropConstraint, tableName, "PKPropertiesOwnerParentPropertyName", GetDropConstraintCommand(tableName, "PKPropertiesOwnerParentPropertyName"));
            AddCommand(commands, CommandType.AddConstraint, tableName, "PK_Properties", GetAddPrimaryKeyConstraintCommand(tableName, "PropertyId", "PK_Properties"));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"OwnerGuid", "ParentGuid", "Name"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"OwnerGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdateSavedSearchesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "SavedSearches";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKSavedSearchGuid", GetDropConstraintCommand(tableName, "PKSavedSearchGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"SavedSearchGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"UserGuid"}));
        }

        private static void GetUpdateSavedSearchesShareCommands(IDictionary<string, string> commands)
        {
            const string tableName = "SavedSearchesShare";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"SavedSearchGuid", "UserGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"SavedSearchGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"UserGuid"}));
        }

        private static void GetUpdateSecurityFunctionsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "SecurityFunctions";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKSecurityFunctionGuid", GetDropConstraintCommand(tableName, "PKSecurityFunctionGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"FunctionGuid"}));
        }

        private static void GetUpdateSecurityInfoCommands(IDictionary<string, string> commands)
        {
            const string tableName = "SecurityInfo";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"CurrentSecurityKeyId", "NewSecurityKeyId", "LastSecurityKeyId"}));
        }

        private static void GetUpdateSecurityPrivilegesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "SecurityPrivileges";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKSecurityPrivilegeGuids", GetDropConstraintCommand(tableName, "PKSecurityPrivilegeGuids"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"MemberGuid", "FunctionGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"MemberGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"FunctionGuid"}));
        }

        private static void GetUpdateStatusesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Statuses";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKStatusGuid", GetDropConstraintCommand(tableName, "PKStatusGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"StatusGuid", "ParentGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"StatusGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdateStoragePlansCommands(IDictionary<string, string> commands)
        {
            const string tableName = "StoragePlans";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKStoragePlanGuid", GetDropConstraintCommand(tableName, "PKStoragePlanGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"StoragePlanGuid", "ApplicationGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"StoragePlanGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ApplicationGuid"}));
        }

        private static void GetUpdateSystemMessagesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "SystemMessages";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKSystemMessageGuid", GetDropConstraintCommand(tableName, "PKSystemMessageGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"SystemMessageGuid"}));
        }

        private static void GetUpdateTaskIndexCommands(IDictionary<string, string> commands)
        {
            const string tableName = "TaskIndex";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"TaskGuid", "ParentGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"TaskGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 4, new List<string> {"TaskGuid", "ParentGuid", "StatusGuid", "PriorityGuid"}));
        }

        private static void GetUpdateTaskListsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "TaskLists";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKTaskListObjectGuid", GetDropConstraintCommand(tableName, "PKTaskListObjectGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"TaskListObjectGuid", "ParentGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"TaskListObjectGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdateTaskListsTemplatesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "TaskListsTemplates";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKTaskListsTemplates", GetDropConstraintCommand(tableName, "PKTaskListsTemplates"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"TaskListObjectGuid", "TaskTemplateObjectGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"TaskListObjectGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"TaskTemplateObjectGuid"}));
        }

        private static void GetUpdateTemplatedObjectsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "TemplatedObjects";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKTemplatedObjectGuid", GetDropConstraintCommand(tableName, "PKTemplatedObjectGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"TemplatedObjectGuid", "ParentGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"TemplatedObjectGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
        }

        private static void GetUpdateUserCustomAnnotationsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "UserCustomAnnotations";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"CustomAnnotationGuid", "UserGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"CustomAnnotationGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"UserGuid"}));
        }

        private static void GetUpdateUsersCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Users";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKUserGuid", GetDropConstraintCommand(tableName, "PKUserGuid"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"UserGuid"}));
        }

        private static void GetUpdateUserSavedSearchesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "UserSavedSearches";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"SavedSearchGuid", "UserGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"SavedSearchGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"UserGuid"}));
        }

        private static void GetUpdateWorkFlowRulesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "WorkFlowRules";

            AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PKWorkFlowRules", GetDropConstraintCommand(tableName, "PKWorkFlowRules"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"RuleGuid", "FolderTypeGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"RuleGuid"}));
            AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"FolderTypeGuid"}));
        }

        public static void GetUpdateAnnotationsCommands(string folderTypeName, IDictionary<string, string> commands)
        {
            if (!string.IsNullOrEmpty(folderTypeName))
            {
                var tableName = $"Custom{folderTypeName}Annotations";
                var primaryKeyConstraintName = $"PKCustom{folderTypeName}AnnotationsGuid";

                AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
                AddCommand(commands, CommandType.DropConstraint, tableName, primaryKeyConstraintName, GetDropConstraintCommand(tableName, primaryKeyConstraintName));
                AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"AnnotationGuid", "AttachmentGuid"}));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"AnnotationGuid"}));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"AttachmentGuid"}));
            }
        }

        public static void GetUpdateAppliedContactsCommands(string folderTypeName, IDictionary<string, string> commands)
        {
            if (!string.IsNullOrEmpty(folderTypeName))
            {
                var tableName = $"Custom{folderTypeName}AppliedContacts";
                var primaryKeyConstraintName = $"PKCustom{folderTypeName}AppliedContacts";

                AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
                AddCommand(commands, CommandType.DropConstraint, tableName, primaryKeyConstraintName, GetDropConstraintCommand(tableName, primaryKeyConstraintName));
                AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"FolderGuid", "AppliedContactGuid", "ParentGuid", "ParentType"}));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"AppliedContactGuid"}));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid", "ParentType"}));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 4, new List<string> {"ParentGuid"}));
            }
        }

        public static void GetUpdateAttachmentsCommands(string folderTypeName, IDictionary<string, string> commands)
        {
            if (!string.IsNullOrEmpty(folderTypeName))
            {
                var tableName = $"Custom{folderTypeName}Attachments";
                var primaryKeyConstraintName = $"PKCustom{folderTypeName}AttachmentGuid";

                AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
                AddCommand(commands, CommandType.DropConstraint, tableName, primaryKeyConstraintName, GetDropConstraintCommand(tableName, primaryKeyConstraintName));
                AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"AttachmentGuid", "ParentGuid"}));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"AttachmentGuid"}));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
            }
        }

        public static void GetUpdateFoldersCommands(string folderTypeName, IDictionary<string, string> commands)
        {
            if (!string.IsNullOrEmpty(folderTypeName))
            {
                var tableName = $"Custom{folderTypeName}Folders";
                var primaryKeyConstraintName = $"PKCustom{folderTypeName}FolderGuid";

                AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
                AddCommand(commands, CommandType.DropConstraint, tableName, primaryKeyConstraintName, GetDropConstraintCommand(tableName, primaryKeyConstraintName));
                AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"FolderGuid", "FolderTypeGuid"}));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"FolderGuid"}));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"FolderTypeGuid"}));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 4, new List<string> {"FCN"}));
            }
        }

        public static void GetUpdateMessagesCommands(string folderTypeName, IDictionary<string, string> commands)
        {
            if (!string.IsNullOrEmpty(folderTypeName))
            {
                var tableName = $"Custom{folderTypeName}Messages";
                var primaryKeyConstraintName = $"PKCustom{folderTypeName}MessageGuid";

                AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
                AddCommand(commands, CommandType.DropConstraint, tableName, primaryKeyConstraintName, GetDropConstraintCommand(tableName, primaryKeyConstraintName));
                AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"MessageGuid", "ParentGuid"}));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"MessageGuid"}));
            }
        }

        public static void GetUpdateNotesCommands(string folderTypeName, IDictionary<string, string> commands)
        {
            if (!string.IsNullOrEmpty(folderTypeName))
            {
                var tableName = $"Custom{folderTypeName}Notes";
                var primaryKeyConstraintName = $"PKCustom{folderTypeName}NoteGuid";

                AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
                AddCommand(commands, CommandType.DropConstraint, tableName, primaryKeyConstraintName, GetDropConstraintCommand(tableName, primaryKeyConstraintName));
                AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"NoteGuid", "ParentGuid"}));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"NoteGuid"}));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
            }
        }

        public static void GetUpdateTasksCommands(string folderTypeName, IDictionary<string, string> commands)
        {
            if (!string.IsNullOrEmpty(folderTypeName))
            {
                var tableName = $"Custom{folderTypeName}Tasks";
                var primaryKeyConstraintName = $"PKCustom{folderTypeName}TaskGuid";

                AddCommand(commands, CommandType.AddIdIdentity, tableName, "", GetAddIdIdentityCommand(tableName));
                AddCommand(commands, CommandType.DropConstraint, tableName, primaryKeyConstraintName, GetDropConstraintCommand(tableName, primaryKeyConstraintName));
                AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "", GetAddPrimaryKeyConstraintCommand(tableName));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 1, new List<string> {"TaskGuid", "ParentGuid"}));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 2, new List<string> {"TaskGuid"}));
                AddCommand(commands, CommandType.CreateNonClusteredIndex, tableName, "", GetCreateNonClusteredIndexCommand(tableName, 3, new List<string> {"ParentGuid"}));
            }
        }

        #endregion Schema Update Content Methods


        #region Schema Rollback Content Methods

        private static void GetRollbackApplicationInfoCommands(IDictionary<string, string> commands)
        {
            const string tableName = "ApplicationInfo";

            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
        }

        private static void GetRollbackApplicationsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Applications";

            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
        }

        private static void GetRollbackAttachmentTypesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "AttachmentTypes";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKAttachmentGuidParentGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKAttachmentGuidParentGuid", new List<string>{ "AttachmentTypeGuid", "ParentGuid" }));
        }

        private static void GetRollbackAuditTypesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "AuditTypes";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKAuditTypeGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKAuditTypeGuid", new List<string>{ "AuditTypeGuid" }));
        }

        private static void GetRollbackContactsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Contacts";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKContactGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKContactGuid", new List<string> { "ContactGuid" }));
        }

        private static void GetRollbackCustomAnnotationsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "CustomAnnotations";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKCustomAnnotationGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKCustomAnnotationGuid", new List<string> { "CustomAnnotationGuid" }));
        }

        private static void GetRollbackCustomAnnotationsShareCommands(IDictionary<string, string> commands)
        {
            const string tableName = "CustomAnnotationsShare";

            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
        }

        private static void GetRollbackDatabaseStoresCommands(IDictionary<string, string> commands)
        {
            const string tableName = "DatabaseStores";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKDatabaseStoreGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKDatabaseStoreGuid", new List<string> { "DatabaseStoreGuid" }));
        }

        private static void GetRollbackDataPoliciesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "DataPolicies";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
        }

        private static void GetRollbackFavoritesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Favorites";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
        }

        private static void GetRollbackFieldsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Fields";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKFieldGuidParentGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKFieldGuidParentGuid", new List<string> { "FieldGuid", "ParentGuid" }));
        }

        private static void GetRollbackFileStoresCommands(IDictionary<string, string> commands)
        {
            const string tableName = "FileStores";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKFileStoreGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKFileStoreGuid", new List<string> { "FileStoreGuid" }));
        }

        private static void GetRollbackFolderIndexCommands(IDictionary<string, string> commands)
        {
            const string tableName = "FolderIndex";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 4));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 5));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 6));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKFolderIndex", GetAddPrimaryKeyConstraintCommand(tableName, "PKFolderIndex", new List<string> { "FolderGuid" }));
        }

        private static void GetRollbackFolderTypesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "FolderTypes";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKFolderTypeGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKFolderTypeGuid", new List<string> { "FolderTypeGuid" }));
        }

        private static void GetRollbackGroupMembersCommands(IDictionary<string, string> commands)
        {
            const string tableName = "GroupMembers";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKGroupMemberGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKGroupMemberGuid", new List<string> { "MemberGuid", "GroupGuid" }));
        }

        private static void GetRollbackGroupsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Groups";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKGroupGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKGroupGuid", new List<string> { "GroupGuid" }));
        }

        private static void GetRollbackLabeledAttachmentIndexCommands(IDictionary<string, string> commands)
        {
            const string tableName = "LabeledAttachmentIndex";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKLabeledAttachIndexGuids", GetAddPrimaryKeyConstraintCommand(tableName, "PKLabeledAttachIndexGuids", new List<string> { "LabelGuid", "AttachmentGuid" }));
        }

        private static void GetRollbackLabelsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Labels";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKLabelGuidParentGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKLabelGuidParentGuid", new List<string> { "LabelGuid", "ParentGuid" }));
        }

        private static void GetRollbackLinksCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Links";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKLinkGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKLinkGuid", new List<string> { "LinkGuid" }));
        }

        private static void GetRollbackLinkTypesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "LinkTypes";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKLinkTypeGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKLinkTypeGuid", new List<string> { "LinkTypeGuid" }));
        }

        private static void GetRollbackMessageQueueCommands(IDictionary<string, string> commands)
        {
            const string tableName = "MessageQueue";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKMessageQueue", GetAddPrimaryKeyConstraintCommand(tableName, "PKMessageQueue", new List<string> { "MessageGuid" }));
        }

        private static void GetRollbackMessageTemplatesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "MessageTemplates";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKMessageTemplateGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKMessageTemplateGuid", new List<string> { "MessageTemplateGuid" }));
        }

        private static void GetRollbackNotificationTemplatesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "NotificationTemplates";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKNotificationTemplates", GetAddPrimaryKeyConstraintCommand(tableName, "PKNotificationTemplates", new List<string> { "TemplateGuid" }));
        }

        private static void GetRollbackObjectAccessCommands(IDictionary<string, string> commands)
        {
            const string tableName = "ObjectAccess";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKObjectAccessParentGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKObjectAccessParentGuid", new List<string> { "ParentGuid" }));
        }

        private static void GetRollbackObjectAccessPermissionsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "ObjectAccessPermissions";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKAccessPermissionsGuidsAndType", GetAddPrimaryKeyConstraintCommand(tableName, "PKAccessPermissionsGuidsAndType", new List<string> { "ParentGuid", "MemberGuid", "AccessType" }));
        }

        private static void GetRollbackPartiesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Parties";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKPartyGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKPartyGuid", new List<string> { "PartyGuid" }));
        }

        private static void GetRollbackPrioritiesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Priorities";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKPriorityGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKPriorityGuid", new List<string> { "PriorityGuid" }));
        }

        private static void GetRollbackPropertiesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Properties";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropConstraint, tableName, "PK_Properties", GetDropConstraintCommand(tableName, "PK_Properties"));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKPropertiesOwnerParentPropertyName", GetAddPrimaryKeyConstraintCommand(tableName, "PKPropertiesOwnerParentPropertyName", new List<string> { "OwnerGuid", "ParentGuid", "Name" }));
        }

        private static void GetRollbackSavedSearchesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "SavedSearches";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKSavedSearchGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKSavedSearchGuid", new List<string> { "SavedSearchGuid" }));
        }

        private static void GetRollbackSavedSearchesShareCommands(IDictionary<string, string> commands)
        {
            const string tableName = "SavedSearchesShare";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
        }

        private static void GetRollbackSecurityFunctionsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "SecurityFunctions";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKSecurityFunctionGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKSecurityFunctionGuid", new List<string> { "FunctionGuid" }));
        }

        private static void GetRollbackSecurityInfoCommands(IDictionary<string, string> commands)
        {
            const string tableName = "SecurityInfo";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
        }

        private static void GetRollbackSecurityPrivilegesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "SecurityPrivileges";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKSecurityPrivilegeGuids", GetAddPrimaryKeyConstraintCommand(tableName, "PKSecurityPrivilegeGuids", new List<string> { "MemberGuid", "FunctionGuid" }));
        }

        private static void GetRollbackStatusesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Statuses";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKStatusGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKStatusGuid", new List<string> { "StatusGuid" }));
        }

        private static void GetRollbackStoragePlansCommands(IDictionary<string, string> commands)
        {
            const string tableName = "StoragePlans";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKStoragePlanGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKStoragePlanGuid", new List<string> { "StoragePlanGuid" }));
        }

        private static void GetRollbackSystemMessagesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "SystemMessages";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKSystemMessageGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKSystemMessageGuid", new List<string> { "SystemMessageGuid" }));
        }

        private static void GetRollbackTaskIndexCommands(IDictionary<string, string> commands)
        {
            const string tableName = "TaskIndex";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 4));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
        }

        private static void GetRollbackTaskListsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "TaskLists";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKTaskListObjectGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKTaskListObjectGuid", new List<string> { "TaskListObjectGuid" }));
        }

        private static void GetRollbackTaskListsTemplatesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "TaskListsTemplates";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKTaskListsTemplates", GetAddPrimaryKeyConstraintCommand(tableName, "PKTaskListsTemplates", new List<string> { "TaskListObjectGuid", "TaskTemplateObjectGuid" }));
        }

        private static void GetRollbackTemplatedObjectsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "TemplatedObjects";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKTemplatedObjectGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKTemplatedObjectGuid", new List<string> { "TemplatedObjectGuid" }));
        }

        private static void GetRollbackUserCustomAnnotationsCommands(IDictionary<string, string> commands)
        {
            const string tableName = "UserCustomAnnotations";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
        }

        private static void GetRollbackUsersCommands(IDictionary<string, string> commands)
        {
            const string tableName = "Users";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKUserGuid", GetAddPrimaryKeyConstraintCommand(tableName, "PKUserGuid", new List<string> { "UserGuid" }));
        }

        private static void GetRollbackUserSavedSearchesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "UserSavedSearches";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
        }

        private static void GetRollbackWorkFlowRulesCommands(IDictionary<string, string> commands)
        {
            const string tableName = "WorkFlowRules";

            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
            AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
            AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
            AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
            AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, "PKWorkFlowRules", GetAddPrimaryKeyConstraintCommand(tableName, "PKWorkFlowRules", new List<string> { "RuleGuid" }));
        }

        public static void GetRollbackAnnotationsCommands(string folderTypeName, IDictionary<string, string> commands)
        {
            if (!string.IsNullOrEmpty(folderTypeName))
            {
                var tableName = $"Custom{folderTypeName}Annotations";
                var primaryKeyConstraintName = $"PKCustom{folderTypeName}AnnotationsGuid";

                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
                AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
                AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
                AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, primaryKeyConstraintName, GetAddPrimaryKeyConstraintCommand(tableName, primaryKeyConstraintName, new List<string> { "AnnotationGuid" }));
            }
        }

        public static void GetRollbackAppliedContactsCommands(string folderTypeName, IDictionary<string, string> commands)
        {
            if (!string.IsNullOrEmpty(folderTypeName))
            {
                var tableName = $"Custom{folderTypeName}AppliedContacts";
                var primaryKeyConstraintName = $"PKCustom{folderTypeName}AppliedContacts";

                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 4));
                AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
                AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
                AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, primaryKeyConstraintName, GetAddPrimaryKeyConstraintCommand(tableName, primaryKeyConstraintName, new List<string> { "FolderGuid", "AppliedContactGuid", "ParentGuid" }));
            }
        }

        public static void GetRollbackAttachmentsCommands(string folderTypeName, IDictionary<string, string> commands)
        {
            if (!string.IsNullOrEmpty(folderTypeName))
            {
                var tableName = $"Custom{folderTypeName}Attachments";
                var primaryKeyConstraintName = $"PKCustom{folderTypeName}AttachmentGuid";

                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
                AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
                AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
                AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, primaryKeyConstraintName, GetAddPrimaryKeyConstraintCommand(tableName, primaryKeyConstraintName, new List<string> { "AttachmentGuid" }));
            }
        }

        public static void GetRollbackFoldersCommands(string folderTypeName, IDictionary<string, string> commands)
        {
            if (!string.IsNullOrEmpty(folderTypeName))
            {
                var tableName = $"Custom{folderTypeName}Folders";
                var primaryKeyConstraintName = $"PKCustom{folderTypeName}FolderGuid";

                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 4));
                AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
                AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
                AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, primaryKeyConstraintName, GetAddPrimaryKeyConstraintCommand(tableName, primaryKeyConstraintName, new List<string> { "FolderGuid" }));
            }
        }

        public static void GetRollbackMessagesCommands(string folderTypeName, IDictionary<string, string> commands)
        {
            if (!string.IsNullOrEmpty(folderTypeName))
            {
                var tableName = $"Custom{folderTypeName}Messages";
                var primaryKeyConstraintName = $"PKCustom{folderTypeName}MessageGuid";

                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
                AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
                AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
                AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, primaryKeyConstraintName, GetAddPrimaryKeyConstraintCommand(tableName, primaryKeyConstraintName, new List<string> { "MessageGuid" }));
            }
        }

        public static void GetRollbackNotesCommands(string folderTypeName, IDictionary<string, string> commands)
        {
            if (!string.IsNullOrEmpty(folderTypeName))
            {
                var tableName = $"Custom{folderTypeName}Notes";
                var primaryKeyConstraintName = $"PKCustom{folderTypeName}NoteGuid";

                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
                AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
                AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
                AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, primaryKeyConstraintName, GetAddPrimaryKeyConstraintCommand(tableName, primaryKeyConstraintName, new List<string> { "NoteGuid" }));
            }
        }

        public static void GetRollbackTasksCommands(string folderTypeName, IDictionary<string, string> commands)
        {
            if (!string.IsNullOrEmpty(folderTypeName))
            {
                var tableName = $"Custom{folderTypeName}Tasks";
                var primaryKeyConstraintName = $"PKCustom{folderTypeName}TaskGuid";

                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 1));
                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 2));
                AddCommand(commands, CommandType.DropNonClusteredIndex, tableName, "", GetDropNonClusteredIndexCommand(tableName, 3));
                AddCommand(commands, CommandType.DropPrimaryKeyConstraint, tableName, "", GetDropPrimaryKeyConstraintCommand(tableName));
                AddCommand(commands, CommandType.DropIdIdentity, tableName, "", GetDropIdIdentityCommand(tableName));
                AddCommand(commands, CommandType.AddPrimaryKeyConstraint, tableName, primaryKeyConstraintName, GetAddPrimaryKeyConstraintCommand(tableName, primaryKeyConstraintName, new List<string> { "TaskGuid" }));
            }
        }

        #endregion Schema Rollback Content Methods


        #region Private Methods

        private static string GetCommandKey(CommandType commandType, string tableName, string constraintName)
        {
            var result = "";

            if (!string.IsNullOrEmpty(tableName))
            {
                switch (commandType)
                {
                    case CommandType.AddIdIdentity:
                        result = $"Adding identity column 'Id' to table '{tableName}'";
                        break;
                    case CommandType.AddPrimaryKeyConstraint:
                        result = !string.IsNullOrEmpty(constraintName) ? $"Adding primary key constraint '{constraintName}' to table '{tableName}'" : $"Adding primary key constraint 'Id' to table '{tableName}'";
                        break;
                    case CommandType.AddConstraint:
                        result = !string.IsNullOrEmpty(constraintName) ? $"Adding constraint '{constraintName}' to table '{tableName}'" : $"Adding constraint to table '{tableName}'";
                        break;
                    case CommandType.DropConstraint:
                        result = !string.IsNullOrEmpty(constraintName) ? $"Dropping constraint '{constraintName}' to table '{tableName}'" : $"Dropping constraint from table '{tableName}'";
                        break;
                    case CommandType.CreateNonClusteredIndex:
                        result = !string.IsNullOrEmpty(constraintName) ? $"Adding non clustered index '{constraintName}' to table '{tableName}'" : $"Adding non clustered index to table '{tableName}'";
                        break;
                    case CommandType.DropIdIdentity:
                        result = $"Dropping identity column 'Id' from table '{tableName}'";
                        break;
                    case CommandType.DropPrimaryKeyConstraint:
                        result = $"Dropping primary key constraint 'Id' from table '{tableName}'";
                        break;
                    case CommandType.DropNonClusteredIndex:
                        result = !string.IsNullOrEmpty(constraintName) ? $"Dropping non clustered index '{constraintName}' to table '{tableName}'" : $"Dropping non clustered index to table '{tableName}'";
                        break;
                }
            }

            return result;
        }

        private static void AddCommand(IDictionary<string, string> commands, CommandType commandType, string tableName, string constraintName, string command)
        {
            if ((commands != null) && 
                !string.IsNullOrEmpty(command) && 
                !string.IsNullOrEmpty(tableName))
            {
                var key = GetCommandKey(commandType, tableName, constraintName);

                if (!commands.ContainsKey(key))
                {
                    commands.Add(key, command);
                }
            }
        }

        private static string GetAddIdIdentityCommand(string tableName)
        {
            return !string.IsNullOrEmpty(tableName) ? $"ALTER TABLE {tableName} ADD Id BIGINT IDENTITY(1,1)" : "";
        }

        private static string GetAddPrimaryKeyConstraintCommand(string tableName)
        {
            return !string.IsNullOrEmpty(tableName) ? $"ALTER TABLE {tableName} ADD CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED ([Id] ASC) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" : "";
        }

        private static string GetAddPrimaryKeyConstraintCommand(string tableName, string columnName, string constraintName)
        {
            return !string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(columnName) && !string.IsNullOrEmpty(constraintName) ? $"ALTER TABLE {tableName} ADD CONSTRAINT [{constraintName}] PRIMARY KEY CLUSTERED ([{columnName}] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)" : "";
        }

        private static string GetDropConstraintCommand(string tableName, string constraintName)
        {
            return !string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(constraintName) ? $"ALTER TABLE {tableName} DROP CONSTRAINT [{constraintName}]" : "";
        }

        private static string GetDropNonClusteredIndexCommand(string tableName, int index)
        {
            return !string.IsNullOrEmpty(tableName) ? $"DROP INDEX [NonClusteredIndex{tableName}Index{index}] ON {tableName}" : "";
        }

        private static string GetDropIdIdentityCommand(string tableName)
        {
            return !string.IsNullOrEmpty(tableName) ? $"ALTER TABLE {tableName} DROP COLUMN Id" : "";
        }

        private static string GetDropPrimaryKeyConstraintCommand(string tableName)
        {
            return !string.IsNullOrEmpty(tableName) ? $"ALTER TABLE {tableName} DROP CONSTRAINT [PK_{tableName}]" : "";
        }

        private static string GetAddPrimaryKeyConstraintCommand(string tableName, string constraintName, IReadOnlyCollection<string> columnNames)
        {
            var result = new StringBuilder();

            if (!string.IsNullOrEmpty(tableName) &&
                !string.IsNullOrEmpty(constraintName) &&
                (columnNames?.Count > 0))
            {
                var columnsClause = new StringBuilder();

                foreach (var columnName in columnNames.Where(columnName => !string.IsNullOrEmpty(columnName)))
                {
                    if (columnsClause.Length > 0)
                    {
                        columnsClause.Append(", ");
                    }

                    columnsClause.Append($"[{columnName}] ASC");
                }

                if (columnsClause.Length > 0)
                {
                    result.Append($"ALTER TABLE {tableName} ADD CONSTRAINT [{constraintName}] PRIMARY KEY CLUSTERED ({columnsClause}) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)");
                }
            }

            return result.ToString();
        }

        private static string GetCreateNonClusteredIndexCommand(string tableName, int index, IReadOnlyCollection<string> columnNames)
        {
            var result = new StringBuilder();

            if (!string.IsNullOrEmpty(tableName) &&
                (index > 0) &&
                (columnNames?.Count > 0))
            {
                var columnsClause = new StringBuilder();

                foreach (var columnName in columnNames.Where(columnName => !string.IsNullOrEmpty(columnName)))
                {
                    if (columnsClause.Length > 0)
                    {
                        columnsClause.Append(", ");
                    }

                    columnsClause.Append($"[{columnName}] ASC");
                }

                if (columnsClause.Length > 0)
                {
                    result.Append($"CREATE NONCLUSTERED INDEX [NonClusteredIndex{tableName}Index{index}] ON {tableName} ({columnsClause}) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)");
                }
            }

            return result.ToString();
        }

        private bool GetDefaultDatabaseInformation(Guid databaseStoreGuid, out string defaultDatabaseServer, out string defaultDatabaseName, ref string errorMessage)
        {
            defaultDatabaseServer = "";
            defaultDatabaseName = "";

            try
            {
                if (_databaseConnection?.Connected() ?? false)
                {
                    if (databaseStoreGuid == Guid.Empty)
                    {
                        errorMessage = "Invalid database store id found while trying to get default database information.";
                    }

                    else
                    {
                        using (var dataSet = _databaseConnection.ExecuteQuery($"SELECT [DatabaseInstance], [DatabaseName] FROM DatabaseStores WHERE [DatabaseStoreGuid] = '{databaseStoreGuid}'", ref errorMessage))
                        {
                            if (!DataSetUtility.ValidateQueryResults(dataSet, out var queryResults))
                            {
                                if (string.IsNullOrEmpty(errorMessage))
                                {
                                    errorMessage = $"Invalid query results found while trying to get default database information for database store id '{databaseStoreGuid}'.";
                                }
                            }

                            else
                            {
                                if (DataSetUtility.ValidateDictionaryList(DataSetUtility.ToDictionaryList(queryResults), out Dictionary<string, object> firstFieldDictionary))
                                {
                                    defaultDatabaseServer = SafeConvert.ToString(GetValue(firstFieldDictionary, "DatabaseInstance"));
                                    defaultDatabaseName = SafeConvert.ToString(GetValue(firstFieldDictionary, "DatabaseName"));

                                    if (!string.IsNullOrEmpty(defaultDatabaseServer))
                                    {
                                        if (defaultDatabaseServer.ToLower().Contains("(local)"))
                                        {
                                            var databasePieces = defaultDatabaseServer.Split('\\');
                                            var defaultDatabasePieces = defaultDatabaseServer.Split('\\');

                                            var newDefaultDatabaseName = new StringBuilder();

                                            var count = databasePieces.Length;

                                            for (var index = 0; index < count; index++)
                                            {
                                                var databasePiece = databasePieces[index].Trim();
                                                var defaultDatabasePiece = defaultDatabasePieces[index].Trim();

                                                if (newDefaultDatabaseName.Length > 0)
                                                {
                                                    newDefaultDatabaseName.Append("\\");
                                                }

                                                if (!string.IsNullOrEmpty(databasePiece) &&
                                                    !string.IsNullOrEmpty(defaultDatabasePiece) &&
                                                    (defaultDatabasePiece.ToLower() == "(local)"))
                                                {
                                                    newDefaultDatabaseName.Append(databasePiece);
                                                }

                                                else if (!string.IsNullOrEmpty(defaultDatabasePiece))
                                                {
                                                    newDefaultDatabaseName.Append(defaultDatabasePiece);
                                                }
                                            }

                                            defaultDatabaseServer = newDefaultDatabaseName.ToString();
                                        }
                                    }
                                }

                                else
                                {
                                    errorMessage = $"Unable to get query results while trying to retrieve default database information for database store id '{databaseStoreGuid}'.";
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return !string.IsNullOrEmpty(defaultDatabaseServer) && !string.IsNullOrEmpty(defaultDatabaseName);
        }

        private Guid GetDatabaseStoreGuid(Guid applicationGuid, Guid folderTypeGuid, ref string errorMessage)
        {
            var result = Guid.Empty;

            try
            {
                if (_databaseConnection?.Connected() ?? false)
                {
                    if (applicationGuid == Guid.Empty)
                    {
                        errorMessage = "Invalid application id found while trying to get default database information.";
                    }

                    else if (folderTypeGuid == Guid.Empty)
                    {
                        errorMessage = "Invalid folder type id found while trying to get default database information.";
                    }

                    else
                    {
                        var success = true;

                        var query = $"SELECT [StoragePlanGuid], [ApplicationGuid], [Name], [EvaluationCriteria], [DatabaseStoreGuid], [FileStoreGuid], [Description] FROM StoragePlans WHERE [ApplicationGuid] = '{applicationGuid}'";

                        using (var dataSet1 = _databaseConnection.ExecuteQuery(query, ref errorMessage))
                        {
                            if (!DataSetUtility.ValidateQueryResults(dataSet1, out var queryResults))
                            {
                                using (var dataSet2 = _databaseConnection.ExecuteQuery($"SELECT DISTINCT [DatabaseStoreGuid] FROM FolderIndex WHERE [ApplicationGuid] = '{applicationGuid}' AND [FolderTypeGuid] = '{folderTypeGuid}'", ref errorMessage))
                                {
                                    if (!DataSetUtility.ValidateQueryResults(dataSet2, out queryResults))
                                    {
                                        if (string.IsNullOrEmpty(errorMessage))
                                        {
                                            errorMessage = $"Unable to find database store for application '{applicationGuid}', folder type '{folderTypeGuid}'. Baseline information query results were invalid.";
                                        }

                                        success = false;
                                    }
                                }
                            }

                            if (success)
                            {
                                if (DataSetUtility.ValidateDictionaryList(DataSetUtility.ToDictionaryList(queryResults), out Dictionary<string, object> firstFieldDictionary))
                                {
                                    result = SafeConvert.ToGuid(GetValue(firstFieldDictionary, "DatabaseStoreGuid"));
                                }

                                else
                                {
                                    errorMessage = $"Unable to find database store for application '{applicationGuid}', folder type '{folderTypeGuid}'. Baseline information query failed.";
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                errorMessage = $"Unable to find database store for application '{applicationGuid}', folder type '{folderTypeGuid}'. Error: {ex.Message}";
            }

            return result;
        }

        #endregion Private Methods
    }
}
