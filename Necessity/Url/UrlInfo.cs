using System.Collections.Generic;
using System.Linq;

namespace Necessity.Url
{
    public class UrlInfo
    {
        public string BaseAddress { get; set; }
        public string Path { get; set; }
        public Dictionary<string, string> QueryParameters { get; set; } = new Dictionary<string, string>();
        public string Fragment { get; set; }

        public bool IsAbsolute => !string.IsNullOrWhiteSpace(BaseAddress);

        public static UrlInfo Parse(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            var parseResult = UrlParser.Parse(url);

            return new UrlInfo
            {
                BaseAddress = parseResult.IsAbsolute
                    ? parseResult.Scheme + "://" + parseResult.Domain
                    : null,

                Path = parseResult.Path,

                QueryParameters = parseResult
                    .QueryString
                        ?.Pipe(qs => qs
                            .Split('&')
                            .Select(qp => qp
                                .Split('=')))
                        .ToDictionary(qp => qp[0], qp => qp.Length > 1 ? qp[1] : null)
                    ?? new Dictionary<string, string>(),

                Fragment = parseResult.Fragment
            };
        }

        public override string ToString()
        {
            return UrlUtils.JoinPaths(BaseAddress ?? string.Empty, Path)
                + UrlUtils.FormatQueryString(QueryParameters)
                + UrlUtils.FormatFragment(Fragment);
        }
    }
}
