using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Necessity.Serialization.Abstractions;

namespace Necessity.Rest
{
    public static class RestClientHelpers
    {
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
                r.RequestUri = new Uri(path, UriKind.Relative);
            });
        }

        public static HttpRequestMessage QueryParams(this HttpRequestMessage req, object queryParams)
        {
            return req.Pipe(r =>
            {
                r.RequestUri = r.RequestUri.AppendQueryParameters(queryParams);
            });
        }

        public static HttpRequestMessage Body(this HttpRequestMessage req, object content, bool serializeContent = true, string contentType = "application/json")
        {
            return req.Pipe(r =>
            {
                if (!serializeContent)
                {
                    r.Content = new StringContent(
                        content.ToString(),
                        Encoding.UTF8,
                        contentType);

                    return;
                }

                var serializer = r.Properties[RestClient.SerializerReferenceKey] as ISerializer;

                if (serializer.CanSerializeToStream)
                {
                    r.Header("Content-Type", contentType);
                    r.Content = new StreamContent(
                        new MemoryStream()
                            .Pipe(s => serializer.SerializeToStream(content, s)));

                    return;
                }

                r.Content = new StringContent(
                    serializer.Serialize(content),
                    Encoding.UTF8,
                    contentType);
            });
        }

        public static HttpRequestMessage Header(this HttpRequestMessage req, string key, string value)
        {
            return req.Pipe(r =>
            {
                r.Headers.Add(key, value);
            });
        }

        public static HttpClient BaseAddress(this HttpClient client, string baseAddress)
        {
            return client.Pipe(c =>
            {
                c.BaseAddress = new Uri(baseAddress);
            });
        }

        public static HttpClient BasicAuth(this HttpClient client, string username, string password)
        {
            return client.Pipe(c =>
            {
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        Encoding.GetEncoding("ASCII")
                            .GetBytes($"{username}:{password}")));
            });
        }
    }
}