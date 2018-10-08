using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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
                        return default;
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

                return default;
            };
        }

        public static Func<HttpResponseMessage, Stream, Task<T>> GetJsonDeserializer<T>(Func<StreamReader, T> deserializerFunc)
        {
            return (message, stream) =>
                JsonDeserialize<T>(message, stream, deserializerFunc);
        }

        public static Task<T> JsonDeserialize<T>(HttpResponseMessage response, Stream stream, Func<StreamReader, T> deserializerFunc)
        {
            using (var textReader = new StreamReader(stream))
            {
                return Task.FromResult(deserializerFunc(textReader));
            }
        }
    }
}