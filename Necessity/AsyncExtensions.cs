using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Necessity
{
    public static class AsyncExtensions
    {
        public static Task<List<T>> ToListAsync<T>(this Task<IEnumerable<T>> target)
        {
            return target.ContinueWith(x => x.Result.ToList(), TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}
