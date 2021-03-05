using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Megatokyo.Server.Database.Models
{
    public partial class Chapters
    {
        [Key]
        public int ChapterId { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Category { get; set; }
    }
}
