namespace SqlDocStore.MsSql.Tests
{
    using System;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;

    public class MsSqlDocumentStoreTests
    {
        private const string Tables = "Tables";

        public MsSqlDocumentStoreFixture GetFixture()
        {
            return new MsSqlDocumentStoreFixture();
        }

        [Fact]
        public async Task create_session_should_fail_when_disposed()
        {
            using (var fixture = GetFixture())
            {
                var store = new MsSqlDocumentStore(new DocumentStoreSettings(fixture.ConnectionString));
                await fixture.CreateDatabase();
                await store.InitializeSchema();
                store.Dispose();

                Func<Task> session = () => store.CreateSession();

                session.ShouldThrow<ObjectDisposedException>();
            }
        }

        [Fact]
        public async Task should_create_session()
        {
            using (var fixture = GetFixture())
            {
                var store = new MsSqlDocumentStore(new DocumentStoreSettings(fixture.ConnectionString));
                await fixture.CreateDatabase();
                await store.InitializeSchema();
                var session = await store.CreateSession();
                session.GetType().ShouldBe(typeof(MsSqlDocumentSession));
            }
        }

    //TODO: Rewrite below so can do schema tests.
    //    [Fact]
    //    public async Task should_initialize_custom_schema_when_required()
    //    {
    //        const string schema = "newschema";
    //        const string table = "Test";
    //        using (var fixture = GetFixture())
    //        {
    //            var store = new MsSqlDocumentStore(new DocumentStoreSettings(fixture.ConnectionString)
    //            {
    //                Schema = schema,
    //                Table = table
    //            });

    //            await fixture.CreateDatabase();

    //            await store.InitializeSchema();

    //            using (var connection = new SqlConnection(fixture.ConnectionString))
    //            {
    //                await connection.OpenAsync().ConfigureAwait(false);
    //                var schemas = connection.GetSchema(Tables);
    //                schemas.Rows.Count.ShouldBe(1);
    //                schemas.Rows[0]["TABLE_SCHEMA"].ShouldBe(schema);
    //                schemas.Rows[0]["TABLE_NAME"].ShouldBe(table);
    //            }
    //        }
    //    }

    //    [Fact]
    //    public async Task should_initialize_default_schema_when_required()
    //    {
    //        using (var fixture = GetFixture())
    //        {
    //            var settings = new DocumentStoreSettings(fixture.ConnectionString);
    //            var store = new MsSqlDocumentStore(settings);

    //            await fixture.CreateDatabase();

    //            await store.InitializeSchema();

    //            using (var connection = new SqlConnection(fixture.ConnectionString))
    //            {
    //                await connection.OpenAsync().ConfigureAwait(false);
    //                var schemas = connection.GetSchema(Tables);
    //                schemas.Rows.Count.ShouldBe(1);
    //                schemas.Rows[0]["TABLE_SCHEMA"].ShouldBe(settings.Schema);
    //                schemas.Rows[0]["TABLE_NAME"].ShouldBe(settings.Table);
    //            }
    //        }
    //    }

    //    [Fact]
    //    public async Task should_not_initialize_custom_schema_when_schemacreation_none()
    //    {
    //        const string schema = "different";
    //        const string table = "MyDocs";

    //        using (var fixture = GetFixture())
    //        {
    //            var settings = new DocumentStoreSettings(fixture.ConnectionString)
    //            {
    //                SchemaCreation = SchemaCreation.None,
    //                Schema = schema,
    //                Table = table
    //            };
    //            var store = new MsSqlDocumentStore(settings);

    //            await fixture.CreateDatabase();

    //            await store.InitializeSchema();

    //            using (var connection = new SqlConnection(fixture.ConnectionString))
    //            {
    //                await connection.OpenAsync().ConfigureAwait(false);
    //                var schemas = connection.GetSchema(Tables);
    //                schemas.Rows.Count.ShouldBe(0);
    //            }
    //        }
    //    }

    //    [Fact]
    //    public async Task should_not_initialize_default_schema_when_schemacreation_none()
    //    {
    //        using (var fixture = GetFixture())
    //        {
    //            var settings =
    //                new DocumentStoreSettings(fixture.ConnectionString) {SchemaCreation = SchemaCreation.None};
    //            var store = new MsSqlDocumentStore(settings);

    //            await fixture.CreateDatabase();

    //            await store.InitializeSchema();

    //            using (var connection = new SqlConnection(fixture.ConnectionString))
    //            {
    //                await connection.OpenAsync().ConfigureAwait(false);
    //                var schemas = connection.GetSchema(Tables);
    //                schemas.Rows.Count.ShouldBe(0);
    //            }
    //        }
    //    }
    }
}