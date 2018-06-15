namespace SqlDocStore.MsSql.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Shouldly;
    using SqlDocStore.Tests.Documents;
    using Xunit;

    public class MsSqlDocumentSessionLinqTests
    {
        private MsSqlDocumentStoreFixture GetFixture()
        {
            return new MsSqlDocumentStoreFixture();
        }

        [Fact]
        public async Task single_should_be_queryable()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore(ConcurrencyModel.Optimistic))
                {
                    var session = await store.CreateSession();

                    for (var i = 1; i <= 10; i++)
                    {
                        var doc = new SimpleDoc() { Id = i, Description = $"Description{i}" };
                        session.Store(doc);
                    }

                    await session.SaveChanges();

                    var foundDoc = session.Query<SimpleDoc>().Single(doc => doc.Id == 3);

                    foundDoc.Id.ShouldBe(3);
                }
            }
        }

        [Fact]
        public async Task multiple_should_be_queryable()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore(ConcurrencyModel.Optimistic))
                {
                    var session = await store.CreateSession();

                    for (var i = 1; i <= 10; i++)
                    {
                        var doc = new SimpleDoc() { Id = i, Description = $"Description{i}" };
                        session.Store(doc);
                    }

                    await session.SaveChanges();

                    var foundDocs = session.Query<SimpleDoc>().Where(doc => doc.Id != 3).ToList();

                    foundDocs.Count.ShouldBe(9);
                }
            }
        }
    }
}
