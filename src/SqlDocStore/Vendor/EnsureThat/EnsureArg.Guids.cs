namespace SqlDocStore.Vendor.EnsureThat
{
    using System;
    using JetBrains.Annotations;

    public static partial class EnsureArg
    {
        public static Guid IsNotEmpty(Guid value, [InvokerParameterName] string paramName = Param.DefaultName, OptsFn optsFn = null)
            => Ensure.Guid.IsNotEmpty(value, paramName, optsFn);
    }
}