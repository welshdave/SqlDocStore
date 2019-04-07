namespace SqlDocStore.Linq
{
    using System.Linq;
    using System.Linq.Expressions;
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
    }
}
