using System;

namespace Megatokyo.Server.DTO.v1
{
    internal class StripOutputDTO
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public Uri Url { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string Category { get; set; }
    }
}
