using System;

namespace Megatokyo.Infrastructure.Repository.EF.Entity
{
    public class StripEntity
    {
        public string Title { get; set; }
        public int Number { get; set; }
        public string Category { get; set; }
        public Uri Url { get; set; }
        public DateTime DateTime { get; set; }
        public ChapterEntity Chapter { get; internal set; }
    }
}
