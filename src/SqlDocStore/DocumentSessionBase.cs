﻿namespace SqlDocStore
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Threading;
    using System.Threading.Tasks;
    using Linq;
    using Remotion.Linq;
    using Remotion.Linq.Parsing.Structure;

    public abstract class DocumentSessionBase : DisposableBase, IDocumentSession
    {
        private static readonly List<Type> IdentityTypes = new List<Type>
        {
            typeof(string),
            typeof(int),
            typeof(long),
            typeof(Guid)
        };

        private readonly IQueryParser _parser;

        protected DocumentSessionBase()
        {
            _parser = QueryParserFactory.CreateQueryParser();
        }

        public ChangeTracker ChangeTracker { get; set; }

        protected abstract IQueryExecutor Executor { get; set; }

        public IDocumentStore DocumentStore { get; protected set; }

        public IEnumerable<ITrackedDocument> PendingChanges => ChangeTracker.Changes;

        public void Delete<T>(T document)
        {
            CheckNotDisposed();
            if (IdentityTypes.Contains(document.GetType()))
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
            if (document is IDynamicMetaObjectProvider) throw new InvalidDocumentException("Document cannot be Dynamic");
            if (document.GetType().IsAnonymousType()) throw new InvalidDocumentException("Document cannot be Anonymous");
            IdentityHelper.ValidateDocumentId(document);
            StoreInternal(document);
        }

        public Task SaveChanges(CancellationToken token = new CancellationToken())
        {
            CheckNotDisposed();
            return SaveChangesInternal(token);
        }

        public ISqlDocStoreQueryable<T> Query<T>()
        {
            CheckNotDisposed();
            return new SqlDocStoreQueryable<T>(_parser,Executor);
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

        protected abstract void DeleteInternal<T>(T document);

        protected abstract void DeleteByIdInternal(object id);

        protected abstract void StoreInternal<T>(T document);

        protected abstract Task SaveChangesInternal(CancellationToken token);

        protected abstract Task<T> LoadInternal<T>(object id, CancellationToken token);
    }
}