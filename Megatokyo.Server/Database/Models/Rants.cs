using System;
using System.ComponentModel.DataAnnotations;

namespace Megatokyo.Server.Database.Models
{
    public partial class Rants
    {
        [Key]
        public int RantId { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Author { get; set; }
    }
}
