using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MVVMTest2.Model;

namespace MVVMTest2.ViewModel
{
    /// <summary>
    /// Simple ViewModel with title and two buttons:
    /// one for opening a file, second for opening
    /// an entire directory.
    /// </summary>
    public class StartupViewModel : ViewModelBase, IStartupViewModel
    {
        #region Static Texts

        public string MainWindowTitle => Properties.Resources.Title;
        public string OpenFileText => Properties.Resources.Startup_OpenFile;
        public string OpenDirectoryText => Properties.Resources.Startup_OpenDirectory;
        public string LoadingText => Properties.Resources.Startup_Loading;

        #endregion

        #region Commands

        public ICommand OpenFileCommand { get; set; }
        public ICommand OpenDirectoryCommand { get; set; }

        #endregion

        #region Observed Properties

        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set => Set(ref isLoading, value);
        }

        private int currentlyLoadedFileIndex;
        public int CurrentFileIndex
        {
            get => currentlyLoadedFileIndex;
            set => Set(ref currentlyLoadedFileIndex, value);
        }

        private int allFilesToLoadCount;
        public int AllFilesToLoadCount
        {
            get => allFilesToLoadCount;
            set => Set(ref allFilesToLoadCount, value);
        }

        #endregion

        private readonly ISwitch parent;
        private readonly IDataService dataService;
        private string quizName;

        public StartupViewModel(ISwitch parent)
        {
            this.parent = parent;

            dataService = new LocalFileDataService();

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            OpenFileCommand = new RelayCommand(OpenFile);
            OpenDirectoryCommand = new RelayCommand(OpenDirectory);
        }

        private void OnLoadingFinished(Task<DataItem> obj)
        {
            IsLoading = false;
            RunQuiz(obj.Result, quizName);
        }

        private void RunQuiz(DataItem item, string path)
        {
            //the quiz shouldn't be empty...
            if (item.Words.Count > 0)
            {
                parent.SwitchTo(new QuizViewModel(parent, item, path));
            }
            else
            {
                MessageBox.Show(Properties.Resources.Startup_NoWordsFound, MainWindowTitle);
            }
        }

        #region Command Callbacks

        private void OpenFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Text|*.txt" })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    quizName = openFileDialog.SafeFileName;
                    IsLoading = true;
                    Task<DataItem> openingTheFile = OpenFileAsync(openFileDialog.FileName);
                    openingTheFile.ContinueWith(OnLoadingFinished);
                }
            }
        }

        private void OpenDirectory()
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    quizName = folderBrowserDialog.SelectedPath;
                    IsLoading = true;
                    Task<DataItem> openingTheFiles = OpenDirectoryAsync(folderBrowserDialog.SelectedPath);
                    openingTheFiles.ContinueWith(OnLoadingFinished);
                }
            }
        }

        //open single file and load words
        private async Task<DataItem> OpenFileAsync(string path)
        {
            AllFilesToLoadCount = CurrentFileIndex = 1;
            return await dataService.GetData(path, DataItem.TranslationDirection.PLToEng);
        }

        //search the directory for files
        private async Task<DataItem> OpenDirectoryAsync(string path)
        {
            List<string> paths = new List<string>();
            foreach (string file in Directory.EnumerateFiles(path))
            {
                if (Path.GetExtension(file) == ".txt")
                {
                    paths.Add(file);
                }
            }
            AllFilesToLoadCount = paths.Count;
            return await dataService.GetData(paths, DataItem.TranslationDirection.PLToEng, (index, count) => CurrentFileIndex = index);
        }

        #endregion
    }
}
