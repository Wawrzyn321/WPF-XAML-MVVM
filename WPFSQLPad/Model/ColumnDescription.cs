using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// Description of DB column filled with properties
    /// from result of DESC-like query.
    /// </summary>
    public class ColumnDescription : TreeItem
    {
        public const string CanBeNull_Yes = "YES";

        public string Name { get; set; }
        public string Type { get; set; }
        public bool CanBeNull { get; set; }
        public string Key { get; set; }
        public string Default { get; set; }
        public string Extra { get; set; }

        public ColumnDescription(string name, string type, bool canBeNull, string key, string defaultValue, string extra, DatabaseConnection connection)
            : base(connection)
        {
            Name = name;
            Type = type;
            CanBeNull = canBeNull;
            Key = key;
            Default = defaultValue;
            Extra = extra;
        }

        public ColumnDescription(string name, string type, bool canBeNull, DatabaseConnection connection)
            : base(connection)
        {
            Name = name;
            Type = type;
            CanBeNull = canBeNull;
        }

        public override string ToString()
        {
            //create a list of properties
            List<string> list = new List<string>();
            if (!CanBeNull)
            {
                list.Add("NOT NULL");
            }
            if (Key != string.Empty)
            {
                list.Add(Key);
            }
            if (Default != string.Empty)
            {
                list.Add("Default: " + Default);
            }
            if (Extra != string.Empty)
            {
                list.Add(Extra);
            }
            return $"{Name}: {Type} ({string.Join(", ", list)})";
        }
    }
}