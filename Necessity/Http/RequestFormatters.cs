using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Necessity.Http
{
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
}