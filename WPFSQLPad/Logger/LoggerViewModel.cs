using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Logger
{
    public class LoggerViewModel : INotifyPropertyChanged
    {
        #region Observed Properties

        private string log;
        public string Log
        {
            get => log;
            set => Set(ref log, value);
        }

        #endregion

        #region Commands

        public ICommand ClearLogCommand { get; private set; }
        public ICommand CopyLogCommand { get; private set; }

        #endregion

        private readonly LogContainer logContainer;

        public LoggerViewModel()
        {
            InitializeCommands();
            logContainer = new LogContainer();
        }

        private void InitializeCommands()
        {
            ClearLogCommand = new ActionCommand(Clear);
            CopyLogCommand = new ActionCommand(CopyToClipboard);
        }

        public void Write(string text, int indent = 0)
        {
            logContainer.Write(text, indent);
            Flush();
        }

        public void WriteLine(string text, int indent = 0)
        {
            logContainer.WriteLine(text, indent);
            Flush();
        }

        public void Clear()
        {
            logContainer.Clear();
            Flush();
        }

        public void CopyToClipboard()
        {
            Clipboard.SetText(logContainer.Content);
            WriteLine("Log has been copied to clipboard.");
            Flush();
        }

        private void Flush()
        {
            Log = logContainer.Content;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool Set<T>(ref T oldValue, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(oldValue, value))
            {
                return false;
            }
            else
            {
                oldValue = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }

        #endregion
    }
}
