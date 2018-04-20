using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace Business
{
    /// <summary>
    /// A question with caption and
    /// collection of answers.
    /// </summary>
    [System.Serializable]
    public class Question : ObservableObject
    {
        private ObservableCollection<Answer> _answers;
        private string _questionText;

        public string QuestionText
        {
            get { return _questionText; }
            set { Set(ref _questionText, value); }
        }
        public ObservableCollection<Answer> Answers
        {
            get { return _answers; }
            set { Set(ref _answers, value); }
        }

        [System.NonSerialized, XmlIgnore, JsonIgnore]
        public bool IsMultipleChoice;

        public Question()
        {
        }

        public Question(string questionText, List<Answer> answers, bool forceIsMultipleChoice = false)
        {
            QuestionText = questionText;
            Answers = new ObservableCollection<Answer>(answers);
            IsMultipleChoice = forceIsMultipleChoice || answers.Count(answer => answer.IsValid) != 1;
        }

        public bool Verify(out List<int> badAnswerIndices)
        {
            badAnswerIndices = new List<int>();

            for (int i = 0; i < Answers.Count; i++)
            {
                if (Answers[i].IsValid != Answers[i].IsChecked)
                {
                    badAnswerIndices.Add(i);
                }
            }
            return badAnswerIndices.Count == 0;
        }
    }
}
