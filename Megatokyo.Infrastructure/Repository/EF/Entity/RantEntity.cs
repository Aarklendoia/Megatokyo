using System;

namespace Megatokyo.Infrastructure.Repository.EF.Entity
{
    public class RantEntity
    {
        public string Title { get; set; }
        public int Number { get; set; }
        public string Author { get; set; }
        public Uri Url { get; set; }
        public DateTime DateTime { get; set; }
        public string Content { get; set; }
    }
}
