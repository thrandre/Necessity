using System;
using System.IO;
using System.Threading.Tasks;

namespace Necessity.Http
{
    public static class StreamExtensions
    {
        public static Task<T> DeserializeJsonStreamAs<T>(this Stream stream, Func<StreamReader, T> deserializerFunc)
        {
            using (var textReader = new StreamReader(stream))
             {
                return Task.FromResult(deserializerFunc(textReader));
            }
        }
    }
}
