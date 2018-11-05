namespace SqlDocStore.MsSql.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Remotion.Linq.Parsing;
    using SqlDocStore.Linq;

    internal abstract class WhereClauseVisitorBase : RelinqExpressionVisitor
    {
        protected readonly Type DocType;
        protected readonly MsSqlQueryParts Query;

        private static readonly Dictionary<ExpressionType, string> Operators = new Dictionary<ExpressionType, string>
        {
            { ExpressionType.Equal, " = " },
            { ExpressionType.NotEqual, " <> " },
            { ExpressionType.GreaterThan, " > " },
            { ExpressionType.GreaterThanOrEqual, " >= " },
            { ExpressionType.LessThan, " < " },
            { ExpressionType.LessThanOrEqual, " <= " },
            { ExpressionType.AndAlso, " AND " },
            { ExpressionType.OrElse, " OR " },
            { ExpressionType.Add, " + " },
            { ExpressionType.Subtract, " - " },
            { ExpressionType.Multiply, " * " },
            { ExpressionType.Divide, " / " },
            { ExpressionType.Modulo, " % " },
            { ExpressionType.And, " & " },
            { ExpressionType.Or, " | " }
        };

        protected WhereClauseVisitorBase(Type docType, MsSqlQueryParts query)
        {
            DocType = docType;
            Query = query;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Query.WhereBuilder.Append("(");

            //https://www.re-motion.org/blogs/mix/2009/10/16/vb-net-specific-text-comparison-in-linq-queries/
            node = ConvertVbStringCompare(node);

            var left = node.Left;
            var right = node.Right;
            if ((right.NodeType == ExpressionType.MemberAccess) && (((MemberExpression)right).Member.DeclaringType == DocType))
            {
                left = node.Right;
                right = node.Left;
            }

            Visit(left);
            if (node.NodeType == ExpressionType.NotEqual && right.IsNull())
            {
                Query.WhereBuilder.Append(" IS NOT NULL ");
            }
            else if (Operators.ContainsKey(node.NodeType))
            {
                Query.WhereBuilder.Append(Operators[node.NodeType]);
            }
            else
            {
                throw new NotSupportedException($"{node.NodeType.ToString()} statement is not supported");
            }

            if (!right.IsNull())
                Visit(right);
            Query.WhereBuilder.Append(")");
            return node;
        }

        private static BinaryExpression ConvertVbStringCompare(BinaryExpression exp)
        {
            if (exp.Left.NodeType == ExpressionType.Call)
            {
                var compareStringCall = (MethodCallExpression)exp.Left;
                if (compareStringCall.Method.DeclaringType.FullName == "Microsoft.VisualBasic.CompilerServices.Operators" && compareStringCall.Method.Name == "CompareString")
                {
                    var arg1 = compareStringCall.Arguments[0];
                    var arg2 = compareStringCall.Arguments[1];

                    switch (exp.NodeType)
                    {
                        case ExpressionType.LessThan:
                            return Expression.LessThan(arg1, arg2);
                        case ExpressionType.LessThanOrEqual:
                            return Expression.LessThanOrEqual(arg1, arg2);
                        case ExpressionType.GreaterThan:
                            return Expression.GreaterThan(arg1, arg2);
                        case ExpressionType.GreaterThanOrEqual:
                            return Expression.GreaterThanOrEqual(arg1, arg2);
                        case ExpressionType.NotEqual:
                            return Expression.NotEqual(arg1, arg2);
                        default:
                            return Expression.Equal(arg1, arg2);
                    }
                }
            }
            return exp;
        }
    }
}
