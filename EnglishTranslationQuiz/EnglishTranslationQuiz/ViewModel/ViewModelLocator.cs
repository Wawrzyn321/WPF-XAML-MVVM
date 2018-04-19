using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace MVVMTest2.ViewModel
{
    /// <summary>
    /// Standard MVVM Light ViewModel Locator.
    /// </summary>
    public class ViewModelLocator
    {
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public static void Cleanup()
        {
        }
    }
}