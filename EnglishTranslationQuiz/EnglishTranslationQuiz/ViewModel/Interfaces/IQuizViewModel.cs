using System.Windows.Input;

namespace EnglishTranslationQuiz.ViewModel.Interfaces
{
    public interface IQuizViewModel
    {
        ICommand AgainCommand { get; }
        string AgainText { get; }
        ICommand BackCommand { get; }
        string BackText { get; }
        string FileName { get; set; }
        ICommand IDontKnowCommand { get; }
        string IDontKnowText { get; }
        ICommand IKnowItCommand { get; }
        string IKnowItText { get; }
        ICommand ShowTranslationCommand { get; }
        string ShowTranslationText { get; }
        QuizViewModel.QuizState State { get; set; }
        WordService WordsService { get; }
    }
}