using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using EnglishTranslationQuiz.ViewModel;

namespace EnglishTranslationQuiz.View.Converters
{
    /// <summary>
    /// A converter for "Show translation" button
    /// 
    /// The button should be Hidden unless we want to allow
    /// the user to see the translation.
    /// </summary>
    public class ShowingWordOnlyValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((QuizViewModel.QuizState)value)
            {
                case QuizViewModel.QuizState.ShowingWord:
                    return Visibility.Visible;
                default:
                    return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// A converter for choice buttons:
    ///     "I don't know
    ///     "Again"
    ///     "I know it"
    /// 
    /// They should be visible when the user sees the word
    /// with proper translation
    /// </summary>
    public class ChoiceValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((QuizViewModel.QuizState)value)
            {
                case QuizViewModel.QuizState.ShowingWordWithTranslation:
                    return Visibility.Visible;
                default:
                    return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}