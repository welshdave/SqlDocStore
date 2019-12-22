namespace SqlDocStore.MsSql.Tests
{
    using Shouldly;
    using SqlDocStore.Tests.Documents;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class MsSqlDocumentSessionLinqMethodsCallTests : MsSqlDocumentSessionTestsBase
    {
        [Fact]
        public async Task should_allow_querying_with_startswith()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore(ConcurrencyModel.Optimistic))
                {
                    var session = await store.CreateSession();

                    GenerateAndStoreComplexDocs(10, session);

                    await session.SaveChanges();

                    var docs = session.Query<Company>().Where(doc => doc.Name.StartsWith("Company")).ToList();

                    docs.Count.ShouldBe(10);
                }
            }
        }

        [Fact]
        public async Task should_allow_querying_with_contains()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore(ConcurrencyModel.Optimistic))
                {
                    var session = await store.CreateSession();

                    GenerateAndStoreComplexDocs(10, session);

                    await session.SaveChanges();

                    var docs = session.Query<Company>().Where(doc => doc.Name.Contains("ompan")).ToList();

                    docs.Count.ShouldBe(10);
                }
            }
        }
    }
}
