using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Business;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using QuizLight.View;

namespace QuizLight.ViewModel
{
    /// <summary>
    /// ViewModel for quiz part.
    /// </summary>
    public class QuizViewModel : ViewModelBase
    {
        #region Static Texts

        public string ReturnToMenuText => Properties.Resources.ReturnToMenu;

        #endregion

        #region Observed Properties

        private ObservableCollection<ViewQuestion> itemsSource;
        public ObservableCollection<ViewQuestion> ItemsSource
        {
            get => itemsSource;
            set => Set(ref itemsSource, value);
        }

        private string titleText;
        public string TitleText
        {
            get => titleText;
            set => Set(ref titleText, value);
        }

        private string timerValue;
        public string TimerValue
        {
            get => timerValue;
            set => Set(ref timerValue, value);
        }

        private string rightButtonText;
        public string RightButtonText
        {
            get => rightButtonText;
            set => Set(ref rightButtonText, value);
        } 

        #endregion

        #region Commands

        public ICommand ReturnToMenuCommand { get; private set; }
        public ICommand FinishTestCommand { get; private set; }

        #endregion

        public bool IsTestFinished { get; private set; }
        public int CorrectQuestionsCount { get; private set; }

        private ExternalTimeDispatcher timeDispatcher;
        private readonly ISwitch parent;
        private readonly QuestionsSet questionSet;

        public QuizViewModel(ISwitch parent, QuestionsSet questionSet)
        {
            this.parent = parent;
            this.questionSet = questionSet;

            IsTestFinished = false;
            RightButtonText = Properties.Resources.FinishTest;

            if (questionSet.HasTimeLimit)
            {
                InitTimeDispatcher(questionSet.TimeLimit);
            }
            else
            {
                TimerValue = string.Empty;
            }

            TitleText = "ee?";

            InitializeItemsSource();
            InitializeCommands();
        }
        
        private void InitializeCommands()
        {
            ReturnToMenuCommand = new RelayCommand(ReturnToMenu);
            FinishTestCommand = new RelayCommand(FinishTest);
        }

        private void InitializeItemsSource()
        {
            var questions = new ObservableCollection<ViewQuestion>();
            foreach (Question question in questionSet.Questions)
            {
                questions.Add(ViewQuestion.FromQuestion(question));
            }
            ItemsSource = questions;
        }

        private void InitTimeDispatcher(int timeout)
        {
            timeDispatcher = new ExternalTimeDispatcher(timeout);
            timeDispatcher.OnTimeout += OnTimeout;
            timeDispatcher.OnTick += OnTimerTick;
            OnTimerTick(timeout);
        }

        private void OnTimerTick(int remainingSeconds)
        {
            TimeSpan remainingTime = TimeSpan.FromSeconds(remainingSeconds);
            TimerValue = $"{remainingTime:hh}:{remainingTime:mm}:{remainingTime:ss}";
        }

        private void OnTimeout()
        {
            TimerValue = Properties.Resources.TimeOut;
            OnTestFinished();
        }

        private void OnTestFinished()
        {
            if (IsTestFinished) return;

            IsTestFinished = true;
            RightButtonText = Properties.Resources.ShowScore;

            //count valid answers
            for (int i = 0; i < ItemsSource.Count; i++)
            {
                ViewQuestion item = ItemsSource[i];
                Question question = questionSet.Questions[i];
                for (int j = 0; j < item.Answers.Count; j++)
                {
                    ViewAnswer currentAnswer = item.Answers[j];

                    question.Answers[j].IsChecked = currentAnswer.IsChecked;
                    currentAnswer.ShouldBeChecked = question.Answers[j].IsValid;
                }
                bool isAnswerCorrect = question.Verify(out _);
                item.QuestionState = isAnswerCorrect ? ViewQuestion.ViewQuestionState.Correct : ViewQuestion.ViewQuestionState.Wrong;
                if (isAnswerCorrect)
                {
                    CorrectQuestionsCount++;
                }
            }
            ShowResults();
        }

        private void ShowResults()
        {
            float validAnswersRatio = CorrectQuestionsCount / (float)questionSet.Questions.Count;
            char fin = CorrectQuestionsCount == questionSet.Questions.Count ? '?' : '.';
            MessageBox.Show($"{Properties.Resources.ScoreMessage} {validAnswersRatio:P2}{fin}");
        }

        public static void CreateTestSet()
        {
            var qs = new List<Question>
            {
                new Question("Here's question one", new List<Answer> { new Answer("YES", true), new Answer("NO", false)}),
                new Question("That's gonna do", new List<Answer> { new Answer("odpowiedź a", true), new Answer("odpowiedź b", true)}),
                new Question("Heheszki", new List<Answer> { new Answer("nie ma innych odpowiedzi, ale jest fajnie", false)}),
                new Question("Ruhu huhu hu", new List<Answer> { new Answer("A", false), new Answer("b", false)}),
                new Question("Nie ma to jak gromnica", new List<Answer> { new Answer("Tak", true), new Answer("NIGDY W ŻYCIU", false), new Answer("no spoko", true)}),
                new Question("Gdzie zaczyna się mój świat?", new List<Answer>
                {
                    new Answer("Tam gdzie słońce nie sięga", false),
                    new Answer("W miejscu do którego należę", false),
                    new Answer("Tam gdzie kończy twoja wyobraźnia", true),
                    new Answer("Najpierw kultura, potem cała reszta", false),
                }),
                new Question("Zaznacz najdłuższą odpowiedź:", new List<Answer>
                {
                    new Answer("Własne mieszkanie pachnące nowością", false),
                    new Answer("Tak jakby cały dom należał do niego, co za dramatyczna muzyka", false),
                    new Answer("Pole pole łyse pole ale mam już plan pomalutku bez pośpiechu wszystko zrobię sam nad makietą się męczyłem ładnych parę lat ale za to zwiedzać cudo będzie cały świat", true),
                }),
            };

            QuestionsSet set = new QuestionsSet("TESTOWY SET YEAH!", qs, TimeSpan.FromHours(0.25));
            SaveLoadManager saveLoad = new SaveLoadManager(new XmlSerializer());
            saveLoad.Save(set, "", set.Name);
        }

        #region Command Callbacks

        private void ReturnToMenu()
        {
            timeDispatcher?.Stop();
            parent.SwitchToDefault();
        }

        private void FinishTest()
        {
            if (!IsTestFinished)
            {
                OnTestFinished();
                timeDispatcher?.Stop();
            }
            else
            {
                ShowResults();
            }
        }

        #endregion

    }
}