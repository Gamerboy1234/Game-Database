
using System;
using ActiveNet.Types.Base;
using ActiveNet.Types.Interfaces;
using Daffinity.Model;

namespace DaffinityModelTest.UnitTests
{
    public class CollectorDbTestBase : IDisposable
    {
        #region Public Fields

        protected readonly ISecureSyncDatabaseAdapter SqlServerConnection;
        protected readonly ISecureSyncDatabaseAdapter PostgresConnection;

        protected readonly bool IsSqlServerConnected;
        protected readonly bool IsPostgresConnected;

        #endregion Public Fields


        #region Construct and Dispose

        protected CollectorDbTestBase()
        {
            AnOperatingSystem.SetCachedAdaptersPath(AnOperatingSystem.GetAdaptersPath(Settings.GetSetting(Settings.AdaptersPathKey), ""));

            DatabaseAdapterHelper.LoadAdapters(out var sqlServerConnection, out var postgresConnection);

            SqlServerConnection = sqlServerConnection;
            PostgresConnection = postgresConnection;

            IsSqlServerConnected = SqlServerConnection?.Connect(DatabaseAdapterHelper.SqlServerAdapterCollectorConnectionString, 0) ?? false;

            IsPostgresConnected = PostgresConnection?.Connect(DatabaseAdapterHelper.PostgresAdapterCollectorConnectionString, 0) ?? false;
        }

        public void Dispose()
        {
            // Nothing Needed
        }

        #endregion Construct and Dispose
    }
}
