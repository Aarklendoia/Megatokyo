using System;
using System.Linq;

namespace Megatokyo.Domain
{
    public class StripDomain
    {
        public ChapterDomain Chapter { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public Uri Url { get; set; }
        public DateTime Timestamp { get; set; }
        public string Category { get; set; }

        public StripDomain(ChapterDomain chapter, int number, string title, Uri url, DateTime timestamp)
        {
            Chapter = chapter;
            Number = number;
            Title = title;
            Url = url;
            Timestamp = timestamp;

            ChapterValidator(Chapter);
        }

        public StripDomain(int number)
        {
            Number = number;
        }

        private static void ChapterValidator(ChapterDomain chapter)
        {
            ChaptersDomain chapterDomain = new();
            if (!chapterDomain.Contains(chapter))
            {
                throw new ArgumentException("Invalid chapter");
            }
        }
    }
}
