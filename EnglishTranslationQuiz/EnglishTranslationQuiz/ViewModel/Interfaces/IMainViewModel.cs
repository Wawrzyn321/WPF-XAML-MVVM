using GalaSoft.MvvmLight;

namespace EnglishTranslationQuiz.ViewModel.Interfaces
{
    public interface IMainViewModel : ISwitch
    {
        ViewModelBase CurrentViewModel { get; set; }
        string WindowTitle { get; }
    }
}