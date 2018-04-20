using Business;
using Creator.Model.Contract;
using Creator.ViewModel.Contract;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Creator.ViewModel
{
    class StartupViewModel : ViewModelBase
    {
        private string _createNewText;
        private string _openExistingText;
        private ISaveLoadAgent _saveLoadAgent;
        private ISwitchToEditor _switchToEditor;


        public string CreateNewText
        {
            get { return _createNewText; }
        }

        public string OpenExistingText
        {
            get { return _openExistingText; }
        }


        public ICommand CreateNewCommand { get; private set; }
        public ICommand OpenExistingCommand { get; private set; }


        public StartupViewModel(ISwitchToEditor switchToEditor, ISaveLoadAgent saveLoadAgent)
        {
            _switchToEditor = switchToEditor;
            _saveLoadAgent = saveLoadAgent;
            initializeTexts();
            initializeCommands();
        }

        private void initializeTexts()
        {
            _createNewText = Properties.Resources.CreateNewQuestionSetText;
            _openExistingText = Properties.Resources.OpenExistingQuestionSetText;
        }

        private void initializeCommands()
        {
            CreateNewCommand = new RelayCommand(createNew);
            OpenExistingCommand = new RelayCommand(openExisting);
        }

        private void createNew()
        {
            var questionSet = new QuestionsSet();
            questionSet.Questions = new ObservableCollection<Question>();
            _switchToEditor.LoadEditorView(questionSet);
        }

        private void openExisting()
        {
            try
            {
                var questionSet = _saveLoadAgent.Load();
                _switchToEditor.LoadEditorView(questionSet);
            }
            catch(InvalidExtensionException)
            {
                
            }
            catch (Exception)
            {

            }
        }
    }
}
