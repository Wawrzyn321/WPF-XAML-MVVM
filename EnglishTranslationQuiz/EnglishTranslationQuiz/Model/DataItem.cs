using System.Collections.Generic;

namespace MVVMTest2.Model
{
    /// <summary>
    /// Container for dictionary of
    /// words and their translations
    /// along with direction of translation:
    /// currently translation English to Polish
    /// is disabled.
    /// </summary>
    public class DataItem
    {
        public enum TranslationDirection
        {
            PLToEng,
            EngToPl,
        }

        public Dictionary<string, string> Words { get; }
        public TranslationDirection Direction { get; }

        public DataItem(Dictionary<string, string> words, TranslationDirection direction)
        {
            Words = words;
            Direction = direction;
        }

    }
}