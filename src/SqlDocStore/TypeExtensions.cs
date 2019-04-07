namespace SqlDocStore
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public static class TypeExtensions
    {
        public static bool IsAnonymousType(this Type type)
        {
            var hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0;
            var nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            return hasCompilerGeneratedAttribute && nameContainsAnonymousType;
        }
    }
}
