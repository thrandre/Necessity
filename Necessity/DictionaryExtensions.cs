using System.Collections.Generic;

namespace Necessity
{
    public static class DictionaryExtensions
    {
        public static TVal GetOrDefault<TKey, TVal>(this IDictionary<TKey, TVal> target, TKey key)
        {
            return target.ContainsKey(key)
                ? target[key]
                : default(TVal);
        }
    }
}
