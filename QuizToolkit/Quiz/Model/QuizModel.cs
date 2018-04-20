using System.IO;
using Business;

namespace Quiz.Model
{
    public static class QuizModel
    {
        public static QuestionsSet LoadQuestionsSet(string path)
        {
            ISerializer serializer;
            switch (Path.GetExtension(path))
            {
                case ".xml":
                    serializer = new XmlSerializer();
                    break;
                case ".json":
                    serializer = new JsonSerializer();
                    break;
                case ".dat":
                    serializer = new BinarySerializer();
                    break;
                default:
                    throw new InvalidExtensionException("It ain't gonna work mate. Try with .xml, .json or .dat.");
            }
            SaveLoadManager manager = new SaveLoadManager(serializer);
            return manager.Load(path);
        }

    }
}
