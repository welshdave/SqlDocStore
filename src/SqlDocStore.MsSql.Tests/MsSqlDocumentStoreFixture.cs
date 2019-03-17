namespace SqlDocStore.MsSql.Tests
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;

    public class MsSqlDocumentStoreFixture : DisposableBase
    {
        private readonly string _databaseName;
        private readonly DockerMsSqlServerDatabase _databaseInstance;
        public readonly string ConnectionString;

        public MsSqlDocumentStoreFixture()
        {
            _databaseName = $"DocStoreTests-{Guid.NewGuid()}";
            _databaseInstance = new DockerMsSqlServerDatabase(_databaseName);
            var connectionStringBuilder = _databaseInstance.CreateConnectionStringBuilder();
            connectionStringBuilder.MultipleActiveResultSets = true;
            connectionStringBuilder.InitialCatalog = _databaseName;
            ConnectionString = connectionStringBuilder.ToString();
        }

        public async Task CreateDatabase()
        {
            await _databaseInstance.CreateDatabase();
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

                using (var connection = _databaseInstance.CreateConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand($"ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", connection))
                    {
                        command.ExecuteNonQuery();
                    }
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