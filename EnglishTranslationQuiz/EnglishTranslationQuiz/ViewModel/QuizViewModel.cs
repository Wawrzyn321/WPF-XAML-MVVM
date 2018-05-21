using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MVVMTest2.Model;

namespace MVVMTest2.ViewModel
{

    /// <summary>
    /// ViewModel presenting the quiz part of application.
    /// The user sees the word, thinks of possible translations
    /// and presses the "Show Translation" button. After seeing
    /// the translations, decides if they want to repeat the word
    /// or remove it as already known.
    /// 
    /// The "I don't know" and "Again" buttons are doing the same,
    /// they are just here for psychology.
    /// </summary>
    public class QuizViewModel : ViewModelBase
    {
        public enum QuizState
        {
            ShowingWord,
            ShowingWordWithTranslation,
            Finished,
        }

        #region Static Texts

        public string IDontKnowText => Properties.Resources.Quiz_IDontKnow;
        public string AgainText => Properties.Resources.Quiz_Again;
        public string IKnowItText => Properties.Resources.Quiz_Know;
        public string ShowTranslationText => Properties.Resources.Quiz_ShowTranslation;
        public string BackText => Properties.Resources.Quiz_Back;

        #endregion

        #region Observed Properties

        private QuizState _state;
        public QuizState State
        {
            get => _state;
            set => Set(ref _state, value);
        }

        private string fileName;
        public string FileName
        {
            get => fileName;
            set => Set(ref fileName, value);
        }

        public WordService WordsService => wordsService;

        #endregion

        #region Commands

        public ICommand IDontKnowCommand { get; private set; }
        public ICommand AgainCommand { get; private set; }
        public ICommand IKnowItCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand ShowTranslationCommand { get; private set; }

        #endregion

        private readonly ISwitch parent;
        private readonly WordService wordsService;

        public QuizViewModel(ISwitch parent, DataItem item, string path)
        {
            this.parent = parent;

            wordsService = new WordService(item);
            FileName = path;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            IDontKnowCommand = new RelayCommand(RepeatWord);
            AgainCommand = new RelayCommand(RepeatWord);
            IKnowItCommand = new RelayCommand(SetWordAsLearned);
            BackCommand = new RelayCommand(BackToMenu);
            ShowTranslationCommand = new RelayCommand(ShowTranslation);
        }

        private void Finish()
        {
            ShowTranslation();
            MessageBox.Show(Properties.Resources.Startup_NoWordsFound, Properties.Resources.Quiz_AllWordsLearned);
        }

        #region Command Callbacks

        private void BackToMenu()
        {
            parent.SwitchToDefault();
        }

        private void ShowTranslation()
        {
            wordsService.ShowCurrentTranslation();
            State = QuizState.ShowingWordWithTranslation;
        }

        private void SetWordAsLearned()
        {
            wordsService.SetCurrentWordAsLearned();
            if (wordsService.HasLearnedAllTheWords())
            {
                //finish the quiz
                Finish();
            }
            else
            {
                //get next word
                State = QuizState.ShowingWord;
            }
        }

        private void RepeatWord()
        {
            //set word as to repeat
            wordsService.RepeatCurrentWord();

            State = QuizState.ShowingWord;
        }

        #endregion
    }
}
