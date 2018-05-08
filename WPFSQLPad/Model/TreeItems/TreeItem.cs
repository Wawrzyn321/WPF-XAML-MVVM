namespace Model.TreeItems
{
    public abstract class TreeItem
    {
        public DatabaseConnection ConnectionReference { get; protected set; }

        protected TreeItem(DatabaseConnection connectionReference)
        {
            ConnectionReference = connectionReference;
        }
    }
}