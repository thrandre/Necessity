using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Necessity
{
    public static class ResponseHandlers
    {
        public static Func<HttpResponseMessage, Stream, Task<T>> HandleErrors<T>(Func<HttpResponseMessage, Stream, Task<T>> innerFn, Action<HttpResponseMessage> onErrorAct)
        {
            return (res, stream) =>
            {
                if (res.IsSuccessStatusCode)
                {
                    return innerFn(res, stream);
                }

                onErrorAct(res);
                return Task.FromResult(default(T));
            };
        }

        public static Func<HttpResponseMessage, Stream, Task<T>> Deserialize<T>(Func<HttpResponseMessage, Stream, Task<T>> innerFn)
        {
            return (res, stream) =>
            {
                using (var textReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(textReader))
                {
                    return Task.FromResult(new JsonSerializer().Deserialize<T>(jsonReader));
                }
            };
        }
    }

    public static class HttpClientExtensions
    {
        public static async Task<T> SendAndStreamResultAsync<T>(
            this HttpClient client,
            Action<HttpRequestMessage> configureRequest,
            Func<HttpResponseMessage, Stream, Task<T>> responseHandler)
        {
            var res = await client
                .SendAsync(new HttpRequestMessage()
                    .Pipe(configureRequest ?? (_ => { })), HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            var responseStream = res.Content != null
                ? await res.Content.ReadAsStreamAsync().ConfigureAwait(false)
                : Stream.Null;

            using (responseStream)
            {
                return await responseHandler(
                        res,
                        responseStream)
                    .ConfigureAwait(false);
            }
        }
    }
}
