namespace SqlDocStore.Vendor.EnsureThat
{
    using JetBrains.Annotations;

    public static partial class EnsureArg
    {
        public static T? IsNotNull<T>(T? value, [InvokerParameterName] string paramName = Param.DefaultName, OptsFn optsFn = null) where T : struct
            => Ensure.Any.IsNotNull(value, paramName, optsFn);
    }
}