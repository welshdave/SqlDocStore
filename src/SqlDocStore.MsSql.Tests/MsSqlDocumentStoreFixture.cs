namespace SqlDocStore.MsSql.Tests
{
    using System;
    using System.Data.SqlClient;
    using System.Data.SqlLocalDb;
    using System.Linq;
    using System.Threading.Tasks;

    public class MsSqlDocumentStoreFixture : DisposableBase
    {
        private const int Sql2016 = 13;

        private static readonly string LocalDbVersion = new SqlLocalDbProvider()
            .GetVersions()
            .Where(provider => provider.Exists && provider.Version.Major >= Sql2016)
            .Max(provider => provider.Version)
            .ToString(2);

        private readonly string _databaseName;
        private readonly ISqlLocalDbInstance _localDbInstance;
        public readonly string ConnectionString;

        public MsSqlDocumentStoreFixture()
        {
            var localDbProvider = new SqlLocalDbProvider
            {
                Version = LocalDbVersion
            };
            _localDbInstance = localDbProvider.GetOrCreateInstance("DocStoreTests");
            _localDbInstance.Start();

            _databaseName = $"DocStoreTests-{Guid.NewGuid()}";

            ConnectionString = CreateConnectionString();
        }

        private string CreateConnectionString()
        {
            var connectionStringBuilder = _localDbInstance.CreateConnectionStringBuilder();
            connectionStringBuilder.MultipleActiveResultSets = true;
            connectionStringBuilder.IntegratedSecurity = true;
            connectionStringBuilder.InitialCatalog = _databaseName;

            return connectionStringBuilder.ToString();
        }

        public async Task CreateDatabase()
        {
            using (var connection = _localDbInstance.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand($"CREATE DATABASE  [{_databaseName}]", connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<MsSqlDocumentStore> GetDocumentStore(ConcurrencyModel concurrencyModel = ConcurrencyModel.Optimistic)
        {
            await CreateDatabase();
            var settings = new DocumentStoreSettings(ConnectionString)
            {
                ConcurrencyModel = concurrencyModel,
                SchemaCreation = SchemaCreation.Create
            };
            var store = new MsSqlDocumentStore(settings);
            await store.InitializeSchema();
            return store;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                using (var sqlConnection = new SqlConnection(ConnectionString))
                {
                    SqlConnection.ClearPool(sqlConnection);
                }

                using (var connection = _localDbInstance.CreateConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand($"DROP DATABASE [{_databaseName}]", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            base.Dispose(true);
        }
    }
}