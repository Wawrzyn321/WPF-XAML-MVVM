using System.Windows;
using System.Windows.Controls;

namespace TextSelectionInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TextModel textModel;

        public MainWindow()
        {
            InitializeComponent();
            textModel = new TextModel();
            DataContext = textModel;
        }

        private void TextBoxBase_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textModel.UpdateSelection(textBox.Text, textBox.SelectionStart, textBox.SelectionStart);
        }

    }
}
