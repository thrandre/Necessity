using System.IO;

namespace Necessity.Serialization.Abstractions
{
    public interface ISerializer
    {
        bool CanDeserializeAnonymousObject { get; }
        bool CanSerializeToStream { get; }
        bool CanDeserializeFromStream { get; }
        bool CanDeserializeAnonymousObjectFromStream { get; }

        string Serialize(object @object);
        void SerializeToStream(object @object, Stream stream);
        T Deserialize<T>(string serializedObject);
        T DeserializeAnonymousObject<T>(string serializedObject, T anonymousObjectPrototype);
        T DeserializeFromStream<T>(Stream stream);
        T DeserializeAnonymousObjectFromStream<T>(Stream stream, T objectPrototype);
    }
}