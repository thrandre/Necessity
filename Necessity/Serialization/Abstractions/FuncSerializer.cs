using System;
using System.IO;

namespace Necessity.Serialization.Abstractions
{
    public class FuncSerializer : ISerializer
    {
        public FuncSerializer(
            Func<object, string> serializeFunc,
            Func<string, Type, object> deserializeFunc,
            Func<string, object, object> deserializeAnonymousFunc = null,
            Action<object, Stream> serializeToStreamFunc = null,
            Func<Stream, object> deserializeFromStreamFunc = null)
        {
            SerializeFunc = serializeFunc;
            DeserializeFunc = deserializeFunc;
            DeserializeAnonymousFunc = deserializeAnonymousFunc;
            SerializeToStreamFunc = serializeToStreamFunc;
            DeserializeFromStreamFunc = deserializeFromStreamFunc;
        }

        public bool CanDeserializeAnonymousObject => DeserializeAnonymousFunc != null;
        public bool CanSerializeToStream => SerializeToStreamFunc != null;
        public bool CanDeserializeFromStream => DeserializeFromStreamFunc != null;

        private Func<object, string> SerializeFunc { get; }
        private Func<string, Type, object> DeserializeFunc { get; }
        private Func<string, object, object> DeserializeAnonymousFunc { get; }
        private Action<object, Stream> SerializeToStreamFunc { get; }
        private Func<Stream, object> DeserializeFromStreamFunc { get; }

        public string Serialize(object @object)
        {
            return SerializeFunc(@object);
        }

        public T Deserialize<T>(string serializedObject)
        {
            return (T)DeserializeFunc(serializedObject, typeof(T));
        }

        public T DeserializeAnonymousObject<T>(string serializedObject, T anonymousObjectPrototype)
        {
            if (!CanDeserializeAnonymousObject)
            {
                throw new InvalidOperationException("Unsupported");
            }

            return (T)DeserializeAnonymousFunc(serializedObject, anonymousObjectPrototype);
        }

        public T DeserializeFromStream<T>(Stream stream)
        {
            if (!CanDeserializeFromStream)
            {
                throw new InvalidOperationException("Unsupported");
            }

            return (T)DeserializeFromStreamFunc(stream);
        }

        public void SerializeToStream(object @object, Stream stream)
        {
            if (!CanSerializeToStream)
            {
                throw new InvalidOperationException("Unsupported");
            }

            SerializeToStreamFunc(@object, stream);
        }
    }
}