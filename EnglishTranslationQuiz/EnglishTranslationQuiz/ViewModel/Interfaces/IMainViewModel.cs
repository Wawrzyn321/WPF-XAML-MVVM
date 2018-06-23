using GalaSoft.MvvmLight;

namespace MVVMTest2.ViewModel
{
    public interface IMainViewModel : ISwitch
    {
        ViewModelBase CurrentViewModel { get; set; }
        string WindowTitle { get; }
    }
}