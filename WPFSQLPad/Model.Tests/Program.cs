using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Tests
{
    class Program
    {
        static int Main(string[] args)
        {
            
            ISyntaxHighlighter highlighter = new SyntaxHighlighter();
            var s = highlighter.GetHighlightableWords("1 '4 6' 9 Name = \"?\"");

            return 0;
        }
    }
}
