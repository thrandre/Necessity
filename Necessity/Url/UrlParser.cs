using System;
using System.Collections.Generic;

namespace Necessity.Url
{
    public class UrlParseResult
    {
        public string Scheme { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string Fragment { get; set; }

        public bool IsAbsolute => !string.IsNullOrWhiteSpace(Scheme)
            && !string.IsNullOrWhiteSpace(Domain);
    }

    public static class UrlParser
    {
        private static readonly Dictionary<int, Action<UrlParseResult, string>> Setters = new Dictionary<int, Action<UrlParseResult, string>>
        {
            { 0, (r, s) => r.Scheme = s.Trim(':') },
            { 1, (r, s) => r.Domain = s },
            { 2, (r, s) => r.Path = s},
            { 3, (r, s) => r.QueryString = s},
            { 4, (r, s) => r.Fragment = s}
        };

        private static readonly string[] Delimiters = { "//", "/", "?", "#" };

        public static UrlParseResult Parse(string url)
        {
            var start = url.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? 0
            : 2;

            var current = url;
            var offset = 0;

            var result = new UrlParseResult();

            for (var i = start; i <= Delimiters.Length; i++)
            {
                if (i == Delimiters.Length)
                {
                    Setters[(i - offset)](result, current);
                    continue;
                }

                var split = current.Split(
                    new[] { Delimiters[i] },
                    2,
                    StringSplitOptions.RemoveEmptyEntries);

                if (split.Length > 1)
                {
                    Setters[(i - offset)](result, split[0]);

                    offset = 0;
                    current = split[1];

                    continue;
                }

                offset++;
            }

            return result;
        }
    }
}
