using System;
using System.Globalization;
using System.Windows.Data;

namespace Quiz.View
{
    public class AnswerStateToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ViewQuestion.ViewQuestionState) value == ViewQuestion.ViewQuestionState.Unanswered;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}