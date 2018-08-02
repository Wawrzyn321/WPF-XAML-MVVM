using System.ComponentModel;

namespace WPFSQLPad.ViewModel
{
    public interface ILogRecipient : INotifyPropertyChanged
    {
        string Log { get; set; }
    }
}