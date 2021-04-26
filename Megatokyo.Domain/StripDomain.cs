using System;

namespace Megatokyo.Domain
{
    public class StripDomain
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public Uri Url { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string Category { get; set; }

        public StripDomain(string category, int number, string title, Uri url, DateTimeOffset publishDate)
        {
            Number = number;
            Title = title;
            Url = url;
            PublishDate = publishDate;
            Category = category;
        }

        public StripDomain(int number)
        {
            Number = number;
        }
    }
}
