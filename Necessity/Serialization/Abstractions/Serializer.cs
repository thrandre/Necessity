using System;

namespace Necessity.Serialization.Abstractions
{
    public class Serializer
    {
        public Serializer(Func<object, string> serializeFunc, Func<string, Type, object> deserializeFunc)
        {
            SerializeFunc = serializeFunc;
            DeserializeFunc = deserializeFunc;
        }

        private Func<object, string> SerializeFunc { get; }
        private Func<string, Type, object> DeserializeFunc { get; }

        public string Serialize<T>(object @object)
        {
            return SerializeFunc(@object);
        }

        public T Deserialize<T>(string serializedObject)
        {
            return (T)DeserializeFunc(serializedObject, typeof(T));
        }
    }
}