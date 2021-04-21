using System;

namespace Megatokyo.Domain
{
    public class RantDomain
    {
        public int RantId { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public string Author { get; set; }
        public Uri Url { get; set; }
        public DateTime DateTime { get; set; }
        public string Content { get; set; }
    }
}
