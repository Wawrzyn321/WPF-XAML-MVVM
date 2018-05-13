using System.Collections.Generic;

namespace Model
{
    public interface ISyntaxHighlighter
    {
        List<IndicesPair> GetHighlightableWords(string s);
    }
}