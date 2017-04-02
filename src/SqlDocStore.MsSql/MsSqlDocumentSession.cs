namespace SqlDocStore.MsSql
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Deployment.Internal;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Sql;

    public class MsSqlDocumentSession : DocumentSessionBase
    {
        private readonly Func<SqlConnection> _createConnection;
        private readonly MsSqlQueryParser _parser = new MsSqlQueryParser();
        private readonly Scripts _scripts;


        public MsSqlDocumentSession(Func<SqlConnection> createConnection, IDocumentStore store)
        {
            _createConnection = createConnection;
            DocumentStore = store;
            _scripts = new Scripts(DocumentStore.Settings.Schema, DocumentStore.Settings.Table);
        }

        protected override void DeleteInternal<T>(T entity)
        {
            ChangeTracker.Delete(entity);
        }

        protected override void DeleteByIdInternal(object id)
        {
            ChangeTracker.DeleteById(id);
        }

        protected override void StoreInternal<T>(T entity)
        {
            ChangeTracker.Insert(entity);
        }

        protected override async Task SaveChangesInternal(CancellationToken token)
        {
            using (var connection = _createConnection())
            {
                await connection.OpenAsync(token).ConfigureAwait(false);
                var tran = connection.BeginTransaction();
                foreach (var document in ChangeTracker.Inserts)
                {
                    var json = SimpleJson.SerializeObject(document);
                    using (var command = new SqlCommand(_scripts.UpsertDocument, connection))
                    {
                        command.Transaction = tran;
                        command.Parameters.AddWithValue("@document", json);
                        await command.ExecuteNonQueryAsync(token).ConfigureAwait(false);
                    }
                }
                foreach (var document in ChangeTracker.Updates)
                {
                    var json = SimpleJson.SerializeObject(document);
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
                tran.Commit();
                ChangeTracker.MarkChangesSaved();
            }
        }

        protected override IQueryable<T> QueryInternal<T>()
        {
            using (var connection = _createConnection())
            {
                return new List<T>().AsQueryable();
            }
        }

        protected override Task<T> LoadInternal<T>(object id, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}