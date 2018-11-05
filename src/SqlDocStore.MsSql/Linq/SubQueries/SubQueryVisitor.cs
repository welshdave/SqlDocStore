namespace SqlDocStore.MsSql.Linq.SubQueries
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Remotion.Linq.Clauses.Expressions;
    using Remotion.Linq.Clauses.ResultOperators;
    using Remotion.Linq.Parsing;
    using SqlDocStore.Linq;

    internal class SubQueryVisitor : RelinqExpressionVisitor
    {
        private readonly Type _docType;
        private readonly string _suffix;
        private readonly MsSqlQueryParts _query;

        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        public SubQueryVisitor(Type docType, MsSqlQueryParts query)
        {
            _docType = docType;
            _query = query;
            _suffix = Guid.NewGuid().ToString().Replace("-", "");
        }

        protected override Expression VisitSubQuery(SubQueryExpression expression)
        {
            var query = expression.QueryModel;
            
            var fromExpression = query.MainFromClause.FromExpression as MemberExpression;


            _query.FromBuilder.AppendFormat(
                " CROSS APPLY OPENJSON(JSON_QUERY(Document, '$.{0}')) as subDocs_{1} ", fromExpression.Member.Name, _suffix);
            _query.FromBuilder.AppendFormat("CROSS APPLY OPENJSON(subDocs_{0}.[value], '$') as SubDoc_{0} ", _suffix);

            foreach(var op in query.ResultOperators)
            {
                if (op is AllResultOperator allOp)
                {
                    VisitBinary(allOp.Predicate as BinaryExpression);
                }
            }

            return base.VisitSubQuery(expression);
        }

        protected override Expression VisitBinary(BinaryExpression expression)
        {
            _query.WhereBuilder.Append("(");

            //https://www.re-motion.org/blogs/mix/2009/10/16/vb-net-specific-text-comparison-in-linq-queries/
            expression = ExpressionHelper.ConvertVbStringCompare(expression);

            var left = expression.Left;
            var right = expression.Right;
            if ((right.NodeType == ExpressionType.MemberAccess) && (((MemberExpression)right).Member.DeclaringType == _docType))
            {
                left = expression.Right;
                right = expression.Left;
            }

            Visit(left);
            if (expression.NodeType == ExpressionType.NotEqual && right.IsNull())
            {
                _query.WhereBuilder.Append(" IS NOT NULL ");
            }
            else if (ExpressionHelper.Operators.ContainsKey(expression.NodeType))
            {
                _query.WhereBuilder.Append(ExpressionHelper.Operators[expression.NodeType]);
            }
            else
            {
                throw new NotSupportedException($"{expression.NodeType.ToString()} statement is not supported");
            }

            if (!right.IsNull())
                Visit(right);
            _query.WhereBuilder.Append(")");
            return expression;
        }

        protected override Expression VisitMember(MemberExpression expression)
        {
            _query.WhereBuilder.AppendFormat("subDoc_{0}.[key] = '{1}' AND subDoc_{0}.[value]", _suffix, expression.Member.Name);
            return expression;
        }

        protected override Expression VisitConstant(ConstantExpression expression)
        {

            var name = $"@{Parameters.Count.ToString()}_{_suffix}";
            Parameters.Add(name, expression.Value);
            _query.WhereBuilder.Append(name);
            return expression;
        }

    }
}
