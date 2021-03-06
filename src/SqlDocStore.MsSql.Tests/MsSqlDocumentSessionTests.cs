﻿namespace SqlDocStore.MsSql.Tests
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

    public class MsSqlDocumentSessionTests : MsSqlDocumentSessionTestsBase
    {
        [Fact]
        public async Task should_add_document_to_pending_changes()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore())
                {
                    var session = await store.CreateSession();
                    var autofixture = new Fixture();
                    var person = autofixture.Create<Person>();
                    session.Store(person);
                    session.PendingChanges.First().Document.ShouldBeSameAs(person);
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetDocuments))]
        public async Task should_store_documents(object document)
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore())
                {
                    var session = await store.CreateSession();
                    session.Store(document);
                    await session.SaveChanges();

                    using (var connection = new SqlConnection(fixture.ConnectionString))
                    {
                        var id = IdentityHelper.GetIdFromDocument(document);
                        await connection.OpenAsync();
                        var sql =
                            $"SELECT count(1) FROM {store.Settings.Schema}.{store.Settings.Table} WHERE cast(JSON_VALUE(Document,'$.Id') as nvarchar(max)) = @id;";
                        var command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@id", id);
                        var result = command.ExecuteScalar();
                        result.ShouldBe(1);
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetDocuments))]
        public async Task should_delete_document_by_id(object document)
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore())
                {
                    var session = await store.CreateSession();
                    session.Store(document);
                    await session.SaveChanges();
                    var id = IdentityHelper.GetIdFromDocument(document);

                    session.Delete(id);
                    await session.SaveChanges();

                    using (var connection = new SqlConnection(fixture.ConnectionString))
                    {
                        await connection.OpenAsync();
                        var sql =
                            $"SELECT count(1) FROM {store.Settings.Schema}.{store.Settings.Table} WHERE cast(JSON_VALUE(Document,'$.Id') as nvarchar(max)) = @id;";
                        var command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@id", id);
                        var result = command.ExecuteScalar();
                        result.ShouldBe(0);
                    }
                }
            }
        }

        [Fact]
        public async Task should_fail_to_store_document_with_default_id()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore())
                {
                    var session = await store.CreateSession();
                    var autofixture = new Fixture();
                    var simple = autofixture.Create<SimpleDoc>();
                    simple.Id = 0;
                    Should.Throw<InvalidDocumentException>(() => session.Store(simple));
                }
            }
        }

        [Fact]
        public async Task should_fail_to_store_document_with_no_id()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore())
                {
                    var session = await store.CreateSession();
                    var item = new {Name = "Item with no Id"};
                    Should.Throw<InvalidDocumentException>(() => session.Store(item));
                }
            }
        }

        [Fact]
        public async Task should_fail_to_store_null_object()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore())
                {
                    var session = await store.CreateSession();
                    Should.Throw<ArgumentNullException>(() => session.Store((Person) null));
                }
            }
        }

        [Fact]
        public async Task should_fail_to_store_dynamic_object()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore())
                {
                    dynamic doc = new ExpandoObject();
                    doc.Id = (new Random()).Next();
                    doc.Title = $"Title{Guid.NewGuid()}";
                    var session = await store.CreateSession();
                    Should.Throw<InvalidDocumentException>(() => session.Store((object)doc));
                }
            }
        }

        [Fact]
        public async Task should_fail_to_store_anonymous_document()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore())
                {
                    var doc =  new {Id = Guid.NewGuid(), Title = $"Title{Guid.NewGuid()}" };
                    var session = await store.CreateSession();
                    Should.Throw<InvalidDocumentException>(() => session.Store(doc));
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetDocuments))]
        public async Task should_load_dynamic_document_by_id(object document)
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore())
                {
                    var session = await store.CreateSession();
                    session.Store(document);
                    await session.SaveChanges();
                    var id = IdentityHelper.GetIdFromDocument(document);
                    dynamic doc = null;
                    
                    switch (id)
                    {
                        case int i:
                            doc = await session.Load<dynamic>(i);
                            break;
                        case long l:
                            doc = await session.Load<dynamic>(l);
                            break;
                        case string s:
                            doc = await session.Load<dynamic>(s);
                            break;
                        case Guid g:
                            doc = await session.Load<dynamic>(g);
                            break;  
                    }

                    ((string)doc.Id.ToString()).ShouldBe(id.ToString());

                }
            }
        }

        [Fact]
        public async Task should_load_document_by_guid_id()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore())
                {
                    var session = await store.CreateSession();
                    var autofixture = new Fixture();
                    var person = autofixture.Create<Person>();
                    session.Store(person);
                    await session.SaveChanges();

                    var doc = await session.Load<Person>(person.Id);
                    
                    doc.Id.ShouldBe(person.Id);
                }
            }
        }

        [Fact]
        public async Task should_load_document_by_int_id()
        {
            using (var fixture = GetFixture())
            {
                using (var store = await fixture.GetDocumentStore())
                {
                    var session = await store.CreateSession();
                    var autofixture = new Fixture();
                    var simple = autofixture.Create<SimpleDoc>();
                    session.Store(simple);
                    await session.SaveChanges();

                    var doc = await session.Load<SimpleDoc>(simple.Id);

                    doc.Id.ShouldBe(simple.Id);
                }
            }
        }
        
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
        
        public static IEnumerable<object[]> GetDocuments()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<SimpleDoc>() };
            yield return new object[] { fixture.Create<Person>() };
            yield return new object[] { fixture.Create<Company>() };
            yield return new object[] { fixture.Create<AnotherDoc>() };
        }
    }
}