using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Business
{
    /// <summary>
    /// Class modelling answer with caption,
    /// desired value and input value.
    /// </summary>
    [System.Serializable]
    public class Answer
    {
        public string AnswerText { get; set; }
        public bool IsValid { get; set; }

        [System.NonSerialized, XmlIgnore, JsonIgnore]
        public bool IsChecked;

        public Answer()
        {
        }

        public Answer(string answerText, bool isValid)
        {
            AnswerText = answerText;
            IsValid = isValid;
        }
    }
}