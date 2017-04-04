namespace SqlDocStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class DocumentSessionBase : DisposableBase, IDocumentSession
    {
        private static readonly List<Type> IdentityTypes = new List<Type>
        {
            typeof(string),
            typeof(int),
            typeof(long),
            typeof(Guid)
        };

        protected readonly ChangeTracker ChangeTracker = new ChangeTracker();
        
        public IDocumentStore DocumentStore { get; protected set; }

        public IEnumerable<ITrackedDocument> PendingChanges => ChangeTracker.Changes;

        public void Delete<T>(T document)
        {
            CheckNotDisposed();
            if (IdentityTypes.Contains(document.GetType())) //using object, got here when we didn't mean to.
            {
                DeleteByIdInternal(document);
                return;
            }
            IdentityHelper.ValidateDocumentId(document);
            DeleteInternal(document);
        }

        public void Delete(int id)
        {
            CheckNotDisposed();
            DeleteByIdInternal(id);
        }

        public void Delete(long id)
        {
            CheckNotDisposed();
            DeleteByIdInternal(id);
        }

        public void Delete(Guid id)
        {
            CheckNotDisposed();
            DeleteByIdInternal(id);
        }

        public void Delete(string id)
        {
            DeleteByIdInternal(id);
        }

        public void Store<T>(T document)
        {
            CheckNotDisposed();
            if (document == null) throw new ArgumentNullException(nameof(document));
            IdentityHelper.ValidateDocumentId(document);
            StoreInternal(document);
        }

        public Task SaveChanges(CancellationToken token = new CancellationToken())
        {
            CheckNotDisposed();
            return SaveChangesInternal(token);
        }

        public IQueryable<T> Query<T>()
        {
            CheckNotDisposed();
            return QueryInternal<T>();
        }

        public Task<T> Load<T>(int id, CancellationToken token = new CancellationToken())
        {
            CheckNotDisposed();
            return LoadInternal<T>(id, token);
        }

        public Task<T> Load<T>(long id, CancellationToken token = new CancellationToken())
        {
            CheckNotDisposed();
            return LoadInternal<T>(id, token);
        }

        public Task<T> Load<T>(Guid id, CancellationToken token = new CancellationToken())
        {
            CheckNotDisposed();
            return LoadInternal<T>(id, token);
        }

        public Task<T> Load<T>(string id, CancellationToken token = new CancellationToken())
        {
            CheckNotDisposed();
            return LoadInternal<T>(id, token);
        }

        protected abstract void DeleteInternal<T>(T entity);

        protected abstract void DeleteByIdInternal(object id);

        protected abstract void StoreInternal<T>(T entity);

        protected abstract Task SaveChangesInternal(CancellationToken token);

        protected abstract IQueryable<T> QueryInternal<T>();

        protected abstract Task<T> LoadInternal<T>(object id, CancellationToken token);
    }
}