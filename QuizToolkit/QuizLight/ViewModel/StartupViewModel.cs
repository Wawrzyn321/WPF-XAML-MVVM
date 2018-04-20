using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Business;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using QuizLight.Model;

namespace QuizLight.ViewModel
{
    /// <summary>
    /// ViewModel resposible for simple main menu
    /// for Quiz. User can start or create new quiz.
    /// </summary>
    public class StartupViewModel : ViewModelBase
    {
        #region Static Texts

        public string Title => Properties.Resources.Title;
        public string StartQuizText => Properties.Resources.StartQuizText;
        public string CreateQuizText => Properties.Resources.CreateQuizText;

        #endregion

        #region Commands

        public ICommand StartQuizCommand { get; private set; }
        public ICommand CreateQuizCommand { get; private set; }

        #endregion

        private readonly ISwitch parent;

        public StartupViewModel(ISwitch parent)
        {
            this.parent = parent;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            StartQuizCommand = new RelayCommand(StartQuiz);
            CreateQuizCommand = new RelayCommand(CreateQuiz);
        }

        #region Command Callbacks

        private static string TryGetTestPath()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "XML files (*.xml)|*.xml|JSON files (*.json)|*.json|Binary files (*.dat)|*.dat",
            };
            bool? fileSelected = dialog.ShowDialog();
            if (fileSelected.HasValue == false)
            {
                MessageBox.Show("Could not open test!", "Quiz");
                return null;
            }
            else
            {
                if (fileSelected.Value)
                {
                    return dialog.FileName;
                }
                else
                {
                    return null;
                }
            }
        }

        private void StartQuiz()
        {
            string path = TryGetTestPath();

            if (path != null)
            {
                try
                {
                    QuestionsSet quesionSet = QuizModel.LoadQuestionsSet(path);
                    parent.SwitchTo(new QuizViewModel(parent, quesionSet));
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Test file invalid!", "Test");
                }
            }
        }

        private void CreateQuiz()
        {
            try
            {
                Process.Start("..\\..\\..\\Creator\\bin\\Debug\\Creator.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}
