using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Necessity;

public class UriInfo
{
    public string BasePath { get; set; }
    public string RelativePath { get; set; }
    public IDictionary<string, string> UriParams { get; set; } = new Dictionary<string, string>();
    public IDictionary<string, string> QueryParams { get; set; } = new Dictionary<string, string>();
    public string Fragment { get; set; }
    public bool IsAbsolute { get; set; }

    public override string ToString()
    {
        return UriUtils.JoinPaths(BasePath ?? string.Empty, RelativePath)
            + (QueryParams.Any()
                    ? "?" + string.Join("&", QueryParams.Select(p => $"{p.Key}={p.Value}"))
                    : string.Empty)
            + (!string.IsNullOrWhiteSpace(Fragment)
                    ? "#" + Fragment
                    : string.Empty);
    }
}

public static class UriInfoExtensions
{
    public static UriInfo Merge(this UriInfo i1, UriInfo i2)
    {
        return new UriInfo
        {
            BasePath = i1.BasePath,
            RelativePath = !string.IsNullOrWhiteSpace(i2.RelativePath.TrimStart('/'))
                ? UriUtils.JoinPaths(i1.RelativePath, i2.RelativePath)
                : i1.RelativePath,
            QueryParams = i1.QueryParams.Concat(i2.QueryParams).ToNonCollidingDictionary(x => x.Key, x => x.Value, ResolveOption.KeepFirst),
            UriParams = i1.UriParams.Concat(i2.UriParams).ToNonCollidingDictionary(x => x.Key, x => x.Value, ResolveOption.KeepFirst),
            Fragment = i2.Fragment ?? i1.Fragment,
            IsAbsolute = i1.IsAbsolute
        };
    }
}

public class UriParts
{
    public string Scheme { get; set; }
    public string Domain { get; set; }
    public string Path { get; set; }
    public string Query { get; set; }
    public string Fragment { get; set; }

    public bool IsAbsolute => !string.IsNullOrWhiteSpace(Scheme)
        && !string.IsNullOrWhiteSpace(Domain);

    public string RelativePath => "/" + Path.TrimStart('/');

    public string RelativePathAndQuery => RelativePath +
        (!string.IsNullOrWhiteSpace(Query)
            ? "?" + Query
            : string.Empty) +
        (!string.IsNullOrWhiteSpace(Fragment)
            ? "#" + Fragment
            : string.Empty);

    public string BasePath =>
        IsAbsolute
            ? $"{Scheme}://{Domain}"
            : "/";

    public string AbsolutePath => UriUtils.JoinPaths(BasePath, RelativePathAndQuery);

    private static Regex UriRegex = new Regex(@"^(?:([^:\/?#]+):)?(?:\/\/([^\/?#]*))?([^?#]*)(?:\?([^#]*))?(?:#(.*))?");

    private static Dictionary<int, Action<UriParts, string>> MatchMap =
        new Dictionary<int, Action<UriParts, string>>
            {
                { 1, (p, val) => p.Scheme = val },
                { 2, (p, val) => p.Domain = val },
                { 3, (p, val) => p.Path = val },
                { 4, (p, val) => p.Query = val },
                { 5, (p, val) => p.Fragment = val }
            };

    public static UriParts Parse(string uriString)
    {
        var match = UriRegex.Match(uriString);

        if (!match.Success)
        {
            throw new ArgumentException("Not valid!");
        }

        var uriParts = new UriParts();

        for (var i = 1; i < match.Groups.Count; i++)
        {
            if (!match.Groups[i].Success)
            {
                continue;
            }

            MatchMap[i](uriParts, match.Groups[i].Value);
        }

        return uriParts;
    }
}

public static class UriUtils
{
    public static string JoinPaths(string existingPath, string appendablePath)
    {
        return existingPath.TrimEnd('/') + "/" + appendablePath.TrimStart('/');
    }
}

public static class UriExtensions
{
    public static string EncodeUrlFragment(string fragment)
    {
        return WebUtility.UrlEncode(fragment)?.Replace("+", "%20");
    }

    public static string DecodeUrlFragment(string fragment)
    {
        return WebUtility.UrlDecode(fragment);
    }

    private static Dictionary<string, string> ExtractQueryParams(string query)
    {
        return query
            .TrimStart('?')
            .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Split('='))
            .ToNonCollidingDictionary(kv => kv[0], kv => kv[1], ResolveOption.KeepFirst);
    }

    private static Dictionary<string, string> ExtractUriParams(string path, string uriPattern)
    {
        var paramsAndIndices = uriPattern
            .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
            .GetIndices()
            .Where(tpl => tpl.Item1.StartsWith("{") && tpl.Item1.EndsWith("}"))
            .ToNonCollidingDictionary(tpl => tpl.Item2, tpl => tpl.Item1.TrimStart('{').TrimEnd('}'), (e, n) => n);

        return path
            .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
            .GetIndices()
            .Where(tpl => paramsAndIndices.ContainsKey(tpl.Item2))
            .Select(tpl =>
                new
                {
                    Key = paramsAndIndices.GetOrDefault(tpl.Item2),
                    Value = tpl.Item1
                })
            .ToNonCollidingDictionary(tpl => tpl.Key, tpl => tpl.Value, (e, n) => n);
    }

    public static Uri Append(this Uri uri, string appendable)
    {
        var uriInfo = uri.Parse();
        var appendableUriInfo = Parse(appendable);

        var result = uriInfo.Merge(appendableUriInfo);

        return new Uri(result.ToString());
    }

    public static Uri AppendPath(this Uri uri, string appendable)
    {
        return Append(uri, appendable);
    }

    public static Uri AppendQueryParameters(this Uri uri, object parameterObject)
    {
        return parameterObject
            .GetType()
            .GetRuntimeProperties()
            .Select(p => new { Name = p.Name, Value = p.GetValue(parameterObject).ToString() })
            .ToDictionary(x => x.Name, x => x.Value)
            .Pipe(p => uri.AppendQueryParameters(p));
    }

    public static Uri AppendQueryParameters(this Uri uri, IDictionary<string, string> queryParameters)
    {
        return Parse(uri.AbsoluteUri)
            .Pipe(x =>
            {
                x.QueryParams = x
                    .QueryParams
                    .Concat(queryParameters)
                    .ToNonCollidingDictionary(
                        y => y.Key,
                        y => y.Value,
                        ResolveOption.KeepFirst);

                return new Uri(x.ToString());
            });
    }

    public static UriInfo Parse(string uri, string pattern = null)
    {
        return UriParts
            .Parse(uri)
            .Pipe(p =>
                new UriInfo
                {
                    IsAbsolute = p.IsAbsolute,
                    BasePath = p.BasePath,
                    RelativePath = p.Path,
                    QueryParams = ExtractQueryParams(p.Query ?? string.Empty),
                    UriParams = pattern?.Pipe(ptrn => ExtractUriParams(p.Path, ptrn))
                        ?? new Dictionary<string, string>(),
                    Fragment = p.Fragment
                });
    }

    public static UriInfo Parse(this Uri uri, string pattern = null)
    {
        return Parse(uri.AbsoluteUri, pattern);
    }
}