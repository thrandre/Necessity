using System.Collections.Generic;
using System.Threading.Tasks;

namespace Necessity
{
    public static class AsyncExtensions
    {
        public static Task<ICollection<T>> MaterializeAsync<T>(this Task<IEnumerable<T>> target)
        {
            return target.ContinueWith(x => x.Result.Materialize(), TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}
