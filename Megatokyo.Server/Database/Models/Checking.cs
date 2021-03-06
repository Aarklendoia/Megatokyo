using System;
using System.ComponentModel.DataAnnotations;

namespace Megatokyo.Server.Database.Models
{
    public partial class Checking
    {
        [Key]
        public int ChekingId { get; set; }
        [Required]
        public DateTime LastCheck { get; set; }
        [Required]
        public int LastStripNumber { get; set; }
        [Required]
        public int LastRantNumber { get; set; }
    }
}