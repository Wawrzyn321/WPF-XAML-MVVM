using NUnit.Framework;

namespace Model.Tests
{
    [TestFixture]
    public class SyntaxHighlighterTests
    {
        [Test]
        [TestCase(null)]
        public void Highlighter_DoesNotReturnNull(string testString)
        {
            //arrange
            ISyntaxHighlighter highlighter = new SyntaxHighlighter();

            //act
            var s = highlighter.GetHighlightableWords(testString);

            //assert
            Assert.That(s, Is.Not.Null);
        }

        [Test]
        [TestCase("SELECT * FROM Dane WHERE Name = 'Select anything'", 3)]
        public void Highlighter_ReturnWordCount_Return_Desired(string testString, int desiredWordsCount)
        {
            //arrange
            ISyntaxHighlighter highlighter = new SyntaxHighlighter();

            //act
            var s = highlighter.GetHighlightableWords(testString);

            //assert
            Assert.That(s.Count, Is.EqualTo(desiredWordsCount));
        }

        [Test]
        [TestCase("SELECT * FROM DANE")]
        public void Highlighter_AnyReturnIndices_AlwaysInStringRange(string testString)
        {
            //arrange
            ISyntaxHighlighter highlighter = new SyntaxHighlighter();

            //act
            var s = highlighter.GetHighlightableWords(testString);

            //assert
            foreach (var pair in s)
            {
                Assert.That(pair.Start + pair.Count, Is.LessThanOrEqualTo(testString.Length));
            }
        }
    }
}
