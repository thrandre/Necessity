using System.Collections.Generic;
using System.Linq;

namespace Necessity
{
    public static class EnumerableExtensions
    {
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
