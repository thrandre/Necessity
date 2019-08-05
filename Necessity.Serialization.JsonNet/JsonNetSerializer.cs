using System.IO;
using System.Text;
using Necessity.Serialization.Abstractions;
using Newtonsoft.Json;

namespace Necessity.Serialization.JsonNet
{
    public class JsonNetSerializer : ISerializer
    {
        public JsonNetSerializer(JsonSerializer jsonSerializer)
        {
            JsonSerializer = jsonSerializer;
        }

        public bool CanDeserializeAnonymousObject => true;
        public bool CanSerializeToStream => true;
        public bool CanDeserializeFromStream => true;
        public bool CanDeserializeAnonymousObjectFromStream => true;

        private JsonSerializer JsonSerializer { get; }

        public T Deserialize<T>(string serializedObject)
        {
            using (var textReader = new StringReader(serializedObject))
            using (var jsonTextReader = new JsonTextReader(textReader))
            {
                return (T)JsonSerializer.Deserialize(jsonTextReader, typeof(T));
            }
        }

        public T DeserializeAnonymousObject<T>(string serializedObject, T anonymousObjectPrototype)
        {
            return Deserialize<T>(serializedObject);
        }

        public T DeserializeFromStream<T>(Stream stream)
        {
            using (var textReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(textReader))
            {
                return (T)JsonSerializer.Deserialize(jsonTextReader, typeof(T));
            }
        }

        public T DeserializeAnonymousObjectFromStream<T>(Stream stream, T objectPrototype)
        {
            return DeserializeFromStream<T>(stream);
        }

        public string Serialize(object @object)
        {
            var stringBuilder = new StringBuilder();
            using (var textWriter = new StringWriter(stringBuilder))
            {
                JsonSerializer.Serialize(textWriter, @object);
                return stringBuilder.ToString();
            }

        }

        public void SerializeToStream(object @object, Stream stream)
        {
            using (var stringWriter = new StreamWriter(stream))
            {
                JsonSerializer.Serialize(stringWriter, @object);
            }
        }
    }
}
