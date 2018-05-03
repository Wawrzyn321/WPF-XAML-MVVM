using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// Description of DB column filled with properties
    /// from result of DESC query.
    /// </summary>
    public class ColumnDescription : ImplementsPropertyChanged
    {
        public const string CanBeNull_Yes = "YES";
        public const string CanBeNull_No = "NO";

        private string name;
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        private string type;
        public string Type
        {
            get => type;
            set => Set(ref type, value);
        }

        private bool canBeNull;
        public bool CanBeNull
        {
            get => canBeNull;
            set => Set(ref canBeNull, value);
        }

        private string key;
        public string Key
        {
            get => key;
            set => Set(ref key, value);
        }

        private string defaultValue;
        public string Default
        {
            get => defaultValue;
            set => Set(ref defaultValue, value);
        }

        private string extra;
        public string Extra
        {
            get => extra;
            set => Set(ref extra, value);
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