using System;
using System.Net.Http;
using System.Threading.Tasks;
using Necessity.Http;
using Necessity.Serialization.Abstractions;

namespace Necessity.Rest
{
    public class RestClient : IRestClient
    {
        internal const string SerializerReferenceKey = "X-RestClient-Serializer";

        public RestClient(HttpClient client, ISerializer serializer)
        {
            Client = client;
            Serializer = serializer;
        }

        public Action<HttpRequestMessage> PreConfigureRequest { get; set; }

        private HttpClient Client { get; }
        private ISerializer Serializer { get; }

        public Task<T> Request<T>(
            string path,
            Action<HttpRequestMessage> configureRequest,
            Func<HttpResponseMessage, ISerializer, Task<T>> onSuccess)
        {
            return Client
                .RequestAsync(
                    PreConfigureRequest
                        .Compose(r =>
                        {
                            r.Properties.Add(SerializerReferenceKey, Serializer);
                        })
                        .Compose(configureRequest)
                        .Compose(r => r.Path(path))
                        .Compose(r =>
                        {
                            r.Properties.Remove(SerializerReferenceKey);
                        }),
                    async res => await onSuccess(res, Serializer),
                    async res => throw new RestClientException(res.StatusCode, await res.Content.ReadAsStringAsync()));
        }

        protected async Task<T> GetResult<T>(HttpContent content, ISerializer serializer, T anonymousObjectPrototype = default)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)(await content.ReadAsStringAsync());
            }

            if (anonymousObjectPrototype != null)
            {
                if (serializer.CanDeserializeAnonymousObjectFromStream)
                {
                    return serializer.DeserializeAnonymousObjectFromStream<T>(
                        await content.ReadAsStreamAsync(),
                        anonymousObjectPrototype);
                }

                return serializer.DeserializeAnonymousObject<T>(
                    await content.ReadAsStringAsync(),
                    anonymousObjectPrototype);
            }

            if (serializer.CanDeserializeFromStream)
            {
                return serializer.DeserializeFromStream<T>(
                    await content.ReadAsStreamAsync());
            }

            return serializer.Deserialize<T>(
                await content.ReadAsStringAsync());
        }

        public Task<T> Get<T>(string path, Action<HttpRequestMessage> configureRequest = null, T anonymousObjectPrototype = default)
        {
            return Request(
                path,
                configureRequest
                    .Compose(r => r
                        .Method(HttpMethod.Get)),
                (res, serializer) => GetResult<T>(res.Content, serializer, anonymousObjectPrototype));
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