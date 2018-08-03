using System.Windows.Controls;
using Logger;

namespace QueryTabControl
{
    /// <summary>
    /// Interaction logic for QueryTabControl.xaml
    /// </summary>
    public partial class QueryTabView : UserControl
    {
        public QueryTabView(LoggerViewModel logger)
        {
            InitializeComponent();
            TabController = new TabController(logger);
            DataContext = TabController;
        }

        public TabController TabController { get; set; }
    }
}
