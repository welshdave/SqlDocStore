namespace SqlDocStore.MsSql.Tests
{
    using SqlDocStore.Tests.Documents;
    using System;
    using System.Collections.Generic;

    public abstract class MsSqlDocumentSessionTestsBase
    {

        protected MsSqlDocumentStoreFixture GetFixture()
        {
            return new MsSqlDocumentStoreFixture();
        }

        protected void GenerateAndStoreDocs(int numDocs, IDocumentSession session)
        {
            for (var i = 1; i <= numDocs; i++)
            {
                var doc = new SimpleDoc() { Id = i, Description = $"Description{i}" };
                session.Store(doc);
            }
        }

        protected void GenerateAndStoreComplexDocs(int numDocs, IDocumentSession session)
        {
            for (var i = 1; i <= numDocs; i++)
            {
                var doc = new Company() { Id = i, Description = $"Description{i}", Name = $"Company-{i}" };
                var people = new List<Person>();
                for (var j = 0; j < 5; j++)
                {
                    people.Add(new Person { Id = Guid.NewGuid(), DateOfBirth = new DateTime(1950, 1, 1), FullName = $"Person{i}-{j}", PreferredName = $"Person{i}-{j}" });
                }

                doc.Officers = people;
                session.Store(doc);
            }
        }

    }
}
