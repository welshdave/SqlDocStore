namespace SqlDocStore
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public class ChangeTracker
    {
        private readonly ConcurrentDictionary<object, TrackedDocument> _documents =
            new ConcurrentDictionary<object, TrackedDocument>();

        public IEnumerable<TrackedDocument> Changes => _documents.Values.Where(x => x.State != DocumentState.Unchanged);

        public IEnumerable<object> Inserts
            => from doc in _documents where doc.Value.State == DocumentState.Added select doc.Value.Document;

        public IEnumerable<object> Updates
            => from doc in _documents where doc.Value.State == DocumentState.Modified select doc.Value.Document;

        public IEnumerable<object> Deletions
            => from doc in _documents where doc.Value.State == DocumentState.Deleted select doc.Value.Document;

        public IEnumerable<T> InsertsFor<T>()
        {
            return from doc in _documents
                where doc.Value.State == DocumentState.Added && doc.Value.Type == typeof(T)
                select (T) doc.Value.Document;
        }

        public IEnumerable<T> UpdatesFor<T>()
        {
            return from doc in _documents
                where doc.Value.State == DocumentState.Modified && doc.Value.Type == typeof(T)
                select (T) doc.Value.Document;
        }

        public IEnumerable<T> DeletionsFor<T>()
        {
            return from doc in _documents
                where doc.Value.State == DocumentState.Deleted && doc.Value.Type == typeof(T)
                select (T) doc.Value.Document;
        }

        public void Track<T>(T document)
        {
            var id = IdentityHelper.GetIdFromDocument(document);
            var trackedDocument = new TrackedDocument
            {
                Document = document,
                Type = typeof(T),
                State = DocumentState.Unchanged
            };
            _documents.AddOrUpdate(
                id,
                trackedDocument,
                (key, oldValue) => trackedDocument);
        }

        public void Insert<T>(T document)
        {
            var id = IdentityHelper.GetIdFromDocument(document);
            _documents.GetOrAdd(id, new TrackedDocument
            {
                Document = document,
                Type = typeof(T),
                State = DocumentState.Added
            });
        }

        public void Update<T>(T document)
        {
            var id = IdentityHelper.GetIdFromDocument(document);
            if (!_documents.TryGetValue(id, out TrackedDocument trackedDocument))
            {
                throw new InvalidOperationException("Can't update unknown document");
            }
            _documents.TryUpdate(id, new TrackedDocument
            {
                Document = document,
                State = DocumentState.Modified,
                Type = typeof(T)
            }, trackedDocument);
        }

        public void DeleteById(object id)
        {
            if (!_documents.TryGetValue(id, out TrackedDocument trackedDocument))
            {
                throw new InvalidOperationException("Can't delete unknown document");
            }
            _documents.TryUpdate(id, new TrackedDocument
            {
                Document = trackedDocument.Document,
                State = DocumentState.Deleted,
                Type = trackedDocument.Type
            }, trackedDocument);
        }

        public void Delete<T>(T document)
        {
            var id = IdentityHelper.GetIdFromDocument(document);
           DeleteById(id);
        }

        public void MarkChangesSaved()
        {
            foreach (var document in _documents)
            {
                document.Value.State = DocumentState.Unchanged;
            }
        }
        
    }
}