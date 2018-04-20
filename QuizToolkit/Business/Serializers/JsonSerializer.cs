using Newtonsoft.Json;

namespace Business
{
    /// <summary>
    /// Serializer using JSON.
    /// Extension: ".json"
    /// </summary>
    public class JsonSerializer : ISerializer
    {
        public string GetExtension => ".json";

        public string Serialize(QuestionsSet questionSet)
        {
            return JsonConvert.SerializeObject(questionSet);
        }

        public QuestionsSet Deserialize(string data)
        {
            return JsonConvert.DeserializeObject<QuestionsSet>(data);
        }
    }
}