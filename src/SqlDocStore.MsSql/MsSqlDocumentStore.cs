﻿namespace SqlDocStore.MsSql
{
    using System;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;
    using Vendor.EnsureThat;
    using Sql;

    public class MsSqlDocumentStore : DocumentStoreBase
    {
        private const string DefaultSchema = "dbo";
        private const int MinimumSqlVersion = 13;

        private readonly Func<SqlConnection> _createConnection;
        private readonly Scripts _scripts;
        private readonly DocumentStoreSettings _settings;

        public MsSqlDocumentStore(DocumentStoreSettings settings) : base(settings)
        {
            EnsureArg.IsNotNull(settings);
            if (settings.Schema == string.Empty)
                settings.Schema = DefaultSchema;

            _createConnection = () => new SqlConnection(settings.ConnectionString);
            _settings = settings;
            _scripts = new Scripts(settings.Schema, settings.Table);
        }

        protected override async Task<IDocumentSession> CreateSessionInternal(CancellationToken token)
        {
            await CheckSupportedSqlVersion(token);
            return new MsSqlDocumentSession(_createConnection, this);
        }

        protected override async Task SetupDatabase(CancellationToken token)
        {
            if (Settings.SchemaCreation == SchemaCreation.None) return;
            await CheckSupportedSqlVersion(token);
            using (var connection = _createConnection())
            {
                await connection.OpenAsync(token).ConfigureAwait(false);
                if (Settings.Schema != DefaultSchema)
                    using (var command = new SqlCommand(_scripts.CreateNonDefaultSchema, connection))
                    {
                        await command.ExecuteNonQueryAsync(token).ConfigureAwait(false);
                    }
                using (var command = new SqlCommand(_scripts.CreateTable, connection))
                {
                    await command.ExecuteNonQueryAsync(token).ConfigureAwait(false);
                }
            }
        }

        private async Task CheckSupportedSqlVersion(CancellationToken token)
        {
            using (var connection = _createConnection())
            {
                await connection.OpenAsync(token).ConfigureAwait(false);
                if (!SupportedSqlVersion(connection.ServerVersion))
                {
                    throw new UnsupportedDatabaseException($"This version of Sql Server is not supported. A version of {MinimumSqlVersion} or above is required");
                }
            }
        }

        private static bool SupportedSqlVersion(string serverVersion)
        {
            var serverVersionDetails = serverVersion.Split(new[] {"."}, StringSplitOptions.None);
            var versionNumber = int.Parse(serverVersionDetails[0]);
            return versionNumber >= MinimumSqlVersion;
        }
    }
}