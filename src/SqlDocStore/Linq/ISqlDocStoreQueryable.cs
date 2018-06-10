namespace SqlDocStore.Linq
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISqlDocStoreQueryable<T> : IQueryable<T>
    {
        Task<bool> AnyAsync(CancellationToken token = default);
        Task<int> CountAsync(CancellationToken token = default);
        Task<T> SingleAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default);
        Task<T> SingleOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken token = default);

    }
}
