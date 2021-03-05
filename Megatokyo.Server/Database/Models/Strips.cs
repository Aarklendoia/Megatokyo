using System;
using System.ComponentModel.DataAnnotations;

namespace Megatokyo.Server.Database.Models
{
    public partial class Strips
    {
        [Key]
        public int StripId { get; set; }
        [Required]
        public int ChapterId { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public Uri Url { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}

