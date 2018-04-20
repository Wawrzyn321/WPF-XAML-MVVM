using System;
using System.Windows;
using System.Windows.Controls;
using Business;

namespace Quiz
{
    public class AnswerDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement frameworkElement = container as FrameworkElement;
            Question q = item as Question;
            if (frameworkElement != null && q != null)
            {
                if (q.IsMultipleChoice)
                {
                    return frameworkElement.FindResource("CheckboxDataTemplate") as DataTemplate;
                }
                else
                {
                    return frameworkElement.FindResource("RadioDataTemplate") as DataTemplate;
                }
            }
            throw new Exception("kurwa3");
        }
    }
}
