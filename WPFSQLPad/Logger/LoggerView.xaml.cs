using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Logger
{
    /// <summary>
    /// Interaction logic for LoggerView.xaml
    /// </summary>
    public partial class LoggerView : UserControl
    {
        public readonly LoggerViewModel Logger;

        public LoggerView()
        {
            InitializeComponent();
            Logger = new LoggerViewModel();
            DataContext = Logger;
        }

    }


}
