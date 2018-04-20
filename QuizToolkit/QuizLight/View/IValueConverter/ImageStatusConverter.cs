using System;
using System.Globalization;
using System.Windows.Data;
using State = Quiz.View.ViewQuestion.ViewQuestionState;

namespace Quiz.View
{
    public class ImageStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((State) value)
            {
                case State.Unanswered:
                    return "../Images/empty.png";
                case State.Correct:
                    return "../Images/tick.png";
                case State.Wrong:
                    return "../Images/cross.png";
            }
            throw new Exception("kurwa6");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
