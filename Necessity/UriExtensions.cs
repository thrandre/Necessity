using System;

namespace Necessity
{
    public static class UriExtensions
    {
        public static Uri Append<T>(this Uri baseUri, Func<T, string> formatFn, T parameters)
        {
            return new Uri($"{baseUri.AbsoluteUri.TrimEnd('/')}/{formatFn(parameters).TrimStart('/')}");
        }
    }
}
