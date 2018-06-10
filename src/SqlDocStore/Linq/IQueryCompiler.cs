namespace SqlDocStore.Linq
{
    using System.Linq.Expressions;
    using Remotion.Linq;

    public interface IQueryCompiler
    {
        SqlQuery Compile(QueryModel queryModel);
    }
}
