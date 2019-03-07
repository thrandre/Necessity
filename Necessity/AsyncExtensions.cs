using System.Collections.Generic;
using System.Threading.Tasks;

namespace Necessity
{
    public static class AsyncExtensions
    {
        public static async Task<ICollection<T>> MaterializeAsync<T>(this Task<IEnumerable<T>> target)
        {
            return (await target).Materialize();
        }
    }
}
