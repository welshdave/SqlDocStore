namespace SqlDocStore.Vendor.EnsureThat
{
    public static class EnsureValueTypeExtensions
    {
        public static void IsNotDefault<T>(this Param<T> param) where T : struct
        {
            if (!Ensure.IsActive)
                return;

            if (default(T).Equals(param.Value))
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.ValueTypes_IsNotDefault_Failed);
        }
    }
}