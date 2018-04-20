using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Business;

namespace Quiz.View
{
    public class ViewQuestion : INotifyPropertyChanged
    {
        public enum ViewQuestionState
        {
            Unanswered,
            Correct,
            Wrong,
        }

        public string QuestionText { get; set; }
        public List<ViewAnswer> Answers { get; set; }

        private ViewQuestionState questionState;
        public ViewQuestionState QuestionState
        {
            get => questionState;
            set => Set(ref questionState, value);
        }

        public ViewQuestion(string questionText, List<ViewAnswer> answers)
        {
            QuestionText = questionText;
            Answers = answers;
            QuestionState = ViewQuestionState.Unanswered;
        }

        public static ViewQuestion FromQuestion(Question question)
        {
            List<ViewAnswer> viewAnswers = new List<ViewAnswer>();
            foreach (Answer answer in question.Answers)
            {
                viewAnswers.Add(new ViewAnswer(answer.AnswerText));
            }
            return new ViewQuestion(question.QuestionText, viewAnswers);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
