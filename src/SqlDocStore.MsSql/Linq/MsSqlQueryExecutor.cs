namespace SqlDocStore.MsSql.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using Remotion.Linq;
    using Remotion.Linq.Clauses.ResultOperators;
    using SqlDocStore.Linq;

    public class MsSqlQueryExecutor : IQueryExecutor
    {
        private readonly IDocumentSession _session;
        private readonly IQueryCompiler _compiler;
        private readonly Func<SqlConnection> _createConnection;
        private ISerializer _serializer = new SimpleJsonSerializer();

        public MsSqlQueryExecutor(IDocumentSession session, IQueryCompiler compiler)
        {
            _session = session;
            _compiler = compiler;
            _createConnection = () => new SqlConnection(session.DocumentStore.Settings.ConnectionString);
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            var sql = _compiler.Compile(queryModel);
            throw new NotImplementedException();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            var results = ExecuteCollection<T>(queryModel);

            foreach (var resultOperator in queryModel.ResultOperators)
            {
                switch (resultOperator)
                {
                    case LastResultOperator _:
                        return returnDefaultWhenEmpty ? results.LastOrDefault() : results.Last();
                    case SingleResultOperator _:
                        return results.Single();
                }
            }
            return returnDefaultWhenEmpty ? results.FirstOrDefault() : results.First();
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
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
                    if (!reader.HasRows)
                    {
                        yield break;
                    }
                    var docs = new List<Tuple<T, Guid>>();
                    try
                    {
                        while (reader.Read())
                        {
                            var doc = _serializer.Deserialize<T>(reader["Document"].ToString());
                            var eTag = Guid.Parse(reader["ETag"].ToString());
                            docs.Add(new Tuple<T, Guid>(doc,eTag));
                        }
                    }
                    catch (FormatException)
                    {
                        throw new InvalidCastException($"Unable to cast document to Type {typeof(T)}");
                    }

                    foreach (var doc in docs)
                    {
                        _session.ChangeTracker.Track(doc.Item1, doc.Item2);
                        yield return doc.Item1;
                    }
                }
            }
        }
    }
}