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
    }
}
