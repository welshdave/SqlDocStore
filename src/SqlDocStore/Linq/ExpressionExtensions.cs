namespace SqlDocStore.Linq
{
    using System.Linq.Expressions;

    public static class ExpressionExtensions
    {
        public static bool IsNull(this Expression exp)
        {
            var constantExpression = exp as ConstantExpression;
            return constantExpression != null && constantExpression.Value == null;
        }
    }
}
