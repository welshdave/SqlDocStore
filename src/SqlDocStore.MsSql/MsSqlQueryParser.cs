namespace SqlDocStore.MsSql
{
    using System;
    using System.Linq.Expressions;
    using Remotion.Linq;
    using Remotion.Linq.Parsing.Structure;

    public class MsSqlQueryParser : IQueryParser
    {
        public QueryModel GetParsedQuery(Expression expressionTreeRoot)
        {
            throw new NotImplementedException();
        }
    }
}