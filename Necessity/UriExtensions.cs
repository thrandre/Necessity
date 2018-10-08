using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Necessity
{
    public class UriParseResult
    {
        public Uri Base { get; set; }
        public string Path { get; set; }
        public Dictionary<string, string> UriParams { get; set; }
        public Dictionary<string, string> QueryParams { get; set; }
    }

    public static class UriExtensions
    {
        public static Uri AppendPath(this Uri baseUri, string appendablePath)
        {
            return baseUri.AbsoluteUri
                .Split('?')
                .Pipe(x =>
                {
                    var path = x[0];
                    var qs = x.Length > 1 ? x[1] : string.Empty;

                    return path.TrimEnd('/') +
                           "/" +
                           appendablePath.TrimStart('/') + (
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
                .ToDictionary(x => x.Key, x => x.Value?.ToString());

            return InternalAppendQueryStringParameters(uri, props);
        }

        private static Uri InternalAppendQueryStringParameters(this Uri uri, Dictionary<string, string> queryStringParameters)
        {
            var res = uri.Parse();

            foreach (var pkvp in queryStringParameters)
            {
                res.QueryParams.AddOrUpdate(pkvp.Key, (_, __) => pkvp.Value);
            }

            return CreateUri(res.Base.AppendPath(res.Path).ToString(), res.QueryParams);
        }

        public static string EncodeUrlFragment(string fragment)
        {
            return WebUtility.UrlEncode(fragment)?.Replace("+", "%20");
        }

        public static string DecodeUrlFragment(string fragment)
        {
            return WebUtility.UrlDecode(fragment);
        }

        public static Uri CreateUri(string path, IDictionary<string, string> queryStringParameters)
        {
            return (path + "?" +
                   string.Join(
                       "&",
                       queryStringParameters
                           .Where(x => !string.IsNullOrEmpty(x.Value))
                           .Select(x => $"{x.Key}={x.Value}")))
                       .Pipe(x => new Uri(x));
        }

        private static Dictionary<string, string> ExtractQueryStringParams(string pathAndQuery)
        {
            var buffer = new StringBuilder();
            var lookbackBuffer = new StringBuilder();
            var qsSepSeen = false;

            var qs = new Dictionary<string, string>();

            for (var i = 0; i <= pathAndQuery.Length; i++)
            {
                var c = i < pathAndQuery.Length
                    ? pathAndQuery[i]
                    : char.MinValue;

                if ((c == '?' && !qsSepSeen) || (!qsSepSeen && i == pathAndQuery.Length))
                {
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

                if (c == '&' || (c == '?' && qsSepSeen) || i == pathAndQuery.Length)
                {
                    var key = lookbackBuffer.ToString();
                    var it = 0;

                    while (qs.ContainsKey(key))
                    {
                        it++;
                        key = $"{key}_{it}";
                    }

                    qs.Add(key, buffer.ToString());

                    buffer.Clear();
                    lookbackBuffer.Clear();

                    continue;
                }

                buffer.Append(c);
            }

            return qs;
        }

        private static Dictionary<string, string> ExtractUriParams(string path, string uriPattern)
        {
            path = path.StartsWith("/")
                ? path
                : "/" + path;

            var paramsAndIndices = uriPattern
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select((fragment, fragmentIndex) => (Idx: fragmentIndex, Fragment: fragment))
                .Where(fragmentPair => fragmentPair.Fragment.StartsWith("{") && fragmentPair.Fragment.EndsWith("}"))
                .ToDictionary(fragmentPair => fragmentPair.Idx, fragmentPair => fragmentPair.Fragment.TrimStart('{').TrimEnd('}'));

            var extractedParams = path
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select((fragment, fragmentIndex) => paramsAndIndices.ContainsKey(fragmentIndex)
                    ? (Name: paramsAndIndices[fragmentIndex], Value: fragment)
                    : default)
                .Where(fragmentPair => !string.IsNullOrEmpty(fragmentPair.Name))
                .ToDictionary(fragmentPair => fragmentPair.Name, fragmentPair => fragmentPair.Value);

            return paramsAndIndices
                .Select(paramPair => (
                    Name: paramPair.Value,
                    Value: extractedParams.GetOrDefault(paramPair.Value)))
                .ToDictionary(pair => pair.Name, pair => pair.Value);
        }

        private static (Uri Base, string Path, string PathAndQuery) GetBaseAndPath(this Uri target)
        {
            var absUri = target.AbsoluteUri;
            var pathAndQuery = target.PathAndQuery;
            var pathIndex = absUri.IndexOf(pathAndQuery);
            var queryIndex = absUri.IndexOf('?');

            var baseAndPath = absUri.Substring(0, queryIndex > -1 ? queryIndex : absUri.Length);
            var @base = new Uri(
                baseAndPath.Substring(
                    0,
                    pathIndex > -1
                        ? pathIndex
                        : baseAndPath.Length));

            var path = absUri.Substring(
                pathIndex,
                (queryIndex - pathIndex)
                    .Pipe(x => x > 0 ? x : pathAndQuery.Length));

            return (@base, path, pathAndQuery);
        }

        public static UriParseResult Parse(this Uri target, string uriPattern = null)
        {
            var (@base, path, pathAndQuery) = GetBaseAndPath(target);

            return new UriParseResult
            {
                Base = @base,
                Path = path,
                QueryParams = ExtractQueryStringParams(pathAndQuery),
                UriParams = uriPattern != null
                    ? ExtractUriParams(pathAndQuery.Split('?').First(), uriPattern)
                    : null
            };
        }
    }
}
