using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;

namespace QuizLight.ViewModel
{
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