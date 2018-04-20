using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuizLight.View
{
    /// <summary>
    /// Answer from Business, tailored
    /// to fit the ViewModel.
    /// </summary>
    public class ViewAnswer : INotifyPropertyChanged
    {
        public string AnswerText { get; set; }
        public bool IsChecked { get; set; }

        private bool shouldBeChecked;
        public bool ShouldBeChecked
        {
            get => shouldBeChecked;
            set => Set(ref shouldBeChecked, value);
        }

        public ViewAnswer(string answerText)
        {
            AnswerText = answerText;
            IsChecked = false;
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