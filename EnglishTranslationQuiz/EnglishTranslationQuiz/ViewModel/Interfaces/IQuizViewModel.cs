using System.Windows.Input;

namespace MVVMTest2.ViewModel
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