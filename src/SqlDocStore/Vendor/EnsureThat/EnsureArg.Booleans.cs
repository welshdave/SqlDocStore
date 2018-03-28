namespace SqlDocStore.Vendor.EnsureThat
{
    using JetBrains.Annotations;

    public static partial class EnsureArg
    {
        public static bool IsTrue(bool value, [InvokerParameterName] string paramName = Param.DefaultName, OptsFn optsFn = null)
            => Ensure.Bool.IsTrue(value, paramName, optsFn);

        public static bool IsFalse(bool value, [InvokerParameterName] string paramName = Param.DefaultName, OptsFn optsFn = null)
            => Ensure.Bool.IsFalse(value, paramName, optsFn);
    }
}