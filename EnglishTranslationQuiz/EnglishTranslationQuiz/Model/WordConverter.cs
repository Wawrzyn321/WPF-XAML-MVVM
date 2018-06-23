using System;
using System.Collections.Generic;
using System.Text;
using Direction = MVVMTest2.Model.DataItem.TranslationDirection;

namespace MVVMTest2.Model
{
    /// <summary>
    /// Class converting loaded string of words (kept in
    /// StringBuilder) to dictionary of pairs (word, translation).
    /// </summary>
    public class WordConverter
    {
        private const char SEPARATOR = '-';
        private const char WHITE_SPACE = ' ';
        private readonly StringBuilder sb;

        public WordConverter()
        {
            sb = new StringBuilder();
        }

        //add new text
        public void Append(string text)
        {
            sb.Append(text);
        }

        //clear StringBuilder
        public void Clear()
        {
            sb.Length = 0;
            sb.Capacity = 0;
        }

        //convert string to pairs
        public Dictionary<string, string> GetWordsDictionary(Direction plToEng)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            //split StringBuilder into lines
            string[] listOfStrings = sb.ToString().Split(
                new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string str in listOfStrings)
            {
                int i = 0;
                //go through the entire string...
                while (i < str.Length)
                {
                    if (str[i] == WHITE_SPACE)
                    {
                        i++;
                        //finish
                        if (i >= str.Length)
                        {
                            break;
                        }
                        
                        if (str[i] == SEPARATOR)
                        {
                            if (i >= str.Length)
                            {
                                break;
                            }
                            else
                            {
                                if (str[i + 1] == WHITE_SPACE)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    i++;
                }
                if (i >= str.Length || i + 2 >= str.Length) continue;

                //get key and value based on translation direction
                string value, key;
                if (plToEng == Direction.PLToEng)
                {
                    value = str.Substring(0, i);
                    i += 2;
                    key = str.Substring(i);
                }
                else
                {
                    key = str.Substring(0, i);
                    i += 2;
                    value = str.Substring(i);
                }

                //try to add key and value to dictionary
                if (!dict.ContainsKey(key) && key != string.Empty)
                {
                    dict.Add(key, value);
                }
            }

            return dict;
        }

    }
}
