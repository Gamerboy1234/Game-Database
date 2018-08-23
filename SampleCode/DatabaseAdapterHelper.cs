
using System;
using System.Reflection;
using ActiveNet.Types.Base;
using ActiveNet.Types.Interfaces;
using Daffinity.Model;

namespace DaffinityModelTest.UnitTests
{
    public static class DatabaseAdapterHelper
    {
        #region Constants

        private static string SqlServerName => AnOperatingSystem.IsLinux || AnOperatingSystem.IsOsx ? "192.168.1.227" : "127.0.0.1"; 
        private static string PostgresServerName => AnOperatingSystem.IsLinux || AnOperatingSystem.IsOsx ? "192.168.1.227" : "127.0.0.1"; 
        
        private const string NodeManagementDatabaseName = "di-node-settings";
        private const string CollectorDatabaseName = "di-collector";
        private const string NodeDatabaseName = "di-cloud-node";

        private const string SqlServerUsername = "daffinity";
        private const string PostgresUsername = "postgres";
        private const string SqlServerPassword = "n0mud4";
        private const string PostgresPassword = "n0mud4";

        private const string SqlServerAdapterDllName = "SqlServer.Adapter.dll";
        private const string SqlServerAdapterClassName = "SqlServer.Adapter.AnSqlServerAdapter";

        private const string PostgresAdapterDllName = "Postgres.Adapter.dll";
        private const string PostgresAdapterClassName = "Postgres.Adapter.AnPostgresAdapter";

        public static string SqlServerAdapterAccountConnectionString => $"Server={SqlServerName};Database={NodeManagementDatabaseName};User Id={SqlServerUsername};Password={SqlServerPassword};";
        public static string PostgresAdapterAccountConnectionString => $"Server={PostgresServerName};Port=5432;Database={NodeManagementDatabaseName};User Id={PostgresUsername};Password={PostgresPassword};"; 

        public static string SqlServerAdapterCollectorConnectionString => $"Server={SqlServerName};Database={CollectorDatabaseName};User Id={SqlServerUsername};Password={SqlServerPassword};";
        public static string PostgresAdapterCollectorConnectionString => $"Server={PostgresServerName};Port=5432;Database={CollectorDatabaseName};User Id={PostgresUsername};Password={PostgresPassword};"; 

        public static string SqlServerAdapterLocalNodeConnectionString => $"Server={SqlServerName};Database={NodeDatabaseName};User Id={SqlServerUsername};Password={SqlServerPassword};";
        public static string PostgresAdapterLocalNodeConnectionString => $"Server={PostgresServerName};Port=5432;Database={NodeDatabaseName};User Id={PostgresUsername};Password={PostgresPassword};";

        #endregion Constants


        #region Public Methods

        public static void LoadAdapters(out ISecureSyncDatabaseAdapter sqlServerConnection, out ISecureSyncDatabaseAdapter postgresConnection)
        {
            sqlServerConnection = null;
            postgresConnection = null;

            var sqlServerAssembly = Assembly.LoadFrom($"{AnOperatingSystem.AppendDirectorySeperatorChar(AnOperatingSystem.GetAdaptersPath(Settings.GetSetting(Settings.AdaptersPathKey), ""))}{SqlServerAdapterDllName}");

            var sqlServerAdapterType = sqlServerAssembly.GetType(SqlServerAdapterClassName);

            if (sqlServerAdapterType != null)
            {
                sqlServerConnection = Activator.CreateInstance(sqlServerAdapterType) as ISecureSyncDatabaseAdapter;
            }

            var postgresAssembly = Assembly.LoadFrom($"{AnOperatingSystem.AppendDirectorySeperatorChar(AnOperatingSystem.GetAdaptersPath(Settings.GetSetting(Settings.AdaptersPathKey), ""))}{PostgresAdapterDllName}");

            var postgresAdapterType = postgresAssembly.GetType(PostgresAdapterClassName);

            if (postgresAdapterType != null)
            {
                postgresConnection = Activator.CreateInstance(postgresAdapterType) as ISecureSyncDatabaseAdapter;
            }
        }

        #endregion Public Methods
    }
}
