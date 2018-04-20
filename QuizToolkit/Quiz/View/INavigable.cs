using System.Windows.Controls;

namespace Quiz
{
    public interface INavigable
    {
        void Navigate(UserControl newPage);

        void Navigate(UserControl nextPage, object state);
    }
}