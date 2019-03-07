using System;

namespace Necessity.Serialization.Abstractions
{
    public class FuncSerializer : ISerializer
    {
        public FuncSerializer(Func<object, string> serializeFunc, Func<string, Type, object> deserializeFunc, Func<string, object, object> deserializeAnonymousFunc = null)
        {
            SerializeFunc = serializeFunc;
            DeserializeFunc = deserializeFunc;
            DeserializeAnonymousFunc = deserializeAnonymousFunc;
        }

        private Func<object, string> SerializeFunc { get; }
        private Func<string, Type, object> DeserializeFunc { get; }
        public Func<string, object, object> DeserializeAnonymousFunc { get; }

        public string Serialize<T>(object @object)
        {
            return SerializeFunc(@object);
        }

        public T Deserialize<T>(string serializedObject)
        {
            return (T)DeserializeFunc(serializedObject, typeof(T));
        }

        public T DeserializeAnonymousObject<T>(string serializedObject, T anonymousObjectPrototype)
        {
            if (DeserializeAnonymousFunc == null)
            {
                throw new InvalidOperationException("Not supported.");
            }

            return (T)DeserializeAnonymousFunc(serializedObject, anonymousObjectPrototype);
        }
    }
}