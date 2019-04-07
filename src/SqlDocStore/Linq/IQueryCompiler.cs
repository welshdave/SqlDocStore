namespace SqlDocStore.Linq
{
    using Remotion.Linq;

    public interface IQueryCompiler
    {
        SqlQuery Compile(QueryModel queryModel);
    }
}
