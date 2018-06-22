using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Necessity.Http
{
    public static class StreamExtensions
    {
        public static Task<T> DeserializeJsonStreamAs<T>(this Stream stream, JsonSerializer serializer = null)
        {
            using (var textReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(textReader))
            {
                return Task.FromResult((serializer ?? new JsonSerializer()).Deserialize<T>(jsonReader));
            }
        }
    }
}
