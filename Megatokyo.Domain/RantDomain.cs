using System;

namespace Megatokyo.Domain
{
    public class RantDomain
    {
        public string Title { get; set; }
        public int Number { get; set; }
        public string Author { get; set; }
        public Uri Url { get; set; }
        public DateTime Timestamp { get; set; }
        public string Content { get; set; }

        public RantDomain(string title, int number, string author, Uri url, DateTime timestamp, string content)
        {
            Title = title;
            Number = number;
            Author = author;
            Url = url;
            Timestamp = timestamp;
            Content = content;
        }

        public RantDomain(int number)
        {
            Number = number;
        }
    }
}
