using System.Windows;
using System.Windows.Input;
using ImageSelectionInfo.Model;
using ImageSelectionInfo.View;
using Microsoft.Win32;

namespace ImageSelectionInfo.ViewModel
{
    public class ViewModel
    {
        public RectangleSelectionModel SelectionModel { get; }

        public ViewModel(IView view)
        {
            SelectionModel = new RectangleSelectionModel();

            view.OnOpenButtonPressed += OpenButtonPress;
            view.OnImageMouseUp += MouseUp;
            view.OnImageMouseMove += MouseMove;
            view.OnImageMouseDown += MouseDown;
        }

        #region Events Relay

        private void OpenButtonPress()
        {
            string path = OpenImageAgent.OpenFile();
            if (path != null)
            {
                SelectionModel.ImagePath = path;
            }
        }

        private void MouseUp(object sender, MouseButtonEventArgs args)
        {
            SelectionModel.StopDragging();
        }

        private void MouseMove(object sender, MouseEventArgs args)
        {
            var image = (FrameworkElement)sender;
            Point position = args.GetPosition(image.Parent as IInputElement);
            SelectionModel.ContinueDragging(position);
        }

        private void MouseDown(object sender, MouseButtonEventArgs args)
        {
            var image = (FrameworkElement)sender;
            Point position = args.GetPosition(image.Parent as IInputElement);
            SelectionModel.StartDragging(position);
        } 

        #endregion
    }
}
