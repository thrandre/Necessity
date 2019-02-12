using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Necessity.Http;
using Necessity.Serialization.Abstractions;

namespace Necessity.Rest
{
    public class RestClient : IRestClient
    {
        private const string BodyContentKey = "X-Body-Content";

        public RestClient(Func<HttpClient> getHttpClient, ISerializer serializer)
        {
            GetHttpClient = getHttpClient;
            Serializer = serializer;
        }

        private Func<HttpClient> GetHttpClient { get; }
        private ISerializer Serializer { get; }

        public Action<HttpRequestMessage> CommonConfigure { get; set; }

        public Task<T> Request<T>(
            string path,
            Action<HttpRequestMessage> configureRequest,
            Func<HttpResponseMessage, ISerializer, Task<T>> onSuccess)
        {
            return GetHttpClient()
                .RequestAsync(
                    configureRequest
                        .Compose(CommonConfigure)
                        .Compose(r => r.Path(path))
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
                    async res => await onSuccess(res, Serializer),
                    async res => throw new RestClientException(res.StatusCode, await res.Content.ReadAsStringAsync()));
        }

        public Task<T> Get<T>(string path, Action<HttpRequestMessage> configureRequest = null, T anonymousObjectPrototype = default)
        {
            return Request(
                path,
                configureRequest
                    .Compose(r => r
                        .Method(HttpMethod.Get)),
                (res, serializer) => res.Content
                    .ReadAsStringAsync()
                    .Pipe(x => anonymousObjectPrototype == default
                        ? Serializer.Deserialize<T>(x)
                        : Serializer.DeserializeAnonymousObject<T>(x, anonymousObjectPrototype)));
        }

        public Task Post(string path, Action<HttpRequestMessage> configureRequest = null)
        {
            return Request(
                path,
                configureRequest
                    .Compose(r => r
                        .Method(HttpMethod.Post)),
                (res, _) => Task.FromResult(true));
        }

        public Task Put(string path, Action<HttpRequestMessage> configureRequest = null)
        {
            return Request(
                path,
                configureRequest
                    .Compose(r => r
                        .Method(HttpMethod.Put)),
                (res, _) => Task.FromResult(true));
        }

        public Task Delete(string path, Action<HttpRequestMessage> configureRequest = null)
        {
            return Request(
                path,
                configureRequest
                    .Compose(r => r
                        .Method(HttpMethod.Delete)),
                (res, _) => Task.FromResult(true));
        }
    }
}