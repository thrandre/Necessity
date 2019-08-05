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
            Func<Stream, object> deserializeFromStreamFunc = null,
            Func<Stream, object, object> deserializeAnonymousFromStreamFunc = null)
        {
            SerializeFunc = serializeFunc;
            DeserializeFunc = deserializeFunc;
            DeserializeAnonymousFunc = deserializeAnonymousFunc;
            SerializeToStreamFunc = serializeToStreamFunc;
            DeserializeFromStreamFunc = deserializeFromStreamFunc;
            DeserializeAnonymousFromStreamFunc = deserializeAnonymousFromStreamFunc;
        }

        public bool CanDeserializeAnonymousObject => DeserializeAnonymousFunc != null;
        public bool CanSerializeToStream => SerializeToStreamFunc != null;
        public bool CanDeserializeFromStream => DeserializeFromStreamFunc != null;
        public bool CanDeserializeAnonymousObjectFromStream => DeserializeAnonymousFromStreamFunc != null;

        private Func<object, string> SerializeFunc { get; }
        private Func<string, Type, object> DeserializeFunc { get; }
        private Func<string, object, object> DeserializeAnonymousFunc { get; }
        private Action<object, Stream> SerializeToStreamFunc { get; }
        private Func<Stream, object> DeserializeFromStreamFunc { get; }
        private Func<Stream, object, object> DeserializeAnonymousFromStreamFunc { get; }

        public string Serialize(object @object)
        {
            return SerializeFunc(@object);
        }

        public void SerializeToStream(object @object, Stream stream)
        {
            if (!CanSerializeToStream)
            {
                throw new InvalidOperationException("Unsupported");
            }

            SerializeToStreamFunc(@object, stream);
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

        public T DeserializeAnonymousObjectFromStream<T>(Stream stream, T objectPrototype)
        {
            if (!CanDeserializeAnonymousObjectFromStream)
            {
                throw new InvalidOperationException("Unsupported");
            }

            return (T)DeserializeAnonymousFromStreamFunc(@stream, objectPrototype);
        }
    }
}