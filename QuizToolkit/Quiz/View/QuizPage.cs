using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Business;
using Quiz.Model;
using Quiz.View;
using Quiz.ViewModel;

namespace Quiz
{
    public partial class QuizPage : UserControl, ISwitchable
    {
        public bool IsTestFinished { get; private set; }
        public int CorrectQuestionsCount { get; private set; }
        public UserControl SwitchableParent { get; set; }

        private readonly QuestionsSet questionsSet;
        private ExternalTimeDispatcher timeDispatcher;

		public QuizPage(QuestionsSet questionsSet, UserControl parent)
        {
            InitializeComponent();
            this.questionsSet = questionsSet;
            SwitchableParent = parent;

            tbQuizTitle.Text = questionsSet.Name;
            if (questionsSet.HasTimeLimit)
            {
                InitTimeDispatcher(questionsSet.TimeLimit);
            }
            else
            {
                tbTimer.Text = string.Empty;
            }

            InitializeItemsSource(questionsSet);

            //CreateTestSet();

            IsTestFinished = false;
        }

        private void InitializeItemsSource(QuestionsSet questionsSet)
        {
            var questions = new ObservableCollection<ViewQuestion>();
            foreach (Question question in questionsSet.Questions)
            {
                questions.Add(ViewQuestion.FromQuestion(question));
            }
            lb_Right.ItemsSource = questions;
            lb_Left.ItemsSource = questions;
        }

        private void InitTimeDispatcher(int timeout)
        {
            timeDispatcher = new ExternalTimeDispatcher(timeout);
            timeDispatcher.OnTimeout += OnTimeout;
            timeDispatcher.OnTick += OnTick;
            OnTick(timeout);
        }

        private void OnTick(int remainingSeconds)
        {
            TimeSpan remainingTime = TimeSpan.FromSeconds(remainingSeconds);
            tbTimer.Text = $"{remainingTime:hh}:{remainingTime:mm}:{remainingTime:ss}";
        }

        private void OnTimeout()
        {
            tbTimer.Text = "Time out!";
            OnTestFinished();
        }

        public void UtilizeState(object state)
        {
        }

        private void ShowResults()
        {
            double score = Math.Round(100.0 * CorrectQuestionsCount / questionsSet.Questions.Count, 2);
            MessageBox.Show($"Your score is {score} %.");
        }

        private void OnTestFinished()
        {
            if (IsTestFinished) return;

            IsTestFinished = true;
            btnRight.Content = "Show results";
            for (int i = 0; i < lb_Right.Items.Count; i++)
            {
                ViewQuestion item = (ViewQuestion)lb_Right.Items[i];
                Question question = questionsSet.Questions[i];
                for (int j = 0; j < item.Answers.Count; j++)
                {
                    ViewAnswer answer = item.Answers[j];
                    question.Answers[j].IsChecked = answer.IsChecked;
                    item.Answers[j].ShouldBeChecked = question.Answers[j].IsValid;
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

        private static void CreateTestSet()
        {
            //var qs = new List<Question>
            //{
            //    new Question("Here's question one", new List<Answer> { new Answer("YES", true), new Answer("NO", false)}),
            //    new Question("That's gonna do", new List<Answer> { new Answer("odpowiedź a", true), new Answer("odpowiedź b", true)}),
            //    new Question("Heheszki", new List<Answer> { new Answer("nie ma innych odpowiedzi, ale jest fajnie", false)}),
            //    new Question("Ruhu huhu hu", new List<Answer> { new Answer("A", false), new Answer("b", false)}),
            //    new Question("Nie ma to jak gromnica", new List<Answer> { new Answer("Tak", true), new Answer("NIGDY W ŻYCIU", false), new Answer("no spoko", true)}),
            //    new Question("Gdzie zaczyna się mój świat?", new List<Answer>
            //    {
            //        new Answer("Tam gdzie słońce nie sięga", false),
            //        new Answer("W miejscu do którego należę", false),
            //        new Answer("Tam gdzie kończy twoja wyobraźnia", true),
            //        new Answer("Najpierw kultura, potem cała reszta", false),
            //    }),
            //    new Question("Zaznacz najdłuższą odpowiedź:", new List<Answer>
            //    {
            //        new Answer("Własne mieszkanie pachnące nowością", false),
            //        new Answer("Tak jakby cały dom należał do niego, co za dramatyczna muzyka", false),
            //        new Answer("Pole pole łyse pole ale mam już plan pomalutku bez pośpiechu wszystko zrobię sam nad makietą się męczyłem ładnych parę lat ale za to zwiedzać cudo będzie cały świat", true),
            //    }),
            //};

            //QuestionsSet set = new QuestionsSet("TESTOWY SET YEAH!", qs, true, TimeSpan.FromHours(0.25));
            //Debug.WriteLine(set.TimeLimit);
            //SaveLoadManager saveLoad = new SaveLoadManager(new XmlSerializer());
            //saveLoad.Save(set, "", set.Name);}
        }

    }

}