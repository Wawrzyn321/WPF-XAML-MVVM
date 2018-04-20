using GalaSoft.MvvmLight;

namespace QuizLight.ViewModel
{
    /// <summary>
    /// Interface for class responsible for
    /// switching between views
    /// </summary>
    public interface ISwitch
    {
        //switch to given ViewModelBase
        void SwitchTo(ViewModelBase viewModel);

        //switch to default ViewModelBase (definied in concrete class)
        void SwitchToDefault();
    }
}