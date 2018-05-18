using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Necessity
{
    public static class UriExtensions
    {
        public static Uri AppendPath(this Uri baseUri, string appendable)
        {
            return new Uri(
                baseUri.AbsoluteUri.TrimEnd('/')
                    + "/"
                    + appendable.TrimStart('/'));
        }

        public static Uri AppendPath<T>(this Uri baseUri, Func<T, string> formatFn, T parameters)
        {
            return AppendPath(baseUri, formatFn(parameters));
        }

        public static Uri AppendQueryStringParameters(this Uri uri, Dictionary<string, string> queryStringParameters)
        {
            var (path, qsp) = uri.Parse();

            foreach (var pkvp in queryStringParameters)
            {
                qsp.AddOrUpdate(pkvp.Key, _ => pkvp.Value);
            }

            return FormatUri(path, qsp);
        }

        public static Uri FormatUri(Uri path, IDictionary<string, string> queryStringParameters)
        {
            return $"{path.AbsoluteUri}?{string.Join("&", queryStringParameters.Select(x => $"{WebUtility.UrlEncode(x.Key)}={WebUtility.UrlEncode(x.Value)}"))}"
                .Pipe(x => new Uri(x));
        }

        public static (Uri Path, Dictionary<string, string> QueryParams) Parse(this Uri target)
        {
            var absUri = target.AbsoluteUri;

            var buffer = new StringBuilder();
            var lookbackBuffer = new StringBuilder();

            var path = string.Empty;
            var qs = new List<(string Key, string Value)>();

            var qsSepSeen = false;

            for (var i = 0; i <= absUri.Length; i++)
            {
                var c = i < absUri.Length
                    ? absUri[i]
                    : char.MinValue;

                if (c == '?' || !qsSepSeen && i == absUri.Length)
                {
                    path = buffer.ToString();

                    buffer.Clear();
                    qsSepSeen = true;

                    continue;
                }

                if (!qsSepSeen)
                {
                    buffer.Append(c);
                    continue;
                }

                if (c == '=')
                {
                    lookbackBuffer.Append(buffer);

                    buffer.Clear();

                    continue;
                }

                if (c == '&' || i == absUri.Length)
                {
                    qs.Add((WebUtility.UrlDecode(lookbackBuffer.ToString()), WebUtility.UrlDecode(buffer.ToString())));

                    buffer.Clear();
                    lookbackBuffer.Clear();

                    continue;
                }

                buffer.Append(c);
            }

            return (new Uri(path), qs.ToDictionary(x => x.Key, x => x.Value));
        }
    }
}
