using System;
using System.Net.Http;

namespace Necessity.Rest
{
    public static class RestClientHelpers
    {
        public static HttpRequestMessage BaseUri(this HttpRequestMessage req, string basePath)
        {
            return req.Pipe(r =>
            {
                r.RequestUri = new Uri(basePath);
            });
        }

        public static HttpRequestMessage Method(this HttpRequestMessage req, HttpMethod httpMethod)
        {
            return req.Pipe(r =>
            {
                r.Method = httpMethod;
            });
        }

        public static HttpRequestMessage Path(this HttpRequestMessage req, string path)
        {
            return req.Pipe(r =>
            {
                r.RequestUri = r.RequestUri.Append(path);
            });
        }

        public static HttpRequestMessage QueryParams(this HttpRequestMessage req, object queryParams)
        {
            return req.Pipe(r =>
            {
                r.RequestUri = r.RequestUri.AppendQueryParameters(queryParams);
            });
        }

        public static HttpRequestMessage Content(this HttpRequestMessage req, object content)
        {
            return req.Pipe(r =>
            {
                r.Properties.Add("X-Body-Content", content);
            });
        }
    }
}