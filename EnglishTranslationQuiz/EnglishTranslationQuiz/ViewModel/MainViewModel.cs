﻿using EnglishTranslationQuiz.ViewModel.Interfaces;
using GalaSoft.MvvmLight;

namespace EnglishTranslationQuiz.ViewModel
{
    /// <summary>
    /// Entry ViewModel, uses ISwitch to swap the UserControls
    /// </summary>
    public class MainViewModel : ViewModelBase, IMainViewModel
    {

        public string WindowTitle => Properties.Resources.Title;

        private ViewModelBase currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => currentViewModel;
            set => Set(ref currentViewModel, value);
        }

        private readonly ViewModelBase startupViewModel;

        public MainViewModel()
        {
            startupViewModel = new StartupViewModel(this);
            SwitchTo(startupViewModel);
        }
        
        #region ISwitch Member

        public void SwitchTo(ViewModelBase viewModel)
        {
            CurrentViewModel = viewModel;
        }

        public void SwitchToDefault()
        {
            SwitchTo(startupViewModel);
        }

        #endregion

    }
}
