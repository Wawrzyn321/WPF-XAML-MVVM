namespace Model
{
    /// <summary>
    /// Placeholder used when there's no DBs connected yet.
    /// </summary>
    public class MenuItemPlaceholder : ImplementsPropertyChanged, IMenuItem
    {
        #region IMenuItem Members

        private bool isChoosen;
        public bool IsChoosen
        {
            get => isChoosen;
            set => Set(ref isChoosen, value);
        }

        public bool IsPlaceholder => true;
        public string Description { get; set; }

        #endregion

        public MenuItemPlaceholder()
        {
            Description = "No database choosen!";
        }
    }
}