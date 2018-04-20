using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace Business
{
    /// <summary>
    /// Utility class for saving and loading files,
    /// using ISerializer to encode and decode data.
    /// </summary>
    public class SaveLoadManager
    {
        public string GetSerializerExtension => serializer.GetExtension;

        private readonly ISerializer serializer;

        public SaveLoadManager(ISerializer serializer)
        {
            this.serializer = serializer;
        }

        public void Save(QuestionsSet questionsSet, string path, string fileName)
        {
            string finalPath = Path.Combine(path, fileName) + serializer.GetExtension;
            File.WriteAllText(finalPath, serializer.Serialize(questionsSet));
        }

        public QuestionsSet Load(string path)
        {
            if (!path.EndsWith(serializer.GetExtension))
            {
                throw new InvalidExtensionException(
                    $"Invalid extension -  \"{serializer.GetExtension}\" expected!");
            }
            string data = File.OpenText(path).ReadToEnd();
            return serializer.Deserialize(data);
        }
    }

    /// <summary>
    /// Exception thrown when file with invalid
    /// extension is being loaded.
    /// </summary>
    public class InvalidExtensionException : Exception
    {
        public InvalidExtensionException()
        {
        }

        public InvalidExtensionException(string message) : base(message)
        {
        }
    }
}
