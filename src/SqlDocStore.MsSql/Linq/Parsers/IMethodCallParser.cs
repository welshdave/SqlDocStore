namespace SqlDocStore.MsSql.Linq.Parsers
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    internal interface IMethodCallParser
    {
        bool TryParse(MethodCallExpression expression, out string fragment, out KeyValuePair<string, string> parameter);
    }
}