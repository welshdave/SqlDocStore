namespace SqlDocStore.MsSql
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Remotion.Linq;
    using Sql;
    using MsSqlQueryExecutor = Linq.MsSqlQueryExecutor;

    public class MsSqlDocumentSession : DocumentSessionBase
    {
        private readonly Func<SqlConnection> _createConnection;
        private readonly Scripts _scripts;
        private readonly ConcurrencyModel _concurrencyModel;
        private const int ConcurrencyError = 50001;
        private const int DocumentExists = 50002;
        private readonly ISerializer _serializer = new SimpleJsonSerializer();
        
        public MsSqlDocumentSession(Func<SqlConnection> createConnection, IDocumentStore store)
        {
            _createConnection = createConnection;
            DocumentStore = store;
            _scripts = new Scripts(DocumentStore.Settings.Schema, DocumentStore.Settings.Table);
            _concurrencyModel = DocumentStore.Settings.ConcurrencyModel;
            ChangeTracker = new ChangeTracker(_concurrencyModel);
            Executor = new MsSqlQueryExecutor(this, new MsSqlQueryCompiler(DocumentStore));
        }

        protected sealed override IQueryExecutor Executor { get; set; }

        protected override void DeleteInternal<T>(T document)
        {
            ChangeTracker.Delete(document);
        }

        protected override void DeleteByIdInternal(object id)
        {
            ChangeTracker.DeleteById(id);
        }

        protected override void StoreInternal<T>(T document)
        {
            if (_concurrencyModel == ConcurrencyModel.Pessimistic)
            {
                var id = IdentityHelper.GetIdFromDocument(document);
                if (ChangeTracker.Documents.ContainsKey(id) && ChangeTracker.Documents[id].State != DocumentState.Added)
                    ChangeTracker.Update(document);
            }

            ChangeTracker.Insert(document);
        }

        protected override async Task SaveChangesInternal(CancellationToken token)
        {
            using (var connection = _createConnection())
            {
                await connection.OpenAsync(token).ConfigureAwait(false);
                var tran = connection.BeginTransaction();

                if (_concurrencyModel == ConcurrencyModel.Optimistic)
                {
                    foreach (var document in ChangeTracker.Inserts.Union(ChangeTracker.Updates))
                    {
                        var json = _serializer.Serialize(document);
                        using (var command = new SqlCommand(_scripts.UpsertDocument, connection))
                        {
                            command.Transaction = tran;
                            command.Parameters.AddWithValue("@document", json);
                            await command.ExecuteNonQueryAsync(token).ConfigureAwait(false);
                        }
                    }

                    foreach (var document in ChangeTracker.Deletions)
                    {
                        var id = IdentityHelper.GetIdFromDocument(document);
                        using (var command = new SqlCommand(_scripts.DeleteDocument, connection))
                        {
                            command.Transaction = tran;
                            command.Parameters.AddWithValue("@id", id.ToString());
                            await command.ExecuteNonQueryAsync(token).ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    foreach (var document in ChangeTracker.Inserts)
                    {
                        var json = _serializer.Serialize(document);
                        var id = IdentityHelper.GetIdFromDocument(document);
                        try
                        {
                            using (var command = new SqlCommand(_scripts.InsertDocument, connection))
                            {
                                command.Transaction = tran;
                                command.Parameters.AddWithValue("@document", json);
                                command.Parameters.AddWithValue("@etag", ChangeTracker.Documents[id].ETag);
                                await command.ExecuteNonQueryAsync(token).ConfigureAwait(false);
                            }
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Number == DocumentExists) throw new DocumentIdExistsException(ex.Message);
                            throw new DataStoreException("Error in underlying data store", ex);
                        }
                    }

                    foreach (var document in ChangeTracker.Updates)
                    {
                        var json = _serializer.Serialize(document);
                        var id = IdentityHelper.GetIdFromDocument(document);
                        try
                        {
                            using (var command = new SqlCommand(_scripts.UpdateDocument, connection))
                            {
                                command.Transaction = tran;
                                command.Parameters.AddWithValue("@document", json);
                                command.Parameters.AddWithValue("@etag", ChangeTracker.Documents[id].ETag);
                                var newETag = await command.ExecuteScalarAsync(token).ConfigureAwait(false);
                                ChangeTracker.Documents[id].ETag = Guid.Parse(newETag.ToString());
                            }
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Number == ConcurrencyError) throw new ConcurrencyException(ex.Message);
                            throw new DataStoreException("Error in underlying data store", ex);
                        }
                    }

                    foreach (var document in ChangeTracker.Deletions)
                    {
                        var id = IdentityHelper.GetIdFromDocument(document);
                        try
                        {
                            using (var command = new SqlCommand(_scripts.PessimisticDeleteDocument, connection))
                            {
                                command.Transaction = tran;
                                command.Parameters.AddWithValue("@id", id.ToString());
                                command.Parameters.AddWithValue("@etag", ChangeTracker.Documents[id].ETag);
                                await command.ExecuteNonQueryAsync(token).ConfigureAwait(false);
                            }
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Number == ConcurrencyError) throw new ConcurrencyException(ex.Message);
                            throw new DataStoreException("Error in underlying data store", ex);
                        }
                    }
                }

                tran.Commit();
                if (_concurrencyModel == ConcurrencyModel.Pessimistic)
                {
                    ChangeTracker.MarkChangesSaved();
                    return;
                }

                ChangeTracker.Documents.Clear();
            }
        }

        protected override async Task<T> LoadInternal<T>(object id, CancellationToken token)
        {
            using (var connection = _createConnection())
            {
                await connection.OpenAsync(token).ConfigureAwait(false);
                using (var command = new SqlCommand(_scripts.GetDocumentById, connection))
                {
                    command.Parameters.AddWithValue("@id", id.ToString());
                    var reader = await command.ExecuteReaderAsync(token);
                    if (!reader.HasRows) return default(T);
                    try
                    {
                        reader.Read();
                        var doc = _serializer.Deserialize<T>(reader["Document"].ToString());
                        var eTag = Guid.Parse(reader["ETag"].ToString());
                        ChangeTracker.Track(doc, eTag);
                        return doc;
                    }
                    catch (FormatException)
                    {
                        throw new InvalidCastException($"Unable to cast document with Id {id} to Type {typeof(T)}");
                    }
                }
            }
        }
    }
}