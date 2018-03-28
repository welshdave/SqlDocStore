namespace SqlDocStore.Vendor.EnsureThat
{
    using Annotations;
    using JetBrains.Annotations;

    public static partial class EnsureArg
    {
        [NotNull]
        public static T IsNotNull<T>([NoEnumeration, ValidatedNotNull] T value, [InvokerParameterName] string paramName = Param.DefaultName, OptsFn optsFn = null)
            => Ensure.Any.IsNotNull(value, paramName, optsFn);
    }
}