using System.Collections.Generic;
using System.Linq;

namespace Necessity.Url
{
    public class UrlUtils
    {
        public static string JoinPaths(string existingPath, string appendablePath)
        {
            return (existingPath?.TrimEnd('/') + "/" + appendablePath?.TrimStart('/')).Trim('/');
        }

        public static string FormatQueryString(Dictionary<string, string> queryParameters)
        {
            return queryParameters.Any()
                ? "?" + string.Join("&", queryParameters.Select(p => $"{p.Key}={p.Value}"))
                : string.Empty;
        }

        public static string FormatFragment(string fragment)
        {
            return !string.IsNullOrWhiteSpace(fragment)
                ? "#" + fragment
                : string.Empty;
        }
    }
}
