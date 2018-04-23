using System.Linq;

namespace TextSelectionInfo
{

    /// <summary>
    /// Simple helper class for text operations.
    /// </summary>
    public static class TextHelper
    {
        private static readonly char[] Vowels = {'a', 'o', 'i', 'e', 'u'};

        public static int GetVowelsCount(string str)
        {
            return str.ToLower().Count(c => Vowels.Contains(c));
        }

        public static double GetRatio(string selectedText, string entireText)
        {
            if (entireText.Length == 0)
            {
                return double.NaN;
            }
            else
            {
                return selectedText.Length / (double) entireText.Length;
            }
        }

    }
}
