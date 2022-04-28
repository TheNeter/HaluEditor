using Microsoft.VisualStudio.TestTools.UnitTesting;
using ngprojects.HaluEditor.Document;

namespace ngprojects.HaluEditorTest
{
    [TestClass]
    public class HaluDocumentTest
    {
        private readonly string[] lineEndings = new string[] { "\r", "\r\n", "\n" };

        [TestMethod]
        public void DocumentInitWithoutException()

        {
            HaluDocument document = new HaluDocument();
            Assert.IsTrue(document.Count == 1);
        }

        [TestMethod]
        public void SetDocumentTextLineEndingTest()
        {
            for (int i = 0; i < lineEndings.Length; i++)
            {
                string ending = lineEndings[i];
                HaluDocument document = new HaluDocument();
                string val = $"Hello World{ending}";
                document.Text = val;
                Assert.IsTrue(document.Text.Equals(val), $"failed with line-ending-id {i}");
                Assert.IsTrue(document.Count == 2, $"failed with line-ending-id {i}");
            }
        }

        [TestMethod]
        public void SetDocumentTextToMultiLinesEmpty()
        {
            for (int i = 0; i < lineEndings.Length; i++)
            {
                string ending = lineEndings[i];
                HaluDocument document = new HaluDocument();
                string val = $"{ending}{ending}";
                document.Text = val;
                Assert.IsTrue(document.Text.Equals(val), $"failed with line-ending-id {i}");
                Assert.IsTrue(document.Count == 3, $"failed with line-ending-id {i}");
            }
        }

        [TestMethod]
        public void SetDocumentTextToMultiLineText()
        {
            for (int i = 0; i < lineEndings.Length; i++)
            {
                string ending = lineEndings[i];
                HaluDocument document = new HaluDocument();
                string val = $"Hello World{ending}Hello World";
                document.Text = val;
                Assert.IsTrue(document.Text.Equals(val), $"failed with line-ending-id {i}");
                Assert.IsTrue(document.Count == 2, $"failed with line-ending-id {i}");
            }
        }

        [TestMethod]
        public void SetDocumentTextToNull()
        {
            HaluDocument document = new HaluDocument();
            document.Text = null;
            Assert.IsTrue(document.Count == 1);
        }

        [TestMethod]
        public void SetDocumentTextToOneLineText()
        {
            HaluDocument document = new HaluDocument();
            string val = "Hello World";
            document.Text = val;
            Assert.IsTrue(document.Text.Equals(val));
            Assert.IsTrue(document.Count == 1);
        }
    }
}