using Megatokyo.Models;
using Shouldly;

namespace Megatokyo.Server.UnitTest
{
    public class StringExtractorUnitTest
    {
        const string TEST_STRING = "Chaîne de #test& pour les #extractions& de chaîne";

        [Theory]
        [MemberData(nameof(ExtractorData))]
        public void TestExtractOffsetAndDelimiters(string startDelimiter, string endDelimiter, string expected, int offset, bool includeDelimiters)
        {
            StringExtractor stringExtractor = new(TEST_STRING)
            {
                Offset = offset
            };
            string actual = stringExtractor.Extract(startDelimiter, endDelimiter, includeDelimiters);
            actual.ShouldBe(expected);
        }

        public static TheoryData<string, string, string, int, bool> ExtractorData()
        {
            return new TheoryData<string, string, string, int, bool>
            {
                { "#", "&", "test", 0, false },
                { "#", "&", "#test&", 0, true },
                { "#", "&", "extractions", 20, false },
                { "#", "&", "#extractions&", 20, true }
            };
        }

        [Fact]
        public void TestExtractSourceEmpty()
        {
            StringExtractor stringExtractor = new(string.Empty);
            Should.Throw<ExtractorException>(() => stringExtractor.Extract("#", "&", false));
        }

        [Fact]
        public void TestExtractFirstDelimiterNotFound()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            Should.Throw<ExtractorException>(() => stringExtractor.Extract("@", "&", false));
        }

        [Fact]
        public void TestExtractFirstDelimiterAfterTheEndOfTheText()
        {
            StringExtractor stringExtractor = new(TEST_STRING)
            {
                Offset = 100
            };
            Should.Throw<ExtractorException>(() => stringExtractor.Extract("#", "&", false));
        }

        [Fact]
        public void TestExtractEndDelimiterNotFound()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            Should.Throw<ExtractorException>(() => stringExtractor.Extract("#", "@", false));
        }

        [Fact]
        public void TestExtractStartDelimiterNull()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            Should.Throw<ArgumentNullException>(() => stringExtractor.Extract(null!, "&", false));
        }

        [Fact]
        public void TestExtractEndDelimiterNull()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            Should.Throw<ArgumentNullException>(() => stringExtractor.Extract("#", null!, false));
        }

        [Fact]
        public void TestExtractStartDelimiterEmpty()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            Should.Throw<ExtractorException>(() => stringExtractor.Extract("", "&", false));
        }

        [Fact]
        public void TestExtractEndDelimiterEmpty()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            Should.Throw<ArgumentException>(() => stringExtractor.Extract("#", "", false));
        }

        [Fact]
        public void TestExtractWithAutomaticOffsetNoDelimiters()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            stringExtractor.Extract("#", "&", false);
            string actual2 = stringExtractor.Extract("#", false);
            actual2.ShouldBe(" pour les ");
        }

        [Fact]
        public void TestExtractWithAutomaticOffsetWithDelimiters()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            stringExtractor.Extract("#", "&", true);
            string actual2 = stringExtractor.Extract("#", true);
            actual2.ShouldBe("& pour les #");
        }

        [Fact]
        public void TestExtractWithAutomaticOffsetFirstDelimiterAfterTheEndOfTheText()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            stringExtractor.Extract("#", "&", false);
            stringExtractor.Offset = 100;
            Should.Throw<ExtractorException>(() => stringExtractor.Extract("#", false));
        }

        [Fact]
        public void TestExtractWithAutomaticOffsetEndDelimiterNotFound()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            stringExtractor.Extract("#", "&", true);
            Should.Throw<ExtractorException>(() => stringExtractor.Extract("@", false));
        }

        [Fact]
        public void TestRemoveNoDelimiters()
        {
            StringExtractor stringExtractor = new(TEST_STRING);

            bool rest = stringExtractor.Remove("#", "&", false, out string actual);
            rest.ShouldBe(true);
            actual.ShouldBe("Chaîne de #& pour les #extractions& de chaîne");

            rest = stringExtractor.Remove("#", "&", false, out actual);
            rest.ShouldBe(true);
            actual.ShouldBe("Chaîne de #& pour les #& de chaîne");

            rest = stringExtractor.Remove("#", "&", false, out _);
            rest.ShouldBe(false);
        }

        [Fact]
        public void TestRemoveWithDelimiters()
        {
            StringExtractor stringExtractor = new(TEST_STRING);

            bool rest = stringExtractor.Remove("#", "&", true, out string actual);
            rest.ShouldBe(true);
            actual.ShouldBe("Chaîne de  pour les #extractions& de chaîne");

            rest = stringExtractor.Remove("#", "&", true, out actual);
            rest.ShouldBe(true);
            actual.ShouldBe("Chaîne de  pour les  de chaîne");

            rest = stringExtractor.Remove("#", "&", true, out _);
            rest.ShouldBe(false);
        }

        [Fact]
        public void TestRemoveStartDelimiterNull()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            Should.Throw<ArgumentNullException>(() => stringExtractor.Remove(null!, "&", false, out _));
        }

        [Fact]
        public void TestRemoveEndDelimiterNull()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            Should.Throw<ArgumentNullException>(() => stringExtractor.Remove("#", null!, false, out _));
        }

        [Fact]
        public void TestRemoveStartDelimiterEmpty()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            Should.Throw<ExtractorException>(() => stringExtractor.Remove("", "&", false, out _));
        }

        [Fact]
        public void TestRemoveEndDelimiterEmpty()
        {
            StringExtractor stringExtractor = new(TEST_STRING);
            Should.Throw<ArgumentException>(() => stringExtractor.Remove("#", "", false, out _));
        }
    }
}
