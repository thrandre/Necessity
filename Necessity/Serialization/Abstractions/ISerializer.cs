using System.IO;

namespace Necessity.Serialization.Abstractions
{
    public interface ISerializer
    {
        bool CanDeserializeAnonymousObject { get; }
        bool CanSerializeToStream { get; }
        bool CanDeserializeFromStream { get; }

        string Serialize(object @object);
        T Deserialize<T>(string serializedObject);
        T DeserializeAnonymousObject<T>(string serializedObject, T anonymousObjectPrototype);
        T DeserializeFromStream<T>(Stream stream);
        void SerializeToStream(object @object, Stream stream);
    }
}