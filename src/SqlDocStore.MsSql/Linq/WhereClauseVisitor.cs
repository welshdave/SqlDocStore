namespace SqlDocStore.MsSql.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq.Expressions;
    using System.Text;
    using Remotion.Linq.Parsing;
    using SqlDocStore.Linq;

    internal class WhereClauseVisitor : ThrowingExpressionVisitor
    {
        private readonly StringBuilder _whereClause = new StringBuilder();
        private readonly Dictionary<string,object> _parameters = new Dictionary<string, object>();
        private readonly List<string> _fields = new List<string>();
        private readonly Type _docType;

        private static readonly Dictionary<ExpressionType, string> _operators = new Dictionary<ExpressionType, string>
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

        public WhereClauseVisitor(Type docType)
        {
            _docType = docType;
        }

        public string WhereClause => _whereClause.ToString();

        public Dictionary<string,object> Parameters => _parameters;

        protected override Expression VisitBinary(BinaryExpression expression)
        {
            _whereClause.Append("(");

            //https://www.re-motion.org/blogs/mix/2009/10/16/vb-net-specific-text-comparison-in-linq-queries/
            expression = ConvertVbStringCompare(expression);

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
                _whereClause.Append(" IS NOT NULL ");
            }
            else if (_operators.ContainsKey(expression.NodeType))
            {
                _whereClause.Append(_operators[expression.NodeType]);
            }
            else
            {
                throw new NotSupportedException($"{expression.NodeType.ToString()} statement is not supported");
            }

            if (!right.IsNull())
                Visit(right);
            _whereClause.Append(")");
            return expression;
        }

        protected override Expression VisitMember(MemberExpression expression)
        {
            _whereClause.AppendFormat("JSON_VALUE(Document, '$.{0}')",  expression.Member.Name);
            return expression;
        }

        protected override Expression VisitConstant(ConstantExpression expression)
        {

            var name = $"@{_parameters.Count.ToString()}";
            _parameters.Add(name,expression.Value);
            _whereClause.Append(name);
            return expression;
        }

        

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            throw new NotImplementedException($"{visitMethod} method is not implemented");
        }

        private BinaryExpression ConvertVbStringCompare(BinaryExpression exp)
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
