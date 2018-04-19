using System;
using System.Collections.Generic;
using System.Linq;
using MVVMTest2.Model;

namespace MVVMTest2.ViewModel
{
    /// <summary>
    /// Container for quiz words as pairs of original
    /// word and its translation(s).
    /// 
    /// Handles counting the remaining words and getting
    /// next words for user.
    /// </summary>
    public class WordService
    {
        public Dictionary<string, string> RemainingWords { get; }

        public int LearnedWordsCount { get; private set; }
        public string CurrentWord { get; private set; }
        public string Translation { get; private set; }

        private KeyValuePair<string, string> currentPair;
        private readonly Random r = new Random();

        public WordService(DataItem item)
        {
            RemainingWords = item.Words;
            LearnedWordsCount = 0;

            PrepareNextPair();
        }
        
        //prepare next pair of word-translation for viewing
        private void PrepareNextPair()
        {
            currentPair = GetNextPair();
            CurrentWord = GetRandomWordChunk(currentPair.Key);
            Translation = "?";
        }

        //get next pair and try to get a different than the current
        private KeyValuePair<string, string> GetNextPair()
        {
            KeyValuePair<string, string> next;
            do
            {
                next = RemainingWords.ElementAt(r.Next(0, RemainingWords.Count));
            } while (next.Key == currentPair.Key && RemainingWords.Count > 1);
            return next;
        }

        //words on the right side that are divided with ',' can
        //be shown separatedly
        private string GetRandomWordChunk(string word)
        {
            //only one word, just return it
            if (!word.Contains(","))
            {
                return word;
            }
            else
            {
                //split words by commas and return random of them
                string[] wordsSplit = word.Split(',');
                int pos = r.Next(wordsSplit.Length);
                return wordsSplit[pos].Trim(' ');
            }
        }

        //remove pair from dictionary and prepare net pair
        public void SetCurrentWordAsLearned()
        {
            RemainingWords.Remove(CurrentWord);
            LearnedWordsCount++;

            PrepareNextPair();
        }

        //just prepare next pair of words
        public void RepeatCurrentWord()
        {
            PrepareNextPair();
        }

        //assign the {Translation} its true value
        public void ShowCurrentTranslation()
        {
            Translation = RemainingWords[currentPair.Key];
        }
    }
}
