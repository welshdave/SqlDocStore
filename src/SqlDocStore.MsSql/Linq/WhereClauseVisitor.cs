namespace SqlDocStore.MsSql.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Remotion.Linq.Clauses.Expressions;
    using SqlDocStore.MsSql.Linq.Parsers;
    using SubQueries;

    internal class WhereClauseVisitor : WhereClauseVisitorBase
    {
        private List<IMethodCallParser> _validParsers = new List<IMethodCallParser> 
        { 
            new StartsWithParser(),
            new ContainsParser()
        };

        public WhereClauseVisitor(Type docType, MsSqlQueryParts query) : base(docType, query)
        {
        }

        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        protected override Expression VisitMember(MemberExpression node)
        {
            Query.WhereBuilder.AppendFormat("JSON_VALUE(doc.Document, '$.{0}')",  node.Member.Name);
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var name = $"@{Parameters.Count.ToString()}";
            Parameters.Add(name,node.Value);
            Query.WhereBuilder.Append(name);
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            foreach(var parser in _validParsers)
            { 
                if (parser.TryParse(node, out string fragment, out KeyValuePair<string, string> parameter))
                {
                    Parameters.Add(parameter.Key, parameter.Value);
                    Query.WhereBuilder.Append(fragment);
                    return node;
                }
            }

            throw new NotSupportedException(
                $"SqlDocStore doesn't support Linq queries using the {node.Method.DeclaringType.FullName}.{node.Method.Name}() method");
        }

        protected override Expression VisitSubQuery(SubQueryExpression expression)
        {
            var visitor = new SubQueryVisitor(expression.QueryModel.MainFromClause.ItemType, Query);
            visitor.Visit(expression);
            
            visitor.Parameters.ToList().ForEach(x => Parameters.Add(x.Key, x.Value));

            return base.VisitSubQuery(expression);
        }
    }
}
