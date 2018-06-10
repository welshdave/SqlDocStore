namespace SqlDocStore.Linq
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Remotion.Linq;
    using Remotion.Linq.Parsing.Structure;

    public class SqlDocStoreQueryable<T> : QueryableBase<T>, ISqlDocStoreQueryable<T>
    {
        public SqlDocStoreQueryable(IQueryProvider provider) : base(provider)
        {
        }

        public SqlDocStoreQueryable(IQueryProvider provider, Expression expression) : base(provider, expression)
        {
        }

        public SqlDocStoreQueryable(IQueryParser queryParser, IQueryExecutor executor) : base(
            new DefaultQueryProvider(typeof(SqlDocStoreQueryable<>), queryParser, executor))
        {
        }

        //public IDocumentStore Store => Executor.Store;

        //public SqlDocStoreQueryExecutor Executor => (SqlDocStoreQueryExecutor)((SqlDocStoreQueryProvider) Provider).Executor;

        public Task<bool> AnyAsync(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<T> SingleAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
