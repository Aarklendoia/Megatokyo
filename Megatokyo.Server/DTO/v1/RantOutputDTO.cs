using System;

namespace Megatokyo.Server.DTO.v1
{
    internal class RantOutputDTO
    {
        public int RantId { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public string Author { get; set; }
        public Uri Url { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string Content { get; set; }
    }
}
