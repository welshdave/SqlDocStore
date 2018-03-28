namespace SqlDocStore.Vendor.EnsureThat
{
    public static class EnsureObjectExtensions
    {
        public static void IsNotNull<T>(this Param<T> param)
        {
            if (!Ensure.IsActive)
                return;

            if (param.Value == null)
                throw ExceptionFactory.CreateForParamNullValidation(param, ExceptionMessages.Common_IsNotNull_Failed);
        }
    }
}