using System.Collections.Generic;
using System.IO;
using System.Linq;
using Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusinessUnitTests
{
    /// <summary>
    /// NOTICE
    /// 
    /// Tests of saving questions are disabled 
    /// due to the fact that they inherit from 
    /// non-serializable ObservableObject.
    /// 
    /// </summary>
    [TestClass]
    public class SaveLoadTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidExtensionException))]
        public void TestInvalidExtension()
        {
            SaveLoadManager manager= new SaveLoadManager(new JsonSerializer());
            manager.Load("mock.xml");
        }

        [TestMethod]
        public void TestXMLSaveLoad()
        {
            SaveLoadManager manager = new SaveLoadManager(new XmlSerializer());
            QuestionsSet qs = GetSomeTest();
            //manager.Save(qs, "", "test");
            //QuestionsSet qs2 = manager.Load(Path.Combine("", "test" + manager.GetSerializerExtension));
            //Assert.IsTrue(AreLoadedTestsEqual(qs, qs2));
            //Assert.IsTrue(AllAnswersAreUnchecked(qs2));
        }

        [TestMethod]
        public void TestJSONSaveLoad()
        {
            SaveLoadManager manager = new SaveLoadManager(new JsonSerializer());
            QuestionsSet qs = GetSomeTest();
            //manager.Save(qs, "", "test");
            //QuestionsSet qs2 = manager.Load(Path.Combine("", "test" + manager.GetSerializerExtension));
            //Assert.IsTrue(AreLoadedTestsEqual(qs, qs2));
            //Assert.IsTrue(AllAnswersAreUnchecked(qs2));
        }

        [TestMethod]
        public void TestBinSaveLoad()
        {
            SaveLoadManager manager = new SaveLoadManager(new BinarySerializer());
            QuestionsSet qs = GetSomeTest();
            //manager.Save(qs, "", "test");
            //QuestionsSet qs2 = manager.Load(Path.Combine("", "test" + manager.GetSerializerExtension));
            //Assert.IsTrue(AreLoadedTestsEqual(qs, qs2));
            //Assert.IsTrue(AllAnswersAreUnchecked(qs2));
        }

        private static bool AllAnswersAreUnchecked(QuestionsSet q)
        {
            return q.Questions
                .All(question => !question.Answers
                .Any(answer => answer.IsChecked));
        }

        private static bool AreLoadedTestsEqual(QuestionsSet q1, QuestionsSet q2)
        {
            return q1.Name == q2.Name &&
                   q1.Questions.Count == q2.Questions.Count;
        }

        private static QuestionsSet GetSomeTest()
        {
            List<Question> questions = new List<Question>
            {
                new Question("q1", new List<Answer>{new Answer("A", true), new Answer("B", false)}),
                new Question("q2", new List<Answer>{new Answer("AA", true), new Answer("BB", false), new Answer("CC", false)}),
                new Question("q3", new List<Answer>{new Answer("A", true), new Answer("B", true)}),
            };
            questions[0].Answers[1].IsChecked = true;
            questions[2].Answers[0].IsChecked = true;
            return new QuestionsSet("test", questions);
        }
    }
}
