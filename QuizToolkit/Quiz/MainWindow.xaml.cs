using System;
using System.Windows;
using System.Windows.Controls;

namespace Quiz
{
    public partial class MainWindow : Window, INavigable
    {
        public MainWindow()
        {
            InitializeComponent();
            Switcher.pageSwitcher = this;
            Navigate(new MainMenu());
        }

        public void Navigate(UserControl newPage)
        {
            Content = newPage;
        }

        public void Navigate(UserControl nextPage, object state)
        {
            Content = nextPage;

            if (nextPage is ISwitchable s)
            {
                s.UtilizeState(state);
            }
            else
            {
                throw new ArgumentException($"NextPage {nextPage.Name} is not ISwitchable!");
            }
        }
    }

}
