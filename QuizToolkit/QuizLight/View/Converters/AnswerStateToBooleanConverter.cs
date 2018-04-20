using System;
using System.Globalization;
using System.Windows.Data;

namespace QuizLight.View
{
    /// <summary>
    /// Converts QuestionsState to boolean:
    ///     true only when state is "Unanswered".
    /// </summary>
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