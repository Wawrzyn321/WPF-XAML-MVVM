using Microsoft.Win32;

namespace ImageSelectionInfo.ViewModel
{
    public static class OpenImageAgent
    {
        public static string OpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png",
                InitialDirectory = @"C:\Users\{Environment.UserName}\Pictures",
                Title = "Select an image"
            };

            bool? result = dialog.ShowDialog();
            if (result != null && result.Value)
            {
                return dialog.FileName;
            }

            return null;
        }
    }
}
