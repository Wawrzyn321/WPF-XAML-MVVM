using System.Windows.Input;

namespace MVVMTest2.ViewModel
{
    public interface IStartupViewModel
    {
        int AllFilesToLoadCount { get; set; }
        int CurrentFileIndex { get; set; }
        bool IsLoading { get; set; }
        string LoadingText { get; }
        string MainWindowTitle { get; }
        ICommand OpenDirectoryCommand { get; set; }
        string OpenDirectoryText { get; }
        ICommand OpenFileCommand { get; set; }
        string OpenFileText { get; }
    }
}