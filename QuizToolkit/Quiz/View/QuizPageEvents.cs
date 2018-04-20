using System.Windows;
using System.Windows.Input;

namespace Quiz
{
    public partial class QuizPage
    {
        private void lbLeft_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = lb_Left.SelectedIndex;
            bool isIndexInRange = index >= 0 && index < lb_Right.Items.Count;
            if (isIndexInRange)
            {
                lb_Right.ScrollIntoView(lb_Right.Items[index]);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            timeDispatcher?.Stop();
            Switcher.Switch(SwitchableParent);
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
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

    }
}
