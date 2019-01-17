using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Necessity.Http;
using Necessity.Serialization.Abstractions;

namespace Necessity.Rest
{
    public class SimpleRestClientFactory
    {
        public SimpleRestClientFactory(Func<HttpClient> getHttpClient, ISerializer serializer)
        {
            GetHttpClient = getHttpClient;
            Serializer = serializer;
        }

        public Func<HttpClient> GetHttpClient { get; }
        public ISerializer Serializer { get; }

        public SimpleRestClient Create(Action<HttpRequestMessage> commonConfigure = null)
        {
            return new SimpleRestClient(GetHttpClient, Serializer) { CommonConfigure = commonConfigure };
        }
    }

    public class SimpleRestClientException : Exception
    {
        public SimpleRestClientException(HttpStatusCode statusCode, string errorMessage) : base($"{statusCode}: {errorMessage}")
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }

        public HttpStatusCode StatusCode { get; }
        public string ErrorMessage { get; }
    }

    public static class SimpleRestClientHelpers
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

    public class SimpleRestClient
    {
        private const string BodyContentKey = "X-Body-Content";

        public SimpleRestClient(Func<HttpClient> getHttpClient, ISerializer serializer)
        {
            GetHttpClient = getHttpClient;
            Serializer = serializer;
        }

        private Func<HttpClient> GetHttpClient { get; }
        private ISerializer Serializer { get; }

        public Action<HttpRequestMessage> CommonConfigure { get; set; }

        public Task<T> Request<T>(
            Action<HttpRequestMessage> configureRequest,
            Func<HttpResponseMessage, Task<T>> onSuccess)
        {
            return GetHttpClient()
                .RequestAsync(
                    configureRequest
                        .Compose(CommonConfigure)
                        .Compose(r =>
                        {
                            var bodyContent = r.Properties.GetOrDefault(BodyContentKey);

                            if (bodyContent == null)
                            {
                                return;
                            }

                            r.Content = new StringContent(
                                Serializer.Serialize<T>(bodyContent),
                                Encoding.UTF8,
                                "application/json");

                            r.Properties.Remove(BodyContentKey);
                        }),
                    async res => await onSuccess(res),
                    async res => throw new SimpleRestClientException(res.StatusCode, await res.Content.ReadAsStringAsync()));
        }

        public Task<T> Get<T>(string path, Action<HttpRequestMessage> configureRequest = null)
        {
            return Request(
                configureRequest
                    .Compose(r => r
                        .Method(HttpMethod.Get)
                        .Path(path)),
                async res => Serializer.Deserialize<T>(await res.Content.ReadAsStringAsync()));
        }

        public Task Post<T>(string path, Action<HttpRequestMessage> configureRequest = null)
        {
            return Request(
                configureRequest
                    .Compose(r => r
                        .Method(HttpMethod.Post)
                        .Path(path)),
                res => Task.FromResult(true));
        }

        public Task Put<T>(string path, Action<HttpRequestMessage> configureRequest = null)
        {
            return Request(
                configureRequest
                    .Compose(r => r
                        .Method(HttpMethod.Put)
                        .Path(path)),
                res => Task.FromResult(true));
        }
    }
}