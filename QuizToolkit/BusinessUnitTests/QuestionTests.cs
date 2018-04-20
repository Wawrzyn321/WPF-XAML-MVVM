using System.Collections.Generic;
using Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusinessUnitTests
{
    [TestClass]
    public class QuestionTests
    {
        [TestMethod]
        public void TestBadIndices()    
        {
            Question q1 = new Question("t1", new List<Answer>
            {
                new Answer("a11", true),
                new Answer("a12", true),
                new Answer("a13", true),
                new Answer("a14", false),
            });
            q1.Answers[0].IsChecked = false;
            q1.Answers[1].IsChecked = true;
            q1.Answers[2].IsChecked = false;
            q1.Answers[3].IsChecked = true;

            q1.Verify(out List<int> list);
            Assert.IsTrue(list[0] == 0 && list[1] == 2 && list[2] == 3);
        }
    }
}
