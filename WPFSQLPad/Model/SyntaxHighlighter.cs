using System.Collections.Generic;
using System.Diagnostics;

//W.I.P.
namespace Model
{
    public struct IndicesPair
    {
        public int Start { get; set; }
        public int Count { get; set; }
    }

    public class SyntaxHighlighter : ISyntaxHighlighter
    {

        private List<string> Keywords = new List<string>
        {
            "SELECT", "CREATE", "UPDATE"
        };

        private readonly List<char> Quotations = new List<char>{'\'', '\"'};


        public List<IndicesPair> GetHighlightableWords(string s)
        {
            var list = new List<IndicesPair>();
            if (s == null)
            {
                return list;
            }

            bool isInsideQuotation = false;
            for (int i = 0; i < s.Length; i++)
            {
                int indexOfQuot = s.IndexOf('\'', i);
                int indexSpace = s.IndexOf(' ', i);
                if (isInsideQuotation) indexSpace = -1;

                if (indexSpace < indexOfQuot && indexSpace != -1)
                {
                    Debug.WriteLine("of space <" +s.Substring(i, indexSpace - i) + "> "+i);
                    i += indexSpace - i;
                }
                else if (indexOfQuot != -1)
                {
                    Debug.WriteLine("of quot <" + s.Substring(i, indexOfQuot - i) + "> " + i);
                    i += indexOfQuot - i;
                    Debug.WriteLine(isInsideQuotation ? "end" : "start");
                    isInsideQuotation = !isInsideQuotation;
                }
                else { Debug.WriteLine("k: "+i);}
            }

            return list;
        }

        private List<string> GetTokenizableParts(string s)
        {
            bool isInsideQuotation = false;
            int startIndex = 0;
            var parts = new List<string>();

            for (int i = 0; i < s.Length; i++)
            {
                if (Quotations.Contains(s[i]))
                {
                    if (isInsideQuotation)
                    {
                        isInsideQuotation = false;
                        startIndex = i + 1;
                    }
                    else
                    {
                        parts.Add(s.Substring(startIndex, i - startIndex));
                        isInsideQuotation = true;
                    }
                }
            }

            return parts;
        }
    }
}
