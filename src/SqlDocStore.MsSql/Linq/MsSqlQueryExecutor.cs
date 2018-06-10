namespace SqlDocStore.MsSql.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using Remotion.Linq;
    using SqlDocStore.Linq;

    public class MsSqlQueryExecutor : IQueryExecutor
    {
        private readonly IDocumentStore _store;
        private readonly IQueryCompiler _compiler;
        private readonly Func<SqlConnection> _createConnection;

        public MsSqlQueryExecutor(IDocumentStore store, IQueryCompiler compiler)
        {
            _compiler = compiler;
            _store = store;
            _createConnection = () => new SqlConnection(_store.Settings.ConnectionString);
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            var sql = _compiler.Compile(queryModel);
            throw new NotImplementedException();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            var sql = _compiler.Compile(queryModel);

            using (var connection = _createConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(sql.Sql, connection))
                {
                    foreach (var key in sql.Parameters.Keys)
                    {
                        command.Parameters.AddWithValue(key, sql.Parameters[key]);
                    }
                    var reader = command.ExecuteReader();
                    if (!reader.HasRows) return default(T);
                    try
                    {
                        reader.Read();
                        var doc = SimpleJson.DeserializeObject<T>(reader["Document"].ToString());
                        //var eTag = Guid.Parse(reader["ETag"].ToString());
                        //TODO: include in change tracker.
                        //ChangeTracker.Track(doc, eTag);
                        return doc;
                    }
                    catch (FormatException)
                    {
                        throw new InvalidCastException($"Unable to cast document to Type {typeof(T)}");
                    }
                }
            }

            
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            var sql = _compiler.Compile(queryModel);
            throw new NotImplementedException();
        }
    }
}