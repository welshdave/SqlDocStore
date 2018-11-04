namespace SqlDocStore.MsSql.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using Remotion.Linq.Clauses.Expressions;
    using Remotion.Linq.Parsing;
    using SqlDocStore.Linq;
    using SubQueries;

    internal class WhereClauseVisitor : RelinqExpressionVisitor
    {
        private readonly StringBuilder _whereClause = new StringBuilder();
        private readonly Type _docType;

        
        public WhereClauseVisitor(Type docType)
        {
            _docType = docType;
        }

        public string WhereClause => _whereClause.ToString();
        public string SubQuery { get; private set; }

        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        protected override Expression VisitBinary(BinaryExpression node)
        {
            _whereClause.Append("(");

            //https://www.re-motion.org/blogs/mix/2009/10/16/vb-net-specific-text-comparison-in-linq-queries/
            node = ExpressionHelper.ConvertVbStringCompare(node);

            var left = node.Left;
            var right = node.Right;
            if ((right.NodeType == ExpressionType.MemberAccess) && (((MemberExpression)right).Member.DeclaringType == _docType))
            {
                left = node.Right;
                right = node.Left;
            }

            Visit(left);
            if (node.NodeType == ExpressionType.NotEqual && right.IsNull())
            {
                _whereClause.Append(" IS NOT NULL ");
            }
            else if (ExpressionHelper.Operators.ContainsKey(node.NodeType))
            {
                _whereClause.Append(ExpressionHelper.Operators[node.NodeType]);
            }
            else
            {
                throw new NotSupportedException($"{node.NodeType.ToString()} statement is not supported");
            }

            if (!right.IsNull())
                Visit(right);
            _whereClause.Append(")");
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _whereClause.AppendFormat("JSON_VALUE(doc.Document, '$.{0}')",  node.Member.Name);
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {

            var name = $"@{Parameters.Count.ToString()}";
            Parameters.Add(name,node.Value);
            _whereClause.Append(name);
            return node;
        }

        protected override Expression VisitSubQuery(SubQueryExpression expression)
        {
            var visitor = new SubQueryVisitor(expression.QueryModel.MainFromClause.ItemType);
            visitor.Visit(expression);

            SubQuery = visitor.SubQuery;
            visitor.Parameters.ToList().ForEach(x => Parameters.Add(x.Key, x.Value));

            return base.VisitSubQuery(expression);
        }
    }
}
