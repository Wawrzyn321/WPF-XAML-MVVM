using System.ComponentModel;
using System.Runtime.CompilerServices;
using TextSelectionInfo.Annotations;

namespace TextSelectionInfo
{
    /// <summary>
    /// Model for 
    /// </summary>
    public class TextModel : INotifyPropertyChanged
    {
        #region Observed Properties

        private string tbText;
        public string TbText
        {
            get => tbText;
            set => Set(ref tbText, value);
        }

        private int selectionStartText;
        public int SelectionStartText
        {
            get => selectionStartText;
            set => Set(ref selectionStartText, value);
        }

        private int selectionLengthText;
        public int SelectionLengthText
        {
            get => selectionLengthText;
            set => Set(ref selectionLengthText, value);
        }

        private int vowelsCountText;
        public int VowelsCountText
        {
            get => vowelsCountText;
            set => Set(ref vowelsCountText, value);
        }

        private string selectedRatioText;
        public string SelectedRatioText
        {
            get => selectedRatioText;
            set => Set(ref selectedRatioText, value);
        }

        #endregion

        public TextModel()
        {
            SelectionStartText = 0;
            SelectionLengthText = 0;
            VowelsCountText = 0;
            SelectedRatioText = "0";
        }

        public void UpdateSelection(string text, int selectionStart, int selectionLength)
        {
            string selectedText = text.Substring(selectionStart, selectionLength);

            SelectionStartText = selectionStart;
            SelectionLengthText = selectionLength;
            VowelsCountText = TextHelper.GetVowelsCount(selectedText);
            SelectedRatioText = TextHelper.GetRatio(text, selectedText).ToString("P2");
        }

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
