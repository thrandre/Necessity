using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Necessity.Url
{
    public static class UrlInfoExtensions
    {
        public static UrlInfo Append(this UrlInfo target, string appendable)
        {
            return target.Append(UrlInfo.Parse(appendable));
        }

        public static UrlInfo Append(this UrlInfo target, UrlInfo appendable)
        {
            return new UrlInfo
            {
                BaseAddress = target.BaseAddress ?? appendable.BaseAddress,
                Path = UrlUtils.JoinPaths(target.Path, appendable.Path),
                QueryParameters = target.QueryParameters
                    .Union(appendable.QueryParameters)
                    .ToDictionary(x => x.Key, x => x.Value),
                Fragment = appendable.Fragment ?? target.Fragment
            };
        }

        public static UrlInfo AppendQueryParameters(this UrlInfo target, Dictionary<string, string> queryParams)
        {
            return target.Append(new UrlInfo { QueryParameters = queryParams });
        }

        public static UrlInfo AppendQueryParameters<T>(this UrlInfo target, T queryParams)
        {
            return target.AppendQueryParameters(
                typeof(T)
                    .GetRuntimeProperties()
                    .ToDictionary(x => x.Name, x => x.GetValue(queryParams).ToString()));
        }

        public static Uri ToUri(this UrlInfo urlInfo)
        {
            return new Uri(urlInfo.ToString(), urlInfo.IsAbsolute ? UriKind.Absolute : UriKind.Relative);
        }
    }
}
