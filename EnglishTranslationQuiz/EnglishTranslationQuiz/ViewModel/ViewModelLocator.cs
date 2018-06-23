using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace MVVMTest2.ViewModel
{
    /// <summary>
    /// Standard MVVM Light ViewModel Locator.
    /// </summary>
    public class ViewModelLocator
    {
        public IMainViewModel Main => ServiceLocator.Current.GetInstance<IMainViewModel>();

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IMainViewModel, MainViewModel>();
        }

        public static void Cleanup()
        {
        }
    }
}