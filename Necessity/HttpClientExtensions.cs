using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Necessity
{
    public static class ResponseHandlers
    {
        public static Func<HttpResponseMessage, Stream, Task<T>> GetErrorHandler<T>(Func<HttpResponseMessage, Stream, Task<T>> innerFn, Func<HttpResponseMessage, Task<T>> onHttpError = null, Func<Exception, Task> onException = null)
        {
            return (res, stream) =>
            {
                try
                {
                    return res.IsSuccessStatusCode
                        ? innerFn(res, stream)
                        : onHttpError?.Invoke(res);
                }
                catch (Exception exception)
                {
                    (onException ?? (e => throw e))(exception);
                }

                return Task.FromResult(default(T));
            };
        }

        public static Func<HttpResponseMessage, Stream, Task<T>> GetJsonDeserializer<T>(JsonSerializer serializer)
        {
            return (message, stream) =>
                JsonDeserialize<T>(message, stream, serializer);
        }

        public static Task<T> JsonDeserialize<T>(HttpResponseMessage response, Stream stream, JsonSerializer serializer = null)
        {
            using (var textReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(textReader))
            {
                return Task.FromResult((serializer ?? new JsonSerializer()).Deserialize<T>(jsonReader));
            }
        }
    }

    public static class RequestFormatters
    {
        public static StreamContent GetJsonBody<T>(T obj, JsonSerializer serializer)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);

            serializer.Serialize(streamWriter, obj);
            streamWriter.Flush();

            memoryStream.Position = 0;

            return new StreamContent(memoryStream)
                .Pipe(x => { x.Headers.ContentType = new MediaTypeHeaderValue("application/json"); });
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
