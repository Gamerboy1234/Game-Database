
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GreenFolders.Sql
{
    public class SqlServerConnection
    {
        #region Constants

        private const int ReadBufferPacketSize = 100000;
        private const int MinTimeoutInSeconds = 30;

        #endregion Constants


        #region Private Members

        private string _connectionString = "";
        private int _timeoutInSeconds = MinTimeoutInSeconds;

        #endregion Private Members


        #region Properties

        public string ConnectionString => $"{_connectionString.Trim(';')};Connection Timeout={TimeoutInSeconds}";
        public int TimeoutInSeconds => _timeoutInSeconds < MinTimeoutInSeconds ? MinTimeoutInSeconds : _timeoutInSeconds;
        public bool IsConnected { get; private set; }

        #endregion Properties


        #region Constructors

        public SqlServerConnection()
        {
            IsConnected = false;
        }

        public SqlServerConnection(string connectionString, int timeoutInSeconds)
        {
            try
            {
                Connect(connectionString, timeoutInSeconds);
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion Constructors


        #region Public Methods

        public bool Connect(string connectionString, int timeoutInSeconds)
        {
            try
            {
                _timeoutInSeconds = timeoutInSeconds;

                _connectionString = connectionString;

                if (!string.IsNullOrEmpty(_connectionString))
                {
                    using (var sqlConnection = new SqlConnection(ConnectionString))
                    {
                        sqlConnection.Open();

                        IsConnected = (sqlConnection.State == ConnectionState.Open);

                        sqlConnection.Close();
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex, $"Connection String: {connectionString}");
            }

            return IsConnected;
        }

        public bool Disconnect()
        {
            IsConnected = false;

            return true;
        }

        public bool Connected()
        {
            return IsConnected;
        }

        #endregion Public Methods


        #region Query and Command Methods

        public bool ExecuteCommand(string command, ref string errorMessage)
        {
            var result = false;

            try
            {
                if (!string.IsNullOrEmpty(command))
                {
                    if (!string.IsNullOrEmpty(_connectionString))
                    {
                        using (var sqlConnection = new SqlConnection(ConnectionString))
                        {
                            sqlConnection.Open();

                            if (sqlConnection.State == ConnectionState.Open)
                            {
                                using (var sqlCommand = new SqlCommand(command, sqlConnection))
                                {
                                    sqlCommand.CommandTimeout = TimeoutInSeconds;

                                    sqlCommand.ExecuteNonQuery();

                                    result = true;
                                }
                            }

                            else
                            {
                                errorMessage = "Unable to open connection to database.";
                            }

                            sqlConnection.Close();
                        }
                    }

                    else
                    {
                        errorMessage = "Invalid connection string.";
                    }
                }

                else
                {
                    errorMessage = "Invalid command text.";
                }
            }

            catch (Exception ex)
            {
                errorMessage = ex.Message;

                Logger.Error(ex, $"SQL Command: {command ?? ""}");
            }

            return result;
        }

        public bool ExecuteCommand(string command, ref string errorMessage, out int identityValue, string identityTable = "", string identityColumn = "", string sequenceName = "")
        {
            var result = false;

            identityValue = -1;

            try
            {
                if (!string.IsNullOrEmpty(command))
                {
                    if (!string.IsNullOrEmpty(_connectionString))
                    {
                        using (var sqlConnection = new SqlConnection(ConnectionString))
                        {
                            sqlConnection.Open();

                            if (sqlConnection.State == ConnectionState.Open)
                            {
                                using (var sqlCommand = new SqlCommand(command, sqlConnection))
                                {
                                    sqlCommand.CommandTimeout = TimeoutInSeconds;

                                    sqlCommand.ExecuteNonQuery();

                                    // Get the identity value.

                                    // SCOPE_IDENTITY - Returns the last identity value inserted into an identity column in the same scope. 
                                    // A scope is a module: a stored procedure, trigger, function, or batch. 
                                    // Therefore, two statements are in the same scope if they are in the same stored procedure, function, or batch.

                                    // IDENT_CURRENT
                                    // Returns the last identity value generated for a specified table or view. 
                                    // The last identity value generated can be for any session and any scope.

                                    // @@IDENTITY - Returns the last-inserted identity value.

                                    var identityQueries = new[]
                                    {
                                        !string.IsNullOrEmpty(identityTable) ? $"SELECT IDENT_CURRENT('{identityTable}')" : "",
                                        "SELECT SCOPE_IDENTITY()",
                                        "SELECT @@IDENTITY"
                                    };

                                    foreach (var identityQuery in identityQueries.Where(identityQuery => !string.IsNullOrEmpty(identityQuery)))
                                    {
                                        try
                                        {
                                            sqlCommand.CommandText = identityQuery;
                                            identityValue = Convert.ToInt32(sqlCommand.ExecuteScalar());
                                            break;
                                        }

                                        catch (Exception)
                                        {
                                            identityValue = -1;
                                        }
                                    }

                                    result = true;
                                }
                            }

                            else
                            {
                                errorMessage = "Unable to open connection to database.";
                            }

                            sqlConnection.Close();
                        }
                    }

                    else
                    {
                        errorMessage = "Invalid connection string.";
                    }
                }

                else
                {
                    errorMessage = "Invalid command text.";
                }
            }

            catch (Exception ex)
            {
                errorMessage = ex.Message;

                Logger.Error(ex);
            }

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "DataSet disposed by calling objects")]
        public DataSet ExecuteQuery(string queryText, ref string errorMessage)
        {
            DataSet result = null;

            try
            {
                if (!string.IsNullOrEmpty(queryText))
                {
                    if (!string.IsNullOrEmpty(_connectionString))
                    {
                        using (var sqlConnection = new SqlConnection(ConnectionString))
                        {
                            sqlConnection.Open();

                            if (sqlConnection.State == ConnectionState.Open)
                            {
                                result = new DataSet();

                                using (var sqlDataAdapter = new SqlDataAdapter(queryText, sqlConnection))
                                {
                                    if (sqlDataAdapter.SelectCommand != null)
                                    {
                                        sqlDataAdapter.SelectCommand.CommandTimeout = TimeoutInSeconds;
                                    }

                                    sqlDataAdapter.Fill(result);
                                }
                            }

                            else
                            {
                                errorMessage = "Unable to open connection to database.";
                            }

                            sqlConnection.Close();
                        }
                    }

                    else
                    {
                        errorMessage = "Invalid connection string.";
                    }
                }

                else
                {
                    errorMessage = "Invalid query text.";
                }
            }

            catch (Exception ex)
            {
                errorMessage = ex.Message;

                Logger.Error(ex);
            }

            return result;
        }

        #endregion Query and Command Methods


        #region BLOB handling Methods

        public byte[] ReadBlobData(string queryText, int columnIndex)
        {
            byte[] result = null;

            try
            {
                if (!string.IsNullOrEmpty(_connectionString) && !string.IsNullOrEmpty(queryText))
                {
                    using (var sqlConnection = new SqlConnection(ConnectionString))
                    {
                        using (var sqlCommand = new SqlCommand(queryText, sqlConnection))
                        {
                            sqlConnection.Open();

                            using (var sqlDataReader = sqlCommand.ExecuteReader())
                            {
                                var readBuffer = new byte[ReadBufferPacketSize];
                                long bufferSize = 0;

                                while (sqlDataReader.Read())
                                {
                                    long currentPos = 0;

                                    var bytesRead = sqlDataReader.GetBytes(0, currentPos, readBuffer, 0, ReadBufferPacketSize);

                                    while (bytesRead <= ReadBufferPacketSize)
                                    {
                                        var tempBuffer = result;

                                        result = new byte[bufferSize + bytesRead];

                                        if ((bufferSize > 0) && (tempBuffer != null))
                                        {
                                            Array.Copy(tempBuffer, result, bufferSize);
                                        }

                                        Array.Copy(readBuffer, 0, result, bufferSize, bytesRead);

                                        bufferSize += bytesRead;

                                        currentPos += ReadBufferPacketSize;
                                        bytesRead = sqlDataReader.GetBytes(0, currentPos, readBuffer, 0, ReadBufferPacketSize);

                                        if (bytesRead < 1)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }

                            sqlConnection.Close();
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;
        }

        public bool WriteBlobData(string updateCommand, string parameterName, string sqlTypeText, string text, byte[] data)
        {
            var result = false;

            try
            {
                if (!string.IsNullOrEmpty(_connectionString) &&
                    !string.IsNullOrEmpty(updateCommand) &&
                    !string.IsNullOrEmpty(parameterName) &&
                    ((text != null) || ((data != null) && (data.Length > 0))))
                {
                    var parameter = '@' + parameterName;

                    var sqlType = SqlDbType.VarBinary;

                    switch (sqlTypeText.ToLower())
                    {
                        case "char":
                        case "varchar":
                        case "text":
                            sqlType = SqlDbType.VarChar;
                            break;

                        case "nchar":
                        case "nvarchar":
                        case "ntext":
                            sqlType = SqlDbType.NVarChar;
                            break;

                        case "image":
                        case "binary":
                        case "varbinary":
                            sqlType = SqlDbType.Binary;
                            break;
                    }

                    using (var sqlConnection = new SqlConnection(ConnectionString))
                    {
                        using (var sqlCommand = new SqlCommand(updateCommand, sqlConnection))
                        {
                            SqlParameter sqlParameter = null;

                            if (text != null)
                            {
                                sqlParameter = new SqlParameter(
                                    parameter, 
                                    sqlType, 
                                    text.Length,
                                    ParameterDirection.Input, 
                                    0, 
                                    0, 
                                    null,
                                    DataRowVersion.Current, 
                                    false, 
                                    text, 
                                    null,
                                    null,
                                    null);
                            }

                            else if (data.Length > 0)
                            {
                                sqlParameter = new SqlParameter(
                                    parameter,
                                    sqlType,
                                    data.Length,
                                    ParameterDirection.Input,
                                    0,
                                    0,
                                    null,
                                    DataRowVersion.Current,
                                    false,
                                    data,
                                    null,
                                    null,
                                    null);
                            }

                            if (sqlParameter != null)
                            {
                                sqlCommand.Parameters.Add(sqlParameter);

                                sqlConnection.Open();

                                result = (sqlCommand.ExecuteNonQuery() == 1);

                                sqlConnection.Close();
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;
        }

        public string CreateBlobUpdateStatement(string tableName, string columnName, string primaryKeyWhereClause)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(tableName) &&
                    !string.IsNullOrEmpty(columnName) &&
                    !string.IsNullOrEmpty(primaryKeyWhereClause))
                {
                    result.Append(string.Format("UPDATE {0} SET {1}=@{1} WHERE {2}", tableName, columnName, primaryKeyWhereClause));
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result.ToString();
        }

        #endregion BLOB handling Methods
    }
}
