using System;
using System.Windows.Input;

namespace ImageSelectionInfo.View
{
    public interface IView
    {
        event Action OnOpenButtonPressed;
        event Action<object, MouseButtonEventArgs> OnImageMouseDown;
        event Action<object, MouseEventArgs> OnImageMouseMove;
        event Action<object, MouseButtonEventArgs> OnImageMouseUp;
    }
}