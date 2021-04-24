using System;
using System.ComponentModel.DataAnnotations;

namespace Megatokyo.Infrastructure.Repository.EF.Entity
{
    public class CheckingEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime LastCheck { get; set; }
        [Required]
        public int LastStripNumber { get; set; }
        [Required]
        public int LastRantNumber { get; set; }
    }
}
