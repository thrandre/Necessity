using System;

namespace Necessity
{
    public static class UriExtensions
    {
        public static Uri Append(this Uri baseUri, string appendable)
        {
            return new Uri($"{baseUri.AbsoluteUri.TrimEnd('/')}/{appendable.TrimStart('/')}");
        }

        public static Uri Append<T>(this Uri baseUri, Func<T, string> formatFn, T parameters)
        {
            return Append(baseUri, formatFn(parameters));
        }
    }
}
