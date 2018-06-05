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
    }
}
