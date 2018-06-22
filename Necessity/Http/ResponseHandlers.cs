using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Necessity.Http
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
}