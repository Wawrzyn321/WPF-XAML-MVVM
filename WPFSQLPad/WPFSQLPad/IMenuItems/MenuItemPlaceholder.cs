namespace WPFSQLPad.IMenuItems
{
    /// <summary>
    /// Placeholder used when there's no DBs connected yet.
    /// </summary>
    public sealed class MenuItemPlaceholder : IMenuItem
    {
        public bool IsChoosen { get; set; }
        public bool IsPlaceholder => true;
        public string Description { get; set; }

        public MenuItemPlaceholder()
        {
            Description = "No database choosen!";
        }
    }
}