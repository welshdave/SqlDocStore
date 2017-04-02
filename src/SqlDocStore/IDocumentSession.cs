namespace SqlDocStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDocumentSession
    {
        IEnumerable<ITrackedDocument> PendingChanges { get; }

        IDocumentStore DocumentStore { get; }

        void Delete<T>(T document);

        void Delete(int id);

        void Delete(long id);

        void Delete(Guid id);

        void Delete(string id);

        void Store<T>(T document);

        Task SaveChanges(CancellationToken token = default(CancellationToken));

        IQueryable<T> Query<T>();

        Task<T> Load<T>(object id, CancellationToken token = default(CancellationToken));
    }
}