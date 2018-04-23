using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using ImageSelectionInfo.Properties;

namespace ImageSelectionInfo.Model
{
    public class RectangleSelectionModel : INotifyPropertyChanged
    {
        #region Observed Properties

        private double widthh;
        public double Widthh
        {
            get => widthh;
            set => Set(ref widthh, value);
        }

        private double heighht;
        public double Heighht
        {
            get => heighht;
            set => Set(ref heighht, value);
        }

        private DateTime date;
        public DateTime Date
        {
            get => date;
            set => Set(ref date, value);
        }

        private double endX;
        public double EndX
        {
            get => endX;
            set => Set(ref endX, value);
        }

        private double endY;
        public double EndY
        {
            get => endY;
            set => Set(ref endY, value);
        }

        private double startX;
        public double StartX
        {
            get => startX;
            set => Set(ref startX, value);
        }

        private double startY;
        public double StartY
        {
            get => startY;
            set => Set(ref startY, value);
        }

        private double area;
        public double Area
        {
            get => area;
            set => Set(ref area, value);
        }

        private string imagePath;
        public string ImagePath
        {
            get => imagePath;
            set
            {
                Set(ref imagePath, value);
                ResetValues();
            }
        }

        #endregion

        private bool isDragging;
        private DateTime StartDate => new DateTime(1900, 1, 1);

        private double originalX;
        private double originalY;

        public RectangleSelectionModel()
        {
            Area = 0;
            Date = StartDate;
        }

        private void ResetValues()
        {
            SetStart(0, 0);
            SetSize(0, 0);

            EndX = 0;
            EndX = 0;

            isDragging = false;
        }

        #region Mouse Dragging Handlers

        public void StartDragging(Point position)
        {
            StartX = position.X;
            StartY = position.Y;
            EndX = position.X;
            EndY = position.Y;
            originalX = StartX;
            originalY = StartY;
            SetStart(StartX, StartY);
            SetSize(0, 0);
            isDragging = true;
        }

        public void StopDragging()
        {
            isDragging = false;
        }

        public void ContinueDragging(Point position)
        {
            if (isDragging)
            {
                StartX = Math.Min(position.X, originalX);
                StartY = Math.Min(position.Y, originalY);
                EndX = Math.Max(position.X, originalX);
                EndY = Math.Max(position.Y, originalY);
                SetStart(StartX, StartY);
                SetSize(EndX - StartX, EndY - StartY);
            }
        }

        #endregion

        #region Setting Transform

        private void SetSize(double _width, double _height)
        {
            Widthh = _width;
            Heighht = _height;
            Area = Widthh * Heighht;
            Date = StartDate.AddDays(Area);
        }

        private void SetStart(double x, double y)
        {
            StartX = x;
            StartY = y;
        }

        #endregion

        #region INotifyPropertyChanged Members

        private bool Set<T>(ref T s, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(s, value))
            {
                return false;
            }
            else
            {
                s = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

}
