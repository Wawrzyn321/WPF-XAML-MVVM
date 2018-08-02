namespace WPFSQLPad.IMenuItems
{
    /// <summary>
    /// Contract for MenuItem objects.
    /// Used in Menu control.
    /// </summary>
    public interface IMenuItem
    {
        bool IsChoosen { get; set; }
        bool IsPlaceholder { get; }
        string Description { get; set; }
    }
}