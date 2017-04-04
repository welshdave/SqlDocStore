namespace SqlDocStore
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;

    public static class IdentityHelper
    {
        private const string Identity = "Id";

        private static readonly ConcurrentDictionary<Type, MemberInfo> PropertyCache =
            new ConcurrentDictionary<Type, MemberInfo>();

        private static readonly ConcurrentDictionary<Type, object> DefaultValues =
            new ConcurrentDictionary<Type, object>();

        private static readonly List<Type> AllowedTypes = new List<Type>
        {
            typeof(string),
            typeof(int),
            typeof(long),
            typeof(Guid)
        };

        public static object GetIdFromDocument<T>(T document)
        {
            ValidateDocumentId(document);
            if (document is IDynamicMetaObjectProvider)
            {
                return ((dynamic) document).Id;
            }

            var info = GetIdProperty(document);

            if (info == null)
                throw new InvalidOperationException($"Type {typeof(T)} does not have Identity property {Identity}");
            return info.GetValue(document);
        }

        internal static void ValidateDocumentId<T>(T document)
        {
            if (document is IDynamicMetaObjectProvider)
            {
                ValidateDynamicDocumentId(document);
                return;
            }

            var info = GetIdProperty(document);

            if (info == null)
                throw new InvalidOperationException($"Type {typeof(T)} does not have Identity property {Identity}");

            if (!AllowedTypes.Contains(info.PropertyType))
            {
                throw new InvalidOperationException(
                    $"Identity Property {Identity} for Type {typeof(T)} must be one of these Types: {AllowedTypes.Select(x => x.FullName).Aggregate((i, j) => i + "," + j)}");
            }

            if (!DefaultValues.TryGetValue(info.PropertyType, out object defaultValue))
            {
                defaultValue = info.PropertyType.IsValueType ? Activator.CreateInstance(info.PropertyType) : null;
                DefaultValues.GetOrAdd(info.PropertyType, defaultValue);
            }
            if (defaultValue != null && defaultValue.Equals(info.GetValue(document)))
                throw new InvalidOperationException($"Identity property {Identity} must be of non-default value");
        }

        private static void ValidateDynamicDocumentId(dynamic document)
        {   //TODO: proper type/default value checking.
            try
            {
                var id = document.Id;
            }
            catch
            {
                throw new InvalidOperationException($"Dynamic document does not have Identity property {Identity}");
            }
        }

        private static PropertyInfo GetIdProperty(object document)
        {
            MemberInfo info;
            var type = document.GetType();
            
            try
            {
                if (!PropertyCache.TryGetValue(type, out info))
                {
                    info = type.GetProperties().Single(p => p.Name == Identity);
                    PropertyCache.GetOrAdd(type, info);
                }
            }
            catch
            {
                throw new InvalidOperationException($"Type {type} does not have Identity property {Identity}");
            }

            return info as PropertyInfo;
        }
    }
}