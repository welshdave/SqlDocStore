namespace SqlDocStore.MsSql.Linq.SubQueries
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Remotion.Linq.Clauses.Expressions;
    using Remotion.Linq.Clauses.ResultOperators;

    internal class SubQueryVisitor : WhereClauseVisitorBase
    {
        private readonly string _suffix;

        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        public SubQueryVisitor(Type docType, MsSqlQueryParts query) : base(docType, query)
        {
            _suffix = Guid.NewGuid().ToString().Replace("-", "");
        }

        protected override Expression VisitSubQuery(SubQueryExpression expression)
        {
            var query = expression.QueryModel;
            
            var fromExpression = query.MainFromClause.FromExpression as MemberExpression;


            Query.FromBuilder.AppendFormat(
                " CROSS APPLY OPENJSON(JSON_QUERY(Document, '$.{0}')) as subDocs_{1} ", fromExpression.Member.Name, _suffix);
            Query.FromBuilder.AppendFormat("CROSS APPLY OPENJSON(subDocs_{0}.[value], '$') as SubDoc_{0} ", _suffix);

            foreach(var op in query.ResultOperators)
            {
                if (op is AllResultOperator allOp)
                {
                    VisitBinary(allOp.Predicate as BinaryExpression);
                }
            }

            return base.VisitSubQuery(expression);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            Query.WhereBuilder.AppendFormat("subDoc_{0}.[key] = '{1}' AND subDoc_{0}.[value]", _suffix, node.Member.Name);
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {

            var name = $"@{Parameters.Count.ToString()}_{_suffix}";
            Parameters.Add(name, node.Value);
            Query.WhereBuilder.Append(name);
            return node;
        }

    }
}
