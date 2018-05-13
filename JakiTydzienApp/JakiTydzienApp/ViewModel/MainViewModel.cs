using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using GalaSoft.MvvmLight;
using JakiTydzienApp.Model;

namespace JakiTydzienApp.ViewModel
{
    /// <summary>
    /// ViewModel with one sole responsibility - to load data from DataService
    /// and 
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Observed Properties

        private string weekTypeText;
        public string WeekTypeText
        {
            get => weekTypeText;
            set => Set(ref weekTypeText, value);
        }

        private string weekDetailsText;
        public string WeekDetailsText
        {
            get => weekDetailsText;
            set => Set(ref weekDetailsText, value);
        }

        private string sundayTypeText;
        public string SundayTypeText
        {
            get => sundayTypeText;
            set => Set(ref sundayTypeText, value);
        }

        #endregion

        public MainViewModel(IDataService dataService)
        {
            WeekTypeText = "Wczytywanie...";

            dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        FillTextsWithError(error.InnerException?.Message ?? error.Message);
                        return;
                    }
                    FillTextsWithData(item);
                });
        }

        private void FillTextsWithError(string errorMessage)
        {
            WeekTypeText = "Wystąpił błąd!";
            WeekDetailsText = errorMessage;
            SundayTypeText = ":(";
        }

        private void FillTextsWithData(Data data)
        {
            WeekTypeText = data.tydzien;
            WeekDetailsText = data.details;
            SundayTypeText = "Niedziela " + data.niedziela;
        }

    }
}