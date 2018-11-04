namespace SqlDocStore.MsSql.Tests
{
    using System;
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

                    GenerateAndStoreDocs(10, session);

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

                    GenerateAndStoreDocs(10, session);

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

                    GenerateAndStoreDocs(10, session);

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

                    GenerateAndStoreDocs(10, session);

                    await session.SaveChanges();

                    var foundDocs = session.Query<SimpleDoc>().Where(doc => doc.Id != 3).ToList();

                    foundDocs.Count.ShouldBe(9);
                }
            }
        }

        [Fact]
        public async Task multiple_should_be_queryable_by_multiple_properties()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore(ConcurrencyModel.Optimistic))
                {
                    var session = await store.CreateSession();

                    GenerateAndStoreDocs(10, session);

                    await session.SaveChanges();

                    var foundDocs = session.Query<SimpleDoc>().Where(doc => doc.Id != 3 && doc.Description != "Description4").ToList();

                    foundDocs.Count.ShouldBe(8);
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

        [Fact]
        public async Task should_allow_querying_on_nested_document()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore(ConcurrencyModel.Optimistic))
                {
                    var session = await store.CreateSession();

                    GenerateAndStoreComplexDocs(10, session);

                    await session.SaveChanges();

                    var foundDoc = session.Query<Company>().Single(doc => doc.Officers.Any(p => p.PreferredName == "Person1-2"));

                    foundDoc.Officers.ShouldContain(x => x.PreferredName == "Person1-2");
                    
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

        private void GenerateAndStoreComplexDocs(int numDocs, IDocumentSession session)
        {
            for (var i = 1; i <= numDocs; i++)
            {
                var doc = new Company() {Id = i, Description = $"Description{i}", Name = $"{Guid.NewGuid()}"};
                var people = new List<Person>();
                for (var j = 0; j < 5; j++)
                {
                    people.Add(new Person {Id = Guid.NewGuid(), DateOfBirth = DateTime.Now.AddYears(-50),FullName = $"{Guid.NewGuid()}", PreferredName = $"Person{i}-{j}"});
                }

                doc.Officers = people;
                session.Store(doc);
            }
        }
    }
}
