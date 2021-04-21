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
        public DateTime Date { get; set; }
        public string Category { get; set; }

        public StripDomain(ChapterDomain chapter, int number, string title, Uri url, DateTime date)
        {
            Chapter = chapter;
            Number = number;
            Title = title;
            Url = url;
            Date = date;

            ChapterValidator(Chapter);
        }

        public StripDomain(int number)
        {
            Number = number;
        }

        public StripDomain()
        {
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
