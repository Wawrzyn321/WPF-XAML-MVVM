using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace QuizLight.View
{
    /// <summary>
    /// Converts QuestionState to corresponding
    /// image.
    /// </summary>
    public class ImageStatusConverter : IValueConverter
    {
        private const string path = "../Graphics";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ViewQuestion.ViewQuestionState)value)
            {
                case ViewQuestion.ViewQuestionState.Unanswered:
                    return Path.Combine(path, "empty.png");
                case ViewQuestion.ViewQuestionState.Correct:
                    return Path.Combine(path, "tick.png");
                case ViewQuestion.ViewQuestionState.Wrong:
                    return Path.Combine(path, "cross.png");
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, "ISC::Convert");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
