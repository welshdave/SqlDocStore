namespace SqlDocStore
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.Remoting.Messaging;

    public static class IdentityHelper
    {
        private const string Identity = "Id";

        private static readonly ConcurrentDictionary<Type, MemberInfo> _propertyCache =
            new ConcurrentDictionary<Type, MemberInfo>();

        private static readonly ConcurrentDictionary<Type, object> _defaultValues =
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
            var info = GetIdProperty(document);

            if (info == null)
                throw new InvalidOperationException($"Type {typeof(T)} does not have Identity property {Identity}");
            return info.GetValue(document);
        }

        internal static void ValidateDocumentId<T>(T document)
        {
            var info = GetIdProperty(document);

            if (info == null)
                throw new InvalidOperationException($"Type {typeof(T)} does not have Identity property {Identity}");

            if (!AllowedTypes.Contains(info.PropertyType))
            {
                throw new InvalidOperationException(
                    $"Identity Property {Identity} for Type {typeof(T)} must be one of these Types: {AllowedTypes.Select(x => x.FullName).Aggregate((i, j) => i + "," + j)}");
            }

            if (!_defaultValues.TryGetValue(info.PropertyType, out object defaultValue))
            {
                defaultValue = info.PropertyType.IsValueType ? Activator.CreateInstance(info.PropertyType) : null;
                _defaultValues.GetOrAdd(info.PropertyType, defaultValue);
            }
            if (defaultValue != null && defaultValue.Equals(info.GetValue(document)))
                throw new InvalidOperationException($"Identity property {Identity} must be of non-default value");
        }

        private static PropertyInfo GetIdProperty(object document)
        {
            MemberInfo info;
            var type = document.GetType();
            
            try
            {
                if (!_propertyCache.TryGetValue(type, out info))
                {
                    info = type.GetProperties().Single(p => p.Name == Identity);
                    _propertyCache.GetOrAdd(type, info);
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