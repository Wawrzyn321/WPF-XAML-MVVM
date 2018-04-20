using System.Windows.Controls;
using System.Windows.Input;

namespace QuizLight.View
{
    /// <summary>
    /// Interaction logic for QuizView.xaml
    /// </summary>
    public partial class QuizView : UserControl
    {
        public QuizView()
        {
            InitializeComponent();
        }

        //scroll to selected item
        private void lvLeft_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = lv_Left.SelectedIndex;
            bool isIndexInRange = index >= 0 && index < lv_Right.Items.Count;
            if (isIndexInRange)
            {
                lv_Right.ScrollIntoView(lv_Right.Items[index]);
            }
        }
    }

}
