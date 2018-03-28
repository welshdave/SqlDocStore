namespace SqlDocStore.MsSql.Tests
{
    using System.Data.SqlClient;

    class LocalDbInstance : ILocalDbInstance
    {
        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=SSPI;";
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(connectionString);
        }

        public SqlConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return new SqlConnectionStringBuilder(connectionString);
        }
    }
}
