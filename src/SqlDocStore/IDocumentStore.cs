namespace SqlDocStore
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDocumentStore : IDisposable
    {
        DocumentStoreSettings Settings { get; }

        Task<IDocumentSession> CreateSession(CancellationToken token = default(CancellationToken));

        Task InitializeSchema(CancellationToken token = default(CancellationToken));
    }
}