namespace SqlDocStore.MsSql.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Shouldly;
    using SqlDocStore.Tests.Documents;
    using Xunit;
    public partial class MsSqlDocumentSessionTests
    {
        [Fact]
        public async Task should_not_delete_document_updated_in_another_session()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore(ConcurrencyModel.Pessimistic))
                {
                    var session = await store.CreateSession();
                    var autofixture = new Fixture();
                    var simple = autofixture.Create<SimpleDoc>();
                    session.Store(simple);
                    await session.SaveChanges();

                    var session2 = await store.CreateSession();
                    var simple2 = await session2.Load<SimpleDoc>(simple.Id);

                    simple2.Description = $"Description{Guid.NewGuid()}";
                    session2.Store(simple2);
                    await session2.SaveChanges();
                    session.Delete(simple);
                    Should.Throw<ConcurrencyException>(async () => await session.SaveChanges());
                    
                    
                }
            }
        }
    }
}
