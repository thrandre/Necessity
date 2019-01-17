using System;
using System.Collections.Generic;
using System.Linq;

namespace Necessity
{
    public static class DictionaryExtensions
    {
        public static IDictionary<TVal, TKey> Invert<TKey, TVal>(this IDictionary<TKey, TVal> source)
        {
            return source.ToDictionary(x => x.Value, x => x.Key);
        }

        public static TVal GetOrDefault<TKey, TVal>(this IDictionary<TKey, TVal> target, TKey key)
        {
            return target.ContainsKey(key)
                ? target[key]
                : default(TVal);
        }

        public static TVal GetOrAdd<TKey, TVal>(this IDictionary<TKey, TVal> target, TKey key, Func<TKey, TVal> valueFactoryFn)
        {
            if (target.ContainsKey(key))
            {
                return target[key];
            }

            var createdValue = valueFactoryFn(key);

            target.Add(key, createdValue);

            return createdValue;
        }

        public static void AddOrUpdate<TKey, TVal>(this IDictionary<TKey, TVal> target, TKey key, Func<TKey, TVal, TVal> updateFn)
        {
            if (target.ContainsKey(key))
            {
                target[key] = updateFn(key, target[key]);
                return;
            }

            target.Add(key, updateFn(key, default(TVal)));
        }

        public static Dictionary<TKey, TVal> ToNonCollidingDictionary<T, TKey, TVal>(this IEnumerable<T> target, Func<T, TKey> keySelector, Func<T, TVal> valueSelector, Func<TVal, TVal, TVal> collisionResolver)
        {
            var dict = new Dictionary<TKey, TVal>();

            foreach (var item in target)
            {
                dict.AddOrUpdate(
                    keySelector(item),
                    (key, existingItem) =>
                        existingItem == null
                            ? valueSelector(item)
                            : collisionResolver(existingItem, valueSelector(item)));
            }

            return dict;
        }

        public static Dictionary<TKey, TVal> ToNonCollidingDictionary<T, TKey, TVal>(this IEnumerable<T> target, Func<T, TKey> keySelector, Func<T, TVal> valueSelector, ResolveOption resolveOption)
        {
            return target.ToNonCollidingDictionary(
                keySelector,
                valueSelector,
                (e, n) =>
                    resolveOption == ResolveOption.KeepFirst
                        ? e
                        : n);
        }
    }

    public enum ResolveOption
    {
        KeepFirst,
        KeepLast
    }
}
