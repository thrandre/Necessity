using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Necessity.Http
{
    public static class RequestFormatters
    {
        public static StreamContent GetJsonBody<T>(T obj, Action<StreamWriter, T> serializerFunc)
        {
            return GetStreamContent(sw => serializerFunc(sw, obj), "application/json");
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
}