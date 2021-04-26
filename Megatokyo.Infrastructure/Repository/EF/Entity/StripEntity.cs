using System;
using System.ComponentModel.DataAnnotations;

namespace Megatokyo.Infrastructure.Repository.EF.Entity
{
    public class StripEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public Uri Url { get; set; }
        [Required]
        public DateTimeOffset PublishDate { get; set; }
    }
}
