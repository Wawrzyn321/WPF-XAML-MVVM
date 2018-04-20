using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Business;
using Microsoft.Win32;
using Quiz.Model;

namespace Quiz
{
	public partial class MainMenu : UserControl, ISwitchable
	{

	    public UserControl SwitchableParent { get; set; }

        public MainMenu()
		{
			InitializeComponent();

		    SwitchableParent = null;
		}

	    public void UtilizeState(object state)
        {
            Debug.WriteLine("MainMenu::Navigate");
        }

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

	    private void OpenQuiz()
	    {
	        string path = TryGetTestPath();


	        if (path != null)
	        {
                try
	            {
	                QuestionsSet quesionSet = QuizModel.LoadQuestionsSet(path);
	                Switcher.Switch(new QuizPage(quesionSet, this));
                }
	            catch (InvalidOperationException)
	            {
	                MessageBox.Show("Test file invalid!", "Test");
	            }
	        }
	    }

	    private static void CreateQuiz()
	    {
            try
            {
                // Uruchamianie programu kreatora.
                Process.Start("..\\..\\..\\Creator\\bin\\Debug\\Creator.exe");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            //MessageBox.Show("tu bd edytor", "Quizzical");
	    }

        #region Events

        private void btnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            OpenQuiz();
        }

        private void btnCreateQuiz_Click(object sender, RoutedEventArgs e)
        {
            CreateQuiz();
        }

        #endregion
    }
}