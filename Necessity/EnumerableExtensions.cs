using System;
using System.Collections.Generic;
using System.Linq;

namespace Necessity
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> target, Action<T> act)
        {
            foreach (var item in target)
            {
                act(item);
            }
        }

        public static IEnumerable<Tuple<T, int>> GetIndices<T>(this IEnumerable<T> target)
        {
            return target
                .Select((x, i) => Tuple.Create(x, i));
        }

        public static IEnumerable<int> FindAllIndices<T>(this IEnumerable<T> target, Func<T, bool> predicate)
        {
            return target
                .Select((x, i) => Tuple.Create(x, i))
                .Where(x => predicate(x.Item1))
                .Select(x => x.Item2);
        }

        public static ICollection<T> Materialize<T>(this IEnumerable<T> target)
        {
            return target as ICollection<T> ?? target.ToList();
        }

        public static IEnumerable<List<T>> Partition<T>(this IEnumerable<T> source, int partitionSize)
        {
            var toReturn = new List<T>(partitionSize);
            foreach (var item in source)
            {
                toReturn.Add(item);

                if (toReturn.Count != partitionSize)
                {
                    continue;
                }

                yield return toReturn;
                toReturn = new List<T>(partitionSize);
            }
            if (toReturn.Any())
            {
                yield return toReturn;
            }
        }
    }
}
