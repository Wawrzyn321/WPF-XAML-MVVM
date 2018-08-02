using System.Collections.Generic;

namespace WPFSQLPad.TreeItems
{
    /// <summary>
    /// Description of DB column filled with properties
    /// from result of DESC-like query.
    /// </summary>
    public sealed class ColumnDescription
    {
        public const string CanBeNull_Yes = "YES";

        public string Name { get; set; }
        public string Type { get; set; }
        public bool CanBeNull { get; set; }
        public string Key { get; set; }
        public string Default { get; set; }
        public string Extra { get; set; }

        public ColumnDescription(string name, string type, bool canBeNull, string key, string defaultValue, string extra)
        {
            Name = name;
            Type = type;
            CanBeNull = canBeNull;
            Key = key;
            Default = defaultValue;
            Extra = extra;
        }

        public ColumnDescription(string name, string type, bool canBeNull)
        {
            Name = name;
            Type = type;
            CanBeNull = canBeNull;
        }

        public override string ToString()
        {
            //create a list of properties
            var columnProperties = new List<string>();
            if (!CanBeNull)
            {
                columnProperties.Add("NOT NULL");
            }
            if (!string.IsNullOrWhiteSpace(Key))
            {
                columnProperties.Add(Key);
            }
            if (!string.IsNullOrWhiteSpace(Default))
            {
                columnProperties.Add("Default: " + Default);
            }
            if (!string.IsNullOrWhiteSpace(Extra))
            {
                columnProperties.Add(Extra);
            }
            return $"{Name}: {Type} ({string.Join(", ", columnProperties)})";
        }
    }
}