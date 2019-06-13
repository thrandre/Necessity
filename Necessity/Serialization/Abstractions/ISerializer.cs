namespace Necessity.Serialization.Abstractions
{
    public interface ISerializer
    {
        string Serialize(object @object);
        T Deserialize<T>(string serializedObject);
        T DeserializeAnonymousObject<T>(string serializedObject, T anonymousObjectPrototype);
    }
}