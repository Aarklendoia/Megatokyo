using System;

namespace Megatokyo.Domain
{
    public class Rant
    {
        public string Title { get; set; }
        public int Number { get; set; }
        public string Author { get; set; }
        public Uri Url { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string Content { get; set; }

        public Rant(string title, int number, string author, Uri url, DateTimeOffset publishDate, string content)
        {
            Title = title;
            Number = number;
            Author = author;
            Url = url;
            PublishDate = publishDate;
            Content = content;
        }

        public Rant(int number)
        {
            Number = number;
        }
    }
}
