namespace SqlDocStore.MsSql.Linq.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class StartsWithParser : IMethodCallParser
    {
        public bool TryParse(MethodCallExpression expression, out string fragment, out KeyValuePair<string, string> parameter)
        {
            if (!(expression.Method.Name == nameof(string.StartsWith)))
            {
                fragment = default;
                parameter = default;
                return false;
            }

            var name = $"@{Guid.NewGuid().ToString().Replace("-","")}";
            fragment = $"JSON_VALUE(doc.Document, '$.{((MemberExpression)expression.Object).Member.Name}') LIKE {name} + '%'";
            parameter = new KeyValuePair<string, string>(name, ((ConstantExpression)expression.Arguments[0]).Value.ToString());
            return true;
        }
    }
}