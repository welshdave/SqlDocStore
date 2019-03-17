namespace SqlDocStore.MsSql.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;
    using Polly;

    public class DockerMsSqlServerDatabase
    {
        private readonly string _databaseName;
        private readonly DockerContainer _sqlServerContainer;
        private const string Password = "Passw0rd!123";
        private const string Image = "microsoft/mssql-server-linux";
        private const string Tag = "2017-latest";
        private const int Port = 11433;

        public DockerMsSqlServerDatabase(string databaseName)
        {
            _databaseName = databaseName;

            var ports = new Dictionary<int, int>
            {
                { 1433, Port }
            };

            _sqlServerContainer = new DockerContainer(
                Image,
                Tag,
                HealthCheck,
                ports)
            {
                ContainerName = "sql-stream-store-tests-mssql",
                Env = new[] { "ACCEPT_EULA=Y", $"SA_PASSWORD={Password}" }
            };
        }

        public SqlConnection CreateConnection()
            => new SqlConnection(CreateConnectionStringBuilder().ConnectionString);

        public SqlConnectionStringBuilder CreateConnectionStringBuilder()
            => new SqlConnectionStringBuilder($"server=localhost,{Port};User Id=sa;Password={Password};Initial Catalog=master");

        public async Task CreateDatabase(CancellationToken cancellationToken = default)
        {
            await _sqlServerContainer.TryStart(cancellationToken).WithTimeout(3 * 60 * 1000);

            var policy = Policy
                .Handle<SqlException>()
                .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(1));

            await policy.ExecuteAsync(async () =>
            {
                using (var connection = CreateConnection())
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                    var createCommand = $@"CREATE DATABASE [{_databaseName}]
ALTER DATABASE [{_databaseName}] SET SINGLE_USER
ALTER DATABASE [{_databaseName}] SET COMPATIBILITY_LEVEL=130
ALTER DATABASE [{_databaseName}] SET MULTI_USER";

                    using (var command = new SqlCommand(createCommand, connection))
                    {
                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            });
        }

        private async Task<bool> HealthCheck(CancellationToken cancellationToken)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                    return true;
                }
            }
            catch (Exception) { }

            return false;
        }
    }
}