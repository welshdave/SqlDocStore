namespace SqlDocStore.MsSql.Tests
{
    using System.Collections.Generic;
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
        public async Task first_should_be_queryable()
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

                    var foundDoc = session.Query<SimpleDoc>().First(doc => doc.Id != 3);

                    foundDoc.Id.ShouldBe(1);
                }
            }
        }

        [Fact]
        public async Task last_should_be_queryable()
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

                    var foundDoc = session.Query<SimpleDoc>().Last(doc => doc.Id != 3);

                    foundDoc.Id.ShouldBe(10);
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

        [Fact]
        public async Task should_orderby_ascending()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore(ConcurrencyModel.Optimistic))
                {
                    var session = await store.CreateSession();

                    GenerateAndStoreDocs(10, session);

                    await session.SaveChanges();

                    var foundDocs = session.Query<SimpleDoc>().Where(doc => doc.Id != 3).OrderBy(x => x.Id).ToList();

                    foundDocs.ShouldSatisfyAllConditions
                    (
                        "First and Last Ids should be correct",
                        () => foundDocs.First().Id.ShouldBe(1),
                        () => foundDocs.Last().Id.ShouldBe(10)
                    );
                }
            }
        }

        [Fact]
        public async Task should_orderby_descending()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore(ConcurrencyModel.Optimistic))
                {
                    var session = await store.CreateSession();

                    GenerateAndStoreDocs(10, session);

                    await session.SaveChanges();

                    var foundDocs = session.Query<SimpleDoc>().Where(doc => doc.Id != 3).OrderByDescending(x => x.Id).ToList();

                    foundDocs.ShouldSatisfyAllConditions
                    (
                        "First and Last Ids should be correct",
                        () => foundDocs.First().Id.ShouldBe(10),
                        () => foundDocs.Last().Id.ShouldBe(1)
                    );
                }
            }
        }

        private void GenerateAndStoreDocs(int numDocs, IDocumentSession session)
        {
            for (var i = 1; i <= numDocs; i++)
            {
                var doc = new SimpleDoc() { Id = i, Description = $"Description{i}" };
                session.Store(doc);
            }
        }
    }
}
