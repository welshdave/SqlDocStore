namespace SqlDocStore.Vendor.EnsureThat.Annotations
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class ValidatedNotNullAttribute : Attribute { }
}