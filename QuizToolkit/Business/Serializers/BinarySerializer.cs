using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Business
{
    /// <summary>
    /// Serializer using Base64 code.
    /// Extension: ".dat"
    /// </summary>
    public class BinarySerializer : ISerializer
    {
        public string GetExtension => ".dat";

        public string Serialize(QuestionsSet questionSet)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, questionSet);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public QuestionsSet Deserialize(string data)
        {
            byte[] bytes = Convert.FromBase64String(data);

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return (QuestionsSet)new BinaryFormatter().Deserialize(stream);
            }
        }

    }
}
