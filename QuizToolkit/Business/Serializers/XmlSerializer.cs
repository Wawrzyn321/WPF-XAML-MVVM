using System.IO;
using System.Text;
using SystemSerializer = System.Xml.Serialization.XmlSerializer;

namespace Business
{
    /// <inheritdoc />
    /// <summary>
    /// Serializer using XML.
    /// Extension: ".xml"
    /// </summary>
    public class XmlSerializer : ISerializer
    {
        public string GetExtension => ".xml";

        public string Serialize(QuestionsSet questionSet)
        {
            using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
            {
                SystemSerializer xmlSerializer = new SystemSerializer(typeof(QuestionsSet));
                xmlSerializer.Serialize(stringWriter, questionSet);
                return stringWriter.ToString();
            }
        }
        public QuestionsSet Deserialize(string data)
        {
            SystemSerializer serializer = new SystemSerializer(typeof(QuestionsSet));
            using (var reader = new StringReader(data))
            {
                return (QuestionsSet) serializer.Deserialize(reader);
            }
        }

    }
}
