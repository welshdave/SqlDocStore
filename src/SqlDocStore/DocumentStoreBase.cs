namespace SqlDocStore
{
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class DocumentStoreBase : DisposableBase, IDocumentStore
    {
        protected DocumentStoreBase(DocumentStoreSettings settings)
        {
            Settings = settings;
        }

        public DocumentStoreSettings Settings { get; }

        public async Task<IDocumentSession> CreateSession(CancellationToken token = default)
        {
            CheckNotDisposed();
            return await CreateSessionInternal(token);
        }

        public async Task InitializeSchema(CancellationToken token = new CancellationToken())
        {
            CheckNotDisposed();
            await SetupDatabase(token);
        }

        protected abstract Task<IDocumentSession> CreateSessionInternal(CancellationToken token);

        protected abstract Task SetupDatabase(CancellationToken token);
    }
}