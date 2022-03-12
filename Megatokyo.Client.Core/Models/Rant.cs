using System;

namespace Megatokyo.Client.Core.Models
{
    public class Rant
    {
        public string Title { get; set; }
        public int Number { get; set; }
        public string Author { get; set; }
        public Uri Url { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string Content { get; set; }
    }
}
