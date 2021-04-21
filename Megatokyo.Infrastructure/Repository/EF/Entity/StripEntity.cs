using System;
using System.ComponentModel.DataAnnotations;

namespace Megatokyo.Infrastructure.Repository.EF.Entity
{
    public class StripEntity
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public string Category { get; set; }
        public Uri Url { get; set; }
        public DateTime DateTime { get; set; }
        public ChapterEntity Chapter { get; internal set; }
    }
}
