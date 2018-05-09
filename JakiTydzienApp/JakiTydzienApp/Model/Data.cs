using System;

namespace JakiTydzienApp.Model
{
    [Serializable]
    public class Data
    {
        public string tydzien { get; set; }
        public string details { get; set; }
        public long expires { get; set; }
        public string niedziela { get; set; }
    }

}
