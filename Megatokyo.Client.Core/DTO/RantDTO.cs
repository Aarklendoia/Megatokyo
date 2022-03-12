using System;

namespace Megatokyo.Client.Core.DTO
{
    internal class RantDTO
    {
        public string Title { get; set; }
        public int Number { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string Content { get; set; }
    }
}
