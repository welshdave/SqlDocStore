namespace SqlDocStore.Vendor.EnsureThat
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Extensions;

    public static class EnsureCollectionExtensions
    {
        public static void HasItems<T>(this Param<T> param) where T : ICollection
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count < 1)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_HasItemsFailed);
        }

        public static void HasItems<T>(this Param<Collection<T>> param)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count < 1)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_HasItemsFailed);
        }

        public static void HasItems<T>(this Param<ICollection<T>> param)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count < 1)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_HasItemsFailed);
        }

        public static void HasItems<T>(this Param<T[]> param)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Length < 1)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_HasItemsFailed);
        }

        public static void HasItems<T>(this Param<List<T>> param)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count < 1)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_HasItemsFailed);
        }

        public static void HasItems<T>(this Param<IList<T>> param)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count < 1)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_HasItemsFailed);
        }

        public static void HasItems<TKey, TValue>(this Param<IDictionary<TKey, TValue>> param)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count < 1)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_HasItemsFailed);
        }

        public static void HasItems<T>(this Param<IReadOnlyCollection<T>> param)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count < 1)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_HasItemsFailed);
        }

        public static void HasItems<T>(this Param<IReadOnlyList<T>> param)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count < 1)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_HasItemsFailed);
        }

        public static void SizeIs<T>(this Param<T[]> param, int expected)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Length != expected)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, param.Value.Length));
        }

        public static void SizeIs<T>(this Param<T[]> param, long expected)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Length != expected)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, param.Value.Length));
        }

        public static void SizeIs<T>(this Param<T> param, int expected) where T : ICollection
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count != expected)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, param.Value.Count));
        }

        public static void SizeIs<T>(this Param<T> param, long expected) where T : ICollection
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count != expected)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, param.Value.Count));
        }

        public static void SizeIs<T>(this Param<ICollection<T>> param, int expected)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count != expected)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, param.Value.Count));
        }

        public static void SizeIs<T>(this Param<ICollection<T>> param, long expected)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count != expected)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, param.Value.Count));
        }

        public static void SizeIs<T>(this Param<IList<T>> param, int expected)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count != expected)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, param.Value.Count));
        }

        public static void SizeIs<T>(this Param<IList<T>> param, long expected)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count != expected)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, param.Value.Count));
        }

        public static void SizeIs<TKey, TValue>(this Param<IDictionary<TKey, TValue>> param, int expected)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count != expected)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, param.Value.Count));
        }

        public static void SizeIs<TKey, TValue>(this Param<IDictionary<TKey, TValue>> param, long expected)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (param.Value.Count != expected)
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_SizeIs_Failed.Inject(expected, param.Value.Count));
        }

        public static void ContainsKey<TKey, TValue>(this Param<IDictionary<TKey, TValue>> param, TKey key)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (!param.Value.ContainsKey(key))
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_ContainsKey_Failed.Inject(key));
        }

        public static void ContainsKey<TKey, TValue>(this Param<Dictionary<TKey, TValue>> param, TKey key)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (!param.Value.ContainsKey(key))
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_ContainsKey_Failed.Inject(key));
        }

        public static void Any<T>(this Param<IList<T>> param, Func<T, bool> predicate)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (!param.Value.Any(predicate))
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_Any_Failed);
        }

        public static void Any<T>(this Param<List<T>> param, Func<T, bool> predicate)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (!param.Value.Any(predicate))
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_Any_Failed);
        }

        public static void Any<T>(this Param<ICollection<T>> param, Func<T, bool> predicate)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (!param.Value.Any(predicate))
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_Any_Failed);
        }

        public static void Any<T>(this Param<Collection<T>> param, Func<T, bool> predicate)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (!param.Value.Any(predicate))
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_Any_Failed);
        }

        public static void Any<TKey, TValue>(this Param<IDictionary<TKey, TValue>> param, Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (!param.Value.Any(predicate))
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_Any_Failed);
        }

        public static void Any<TKey, TValue>(this Param<Dictionary<TKey, TValue>> param, Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (!param.Value.Any(predicate))
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_Any_Failed);
        }

        public static void Any<T>(this Param<T[]> param, Func<T, bool> predicate)
        {
            if (!Ensure.IsActive)
                return;

            param.IsNotNull();

            if (!param.Value.Any(predicate))
                throw ExceptionFactory.CreateForParamValidation(param, ExceptionMessages.Collections_Any_Failed);
        }
    }
}