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
                    async res => throw new RestClientException(res.StatusCode, await res.Content.ReadAsStringAsync()));
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

        public Task Post(string path, Action<HttpRequestMessage> configureRequest = null)
        {
            return Request(
                configureRequest
                    .Compose(r => r
                        .Method(HttpMethod.Post)
                        .Path(path)),
                res => Task.FromResult(true));
        }

        public Task Put(string path, Action<HttpRequestMessage> configureRequest = null)
        {
            return Request(
                configureRequest
                    .Compose(r => r
                        .Method(HttpMethod.Put)
                        .Path(path)),
                res => Task.FromResult(true));
        }

        public Task Delete(string path, Action<HttpRequestMessage> configureRequest = null)
        {
            return Request(
                configureRequest
                    .Compose(r => r
                        .Method(HttpMethod.Delete)
                        .Path(path)),
                res => Task.FromResult(true));
        }
    }
}