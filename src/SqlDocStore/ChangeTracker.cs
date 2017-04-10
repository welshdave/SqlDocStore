namespace SqlDocStore
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public class ChangeTracker
    {
        private readonly ConcurrencyModel _concurrencyModel;

        public ChangeTracker(ConcurrencyModel concurrencyModel)
        {
            _concurrencyModel = concurrencyModel;
        }

        public readonly ConcurrentDictionary<object, TrackedDocument> Documents =
            new ConcurrentDictionary<object, TrackedDocument>();

        public IEnumerable<TrackedDocument> Changes => Documents.Values.Where(x => x.State != DocumentState.Unchanged);

        public IEnumerable<object> Inserts
            => from doc in Documents where doc.Value.State == DocumentState.Added select doc.Value.Document;

        public IEnumerable<object> Updates
            => from doc in Documents where doc.Value.State == DocumentState.Modified select doc.Value.Document;

        public IEnumerable<object> Deletions
            => from doc in Documents where doc.Value.State == DocumentState.Deleted select doc.Value.Document;

        public IEnumerable<T> InsertsFor<T>()
        {
            return from doc in Documents
                where doc.Value.State == DocumentState.Added && doc.Value.Type == typeof(T)
                select (T) doc.Value.Document;
        }

        public IEnumerable<T> UpdatesFor<T>()
        {
            return from doc in Documents
                where doc.Value.State == DocumentState.Modified && doc.Value.Type == typeof(T)
                select (T) doc.Value.Document;
        }

        public IEnumerable<T> DeletionsFor<T>()
        {
            return from doc in Documents
                where doc.Value.State == DocumentState.Deleted && doc.Value.Type == typeof(T)
                select (T) doc.Value.Document;
        }

        public void Track<T>(T document, Guid eTag)
        {
            if (UseOptimisticConcurrency) return;
            var id = IdentityHelper.GetIdFromDocument(document);
            Documents.AddOrUpdate(
                id,
                new TrackedDocument
                {
                    Document = document,
                    Type = typeof(T),
                    State = DocumentState.Unchanged,
                    ETag = eTag
                },
                (key, oldValue) =>
                {
                    oldValue.Document = document;
                    oldValue.Type = typeof(T);
                    oldValue.ETag = eTag;
                    return oldValue;
                });
        }

        public void Insert<T>(T document)
        {
            var id = IdentityHelper.GetIdFromDocument(document);
            Documents.AddOrUpdate(id,
                new TrackedDocument
                {
                    Document = document,
                    Type = typeof(T),
                    State = DocumentState.Added,
                    ETag = Guid.NewGuid()
                },
                (key, oldValue) =>
                {
                    oldValue.Document = document;
                    oldValue.Type = typeof(T);
                    return oldValue;
                });
        }

        public void Update<T>(T document)
        {
            var id = IdentityHelper.GetIdFromDocument(document);
            if (!UseOptimisticConcurrency)
                if (!Documents.TryGetValue(id, out _))
                    throw new InvalidOperationException("Can't update unknown document");

            Documents.AddOrUpdate(id,
                new TrackedDocument
                {
                    Document = document,
                    State = DocumentState.Modified,
                    Type = typeof(T)
                },
                (key, oldValue) =>
                {
                    oldValue.Document = document;
                    oldValue.Type = typeof(T);
                    oldValue.State = DocumentState.Modified;
                    return oldValue;
                });
        }

        public void DeleteById(object id)
        {
            if (!UseOptimisticConcurrency)
                if (!Documents.TryGetValue(id, out _))
                    throw new InvalidOperationException("Can't delete unknown document");
            Documents.AddOrUpdate(id, new TrackedDocument
                {
                    Document = new DeletedDocument(id),
                    State = DocumentState.Deleted,
                    Type = typeof(object)
                },
                (key, oldValue) =>
                {
                    oldValue.State = DocumentState.Deleted;
                    return oldValue;
                });
        }

        public void Delete<T>(T document)
        {
            var id = IdentityHelper.GetIdFromDocument(document);
            DeleteById(id);
        }

        public void MarkChangesSaved()
        {
            foreach (var document in Documents)
                document.Value.State = DocumentState.Unchanged;
        }

        private bool UseOptimisticConcurrency => _concurrencyModel == ConcurrencyModel.Optimistic;
    }
}