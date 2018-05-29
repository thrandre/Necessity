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
        public static Func<HttpResponseMessage, Stream, Task<T>> GetErrorHandler<T>(Func<HttpResponseMessage, Stream, Task<T>> innerFn, Func<HttpResponseMessage, string, Task<T>> onHttpError = null, Func<Exception, Task> onException = null)
        {
            return async (res, stream) =>
            {
                try
                {
                    if (res.IsSuccessStatusCode)
                    {
                        return await innerFn(res, stream).ConfigureAwait(false);
                    }

                    if (onHttpError == null)
                    {
                        return default(T);
                    }

                    using (var sw = new StreamReader(stream))
                    {
                        return await onHttpError(res, sw.ReadToEnd()).ConfigureAwait(false);
                    }
                }
                catch (Exception exception)
                {
                    if (onException != null)
                    {
                        await onException(exception).ConfigureAwait(false);
                    }
                    else
                    {
                        throw;
                    }
                }

                return default(T);
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
            return GetStreamContent(sw => serializer.Serialize(sw, obj), "application/json");
        }

        public static StreamContent GetStreamContent(Action<StreamWriter> writeContentAct, string contentType)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);

            writeContentAct(streamWriter);

            streamWriter.Flush();
            memoryStream.Position = 0;

            return new StreamContent(memoryStream)
                .Pipe(x => { x.Headers.ContentType = new MediaTypeHeaderValue(contentType); });
        }
    }

    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> SendAndGetResponseAsync(
            this HttpClient client,
            Action<HttpRequestMessage> configureRequest)
        {
            var res = await client
                .SendAsync(new HttpRequestMessage()
                    .Pipe(configureRequest ?? (_ => { })), HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            return res;
        }

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
