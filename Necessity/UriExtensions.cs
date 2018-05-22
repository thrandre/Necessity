using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Necessity
{
    public static class UriExtensions
    {
        public static Uri AppendPath(this Uri baseUri, string appendable)
        {
            return baseUri.AbsoluteUri
                .Split('?')
                .Pipe(x =>
                {
                    var path = x[0];
                    var qs = x.Length > 1 ? x[1] : string.Empty;

                    return path.TrimEnd('/') +
                           "/" +
                           appendable.TrimStart('/') + (
                               !string.IsNullOrEmpty(qs)
                                   ? "?" + qs
                                   : string.Empty);
                })
                .Pipe(x => new Uri(x));
        }

        public static Uri AppendQueryStringParameters<T>(this Uri uri, T obj)
        {
            var props = typeof(T)
                .GetRuntimeProperties()
                .Select(x => (Key: x.Name, Value: x.GetValue(obj)))
                .ToDictionary(x => x.Key, x => x.Value.ToString());

            return InternalAppendQueryStringParameters(uri, props);
        }

        private static Uri InternalAppendQueryStringParameters(this Uri uri, Dictionary<string, string> queryStringParameters)
        {
            var (path, qsp) = uri.Parse();

            foreach (var pkvp in queryStringParameters)
            {
                qsp.AddOrUpdate(pkvp.Key, _ => pkvp.Value);
            }

            return CreateUri(path, qsp);
        }

        private static string EncodeUrlFragment(string fragment)
        {
            return WebUtility.UrlEncode(fragment)?.Replace("+", "%20");
        }

        private static string DecodeUrlFragment(string fragment)
        {
            return WebUtility.UrlDecode(fragment);
        }

        public static Uri CreateUri(Uri path, IDictionary<string, string> queryStringParameters)
        {
            return (path.AbsoluteUri + "?" +
                   string.Join(
                       "&",
                       queryStringParameters
                           .Where(x => !string.IsNullOrEmpty(x.Value))
                           .Select(x => $"{EncodeUrlFragment(x.Key)}={EncodeUrlFragment(x.Value)}")))
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
                    qs.Add((DecodeUrlFragment(lookbackBuffer.ToString()), DecodeUrlFragment(buffer.ToString())));

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
