using GalaSoft.MvvmLight;
using Creator.Model;
using Creator.Model.Implementation;
using Business;
using Creator.Model.Contract;
using Creator.ViewModel.Contract;

namespace Creator.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, ISwitchToEditor
    {
        private readonly IDataService _dataService;
        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            private set { Set(ref _currentViewModel, value); }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }
                });

            LoadStartupView();
            //LoadEditorView();
        }

        public void LoadStartupView()
        {
            switchView(new StartupViewModel(this, createSaveLoadAgent()));
        }

        public void LoadEditorView(QuestionsSet questionSet)
        {
            switchView(new EditorViewModel(createSaveLoadAgent(), questionSet));
        }

        private void switchView(ViewModelBase viewModel)
        {
            CurrentViewModel = viewModel;
        }

        private ISaveLoadAgent createSaveLoadAgent()
        {
            var saveLoadAgent = new ModalSaveLoadAgent();
            return saveLoadAgent;
        }
    }
}