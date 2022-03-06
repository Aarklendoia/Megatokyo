using Megatokyo.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Megatokyo.Server.UnitTest
{
    [TestClass]
    public class StringExtractorUnitTest
    {

        const string TEST_STRING = "Chaîne de #test& pour les #extractions& de chaîne";

        [TestMethod]
        [DynamicData(nameof(ExtractorData), DynamicDataSourceType.Method)]
        public void TestExtractOffsetAndDelimiters(string startDelimiter, string endDelimiter, string expected, int offset, bool includeDelimiters)
        {
            StringExtractor stringExtractor = new(TEST_STRING)
            {
                Offset = offset
            };
            string actual = stringExtractor.Extract(startDelimiter, endDelimiter, includeDelimiters);
            Assert.AreEqual(expected, actual);
        }

        private static IEnumerable<object[]> ExtractorData()
        {
            yield return new object[] { "#", "&", "test", 0, false };
            yield return new object[] { "#", "&", "#test&", 0, true };
            yield return new object[] { "#", "&", "extractions", 20, false };
            yield return new object[] { "#", "&", "#extractions&", 20, true };
        }

        [TestMethod]
        public void TestExtractSourceEmpty()
        {
            StringExtractor stringExtractor = new(string.Empty);
            try
            {
                string actual = stringExtractor.Extract("#", "&", false);
                string message = "No exception for source string empty";
                Assert.Fail(message);
            }
            catch (ExtractorException)
            {
            }
            catch (Exception)
            {
                string message = "Incorrect exception for source string empty";
                Assert.Fail(message);
            }
        }

        [TestMethod]
        public void TestExtractFirstDelimiterNotFound()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            try
            {
                stringExtractor.Extract("@", "&", false);
                Assert.Fail("No exception for first delimiter not found");
            }
            catch (ExtractorException)
            {
            }
            catch (Exception)
            {
                Assert.Fail("Incorrect exception for first delimiter not found");
            }
        }

        [TestMethod]
        public void TestExtractFirstDelimiterAfterTheEndOfTheText()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            try
            {
                stringExtractor.Offset = 100;
                stringExtractor.Extract("#", "&", false);
                Assert.Fail("No exception for first delimiter after the end of the text");
            }
            catch (ExtractorException)
            {
            }
            catch (Exception)
            {
                Assert.Fail("Incorrect exception for first delimiter after the end of the text");
            }
        }

        [TestMethod]
        public void TestExtractEndDelimiterNotFound()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            try
            {
                string actual = stringExtractor.Extract("#", "@", false);
                string message = "No exception for end delimiter not found";
                Assert.Fail(message);
            }
            catch (ExtractorException)
            {
            }
            catch (Exception)
            {
                string message = "Incorrect exception for end delimiter not found";
                Assert.Fail(message);
            }
        }

        [TestMethod]
        public void TestExtractWithAutomaticOffsetNoDelimiters()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            string expected = " pour les ";
            stringExtractor.Extract("#", "&", false);
            string actual2 = stringExtractor.Extract("#", false);
            Assert.AreEqual(expected, actual2);
        }

        [TestMethod]
        public void TestExtractWithAutomaticOffsetWithDelimiters()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            string expected = "& pour les #";
            stringExtractor.Extract("#", "&", true);
            string actual2 = stringExtractor.Extract("#", true);
            Assert.AreEqual(expected, actual2);
        }

        [TestMethod]
        public void TestExtractWithAutomaticOffsetFirstDelimiterAfterTheEndOfTheText()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            try
            {
                string actual1 = stringExtractor.Extract("#", "&", false);
                stringExtractor.Offset = 100;
                string actual2 = stringExtractor.Extract("#", false);
                string message = "No exception for first delimiter after the end of the text";
                Assert.Fail(message);
            }
            catch (ExtractorException)
            {
            }
            catch (Exception)
            {
                string message = "Incorrect exception for first delimiter after the end of the text";
                Assert.Fail(message);
            }
        }

        [TestMethod]
        public void TestExtractWithAutomaticOffsetEndDelimiterNotFound()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            try
            {
                string actual1 = stringExtractor.Extract("#", "&", true);
                string actual2 = stringExtractor.Extract("@", false);
                string message = "No exception for end delimiter not found";
                Assert.Fail(message);
            }
            catch (ExtractorException)
            {
            }
            catch (Exception)
            {
                string message = "Incorrect exception for end delimiter not found";
                Assert.Fail(message);
            }
        }

        [TestMethod]
        public void TextRemoveNoDelimiters()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            bool rest;
            string expected1 = "Chaîne de #& pour les #extractions& de chaîne";
            rest = stringExtractor.Remove("#", "&", false, out string actual);
            Assert.AreEqual(true, rest);
            Assert.AreEqual(expected1, actual);
            string expected2 = "Chaîne de #& pour les #& de chaîne";
            rest = stringExtractor.Remove("#", "&", false, out actual);
            Assert.AreEqual(true, rest);
            Assert.AreEqual(expected2, actual);
            rest = stringExtractor.Remove("#", "&", false, out _);
            Assert.AreEqual(false, rest);
        }

        [TestMethod]
        public void TextRemoveWithDelimiters()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            bool rest;
            string expected1 = "Chaîne de  pour les #extractions& de chaîne";
            rest = stringExtractor.Remove("#", "&", true, out string actual);
            Assert.AreEqual(true, rest);
            Assert.AreEqual(expected1, actual);
            string expected2 = "Chaîne de  pour les  de chaîne";
            rest = stringExtractor.Remove("#", "&", true, out actual);
            Assert.AreEqual(true, rest);
            Assert.AreEqual(expected2, actual);
            rest = stringExtractor.Remove("#", "&", true, out _);
            Assert.AreEqual(false, rest);
        }
    }
}
