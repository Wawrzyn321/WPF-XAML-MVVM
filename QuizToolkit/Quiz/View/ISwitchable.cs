using System.Windows.Controls;

namespace Quiz
{
    public interface ISwitchable
    {
        UserControl SwitchableParent { get; set; }

        void UtilizeState(object state);
    }
}
