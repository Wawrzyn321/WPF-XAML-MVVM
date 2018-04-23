using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ImageSelectionInfo.View;
using Microsoft.Win32;

namespace ImageSelectionInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        public event Action OnOpenButtonPressed;

        public event Action<object, MouseButtonEventArgs> OnImageMouseDown;
        public event Action<object, MouseEventArgs> OnImageMouseMove;
        public event Action<object, MouseButtonEventArgs> OnImageMouseUp;

        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new ViewModel.ViewModel(this);
            DataContext = viewModel.SelectionModel;
        }

        private void ButtonOpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OnOpenButtonPressed?.Invoke();
        }

        private void Image_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            OnImageMouseDown?.Invoke(sender, e);
        }

        private void Image_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            OnImageMouseUp?.Invoke(sender, e);
        }

        private void Image_OnMouseMove(object sender, MouseEventArgs e)
        {
            OnImageMouseMove?.Invoke(sender, e);
        }

    }
}
