namespace SqlDocStore.MsSql.Linq.SubQueries
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Remotion.Linq.Clauses;
    using Remotion.Linq.Clauses.Expressions;
    using Remotion.Linq.Clauses.ResultOperators;
    using System.Linq;

    internal class SubQueryVisitor : WhereClauseVisitorBase
    {
        private string _currentSuffix;
        private string _fromTemplate;
        
        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        public SubQueryVisitor(Type docType, MsSqlQueryParts query) : base(docType, query)
        {
        }

        protected override Expression VisitSubQuery(SubQueryExpression expression)
        {
            var query = expression.QueryModel;

            var fromExpression = query.MainFromClause.FromExpression as MemberExpression;

            _fromTemplate = $" CROSS APPLY OPENJSON(JSON_QUERY(Document, '$.{fromExpression.Member.Name}')) as subDocs_%%SUFFIX%% CROSS APPLY OPENJSON (subDocs_%%SUFFIX%%.[value], '$') AS SubDoc_%%SUFFIX%% ";

            foreach (var op in query.ResultOperators)
            {
                if (op is AnyResultOperator)
                {
                    foreach (var clause in query.BodyClauses.OfType<WhereClause>())
                    {
                        VisitBinary(clause.Predicate as BinaryExpression);
                    }
                }
                else
                {
                    throw new NotSupportedException(
                        $"SqlDocStore currently only supports 'Any' in nested queries");
                }
            }

            return base.VisitSubQuery(expression);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _currentSuffix = Guid.NewGuid().ToString().Replace("-", "");

            Query.FromBuilder.Append(_fromTemplate.Replace("%%SUFFIX%%", _currentSuffix));

            Query.WhereBuilder.AppendFormat("subDoc_{0}.[key] = '{1}' AND subDoc_{0}.[value]", _currentSuffix, node.Member.Name);
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var name = $"@{Parameters.Count.ToString()}_{_currentSuffix}";
            Parameters.Add(name, node.Value);
            Query.WhereBuilder.Append(name);
            return node;
        }
    }
}
