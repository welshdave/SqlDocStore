namespace SqlDocStore.MsSql.Tests
{
    using System.Data.SqlClient;

    interface ILocalDbInstance
    {
        SqlConnection CreateConnection();
        SqlConnectionStringBuilder CreateConnectionStringBuilder();
    }
}
