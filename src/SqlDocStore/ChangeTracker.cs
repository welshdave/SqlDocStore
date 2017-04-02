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

        private readonly ConcurrentDictionary<object, object> _originalDocuments =
            new ConcurrentDictionary<object, object>();

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
            _documents.GetOrAdd(id, new TrackedDocument
            {
                Document = document,
                Type = typeof(T),
                State = DocumentState.Unchanged
            });
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
            if (_documents.TryGetValue(id, out TrackedDocument trackedDocument))
            {
                if (trackedDocument.State == DocumentState.Unchanged)
                    _originalDocuments.GetOrAdd(id, trackedDocument.Document);
            }
            else
            {
                throw new InvalidOperationException("Can't update unknown document");
            }
            _documents.TryUpdate(id, new TrackedDocument
            {
                Document = document,
                State = DocumentState.Modified,
                Type = trackedDocument.Type
            }, trackedDocument);
        }

        public void DeleteById(object id)
        {
            if (_documents.TryGetValue(id, out TrackedDocument trackedDocument))
            {
                if (trackedDocument.State == DocumentState.Unchanged)
                    _originalDocuments.GetOrAdd(id, trackedDocument.Document);
            }
            else
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

        public void ClearChanges()
        {
            foreach (var id in _documents.Keys)
            {
                switch (_documents[id].State)
                {
                    case DocumentState.Added:
                        _documents.TryRemove(id, out TrackedDocument doc);
                        break;
                    case DocumentState.Modified:
                    case DocumentState.Deleted:
                        if(_originalDocuments.TryGetValue(id, out object originalDocument))
                        {
                            _documents.TryUpdate(id, new TrackedDocument()
                            {
                                Document = originalDocument,
                                State = DocumentState.Unchanged,
                                Type = _documents[id].Type
                            }, _documents[id]);
                        }
                        break;
                }
            }
        }
    }
}