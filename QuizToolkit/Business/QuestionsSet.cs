using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Business
{
    /// <summary>
    /// A set of questions with title
    /// and Question collection,
    /// serialized by SaveLoadManager.
    /// </summary>
    [System.Serializable]
    public class QuestionsSet : ObservableObject
    {
        private ObservableCollection<Question> _questions;

        public string Name { get; set; }
        public bool HasTimeLimit { get; set; }
        public int TimeLimit { get; set; }
        public ObservableCollection<Question> Questions
        {
            get { return _questions; }
            set { Set(ref _questions, value); }
        }

        public QuestionsSet()
        {
        }

        public QuestionsSet(string name, List<Question> questions)
        {
            Name = name;
            Questions = new ObservableCollection<Question>(questions);

            HasTimeLimit = false;
        }

        public QuestionsSet(string name, List<Question> questions, TimeSpan timeLimit)
        {
            Name = name;
            Questions = new ObservableCollection<Question>(questions);

            HasTimeLimit = true;
            TimeLimit = (int)timeLimit.TotalSeconds;
        }
    }
}
