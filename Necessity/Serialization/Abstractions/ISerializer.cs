namespace Necessity.Serialization.Abstractions
{
    public interface ISerializer
    {
        string Serialize<T>(object @object);
        T Deserialize<T>(string serializedObject);
    }
}