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
            var (path, qsp) = uri.Parse();

            foreach (var pkvp in queryStringParameters)
            {
                qsp.AddOrUpdate(pkvp.Key, (_, __) => pkvp.Value);
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

                if (c == '?' || !qsSepSeen && i == pathAndQuery.Length)
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

                if (c == '&' || i == pathAndQuery.Length)
                {
                    qs.Add(DecodeUrlFragment(lookbackBuffer.ToString()), DecodeUrlFragment(buffer.ToString()));

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
                .ToDictionary(pair => DecodeUrlFragment(pair.Name), pair => DecodeUrlFragment(pair.Value));
        }

        public static (Uri HostFragment, Dictionary<string, string> UriParams, Dictionary<string, string> QueryParams) Parse(this Uri target, string uriPattern)
        {
            var absUri = target.AbsoluteUri;
            var pathAndQuery = target.PathAndQuery;
            var queryIndex = pathAndQuery.IndexOf('?');

            var absPath = absUri.Substring(0, queryIndex > -1 ? queryIndex : absUri.Length);

            return (new Uri(absPath), ExtractUriParams(pathAndQuery.Split('?').First(), uriPattern), ExtractQueryStringParams(pathAndQuery));
        }

        public static (Uri Path, Dictionary<string, string> QueryParams) Parse(this Uri target)
        {
            var absUri = target.AbsoluteUri;
            var pathAndQuery = target.PathAndQuery;
            var queryIndex = pathAndQuery.IndexOf('?');

            var absPath = absUri.Substring(0, queryIndex > -1 ? queryIndex : absUri.Length);

            return (new Uri(absPath), ExtractQueryStringParams(pathAndQuery));
        }
    }
}
