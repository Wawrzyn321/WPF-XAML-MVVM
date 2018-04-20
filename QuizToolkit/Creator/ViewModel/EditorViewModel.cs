using Business;
using Creator.Model.Contract;
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
    class EditorViewModel : ViewModelBase
    {
        #region Fields
        private QuestionsSet _questionsSet;
        private Question _selectedQuestion;
        private ISaveLoadAgent _saveLoadAgent;
        private Answer _selectedAnswer;
        #endregion
        #region Properties
        public QuestionsSet QuestionsSet
        {
            get { return _questionsSet; }
            private set { Set(ref _questionsSet, value); }
        }

        public Question SelectedQuestion
        {
            get { return _selectedQuestion; }
            set { Set(ref _selectedQuestion, value); }
        }

        public Answer SelectedAnswer
        {
            get { return _selectedAnswer; }
            set { Set(ref _selectedAnswer, value); }
        }
        #endregion
        #region Commands
        public ICommand SaveCommand { get; private set; }
        public ICommand AddQuestionCommand { get; private set; }
        public ICommand DeleteQuestionCommand { get; private set; }
        public ICommand AddAnswerCommand { get; private set; }
        public ICommand DeleteAnswerCommand { get; private set; }
        #endregion
        #region Methods
        public EditorViewModel(ISaveLoadAgent saveLoadAgent, QuestionsSet questionSet)
        {
            _saveLoadAgent = saveLoadAgent;
            QuestionsSet = questionSet;
            initializeCommands();
        }
    
        public EditorViewModel(ISaveLoadAgent saveLoadAgent):
            this(saveLoadAgent, new QuestionsSet())
        {
            loadTestData();
        }

        private void initializeCommands()
        {
            SaveCommand = new RelayCommand(saveQuestionSet);
            AddQuestionCommand = new RelayCommand(addQuestion);
            DeleteQuestionCommand = new RelayCommand(deleteQuestion);
            AddAnswerCommand = new RelayCommand(addAnswer);
            DeleteAnswerCommand = new RelayCommand(deleteAnswer);
        }

        private void saveQuestionSet()
        {
            try
            {
                _saveLoadAgent.Save(QuestionsSet);
            }
            catch(InvalidExtensionException)
            {

            }
        }

        private void addQuestion()
        {
            var question = new Question();
            question.QuestionText = "New question";
            question.Answers = new ObservableCollection<Answer>();
            question.Answers.Add(new Answer("New answer", false));
            QuestionsSet.Questions.Add(question);
        }

        private void deleteQuestion()
        {
            QuestionsSet.Questions.Remove(SelectedQuestion);
        }

        private void addAnswer()
        {
            try
            {
                var answer = new Answer("New answer", false);
                SelectedQuestion.Answers.Add(answer);
            }
            catch (Exception)
            {

            }
        }

        private void deleteAnswer()
        {
            SelectedQuestion.Answers.Remove(SelectedAnswer);
        }

        private void loadTestData()
        {
            var answers = new List<Answer>();
            answers.Add(new Answer("tak", true));
            answers.Add(new Answer("nie", false));
            QuestionsSet.Questions = new ObservableCollection<Question>();
            QuestionsSet.Questions.Add(new Question("cycki?", answers));

            answers = new List<Answer>();
            answers.Add(new Answer("jak najbardziej", true));
            answers.Add(new Answer("chyba czasami", false));
            QuestionsSet.Questions.Add(new Question("dupa?", answers));
        }
        #endregion
    }
}
