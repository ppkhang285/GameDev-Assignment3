using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class DeepCopyUtility
{
    public static T DeepCopy<T>(T obj)
    {
        if (obj == null) return default;

        using (var memoryStream = new MemoryStream())
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, obj);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(memoryStream);
        }
    }
}
